# Copilot Instructions

## Build & Test

```bash
# Build the solution
dotnet build BackupAssistant.slnx

# Run all tests
dotnet test BackupAssistant.slnx

# Run all tests with coverage
dotnet test BackupAssistant.slnx --collect:"XPlat Code Coverage" --settings test/coverlet.runsettings

# Run a single test class
dotnet test test/BackupAssistant.Test.csproj --filter "FullyQualifiedName~BackupServiceTest"

# Run a single test method
dotnet test test/BackupAssistant.Test.csproj --filter "FullyQualifiedName~BackupServiceTest.Full_Backup_CopiesFiles"
```

> **Note:** Assembly signing requires `src/Deploy/signingkey.snk` (not committed). See README for key generation steps.

## Architecture

This is a **WinUI 3** (Windows App SDK) desktop app using the MVVM pattern via
**CommunityToolkit.Mvvm** and **Microsoft.Extensions.DependencyInjection**.

It is deployed **unpackaged and self-contained** (`WindowsPackageType=None`,
`WindowsAppSDKSelfContained=true`), so it runs as a plain `.exe` with no runtime prerequisite.

### Two projects, one rule

- **`src/BackupAssistant.Core`** (`net10.0-windows`) — data models, services, view models.
  **Must never reference WinUI or any other UI framework.** This is what lets the test suite run
  headless under `dotnet test` with no XAML runtime or STA thread.
- **`src/BackupAssistant`** (`net10.0-windows10.0.19041.0`) — the WinUI 3 app: XAML, code-behind,
  and the UI-specific implementations of the Core abstractions. Composition root is `App.xaml.cs`.

If a view model needs something from the UI (a dialog, the app lifetime), add a method to an
abstraction in `Core/Services` and implement it in the app project. Do not reach for a WinUI type
from Core.

### Layer structure

- **`Core/DataModels/`** — Plain data types and enums (`FileListing`, `BackupType`, `BackupAction`, `BackupSettings`). No dependencies on other layers.
- **`Core/Models/`** — Observable model classes that back the ViewModels (e.g., `MainWindowModel`).
- **`Core/Services/`** — Business logic behind interfaces (`IBackupService`, `IDialogService`, `ISettingsService`, `IApplicationService`).
- **`Core/ViewModels/`** — MVVM ViewModels. `MainWindowViewModel` is split across partial class files by concern (`.Backup.cs`, `.Files.cs`, `.Filtering.cs`, `.Menu.cs`).
- **`Core/Extensions/`** — Internal utility extensions (`ReplaceFirst` string extension, float `Interlocked.Add`).
- **`BackupAssistant/Views/`** — `ContentDialog` subclasses. WinUI has no modal `Window.ShowDialog()`.
- **`BackupAssistant/Services/`** — `DialogService`, `ApplicationService`.

### Backup logic

`BackupService` supports two modes driven by `BackupType`:
- **Full**: deletes the destination directory, then copies all source files (respecting filters). If the source file listing comes back empty *and* an enumeration error was logged during that listing (permissions/I-O failure swallowed by `SafeEnumerateFiles`/`SafeEnumerateDirectories`), the backup aborts before touching the destination rather than deleting an existing backup and replacing it with a partial or empty copy.
- **Incremental**: builds a combined `FileListing` dictionary keyed by abbreviated path (`...\\relative\\path`), computes a `BackupAction` per file (Copy / Overwrite / Delete / None) based on presence and `LastWriteTime`, then processes only changed files.

Concurrency is throttled via `SemaphoreSlim` at `ProcessorCount * 2`. Per-worker progress reports can be delivered out of order relative to one another (there is no ordering guarantee between computing a percentage and the report for it being delivered), so both backup methods report a final `Progress = 100` after `Task.WhenAll` alongside the completion status — that report is the only one guaranteed to run after every worker has finished.

### Filter selection is an include list

Despite the funnel icon, checking a subfolder in the filter dialog *includes* it in the backup, it does not exclude it — when any filter is set, the backup covers files directly in the source root plus the checked subfolders (recursively); an empty filter list means "back up everything." The UI is labelled accordingly ("Choose folders" / "Check the subfolders you want to back up"), and the main window summarises the selection as "All folders" or "N folders selected".

## Key Conventions

### Theming: never hard-code a colour

The app must follow the Windows light/dark app theme. That means:
- Never set `RequestedTheme` anywhere.
- Brushes come from `{ThemeResource ...}` (e.g. `CardBackgroundFillColorDefaultBrush`,
  `TextFillColorSecondaryBrush`); text styles from `{StaticResource ...}`.
- Icons are `FontIcon` glyphs, never bitmaps — glyphs recolour with the theme, PNGs do not.
- `MainWindow` sets `AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode`, because
  the default (`Legacy`) leaves the caption light in dark mode.

### `System.IO.Abstractions` for testability

All file system access goes through `IFileSystem` (injected). Tests use `MockFileSystem` from `System.IO.Abstractions.TestingHelpers` instead of touching the real file system. This includes settings persistence — `JsonSettingsService` reads and writes through `IFileSystem`.

### `IApplicationService` for process/assembly seams
`Environment.Exit` and static `Assembly.GetExecutingAssembly()` calls are not mockable.
`IApplicationService` wraps application shutdown (`Application.Current.Exit()` — WinUI's equivalent
of WPF's `Shutdown()` — so Serilog's `AddSerilog(dispose: true)` gets a chance to flush instead of
the process being torn down mid-write) and the running assembly's version. `ExitCommand` and
`AboutViewModel` depend on the interface, not on the static/process APIs directly.

### Partial ViewModel files
`MainWindowViewModel` is broken into partial class files per concern. New ViewModel behavior should follow this pattern — add a new `MainWindowViewModel.{Concern}.cs` file rather than growing the base file. Mirror the split in tests (`MainWindowViewModelTest.{Concern}.cs`).

### Notifying setters use `SetProperty`
Properties that wrap a backing model (`MainWindowModel`, `FilterSelectionModel`) use `ObservableObject.SetProperty(oldValue, newValue, model, callback)` rather than assigning the field and unconditionally raising `OnPropertyChanged`. This makes a same-value assignment a no-op — no property-changed notification, no redundant settings save — while still comparing null-safely (`EqualityComparer<T>.Default`, unlike a manual `value.Equals(...)` which cannot tolerate a null instance).

**On WinUI this is load-bearing, not just tidiness.** A compiled two-way `x:Bind` writes the value
straight back into the view model, so a setter that notifies unconditionally loops forever and blows
the stack at startup. WPF's classic `Binding` short-circuits equal values and hides the problem, so a
setter ported from the WPF branch without a guard will look fine there and crash here.

### Constructors assign backing model fields directly
When a ViewModel constructor is seeding initial state from settings/services, assign the model's backing field directly (e.g. `_model.Source = ...`) instead of going through the public property setter. The public setters have side effects (raising `PropertyChanged`, saving settings, clearing dependent state) that should only fire in response to a real user- or code-driven change, not just from loading previously-saved values back in on startup.

### Test base classes with strict mocks

Each test area has a base class (e.g., `MainWindowViewModelTestBase`, `BackupServiceTestBase`) that:
- Creates `Mock<T>` with `MockBehavior.Strict`
- Calls `Verify()` (not `VerifyAll()`) on all mocks in `Dispose()`

`Verify()` only checks setups explicitly marked `.Verifiable()`, so a base class's own setups (e.g.
`ISettingsService.Save()`) should generally be left non-verifiable — they exist so
`MockBehavior.Strict` doesn't reject a call, not to assert every test triggers them. Tests that care
whether a particular call happened (or didn't) should assert it directly, e.g.
`mock.Verify(s => s.Save(), Times.Once)` or `Times.Never`. `BackupServiceTestBase` still uses
`VerifyAll()`, since every setup it or its tests add is expected to be exercised by that test's Act
phase — prefer `VerifyAll()` there unless a specific test needs looser checking.

Tests inherit from these bases and only set up what their specific test needs.
`MockBehavior.Strict` is what actually catches unexpected/unwanted calls; verification is for
asserting expected ones.

### `[ExcludeFromCodeCoverage]` usage

Applied to the UI and process glue that cannot be unit tested — `App.xaml.cs`,
`MainWindow.xaml.cs`, the `ContentDialog` classes, `DialogService`, `ApplicationService` (wraps
`Application.Current.Exit()`/`Assembly.GetExecutingAssembly()`) — and to the test assembly itself
via an assembly-level attribute in the `.csproj`.

`test/coverlet.runsettings` carries a filter for generated types. It is currently a no-op on this
branch: the test project references only `BackupAssistant.Core`, so the app assembly (where all
generated XAML code lives) is never instrumented. It is kept so the documented coverage command is
identical to the one on `master`.

### Accessibility

Icon-only buttons need `AutomationProperties.Name` in addition to a tooltip, so screen readers and
UI automation can identify them.

### Settings persistence

User settings (source, destination, filters, backup type) are persisted as JSON at
`%LOCALAPPDATA%\Anaheim_Electronics\settings.json` through `ISettingsService`. The ViewModel saves
settings when a property setter actually changes the value (see `SetProperty` above) — loading
previously-saved values back in at startup must not trigger a re-save, or opening the app rewrites
the settings file on every launch.

### Solution platforms

WinUI 3 cannot build as Any CPU, so `BackupAssistant.slnx` declares `x64`, `x86` and `ARM64` as its
solution platforms, and `BackupAssistant.csproj` derives its `RuntimeIdentifier` from `$(Platform)`.
`BackupAssistant.Core` and `BackupAssistant.Test` are Any CPU and are mapped with
`<Platform Solution="*|*" Project="AnyCPU" />`.

An unqualified `dotnet build` / `dotnet test` still works and falls back to `win-x64`. If you add a
platform, add it in three places: `<Platforms>` in the csproj, the `RuntimeIdentifier` conditions
below it, and `<Configurations>` in the `.slnx`.

### No C++ workload required

`src/BackupAssistant/Directory.Build.targets` overrides the Windows App SDK's `GetVCInstallPath`
target with an empty one. The SDK adds that target to the XAML compiler's DependsOn lists for every
project regardless of language, and it throws `DirectoryNotFoundException` on
`$(VsInstallRoot)\VC\Tools\MSVC` when the MSVC toolset is absent. Because `VsInstallRoot` is only set
inside Visual Studio, command line builds pass while Visual Studio builds fail — so verify build
changes with `-p:VsInstallRoot=<VS install path>` to simulate a Visual Studio build.

Do not "fix" this by installing the C++ workload; this is a C#-only app and does not need it.

### Publishing needs the XAML carried over by hand

The Windows App SDK adds compiled XAML (`.xbf`) and the app's resource index (`.pri`) to the build
output only — its `AddProcessedXamlFilesToCopyLocal` target hooks `GetCopyToOutputDirectoryItems`
and has no publish counterpart. The `AddWinUIXamlAndResourcesToPublish` target in the app csproj
re-adds them, without which a published app dies at startup with a stowed WinRT exception. Keep that
target narrow: globbing every `*.pri` in the output picks up framework files the SDK already
publishes and trips `NETSDK1152`.

### NuGet

`NuGet.config` pins restore to nuget.org so a clone builds identically anywhere, regardless of
machine-wide feed configuration. Shared version, company and signing properties live in
`Directory.Build.props`.

# Copilot Instructions

## Build & Test

```bash
# Build the solution
dotnet build BackupAssistant.slnx

# Run all tests
dotnet test BackupAssistant.slnx

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
- **Full**: deletes the destination directory, then copies all source files (respecting filters).
- **Incremental**: builds a combined `FileListing` dictionary keyed by abbreviated path (`...\\relative\\path`), computes a `BackupAction` per file (Copy / Overwrite / Delete / None) based on presence and `LastWriteTime`, then processes only changed files.

Concurrency is throttled via `SemaphoreSlim` at `ProcessorCount * 2`.

**Filters are an include list, not an exclude list.** When `filterItems` is non-empty, the backup
covers files sitting directly in the source root **plus** the listed subfolders recursively.
An empty filter list means "back up everything".

## Key Conventions

### Theming: never hard-code a colour

The app must follow the Windows light/dark app theme. That means:
- Never set `RequestedTheme` anywhere.
- Brushes come from `{ThemeResource ...}` (e.g. `CardBackgroundFillColorDefaultBrush`,
  `TextFillColorSecondaryBrush`); text styles from `{StaticResource ...}`.
- Icons are `FontIcon` glyphs, never bitmaps — glyphs recolour with the theme, PNGs do not.
- `MainWindow` sets `AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode`, because
  the default (`Legacy`) leaves the caption light in dark mode.

### Always guard property setters against no-op writes

Compiled two-way `x:Bind` writes the value straight back into the view model. A setter that raises
`PropertyChanged` unconditionally will therefore loop forever and blow the stack. Use
`ObservableObject.SetProperty(...)` (or an explicit equality check) in every notifying setter.

### `System.IO.Abstractions` for testability

All file system access goes through `IFileSystem` (injected). Tests use `MockFileSystem` from `System.IO.Abstractions.TestingHelpers` instead of touching the real file system. This includes settings persistence — `JsonSettingsService` reads and writes through `IFileSystem`.

### Partial ViewModel files

`MainWindowViewModel` is broken into partial class files per concern. New ViewModel behavior should follow this pattern — add a new `MainWindowViewModel.{Concern}.cs` file rather than growing the base file.

### Test base classes with strict mocks

Each test area has a base class (e.g., `MainWindowViewModelTestBase`, `BackupServiceTestBase`) that:
- Creates `Mock<T>` with `MockBehavior.Strict`
- Calls `Verify()` on all mocks in `Dispose()`

`MockBehavior.Strict` is what catches unexpected calls. `Dispose` uses `Verify()` rather than
`VerifyAll()` so that a shared setup in the base class does not have to be exercised by every
single test; a test that cares about an interaction asserts it explicitly.

### `[ExcludeFromCodeCoverage]` usage

Applied to the UI glue that cannot be unit tested — `App.xaml.cs`, `MainWindow.xaml.cs`, the
`ContentDialog` classes, `DialogService`, `ApplicationService` — and to the test assembly itself
via an assembly-level attribute in the `.csproj`.

### Accessibility

Icon-only buttons need `AutomationProperties.Name` in addition to a tooltip, so screen readers and
UI automation can identify them.

### Settings persistence

User settings (source, destination, filters, backup type) are persisted as JSON at
`%LOCALAPPDATA%\Anaheim_Electronics\settings.json` through `ISettingsService`. The ViewModel saves
settings immediately on every change. Constructing a ViewModel must *not* write settings back out.

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

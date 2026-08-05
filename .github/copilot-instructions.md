# Copilot Instructions

## Build & Test

```bash
# Build the solution
dotnet build BackupAssistant.slnx

# Run all tests
dotnet test BackupAssistant.slnx

# Run all tests with coverage (excludes WPF-generated noise via test/coverlet.runsettings)
dotnet test BackupAssistant.slnx --collect:"XPlat Code Coverage" --settings test/coverlet.runsettings

# Run a single test class
dotnet test test/BackupAssistant.Test.csproj --filter "FullyQualifiedName~BackupServiceTest"

# Run a single test method
dotnet test test/BackupAssistant.Test.csproj --filter "FullyQualifiedName~BackupServiceTest.Full_Backup_CopiesFiles"
```

> **Note:** Assembly signing requires `src/Deploy/signingkey.snk` (not committed). See README for key generation steps.

## Architecture

This is a WPF desktop app targeting `net10.0-windows`. It uses the MVVM pattern via **CommunityToolkit.Mvvm** and **Microsoft.Extensions.DependencyInjection** for the service container (configured in `App.xaml.cs`).

### Layer structure

- **`src/DataModels/`** — Plain data types and enums (e.g., `FileListing`, `BackupType`, `BackupAction`). No dependencies on other layers.
- **`src/Models/`** — Observable model classes that back the ViewModels (e.g., `MainWindowModel`).
- **`src/Services/`** — Business logic behind interfaces (`IBackupService`, `IDialogService`, `IApplicationService`, `ISettingsService`). All services are registered as singletons except `BackupService`, which is instantiated directly by the ViewModel.
- **`src/ViewModels/`** — MVVM ViewModels. `MainWindowViewModel` is split across multiple partial class files by concern (`.Backup.cs`, `.Files.cs`, `.Filtering.cs`, `.Menu.cs`).
- **`src/Extensions/`** — Internal utility extensions (e.g., `ReplaceFirst` string extension, float `Interlocked.Add`).

### Backup logic

`BackupService` supports two modes driven by `BackupType`:
- **Full**: deletes the destination directory, then copies all source files (respecting filters). If the source file listing comes back empty *and* an enumeration error was logged during that listing (permissions/I-O failure swallowed by `SafeEnumerateFiles`/`SafeEnumerateDirectories`), the backup aborts before touching the destination rather than deleting an existing backup and replacing it with a partial or empty copy.
- **Incremental**: builds a combined `FileListing` dictionary keyed by abbreviated path (`...\\relative\\path`), computes a `BackupAction` per file (Copy / Overwrite / Delete / None) based on presence and `LastWriteTime`, then processes only changed files.

Concurrency is throttled via `SemaphoreSlim` at `ProcessorCount * 2`. Per-worker progress reports can be delivered out of order relative to one another (there is no ordering guarantee between computing a percentage and the report for it being delivered), so both backup methods report a final `Progress = 100` after `Task.WhenAll` alongside the completion status — that report is the only one guaranteed to run after every worker has finished.

### Filter selection is an include list

Despite the funnel icon, checking a subfolder in the filter dialog *includes* it in the backup, it does not exclude it — when any filter is set, the backup covers files directly in the source root plus the checked subfolders (recursively); an empty filter list means "back up everything." The UI is labelled accordingly ("Choose folders to back up" / "Check the subfolders you want to include").

## Key Conventions

### `System.IO.Abstractions` for testability
All file system access goes through `IFileSystem` (injected). Tests use `MockFileSystem` from `System.IO.Abstractions.TestingHelpers` instead of touching the real file system.

### `IApplicationService` for process/assembly seams
`Environment.Exit` and static `Assembly.GetExecutingAssembly()` calls are not mockable. `IApplicationService` wraps application shutdown (`Application.Current.Shutdown()`, so `Log.CloseAndFlush`/Serilog's `AddSerilog(dispose: true)` gets a chance to run instead of the process being torn down mid-flush) and the running assembly's version. `ExitCommand` and `AboutViewModel` depend on the interface, not the static/process APIs directly.

### Partial ViewModel files
`MainWindowViewModel` is broken into partial class files per concern. New ViewModel behavior should follow this pattern — add a new `MainWindowViewModel.{Concern}.cs` file rather than growing the base file. Mirror the split in tests (`MainWindowViewModelTest.{Concern}.cs`).

### Notifying setters use `SetProperty`
Properties that wrap a backing model (`MainWindowModel`, `FilterSelectionModel`) use `ObservableObject.SetProperty(oldValue, newValue, model, callback)` rather than assigning the field and unconditionally raising `OnPropertyChanged`. This makes a same-value assignment a no-op — no property-changed notification, no redundant settings save — while still comparing null-safely (`EqualityComparer<T>.Default`, unlike a manual `value.Equals(...)` which cannot tolerate a null instance).

### Constructors assign backing model fields directly
When a ViewModel constructor is seeding initial state from settings/services, assign the model's backing field directly (e.g. `_model.Source = ...`) instead of going through the public property setter. The public setters have side effects (raising `PropertyChanged`, saving settings, clearing dependent state) that should only fire in response to a real user- or code-driven change, not just from loading previously-saved values back in on startup.

### Test base classes with strict mocks
Each test area has a base class (e.g., `MainWindowViewModelTestBase`, `BackupServiceTestBase`) that:
- Creates `Mock<T>` with `MockBehavior.Strict`
- Calls `Verify()` (not `VerifyAll()`) on all mocks in `Dispose()`

`Verify()` only checks setups explicitly marked `.Verifiable()`, so a base class's own setups (e.g. `ISettingsService.Save()`) should generally be left non-verifiable — they exist so `MockBehavior.Strict` doesn't reject a call, not to assert every test triggers them. Tests that care whether a particular call happened (or didn't) should assert it directly, e.g. `mock.Verify(s => s.Save(), Times.Once)` or `Times.Never`. `BackupServiceTestBase` still uses `VerifyAll()`, since every setup it or its tests add is expected to be exercised by that test's Act phase — prefer `VerifyAll()` there unless a specific test needs looser checking.

Tests inherit from these bases and only set up what their specific test needs. `MockBehavior.Strict` is what actually catches unexpected/unwanted calls; verification is for asserting expected ones.

### `[ExcludeFromCodeCoverage]` usage
Applied to `App.xaml.cs`, `DialogService` (real `Window`/`OpenFolderDialog` glue), `ApplicationService` (wraps `Application.Current.Shutdown()`/`Assembly.GetExecutingAssembly()`), and to the test assembly itself via an assembly-level attribute in the `.csproj`. Apply it to any purely UI/process glue code that cannot be unit tested. `test/coverlet.runsettings` additionally excludes the WPF-generated `XamlGeneratedNamespace.GeneratedInternalTypeHelper` type, which isn't code this project owns.

### Settings persistence
User settings (source, destination, filters, backup type) are persisted through `ISettingsService`, which wraps `Properties.Settings`. The ViewModel saves settings when a property setter actually changes the value (see `SetProperty` above) — loading previously-saved values back in at startup must not trigger a re-save, or opening the app rewrites `user.config` on every launch.

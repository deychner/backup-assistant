# Copilot Instructions

## Build & Test

```bash
# Build the solution
dotnet build BackupAssistant.sln

# Run all tests
dotnet test BackupAssistant.sln

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
- **`src/Services/`** — Business logic behind interfaces (`IBackupService`, `IDialogService`, `ILogService`, `ISettingsService`). All services are registered as singletons except `BackupService`, which is instantiated directly by the ViewModel.
- **`src/ViewModels/`** — MVVM ViewModels. `MainWindowViewModel` is split across multiple partial class files by concern (`.Backup.cs`, `.Files.cs`, `.Filtering.cs`, `.Menu.cs`).
- **`src/Extensions/`** — Internal utility extensions (e.g., `ReplaceFirst` string extension, float `Interlocked.Add`).

### Backup logic

`BackupService` supports two modes driven by `BackupType`:
- **Full**: deletes the destination directory, then copies all source files (respecting filters).
- **Incremental**: builds a combined `FileListing` dictionary keyed by abbreviated path (`...\\relative\\path`), computes a `BackupAction` per file (Copy / Overwrite / Delete / None) based on presence and `LastWriteTime`, then processes only changed files.

Concurrency is throttled via `SemaphoreSlim` at `ProcessorCount * 2`.

## Key Conventions

### `System.IO.Abstractions` for testability
All file system access goes through `IFileSystem` (injected). Tests use `MockFileSystem` from `System.IO.Abstractions.TestingHelpers` instead of touching the real file system.

### Partial ViewModel files
`MainWindowViewModel` is broken into partial class files per concern. New ViewModel behavior should follow this pattern — add a new `MainWindowViewModel.{Concern}.cs` file rather than growing the base file.

### Test base classes with strict mocks
Each test area has a base class (e.g., `MainWindowViewModelTestBase`, `BackupServiceTestBase`) that:
- Creates `Mock<T>` with `MockBehavior.Strict`
- Calls `VerifyAll()` on all mocks in `Dispose()`

Tests inherit from these bases and only set up what their specific test needs. This catches unexpected calls automatically.

### `[ExcludeFromCodeCoverage]` usage
Applied to `App.xaml.cs` (UI bootstrap code) and to the test assembly itself via an assembly-level attribute in the `.csproj`. Apply it to any purely UI glue code that cannot be unit tested.

### Settings persistence
User settings (source, destination, filters, backup type) are persisted through `ISettingsService`, which wraps `Properties.Settings`. The ViewModel saves settings immediately on every change.

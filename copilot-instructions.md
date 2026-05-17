# Copilot Instructions for BackupAssistant

## Project Context
**BackupAssistant** is a WinUI 3 desktop application for Windows that helps users back up files to external media. It supports multiple backup types (full, incremental) with filtering capabilities and is built with C# (.NET, net10.0-windows).

## Architecture & Patterns

### MVVM (Model-View-ViewModel)
- **Views:** XAML files (MainWindow.xaml, About.xaml)
- **ViewModels:** Located in `src/ViewModels/`, orchestrate UI logic and state
- **Models/Services:** Business logic in `src/Models/` and `src/Services/`
- **Data Models:** Domain objects in `src/DataModels/`

### Service Layer Approach
- **Interfaces first:** Services are defined via interfaces (`IBackupService`, `ISettingsService`, `ILogService`, `IDialogService`)
- **Dependency pattern:** ViewModels depend on service interfaces, not concrete implementations
- **Location:** All service interfaces and implementations are in `src/Services/`

### Partial ViewModels
MainWindowViewModel is split across multiple partial classes for organization:
- `MainWindowViewModel.cs` — core properties, initialization
- `MainWindowViewModel.Backup.cs` — backup-related commands and logic
- `MainWindowViewModel.Files.cs` — file selection and listing logic
- `MainWindowViewModel.Filtering.cs` — filter management
- `MainWindowViewModel.Menu.cs` — menu commands

When modifying ViewModel behavior, place changes in the appropriate partial file.

## Code Organization Principles

### Service Boundaries
- **Services handle:** file system operations, settings persistence, logging, dialog presentation, backup orchestration
- **ViewModels handle:** UI state, command binding, data transformation for display, orchestration of service calls
- **Models hold:** domain data structures (FileListing, FilterItem, BackupType, etc.)

### Threading & Async
- Backup operations are potentially long-running; use `async`/`await` patterns
- Check [src/Extensions/Interlocked.cs](src/Extensions/Interlocked.cs) for thread-safe operations
- ViewModels should marshal UI updates back to the main thread

### Testing Philosophy
- Unit tests live in `test/` with a mirror structure to `src/`
- Use test base classes in `test/Base/` to reduce duplication
- Test services independently via their interfaces
- Mock or stub external dependencies (file system, settings, dialogs)
- Tests for backup logic are split by backup type (`BackupServiceTest.Full.cs`, `BackupServiceTest.Incremental.cs`)

## Coding Standards & Conventions

### Naming
- PascalCase for classes, interfaces, properties, public methods
- camelCase for private fields and local variables
- Prefix interfaces with `I` (e.g., `IBackupService`)
- Command names follow pattern: `{Action}Command` (e.g., `StartBackupCommand`, `BrowseDestinationCommand`)

### Null Safety
- Use nullable reference types (C# 8+); `?` for nullable properties
- Validate inputs at service boundaries and ViewModel entry points
- Avoid bare null checks; prefer null-coalescing or guard clauses

### Collections
- Use `List<T>`, `Dictionary<K,V>`, `ObservableCollection<T>` (for WinUI bindings)
- For immutable or query-only operations, consider `IEnumerable<T>`
- Check [src/Extensions/EnumCollectionExtension.cs](src/Extensions/EnumCollectionExtension.cs) for collection utilities

## When Explaining Code
- Reference the MVVM pattern and which layer a piece of code belongs to
- Explain the role of service interfaces vs. implementations
- Point out async/threading concerns if present
- Mention test coverage and how to verify changes

## When Refactoring
1. **Preserve interfaces** — don't change public service interfaces without updating all consumers
2. **Extract to services** — if you're adding business logic, create a new service or extend an existing one via its interface
3. **Keep partials organized** — don't move logic between partial files arbitrarily; respect the organizational split
4. **Update tests first** — when changing behavior, add or update tests in `test/` to capture the desired outcome
5. **Avoid tight coupling** — ViewModels should depend on service interfaces, not concrete implementations or other ViewModels

## When Adding Features
1. **Define the domain model** — add or extend a data class in `src/DataModels/`
2. **Create a service interface** — define the contract in `src/Services/INewService.cs`
3. **Implement the service** — add concrete logic in `src/Services/NewService.cs`
4. **Add ViewModel logic** — update the appropriate `MainWindowViewModel.*.cs` partial to expose the feature
5. **Update XAML & bindings** — modify [src/MainWindow.xaml](src/MainWindow.xaml) or [src/About.xaml](src/About.xaml) as needed
6. **Test thoroughly** — add unit tests in `test/` and include integration tests if the feature spans multiple services

## When Improving/Optimizing
- Profile or identify the bottleneck first (reference logs via `ILogService`)
- Changes to `BackupService` affect both full and incremental backup paths; test both
- When refactoring service methods, ensure all callers are updated (use find-usages)
- Document breaking changes in commit messages

## File Navigation Quick Reference
- **Backup logic:** `src/Services/BackupService.cs` & `IBackupService.cs`
- **UI state & commands:** `src/ViewModels/MainWindowViewModel.*.cs`
- **Settings/persistence:** `src/Services/SettingsService.cs`
- **Logging:** `src/Services/LogService.cs`
- **File operations:** `src/ViewModels/MainWindowViewModel.Files.cs`
- **Filtering:** `src/ViewModels/MainWindowViewModel.Filtering.cs`, `src/DataModels/FilterItem.cs`
- **Test infrastructure:** `test/Base/` for base classes and helpers

## Common Patterns to Follow

### Exposing a New ViewModel Property
```csharp
private bool _isProcessing;
public bool IsProcessing
{
    get => _isProcessing;
    set => SetProperty(ref _isProcessing, value);
}
```

### Creating a Command
```csharp
public ICommand StartBackupCommand { get; }

// In constructor:
StartBackupCommand = new RelayCommand(async () => await StartBackupAsync());
```

### Calling a Service
```csharp
try
{
    var result = await _backupService.StartBackupAsync(...);
    IsProcessing = false;
}
catch (OperationCanceledException)
{
    // Handle cancellation
}
catch (Exception ex)
{
    _logService.LogError($"Backup failed: {ex.Message}");
    // Notify UI
}
```

## When Proposing Changes
- Explain which MVVM layer is affected (View, ViewModel, Model, Service)
- Highlight any breaking changes to service interfaces
- Mention test coverage and whether new tests are needed
- Call out performance or threading implications
- Reference specific file paths and line numbers using markdown links

## Assumptions & Limitations
- **Platform:** Windows only (WinUI 3). Features must be Windows-compatible.
- **Runtime:** net10.0-windows; use modern C# features (records, nullable ref types, top-level statements where appropriate).
- **Testing on macOS:** Cannot fully test WinUI UI locally; focus on unit testing business logic and ViewModels.
- **External dependencies:** Check `.csproj` for declared NuGet packages before suggesting new ones.

## How to Approach Requests
1. **Clarify intent** — understand the goal (refactor for clarity, improve performance, add feature)
2. **Propose minimal changes** — suggest the smallest set of modifications
3. **Suggest tests first** — outline test cases that capture the desired behavior
4. **Implement layered** — start with service/model changes, then ViewModel, then UI if needed
5. **Reference existing patterns** — follow conventions already in the codebase
6. **Check for side effects** — ensure changes don't break existing backup flows or tests

---

**Last Updated:** 2026-05-16  
**For:** Effective guidance on refactoring, explaining, improving, and extending BackupAssistant

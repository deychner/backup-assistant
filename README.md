# backup-assistant

A simple tool to back up your files on Windows.

Built with **WinUI 3** (Windows App SDK) using the MVVM pattern.

## Requirements

- Windows 10 version 1809 (build 17763) or later
- [.NET 10 SDK](https://dotnet.microsoft.com/download)

The Windows App SDK ships with the app (self-contained, unpackaged), so there is no separate
runtime for users to install.

## Theme

The app has no hard-coded colours and never sets `RequestedTheme`. It follows the Windows
**Settings → Personalization → Colors → "Choose your mode"** setting, switching between light and
dark live — window body, title bar and dialogs together.

## Build and test

```bash
dotnet build BackupAssistant.slnx
```

```bash
dotnet test BackupAssistant.slnx
```

WinUI 3 has no Any CPU configuration, so the solution platforms are `x64` (default), `x86` and
`ARM64`. To build a specific architecture:

```bash
dotnet build BackupAssistant.slnx -c Release -p:Platform=ARM64
```

> **Note:** Assembly signing requires `src/Deploy/signingkey.snk` (not committed). See below for
> key generation steps.

## Project layout

| Project | Purpose |
| --- | --- |
| `src/BackupAssistant.Core` | Data models, services and view models. No UI framework dependency, so it is fully unit testable. |
| `src/BackupAssistant` | WinUI 3 app: XAML, code-behind and the UI-specific service implementations. |
| `test/BackupAssistant.Test` | xUnit tests against `BackupAssistant.Core`. Runs headless — no XAML runtime needed. |

## Settings

User settings are stored as JSON at:

```
%LOCALAPPDATA%\Anaheim_Electronics\settings.json
```

Logs are written alongside them in `%LOCALAPPDATA%\Anaheim_Electronics\logs`.

## Create a public-private key pair

1. Open a Visual Studio Developer Command Prompt.
1. Create the signing key.

    `sn -k signingkey.snk`
1. Extract the public key.

    `sn -p signingkey.snk publickey.snk`
1. Display the public key.

    `sn -tp publickey.snk`

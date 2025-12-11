# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```bash
# Debug build
dotnet build

# Release build (development/testing)
dotnet build -c Release
# Output: bin/Release/net8.0-windows/iCUERestarter.exe (requires DLL and JSON files)

# Release build (for distribution - single executable file)
dotnet publish -c Release -o publish
# Output: publish/iCUERestarter.exe (standalone, no additional files needed)
```

## Project Overview

iCUE Restarter is a Windows system tray application that restarts Corsair iCUE software with a single click. It solves the issue where iCUE loses keyboard connection after waking from sleep.

## Architecture

This is a .NET 8.0 Windows Forms application with three main components:

- **Program.cs**: Entry point with single-instance mutex enforcement
- **TrayApplicationContext.cs**: System tray UI logic, handles left-click (restart) and right-click context menu, manages iCUE process termination and restart
- **Settings.cs**: JSON-based settings persistence stored in `settings.json` alongside the executable

## Key Behavior

- Settings are reloaded on each restart action (hot-reload without app restart)
- iCUE process is killed, waits 3 seconds, then relaunched from configured path
- Default iCUE path: `C:\Program Files\Corsair\Corsair iCUE5 Software\iCUE.exe`

## User Guidelines

- リリースの際は、バイナリファイルを添付すること
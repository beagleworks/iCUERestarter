# iCUE Restarter

[日本語](README.md)

A system tray application to restart Corsair iCUE when it loses keyboard connection after waking from sleep.

## Problem

iCUE sometimes fails to recognize the keyboard after waking from sleep, causing lighting control to stop working. Normally, you need to manually close and restart iCUE to fix this.

## Solution

This app stays in the system tray and allows you to restart iCUE with a single click.

## Usage

1. Run `iCUERestarter.exe`
2. An icon appears in the system tray
3. **Left-click**: Restart iCUE immediately
4. **Right-click**: Show menu
   - "Restart iCUE"
   - "Open settings"
   - "Exit"

## Configuration

On first launch, a `settings.json` file is created. To change the iCUE path, right-click the tray icon and select "Open settings".

```json
{
  "IcuePath": "C:\\Program Files\\Corsair\\CORSAIR iCUE 5 Software\\iCUE.exe"
}
```

Restart the app after changing settings.

## Requirements

- Windows 10/11
- .NET 8.0 Runtime
- Corsair iCUE 5

## Build

```bash
dotnet build -c Release
```

Output: `bin/Release/net8.0-windows/iCUERestarter.exe`

## Run at Startup

To launch automatically when Windows starts:

1. Press `Win + R` to open "Run"
2. Type `shell:startup` and press Enter
3. Create a shortcut to `iCUERestarter.exe` in the opened folder

## License

MIT License

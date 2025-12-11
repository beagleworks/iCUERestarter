using System.Diagnostics;
using System.Reflection;

namespace iCUERestarter;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Settings _settings;
    private readonly ContextMenuStrip _contextMenu;
    private readonly Icon _icon;
    private bool _disposed;

    public TrayApplicationContext()
    {
        _settings = Settings.Load();

        _contextMenu = new ContextMenuStrip();
        _contextMenu.Items.Add("iCUE を再起動", null, OnRestart);
        _contextMenu.Items.Add("設定ファイルを開く", null, OnOpenSettings);
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add("終了", null, OnExit);

        _icon = LoadEmbeddedIcon() ?? SystemIcons.Application;

        _notifyIcon = new NotifyIcon
        {
            Icon = _icon,
            ContextMenuStrip = _contextMenu,
            Text = "iCUE Restarter",
            Visible = true
        };

        _notifyIcon.MouseClick += OnMouseClick;
    }

    private async void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            await RestartIcueAsync();
        }
    }

    private async void OnRestart(object? sender, EventArgs e)
    {
        await RestartIcueAsync();
    }

    private async Task RestartIcueAsync()
    {
        try
        {
            // iCUE プロセスを終了
            var processes = Process.GetProcessesByName("iCUE");
            foreach (var proc in processes)
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit(5000);
                }
                catch
                {
                    // プロセスが既に終了している場合は無視
                }
                finally
                {
                    proc.Dispose();
                }
            }

            // 少し待機してから再起動
            await Task.Delay(3000);

            // 設定を再読み込みして iCUE を起動
            var settings = Settings.Load();
            var icuePath = settings.IcuePath;
            if (File.Exists(icuePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = icuePath,
                    UseShellExecute = true
                });

                _notifyIcon.ShowBalloonTip(2000, "iCUE Restarter", "iCUE を再起動しました", ToolTipIcon.Info);
            }
            else
            {
                _notifyIcon.ShowBalloonTip(3000, "エラー", $"iCUE が見つかりません:\n{icuePath}", ToolTipIcon.Error);
            }
        }
        catch (Exception ex)
        {
            _notifyIcon.ShowBalloonTip(3000, "エラー", $"再起動に失敗しました:\n{ex.Message}", ToolTipIcon.Error);
        }
    }

    private void OnOpenSettings(object? sender, EventArgs e)
    {
        var settingsPath = Path.Combine(AppContext.BaseDirectory, "settings.json");
        if (!File.Exists(settingsPath))
        {
            _settings.Save();
        }

        Process.Start(new ProcessStartInfo
        {
            FileName = settingsPath,
            UseShellExecute = true
        });
    }

    private void OnExit(object? sender, EventArgs e)
    {
        Dispose();
        Application.Exit();
    }

    private static Icon? LoadEmbeddedIcon()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("iCUERestarter.ico.app.ico");
        return stream != null ? new Icon(stream) : null;
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _contextMenu.Dispose();
            _icon.Dispose();
        }
        _disposed = true;
        base.Dispose(disposing);
    }
}

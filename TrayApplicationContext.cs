using System.Diagnostics;
using System.Reflection;

namespace iCUERestarter;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Settings _settings;
    private readonly ContextMenuStrip _contextMenu;
    private readonly Icon _icon;
    private readonly bool _ownsIcon;
    private readonly bool _settingsRecovered;
    private readonly CancellationTokenSource _restartCts = new();
    private Task? _restartTask;
    private bool _disposed;

    public TrayApplicationContext()
    {
        _settings = Settings.Load(out _settingsRecovered);

        _contextMenu = new ContextMenuStrip();
        _contextMenu.Items.Add("iCUE を再起動", null, OnRestart);
        _contextMenu.Items.Add("設定ファイルを開く", null, OnOpenSettings);
        _contextMenu.Items.Add(new ToolStripSeparator());
        _contextMenu.Items.Add("終了", null, OnExit);

        var embeddedIcon = LoadEmbeddedIcon();
        _icon = embeddedIcon ?? SystemIcons.Application;
        _ownsIcon = embeddedIcon != null;

        _notifyIcon = new NotifyIcon
        {
            Icon = _icon,
            ContextMenuStrip = _contextMenu,
            Text = "iCUE Restarter",
            Visible = true
        };

        _notifyIcon.MouseClick += OnMouseClick;

        NotifySettingsRecovery(_settingsRecovered);
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            FireAndForgetRestart();
        }
    }

    private void OnRestart(object? sender, EventArgs e)
    {
        FireAndForgetRestart();
    }

    private async Task RestartIcueAsync(CancellationToken cancellationToken)
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
            await Task.Delay(3000, cancellationToken);

            // 設定を再読み込みして iCUE を起動
            var settings = Settings.Load(out var recovered);
            var icuePath = settings.IcuePath;
            NotifySettingsRecovery(recovered);
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
        catch (OperationCanceledException)
        {
            // ignore cancellation
        }
        catch (Exception ex)
        {
            ShowRestartError(ex);
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

    private void ShowSettingsRecoveredNotification()
    {
        _notifyIcon.ShowBalloonTip(3000, "iCUE Restarter", "設定ファイルを復元しました。必要に応じて再設定してください。", ToolTipIcon.Warning);
    }

    private void NotifySettingsRecovery(bool recovered)
    {
        if (recovered)
        {
            ShowSettingsRecoveredNotification();
        }
    }

    private void FireAndForgetRestart()
    {
        _restartTask = HandleRestartAsync();
    }

    private async Task HandleRestartAsync()
    {
        try
        {
            await RestartIcueAsync(_restartCts.Token);
        }
        catch (Exception ex)
        {
            if (_disposed) return;
            ShowRestartError(ex);
        }
    }

    private void ShowRestartError(Exception ex)
    {
        if (_disposed) return;
        Debug.WriteLine(ex);
        _notifyIcon.ShowBalloonTip(3000, "エラー", $"再起動に失敗しました:\n{ex.Message}", ToolTipIcon.Error);
    }

    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _restartCts.Cancel();
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _contextMenu.Dispose();
            if (_ownsIcon)
            {
                _icon.Dispose();
            }
            _restartCts.Dispose();
        }
        _disposed = true;
        base.Dispose(disposing);
    }
}

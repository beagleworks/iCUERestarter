using System.Diagnostics;

namespace iCUERestarter;

public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private readonly Settings _settings;

    public TrayApplicationContext()
    {
        _settings = Settings.Load();

        var contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("iCUE を再起動", null, OnRestart);
        contextMenu.Items.Add("設定ファイルを開く", null, OnOpenSettings);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add("終了", null, OnExit);

        _notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            ContextMenuStrip = contextMenu,
            Text = "iCUE Restarter",
            Visible = true
        };

        _notifyIcon.MouseClick += OnMouseClick;
    }

    private void OnMouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            RestartIcue();
        }
    }

    private void OnRestart(object? sender, EventArgs e)
    {
        RestartIcue();
    }

    private void RestartIcue()
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
            Thread.Sleep(1000);

            // iCUE を起動
            var icuePath = _settings.IcuePath;
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
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        Application.Exit();
    }
}

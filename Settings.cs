using System.Text.Json;
using System.Windows.Forms;

namespace iCUERestarter;

public class Settings
{
    private const string SettingsFileName = "settings.json";
    private static readonly string SettingsPath = Path.Combine(
        AppContext.BaseDirectory,
        SettingsFileName
    );

    public string IcuePath { get; set; } = @"C:\Program Files\Corsair\Corsair iCUE5 Software\iCUE.exe";

    public static Settings Load()
    {
        if (!File.Exists(SettingsPath))
        {
            var defaultSettings = new Settings();
            defaultSettings.Save();
            return defaultSettings;
        }

        try
        {
            var json = File.ReadAllText(SettingsPath);
            return JsonSerializer.Deserialize<Settings>(json) ?? new Settings();
        }
        catch
        {
            var fallback = new Settings();
            fallback.Save();
            MessageBox.Show("設定ファイルが壊れていたため、デフォルト設定に復元しました。", "iCUE Restarter", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return fallback;
        }
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(SettingsPath, json);
    }
}

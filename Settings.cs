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

    public static Settings Load(out bool recoveredFromCorruption)
    {
        recoveredFromCorruption = false;
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
            recoveredFromCorruption = true;
            return fallback;
        }
    }

    public static Settings Load()
    {
        return Load(out _);
    }

    public void Save()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(this, options);
        File.WriteAllText(SettingsPath, json);
    }
}

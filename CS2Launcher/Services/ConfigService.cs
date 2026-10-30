using System.IO;
using System.Text.Json;
using CS2Launcher.Models;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис управления конфигами CS2: чтение/запись autoexec.cfg,
/// сохранение настроек лаунчера в JSON.
/// </summary>
public class ConfigService
{
    private static readonly string AppDataDir =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CS2Launcher");

    private static readonly string LauncherSettingsPath = Path.Combine(AppDataDir, "settings.json");
    private static readonly string ApiKeyPath = Path.Combine(AppDataDir, "steam_api_key.txt");

    private static readonly string CS2_CfgSubPath =
        Path.Combine("steamapps", "common", "Counter-Strike Global Offensive",
                     "game", "csgo", "cfg");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    static ConfigService()
    {
        Directory.CreateDirectory(AppDataDir);
    }

    /// <summary>
    /// Сохраняет настройки лаунчера (CS2Config + LaunchOptions) в JSON.
    /// </summary>
    public void SaveSettings(CS2Config config, LaunchOptions launchOptions)
    {
        var payload = new
        {
            config.Sensitivity,
            config.CrosshairSize,
            config.CrosshairThickness,
            config.CrosshairGap,
            config.CrosshairColor,
            config.CrosshairOutline,
            config.CrosshairDot,
            config.Fov,
            config.ViewmodelFov,
            config.RawInput,
            config.NoTutorial,
            LaunchOptions = new
            {
                launchOptions.NoVid,
                launchOptions.NoJoy,
                launchOptions.High,
                launchOptions.Fullscreen,
                launchOptions.UseThreads,
                launchOptions.ThreadsValue,
                launchOptions.UseTickrate,
                launchOptions.TickrateValue,
                launchOptions.Console,
                launchOptions.NoBrowser,
                launchOptions.CustomArgs,
                launchOptions.ExecutablePath,
                launchOptions.UseSteamProtocol
            },
            LastUpdated = DateTime.Now
        };

        var json = JsonSerializer.Serialize(payload, JsonOpts);
        File.WriteAllText(LauncherSettingsPath, json);
    }

    /// <summary>
    /// Загружает настройки из JSON и применяет их к объектам.
    /// </summary>
    public (CS2Config config, LaunchOptions launch) LoadSettings()
    {
        var config = new CS2Config();
        var launch = new LaunchOptions();

        if (!File.Exists(LauncherSettingsPath))
            return (config, launch);

        try
        {
            var json = File.ReadAllText(LauncherSettingsPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (root.TryGetProperty("Sensitivity", out var v)) config.Sensitivity = v.GetDouble();
            if (root.TryGetProperty("CrosshairSize", out v)) config.CrosshairSize = v.GetInt32();
            if (root.TryGetProperty("CrosshairThickness", out v)) config.CrosshairThickness = v.GetInt32();
            if (root.TryGetProperty("CrosshairGap", out v)) config.CrosshairGap = v.GetInt32();
            if (root.TryGetProperty("CrosshairColor", out v)) config.CrosshairColor = v.GetString() ?? "#00FF00";
            if (root.TryGetProperty("CrosshairOutline", out v)) config.CrosshairOutline = v.GetBoolean();
            if (root.TryGetProperty("CrosshairDot", out v)) config.CrosshairDot = v.GetBoolean();
            if (root.TryGetProperty("Fov", out v)) config.Fov = v.GetInt32();
            if (root.TryGetProperty("ViewmodelFov", out v)) config.ViewmodelFov = v.GetInt32();
            if (root.TryGetProperty("RawInput", out v)) config.RawInput = v.GetBoolean();
            if (root.TryGetProperty("NoTutorial", out v)) config.NoTutorial = v.GetBoolean();

            if (root.TryGetProperty("LaunchOptions", out var lo))
            {
                if (lo.TryGetProperty("NoVid", out v)) launch.NoVid = v.GetBoolean();
                if (lo.TryGetProperty("NoJoy", out v)) launch.NoJoy = v.GetBoolean();
                if (lo.TryGetProperty("High", out v)) launch.High = v.GetBoolean();
                if (lo.TryGetProperty("Fullscreen", out v)) launch.Fullscreen = v.GetBoolean();
                if (lo.TryGetProperty("UseThreads", out v)) launch.UseThreads = v.GetBoolean();
                if (lo.TryGetProperty("ThreadsValue", out v)) launch.ThreadsValue = v.GetInt32();
                if (lo.TryGetProperty("UseTickrate", out v)) launch.UseTickrate = v.GetBoolean();
                if (lo.TryGetProperty("TickrateValue", out v)) launch.TickrateValue = v.GetInt32();
                if (lo.TryGetProperty("Console", out v)) launch.Console = v.GetBoolean();
                if (lo.TryGetProperty("NoBrowser", out v)) launch.NoBrowser = v.GetBoolean();
                if (lo.TryGetProperty("CustomArgs", out v)) launch.CustomArgs = v.GetString() ?? string.Empty;
                if (lo.TryGetProperty("ExecutablePath", out v)) launch.ExecutablePath = v.GetString() ?? string.Empty;
                if (lo.TryGetProperty("UseSteamProtocol", out v)) launch.UseSteamProtocol = v.GetBoolean();
            }
        }
        catch
        {
            // ignore — используем значения по умолчанию
        }

        return (config, launch);
    }

    /// <summary>
    /// Записывает сгенерированный autoexec.cfg в папку cfg CS2.
    /// </summary>
    public bool SaveAutoexec(CS2Config config, string? overrideSteamPath = null)
    {
        var steamPath = overrideSteamPath ?? DetectSteamPath();
        if (string.IsNullOrEmpty(steamPath))
            return false;

        var cfgDir = Path.Combine(steamPath, CS2_CfgSubPath);
        if (!Directory.Exists(cfgDir))
            Directory.CreateDirectory(cfgDir);

        var autoexecPath = Path.Combine(cfgDir, "autoexec.cfg");
        File.WriteAllText(autoexecPath, config.GenerateAutoexec());
        config.AutoexecContent = config.GenerateAutoexec();
        return true;
    }

    /// <summary>
    /// Загружает существующий autoexec.cfg (если есть).
    /// </summary>
    public string LoadAutoexec(string? overrideSteamPath = null)
    {
        var steamPath = overrideSteamPath ?? DetectSteamPath();
        if (string.IsNullOrEmpty(steamPath))
            return string.Empty;

        var autoexecPath = Path.Combine(steamPath, CS2_CfgSubPath, "autoexec.cfg");
        return File.Exists(autoexecPath) ? File.ReadAllText(autoexecPath) : string.Empty;
    }

    /// <summary>
    /// Сохраняет Steam API-ключ в файл (не в plaintext-настройках).
    /// </summary>
    public void SaveApiKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
        {
            if (File.Exists(ApiKeyPath)) File.Delete(ApiKeyPath);
            return;
        }
        File.WriteAllText(ApiKeyPath, key.Trim());
    }

    public string? LoadApiKey()
    {
        return File.Exists(ApiKeyPath) ? File.ReadAllText(ApiKeyPath).Trim() : null;
    }

    private string? DetectSteamPath()
    {
        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam")
        };

        foreach (var p in candidates)
            if (Directory.Exists(p))
                return p;

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            var reg = key?.GetValue("SteamPath")?.ToString();
            if (!string.IsNullOrEmpty(reg) && Directory.Exists(reg))
                return reg.Replace('/', '\\');
        }
        catch { /* ignore */ }

        return null;
    }
}

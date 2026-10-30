using System.Text;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models.ARCRaiders;

/// <summary>
/// Пресет графики ARC Raiders.
/// </summary>
public class GraphicsPreset : ObservableObject
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _hardwareTier = string.Empty;
    private string _accentColor = "#FF3B3B";

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string HardwareTier
    {
        get => _hardwareTier;
        set => SetProperty(ref _hardwareTier, value);
    }

    public string AccentColor
    {
        get => _accentColor;
        set => SetProperty(ref _accentColor, value);
    }

    // Graphics settings
    public int ResolutionQuality { get; set; } = 100;
    public int ViewDistance { get; set; } = 7;
    public int AntiAliasing { get; set; } = 2;
    public int ShadowQuality { get; set; } = 3;
    public int TextureQuality { get; set; } = 3;
    public int EffectsQuality { get; set; } = 3;
    public int FoliageQuality { get; set; } = 3;
    public int ShadingQuality { get; set; } = 3;

    // Performance
    public int FrameRateLimit { get; set; } = 0;
    public bool VSync { get; set; } = true;
    public bool Fullscreen { get; set; } = true;
    public bool UseAllAvailableCores { get; set; } = true;
    public bool NoTextureStreaming { get; set; } = false;

    /// <summary>
    /// Возвращает пресет для слабых ПК (GTX 1650 / 1660 Super, 8 ГБ ОЗУ).
    /// </summary>
    public static GraphicsPreset LowSpec() => new()
    {
        Name = "Слабый ПК",
        HardwareTier = "GTX 1650 / 1660 Super",
        Description = "Максимальная производительность, ~60 FPS на минималках",
        AccentColor = "#3DD68C",
        ResolutionQuality = 70,
        ViewDistance = 3,
        AntiAliasing = 0,
        ShadowQuality = 1,
        TextureQuality = 1,
        EffectsQuality = 1,
        FoliageQuality = 1,
        ShadingQuality = 1,
        FrameRateLimit = 60,
        VSync = false,
        Fullscreen = true,
        UseAllAvailableCores = true,
        NoTextureStreaming = false
    };

    /// <summary>
    /// Пресет для среднего ПК (RTX 3060 / 4060, 16 ГБ ОЗУ).
    /// </summary>
    public static GraphicsPreset MediumSpec() => new()
    {
        Name = "Средний ПК",
        HardwareTier = "RTX 3060 / 4060 / 6600 XT",
        Description = "Баланс графики и FPS, ~80-100 FPS",
        AccentColor = "#FFB454",
        ResolutionQuality = 85,
        ViewDistance = 6,
        AntiAliasing = 2,
        ShadowQuality = 3,
        TextureQuality = 3,
        EffectsQuality = 3,
        FoliageQuality = 3,
        ShadingQuality = 3,
        FrameRateLimit = 120,
        VSync = false,
        Fullscreen = true,
        UseAllAvailableCores = true,
        NoTextureStreaming = false
    };

    /// <summary>
    /// Пресет для мощного ПК (RTX 3070+ / 4070+, 32 ГБ ОЗУ).
    /// </summary>
    public static GraphicsPreset HighSpec() => new()
    {
        Name = "Высокий ПК",
        HardwareTier = "RTX 3070+ / 4070+",
        Description = "Максимум графики, 144 FPS без проблем",
        AccentColor = "#FF3B3B",
        ResolutionQuality = 100,
        ViewDistance = 8,
        AntiAliasing = 4,
        ShadowQuality = 5,
        TextureQuality = 5,
        EffectsQuality = 5,
        FoliageQuality = 5,
        ShadingQuality = 5,
        FrameRateLimit = 144,
        VSync = false,
        Fullscreen = true,
        UseAllAvailableCores = true,
        NoTextureStreaming = true
    };

    /// <summary>
    /// Генерирует содержимое GameUserSettings.ini на основе пресета.
    /// </summary>
    public string GenerateIniContent()
    {
        var sb = new StringBuilder();
        sb.AppendLine("[/Script/ARCGame.ARCGameUserSettings]");
        sb.AppendLine("ResolutionQuality=" + ResolutionQuality);
        sb.AppendLine("ViewDistanceQuality=" + ViewDistance);
        sb.AppendLine("AntiAliasingQuality=" + AntiAliasing);
        sb.AppendLine("ShadowQuality=" + ShadowQuality);
        sb.AppendLine("TextureQuality=" + TextureQuality);
        sb.AppendLine("EffectsQuality=" + EffectsQuality);
        sb.AppendLine("FoliageQuality=" + FoliageQuality);
        sb.AppendLine("ShadingQuality=" + ShadingQuality);
        sb.AppendLine("FrameRateLimit=" + FrameRateLimit);
        sb.AppendLine("bUseVSync=" + (VSync ? "True" : "False"));
        sb.AppendLine("FullscreenMode=" + (Fullscreen ? "1" : "2"));
        sb.AppendLine();
        sb.AppendLine("[SystemSettings]");
        sb.AppendLine("r.TextureStreaming=" + (NoTextureStreaming ? "False" : "True"));
        return sb.ToString();
    }
}

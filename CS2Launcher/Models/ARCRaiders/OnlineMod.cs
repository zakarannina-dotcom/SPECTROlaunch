using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models.ARCRaiders;

/// <summary>
/// Мод в каталоге (для браузера модов).
/// </summary>
public class OnlineMod : ObservableObject
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _author = string.Empty;
    private string _category = "Texture";
    private string _version = "1.0";
    private string _formattedSize = "—";
    private int _downloads;
    private double _rating = 5.0;
    private string _previewUrl = string.Empty;
    private string _downloadUrl = string.Empty;
    private bool _isInstalled;
    private bool _isDownloading;
    private int _downloadProgress;

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

    public string Author
    {
        get => _author;
        set => SetProperty(ref _author, value);
    }

    public string Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string Version
    {
        get => _version;
        set => SetProperty(ref _version, value);
    }

    public string FormattedSize
    {
        get => _formattedSize;
        set => SetProperty(ref _formattedSize, value);
    }

    public int Downloads
    {
        get => _downloads;
        set => SetProperty(ref _downloads, value);
    }

    public string FormattedDownloads => Downloads >= 1000 ? $"{Downloads / 1000.0:F1}K" : Downloads.ToString();

    public double Rating
    {
        get => _rating;
        set => SetProperty(ref _rating, value);
    }

    public string Stars => GetStars(Rating);

    public string PreviewUrl
    {
        get => _previewUrl;
        set => SetProperty(ref _previewUrl, value);
    }

    public string DownloadUrl
    {
        get => _downloadUrl;
        set => SetProperty(ref _downloadUrl, value);
    }

    public bool IsInstalled
    {
        get => _isInstalled;
        set => SetProperty(ref _isInstalled, value);
    }

    public bool IsDownloading
    {
        get => _isDownloading;
        set => SetProperty(ref _isDownloading, value);
    }

    public int DownloadProgress
    {
        get => _downloadProgress;
        set => SetProperty(ref _downloadProgress, value);
    }

    /// <summary>Цвет категории (для бейджа).</summary>
    public string CategoryColor => Category switch
    {
        "Texture" => "#FF3B3B",
        "Sound" => "#FFB454",
        "UI" => "#00AAFF",
        "Gameplay" => "#3DD68C",
        "Map" => "#FF00AA",
        _ => "#A0A0A8"
    };

    public string InstallStatus => IsInstalled ? "✓ Установлен" : (IsDownloading ? "Скачивание..." : "Скачать");

    private static string GetStars(double rating)
    {
        var full = (int)Math.Floor(rating);
        var hasHalf = rating - full >= 0.5;
        var sb = new StringBuilder();
        for (int i = 0; i < full; i++) sb.Append("★");
        if (hasHalf) sb.Append("⯨");
        for (int i = full + (hasHalf ? 1 : 0); i < 5; i++) sb.Append("☆");
        return sb.ToString();
    }
}

/// <summary>
/// Каталог модов для ARC Raiders (демо-данные).
/// В реальном приложении подгружалось бы с сервера.
/// </summary>
public static class OnlineModCatalog
{
    public static List<OnlineMod> GetPopularMods()
    {
        return new List<OnlineMod>
        {
            new()
            {
                Name = "HD Texture Pack",
                Description = "Полная замена текстур на HD-версии (4K). Кровь, металл, окружение.",
                Author = "ModderPro",
                Category = "Texture",
                Version = "2.4",
                FormattedSize = "342 MB",
                Downloads = 18543,
                Rating = 4.8,
                DownloadUrl = "https://example.com/mods/hd-texture-pack.pak"
            },
            new()
            {
                Name = "Realistic Lighting",
                Description = "Реалистичное освещение без потери FPS. Тени, блики, ambient.",
                Author = "LightMaster",
                Category = "Texture",
                Version = "1.8",
                FormattedSize = "78 MB",
                Downloads = 12432,
                Rating = 4.6,
                DownloadUrl = "https://example.com/mods/realistic-lighting.pak"
            },
            new()
            {
                Name = "Immersive Sounds",
                Description = "Полная замена звуковых эффектов. Шаги, выстрелы, окружение.",
                Author = "SoundForge",
                Category = "Sound",
                Version = "3.1",
                FormattedSize = "156 MB",
                Downloads = 9821,
                Rating = 4.9,
                DownloadUrl = "https://example.com/mods/immersive-sounds.pak"
            },
            new()
            {
                Name = "Minimal UI",
                Description = "Минималистичный интерфейс: меньше отвлекает, больше фокуса на игре.",
                Author = "UIGuru",
                Category = "UI",
                Version = "1.2",
                FormattedSize = "12 MB",
                Downloads = 7654,
                Rating = 4.4,
                DownloadUrl = "https://example.com/mods/minimal-ui.pak"
            },
            new()
            {
                Name = "Hardcore Mode",
                Description = "Увеличенный урон, меньше хила, сложнее AI. Для настоящих профи.",
                Author = "HardcoreDev",
                Category = "Gameplay",
                Version = "0.9",
                FormattedSize = "45 MB",
                Downloads = 5432,
                Rating = 4.2,
                DownloadUrl = "https://example.com/mods/hardcore-mode.pak"
            },
            new()
            {
                Name = "Custom Map: Underground",
                Description = "Новая карта — подземный бункер с тайниками и ловушками.",
                Author = "MapSmith",
                Category = "Map",
                Version = "1.0",
                FormattedSize = "210 MB",
                Downloads = 4321,
                Rating = 4.7,
                DownloadUrl = "https://example.com/mods/underground-map.pak"
            },
            new()
            {
                Name = "4K Skin Pack",
                Description = "HD-скины для всего оружия. Каждая деталь в высоком разрешении.",
                Author = "SkinArt",
                Category = "Texture",
                Version = "5.0",
                FormattedSize = "289 MB",
                Downloads = 15234,
                Rating = 4.5,
                DownloadUrl = "https://example.com/mods/4k-skin-pack.pak"
            },
            new()
            {
                Name = "Dynamic Weather",
                Description = "Динамическая погода: дождь, туман, снег. Атмосфера на максимум.",
                Author = "WeatherFX",
                Category = "Texture",
                Version = "2.0",
                FormattedSize = "98 MB",
                Downloads = 8765,
                Rating = 4.3,
                DownloadUrl = "https://example.com/mods/dynamic-weather.pak"
            }
        };
    }
}

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models.ARCRaiders;

/// <summary>
/// Модель мода на текстуры (UE5 .pak файл).
/// </summary>
public class TextureMod : ObservableObject
{
    private string _name = string.Empty;
    private string _description = string.Empty;
    private string _author = string.Empty;
    private string _version = "1.0";
    private string _filePath = string.Empty;
    private long _fileSize;
    private bool _isEnabled;
    private ModCategory _category = ModCategory.Texture;

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

    public string Version
    {
        get => _version;
        set => SetProperty(ref _version, value);
    }

    public string FilePath
    {
        get => _filePath;
        set => SetProperty(ref _filePath, value);
    }

    public long FileSize
    {
        get => _fileSize;
        set => SetProperty(ref _fileSize, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => SetProperty(ref _isEnabled, value);
    }

    public ModCategory Category
    {
        get => _category;
        set => SetProperty(ref _category, value);
    }

    public string FormattedSize => FormatSize(FileSize);

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }
}

public enum ModCategory
{
    Texture,
    Sound,
    UI,
    Gameplay,
    Map,
    Other
}

/// <summary>
/// Каталог модов — управляет коллекцией установленных модов.
/// </summary>
public class ModCatalog : ObservableObject
{
    public ObservableCollection<TextureMod> Mods { get; } = new();

    public int TotalCount => Mods.Count;
    public int EnabledCount => Mods.Count(m => m.IsEnabled);
    public long TotalSizeBytes => Mods.Sum(m => m.FileSize);
    public string TotalSizeFormatted => FormatSize(TotalSizeBytes);

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024) return $"{bytes} B";
        if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
        if (bytes < 1024 * 1024 * 1024) return $"{bytes / (1024.0 * 1024):F1} MB";
        return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
    }

    public void RefreshStats()
    {
        OnPropertyChanged(nameof(TotalCount));
        OnPropertyChanged(nameof(EnabledCount));
        OnPropertyChanged(nameof(TotalSizeBytes));
        OnPropertyChanged(nameof(TotalSizeFormatted));
    }
}

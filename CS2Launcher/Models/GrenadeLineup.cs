using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models;

/// <summary>Тип гранаты.</summary>
public enum GrenadeType
{
    Smoke,
    Flash,
    HE,
    Molotov,
    Decoy
}

/// <summary>Сторона/роль раскидки.</summary>
public enum LineupSide
{
    /// <summary>Атака (T-side).</summary>
    Attack,
    /// <summary>Защита (CT-side).</summary>
    Defense,
    /// <summary>Универсальная.</summary>
    Both
}

/// <summary>Сложность раскидки.</summary>
public enum LineupDifficulty
{
    Easy,
    Medium,
    Hard
}

/// <summary>
/// Описание одной раскидки гранаты на карте.
/// </summary>
public class GrenadeLineup : ObservableObject
{
    private string _name = string.Empty;
    private string _mapName = string.Empty;
    private GrenadeType _grenadeType;
    private LineupSide _side;
    private LineupDifficulty _difficulty = LineupDifficulty.Medium;
    private string _standPosition = string.Empty;
    private string _aimLandmark = string.Empty;
    private bool _jumpThrow;
    private bool _leftClick = true;
    private bool _rightClick;
    private string _description = string.Empty;
    private string _videoUrl = string.Empty;
    private string _thumbnailUrl = string.Empty;
    private bool _isFavorite;

    /// <summary>Название раскидки (например, "A Site Smoke from CT").</summary>
    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    /// <summary>Имя карты (de_dust2, de_mirage, и т.д.).</summary>
    public string MapName
    {
        get => _mapName;
        set => SetProperty(ref _mapName, value);
    }

    public GrenadeType GrenadeType
    {
        get => _grenadeType;
        set => SetProperty(ref _grenadeType, value);
    }

    public LineupSide Side
    {
        get => _side;
        set => SetProperty(ref _side, value);
    }

    public LineupDifficulty Difficulty
    {
        get => _difficulty;
        set => SetProperty(ref _difficulty, value);
    }

    /// <summary>Где встать (точка становления).</summary>
    public string StandPosition
    {
        get => _standPosition;
        set => SetProperty(ref _standPosition, value);
    }

    /// <summary>Куда смотреть / на что прицелиться.</summary>
    public string AimLandmark
    {
        get => _aimLandmark;
        set => SetProperty(ref _aimLandmark, value);
    }

    /// <summary>Нужен ли jumpthrow (прыжок + отпускание).</summary>
    public bool JumpThrow
    {
        get => _jumpThrow;
        set => SetProperty(ref _jumpThrow, value);
    }

    public bool LeftClick
    {
        get => _leftClick;
        set => SetProperty(ref _leftClick, value);
    }

    public bool RightClick
    {
        get => _rightClick;
        set => SetProperty(ref _rightClick, value);
    }

    /// <summary>Краткое описание результата.</summary>
    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    /// <summary>Ссылка на видео (YouTube).</summary>
    public string VideoUrl
    {
        get => _videoUrl;
        set => SetProperty(ref _videoUrl, value);
    }

    /// <summary>URL превью (скриншот точки).</summary>
    public string ThumbnailUrl
    {
        get => _thumbnailUrl;
        set => SetProperty(ref _thumbnailUrl, value);
    }

    /// <summary>Пошаговая инструкция.</summary>
    public ObservableCollection<string> Steps { get; } = new();

    /// <summary>Теги для поиска.</summary>
    public ObservableCollection<string> Tags { get; } = new();

    public bool IsFavorite
    {
        get => _isFavorite;
        set => SetProperty(ref _isFavorite, value);
    }

    // === Отображаемые строки ===

    public string GrenadeTypeDisplay => GrenadeType switch
    {
        GrenadeType.Smoke => "💨 Smoke",
        GrenadeType.Flash => "⚡ Flash",
        GrenadeType.HE => "💥 HE",
        GrenadeType.Molotov => "🔥 Molotov",
        GrenadeType.Decoy => "🎭 Decoy",
        _ => GrenadeType.ToString()
    };

    public string GrenadeEmoji => GrenadeType switch
    {
        GrenadeType.Smoke => "💨",
        GrenadeType.Flash => "⚡",
        GrenadeType.HE => "💥",
        GrenadeType.Molotov => "🔥",
        GrenadeType.Decoy => "🎭",
        _ => "💣"
    };

    public string SideDisplay => Side switch
    {
        LineupSide.Attack => "T",
        LineupSide.Defense => "CT",
        LineupSide.Both => "T/CT",
        _ => Side.ToString()
    };

    public string SideColor => Side switch
    {
        LineupSide.Attack => "#FFB454",
        LineupSide.Defense => "#3DD68C",
        LineupSide.Both => "#A0A0A8",
        _ => "#A0A0A8"
    };

    public string DifficultyDisplay => Difficulty switch
    {
        LineupDifficulty.Easy => "Лёгкая",
        LineupDifficulty.Medium => "Средняя",
        LineupDifficulty.Hard => "Сложная",
        _ => Difficulty.ToString()
    };

    public string DifficultyColor => Difficulty switch
    {
        LineupDifficulty.Easy => "#3DD68C",
        LineupDifficulty.Medium => "#FFB454",
        LineupDifficulty.Hard => "#FF3B3B",
        _ => "#A0A0A8"
    };

    public string ClickInstruction
    {
        get
        {
            if (LeftClick && RightClick) return "ЛКМ + ПКМ";
            if (RightClick) return "ПКМ (drop)";
            return "ЛКМ";
        }
    }

    public string JumpThrowDisplay => JumpThrow ? "🦘 Jumpthrow" : "🎯 Standing";
}

/// <summary>
/// Информация о карте CS2.
/// </summary>
public class CS2MapInfo
{
    public string Name { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Emoji { get; set; } = "🗺️";
    public string Description { get; set; } = string.Empty;

    public static List<CS2MapInfo> GetActiveDutyMaps()
    {
        return new List<CS2MapInfo>
        {
            new() { Name = "de_dust2", DisplayName = "Dust II", Emoji = "🏜️", Description = "Классическая карта" },
            new() { Name = "de_mirage", DisplayName = "Mirage", Emoji = "🕌", Description = "Пустынный город" },
            new() { Name = "de_inferno", DisplayName = "Inferno", Emoji = "🌋", Description = "Италия, узкие улочки" },
            new() { Name = "de_nuke", DisplayName = "Nuke", Emoji = "☢️", Description = "АЭС, два уровня" },
            new() { Name = "de_overpass", DisplayName = "Overpass", Emoji = "🌉", Description = "Мост под дорогой" },
            new() { Name = "de_ancient", DisplayName = "Ancient", Emoji = "🏛️", Description = "Древние руины" },
            new() { Name = "de_anubis", DisplayName = "Anubis", Emoji = "🏺", Description = "Египетский храм" },
            new() { Name = "de_vertigo", DisplayName = "Vertigo", Emoji = "🏢", Description = "Небоскрёб" },
            new() { Name = "de_train", DisplayName = "Train", Emoji = "🚂", Description = "Ж/д станция (вернулась!)" }
        };
    }
}

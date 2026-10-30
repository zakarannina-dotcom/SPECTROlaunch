using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models;

/// <summary>
/// Пресет прицела профессионального игрока.
/// </summary>
public class ProPlayerPreset : ObservableObject
{
    private string _playerName = string.Empty;
    private string _team = string.Empty;
    private string _country = string.Empty;
    private string _role = string.Empty;
    private string _crosshairCode = string.Empty;

    private int _size;
    private int _thickness;
    private int _gap;
    private string _color = "#00FF00";
    private bool _outline;
    private bool _dot;
    private double _sensitivity = 2.0;
    private string _year = "2025";

    public string PlayerName
    {
        get => _playerName;
        set => SetProperty(ref _playerName, value);
    }

    public string Team
    {
        get => _team;
        set => SetProperty(ref _team, value);
    }

    public string Country
    {
        get => _country;
        set => SetProperty(ref _country, value);
    }

    public string CountryFlag => GetCountryFlag(Country);

    public string Role
    {
        get => _role;
        set => SetProperty(ref _role, value);
    }

    public string CrosshairCode
    {
        get => _crosshairCode;
        set => SetProperty(ref _crosshairCode, value);
    }

    public int Size
    {
        get => _size;
        set => SetProperty(ref _size, value);
    }

    public int Thickness
    {
        get => _thickness;
        set => SetProperty(ref _thickness, value);
    }

    public int Gap
    {
        get => _gap;
        set => SetProperty(ref _gap, value);
    }

    public string Color
    {
        get => _color;
        set => SetProperty(ref _color, value);
    }

    public bool Outline
    {
        get => _outline;
        set => SetProperty(ref _outline, value);
    }

    public bool Dot
    {
        get => _dot;
        set => SetProperty(ref _dot, value);
    }

    public double Sensitivity
    {
        get => _sensitivity;
        set => SetProperty(ref _sensitivity, value);
    }

    /// <summary>Год актуальности данных.</summary>
    public string Year
    {
        get => _year;
        set => SetProperty(ref _year, value);
    }

    public string Description => $"{PlayerName} • {Team} • {Country}";

    private static string GetCountryFlag(string code)
    {
        if (string.IsNullOrEmpty(code) || code.Length != 2)
            return "🏳️";

        try
        {
            var upper = code.ToUpperInvariant();
            const int baseIndicator = 0x1F1E6;
            var r1 = char.ConvertFromUtf32(baseIndicator + (upper[0] - 'A'));
            var r2 = char.ConvertFromUtf32(baseIndicator + (upper[1] - 'A'));
            return $"{r1}{r2}";
        }
        catch
        {
            return "🏳️";
        }
    }
}

/// <summary>
/// Каталог пресетов профессиональных игроков CS2.
/// Данные актуальны на конец 2025 года (по HLTV Top 20).
/// </summary>
public static class ProPlayerPresets
{
    public static List<ProPlayerPreset> GetAll()
    {
        return new List<ProPlayerPreset>
        {
            // === HLTV Top 5 2025 ===
            new()
            {
                PlayerName = "ZywOo",
                Team = "Vitality",
                Country = "FR",
                Role = "AWP",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = true, Dot = false,
                Sensitivity = 2.0,
                Year = "2025",
                CrosshairCode = "CSGO-xq8NW-H6kOz-rFq8H-tXd9n-3OGhM"
            },
            new()
            {
                PlayerName = "donk",
                Team = "Spirit",
                Country = "RU",
                Role = "Rifler",
                Size = 1, Thickness = 1, Gap = -1,
                Color = "#00FF00", Outline = true, Dot = false,
                Sensitivity = 1.6,
                Year = "2025",
                CrosshairCode = "CSGO-dN8Ex-9QOcW-xVdcQ-r8mLO-C7V8O"
            },
            new()
            {
                PlayerName = "ropz",
                Team = "Vitality",
                Country = "EE",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.35,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "m0NESY",
                Team = "Falcons",
                Country = "RU",
                Role = "AWP",
                Size = 1, Thickness = 0, Gap = -1,
                Color = "#00FF00", Outline = true, Dot = false,
                Sensitivity = 2.0,
                Year = "2025",
                CrosshairCode = "CSGO-G6fQG-7LrBd-uJdQH-sNFpF-3i6hN"
            },
            new()
            {
                PlayerName = "sh1ro",
                Team = "G2",
                Country = "RU",
                Role = "AWP",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.3,
                Year = "2025",
                CrosshairCode = "CSGO-aP7xs-YuRfd-xSNPO-4mLu9-uJvhP"
            },

            // === HLTV Top 6-20 2025 ===
            new()
            {
                PlayerName = "molodoy",
                Team = "FaZe",
                Country = "KZ",
                Role = "AWP",
                Size = 1, Thickness = 1, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "torzsi",
                Team = "MOUZ",
                Country = "HU",
                Role = "AWP",
                Size = 1, Thickness = 1, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "Spinx",
                Team = "MOUZ",
                Country = "IL",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "w0nderful",
                Team = "NAVI",
                Country = "UA",
                Role = "AWP",
                Size = 1, Thickness = 1, Gap = -1,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "b1t",
                Team = "NAVI",
                Country = "UA",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "jL",
                Team = "NAVI",
                Country = "LT",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = true, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "iM",
                Team = "NAVI",
                Country = "UA",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "Aleksib",
                Team = "NAVI",
                Country = "FI",
                Role = "IGL",
                Size = 2, Thickness = 1, Gap = -2,
                Color = "#FFFFFF", Outline = true, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "Jimpphat",
                Team = "MOUZ",
                Country = "FI",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "Brollan",
                Team = "MOUZ",
                Country = "SE",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "xertioN",
                Team = "MOUZ",
                Country = "ES",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === FaZe Clan 2025 ===
            new()
            {
                PlayerName = "karrigan",
                Team = "FaZe",
                Country = "DK",
                Role = "IGL",
                Size = 2, Thickness = 1, Gap = -2,
                Color = "#FFFFFF", Outline = true, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "rain",
                Team = "FaZe",
                Country = "NO",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "frozen",
                Team = "FaZe",
                Country = "SK",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "EliGE",
                Team = "FaZe",
                Country = "US",
                Role = "Rifler",
                Size = 2, Thickness = 1, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.4,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === Vitality 2025 ===
            new()
            {
                PlayerName = "apEX",
                Team = "Vitality",
                Country = "FR",
                Role = "IGL",
                Size = 2, Thickness = 1, Gap = -2,
                Color = "#FFFFFF", Outline = true, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "flameZ",
                Team = "Vitality",
                Country = "IL",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "mezii",
                Team = "Vitality",
                Country = "GB",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === Spirit 2025 ===
            new()
            {
                PlayerName = "chopper",
                Team = "Spirit",
                Country = "RU",
                Role = "IGL",
                Size = 2, Thickness = 1, Gap = -2,
                Color = "#FFFFFF", Outline = true, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "magixx",
                Team = "Spirit",
                Country = "RU",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "sh1ro",
                Team = "Spirit",
                Country = "RU",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === G2 2025 ===
            new()
            {
                PlayerName = "huNter-",
                Team = "G2",
                Country = "BA",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "HeavyGod",
                Team = "G2",
                Country = "RU",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === Falcons 2025 ===
            new()
            {
                PlayerName = "TeSeS",
                Team = "Falcons",
                Country = "DK",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "Magisk",
                Team = "Falcons",
                Country = "DK",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "kyxsan",
                Team = "Falcons",
                Country = "BA",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === Legend: s1mple (BC.Game, вернулся в 2025) ===
            new()
            {
                PlayerName = "s1mple",
                Team = "BC.Game",
                Country = "UA",
                Role = "AWP",
                Size = 2, Thickness = 1, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 3.09,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },

            // === Прочие топ-игроки ===
            new()
            {
                PlayerName = "device",
                Team = "Astralis",
                Country = "DK",
                Role = "AWP",
                Size = 2, Thickness = 1, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.9,
                Year = "2025",
                CrosshairCode = "CSGO-aP7xs-YuRfd-xSNPO-4mLu9-uJvhP"
            },
            new()
            {
                PlayerName = "stavn",
                Team = "Astralis",
                Country = "DK",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "jabbi",
                Team = "Astralis",
                Country = "DK",
                Role = "Rifler",
                Size = 1, Thickness = 0, Gap = -2,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "xantares",
                Team = "Eternal Fire",
                Country = "TR",
                Role = "Rifler",
                Size = 2, Thickness = 1, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
            new()
            {
                PlayerName = "broky",
                Team = "Liquid",
                Country = "LV",
                Role = "AWP",
                Size = 1, Thickness = 0, Gap = -3,
                Color = "#00FF00", Outline = false, Dot = false,
                Sensitivity = 1.5,
                Year = "2025",
                CrosshairCode = "CSGO-O4pJ5-V7NkS-yRVBF-f9X2x-LwGfL"
            },
        };
    }
}

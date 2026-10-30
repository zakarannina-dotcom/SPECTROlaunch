using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace CS2Launcher.ViewModels;

/// <summary>
/// Конвертер bool → Visibility (true = Visible).
/// </summary>
public class BoolToVisConverter : MarkupExtension, IValueConverter
{
    public static readonly BoolToVisConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Visible;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер bool → Visibility (true = Collapsed).
/// </summary>
public class InverseBoolToVisConverter : MarkupExtension, IValueConverter
{
    public static readonly InverseBoolToVisConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is Visibility v && v == Visibility.Collapsed;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: возвращает "Active", если текущий ключ равен параметру.
/// </summary>
public class NavActiveConverter : MarkupExtension, IValueConverter
{
    public static readonly NavActiveConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var current = value?.ToString() ?? string.Empty;
        var expected = parameter?.ToString() ?? string.Empty;
        return string.Equals(current, expected, StringComparison.OrdinalIgnoreCase) ? "Active" : "Inactive";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: из строки имени берёт первые буквы для аватара.
/// </summary>
public class InitialsConverter : MarkupExtension, IValueConverter
{
    public static readonly InitialsConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var name = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(name))
            return "?";

        var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 1)
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();

        return (parts[0][0].ToString() + parts[1][0].ToString()).ToUpper();
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер bool → "Active"/"Inactive" для булевых свойств (например IsRunning).
/// </summary>
public class BoolToActiveConverter : MarkupExtension, IValueConverter
{
    public static readonly BoolToActiveConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b && b ? "Active" : "Inactive";

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is string s && s == "Active";

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер MatchResult → Visibility (для цветовых меток).
/// </summary>
public class ResultToColorConverter : MarkupExtension, IValueConverter
{
    public static readonly ResultToColorConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CS2Launcher.Models.MatchResult r)
        {
            return r switch
            {
                CS2Launcher.Models.MatchResult.Win => "#3DD68C",
                CS2Launcher.Models.MatchResult.Loss => "#FF3B3B",
                CS2Launcher.Models.MatchResult.Draw => "#FFB454",
                _ => "#A0A0A8"
            };
        }
        return "#A0A0A8";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: инвертирует булево значение.
/// </summary>
public class InverseBoolConverter : MarkupExtension, IValueConverter
{
    public static readonly InverseBoolConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : false;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => value is bool b ? !b : false;

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер строки/числа в строку с суффиксом (например "12 → 12ч").
/// </summary>
public class SuffixConverter : MarkupExtension, IValueConverter
{
    public static readonly SuffixConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var suffix = parameter?.ToString() ?? string.Empty;
        return value == null ? string.Empty : $"{value}{suffix}";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: число → форматированная строка (например, 1000 → "1 000").
/// </summary>
public class NumberFormatConverter : MarkupExtension, IValueConverter
{
    public static readonly NumberFormatConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value == null) return "0";
        if (value is double d) return d.ToString("N1", CultureInfo.InvariantCulture);
        if (value is int i) return i.ToString("N0", CultureInfo.InvariantCulture);
        return value.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер MatchResult → локализованный текст.
/// </summary>
public class MatchResultToTextConverter : MarkupExtension, IValueConverter
{
    public static readonly MatchResultToTextConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is CS2Launcher.Models.MatchResult r)
        {
            var key = r switch
            {
                CS2Launcher.Models.MatchResult.Win => "ResultWin",
                CS2Launcher.Models.MatchResult.Loss => "ResultLoss",
                CS2Launcher.Models.MatchResult.Draw => "ResultDraw",
                _ => string.Empty
            };

            return System.Windows.Application.Current?.TryFindResource(key)?.ToString() ?? r.ToString();
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер OverallStats → список объектов StatCard для отображения сетки.
/// </summary>
public class OverallToStatCardsConverter : MarkupExtension, IValueConverter
{
    public static readonly OverallToStatCardsConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is not CS2Launcher.Models.OverallStats stats)
            return new List<StatCard>();

        return new List<StatCard>
        {
            new("Всего матчей", stats.TotalMatches.ToString("N0"), "#F5F5F7"),
            new("Побед", stats.Wins.ToString("N0"), "#3DD68C"),
            new("Поражений", stats.Losses.ToString("N0"), "#FF3B3B"),
            new("Винрейт", stats.WinRate.ToString("0.0") + "%", "#FFB454"),
            new("Всего убийств", stats.TotalKills.ToString("N0"), "#F5F5F7"),
            new("Всего смертей", stats.TotalDeaths.ToString("N0"), "#F5F5F7"),
            new("K/D", stats.OverallKDR.ToString("0.00"), "#FF3B3B"),
            new("Хедшоты", stats.HeadshotPercent.ToString("0.0") + "%", "#FFB454"),
            new("Часов сыграно", stats.HoursPlayed.ToString("0") + "h", "#F5F5F7"),
            new("Ранг", "Gold Nova II", "#FFB454"),
            new("MVP Avg", (stats.TotalMatches == 0 ? 0 : stats.TotalKills / Math.Max(1, stats.TotalMatches / 4)).ToString("0.0"), "#FF3B3B"),
            new("ADR Avg", (stats.TotalMatches == 0 ? 0 : stats.TotalKills * 100.0 / Math.Max(1, stats.TotalMatches * 30)).ToString("0"), "#F5F5F7")
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер string → SolidColorBrush (для биндинга цвета из строки "#FF3B3B").
/// </summary>
public class StringToBrushConverter : MarkupExtension, IValueConverter
{
    public static readonly StringToBrushConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = value?.ToString() ?? string.Empty;
        try
        {
            var color = (Color)ColorConverter.ConvertFromString(s);
            return new SolidColorBrush(color);
        }
        catch
        {
            return new SolidColorBrush(Colors.White);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// DTO для отображения карточки статистики.
/// </summary>
public class StatCard
{
    public string Label { get; }
    public string Value { get; }
    public string Color { get; }

    public StatCard(string label, string value, string color)
    {
        Label = label;
        Value = value;
        Color = color;
    }
}

/// <summary>
/// Конвертер bool → Color (по параметру: Green / Red / Yellow).
/// </summary>
public class BoolToColorConverter : MarkupExtension, IValueConverter
{
    public static readonly BoolToColorConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var b = value is bool bb && bb;
        var colorHint = parameter?.ToString() ?? "Green";

        return (b, colorHint) switch
        {
            (true, "Green") => Color.FromRgb(0x3D, 0xD6, 0x8C),
            (true, "Red") => Color.FromRgb(0xFF, 0x3B, 0x3B),
            (true, "Yellow") => Color.FromRgb(0xFF, 0xB4, 0x54),
            (false, "Green") => Color.FromRgb(0x5C, 0x5C, 0x66),
            (false, "Red") => Color.FromRgb(0xFF, 0x3B, 0x3B),
            (false, "Yellow") => Color.FromRgb(0xFF, 0xB4, 0x54),
            _ => Color.FromRgb(0xA0, 0xA0, 0xA8)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер string → Visibility (пустая строка → Collapsed, иначе Visible).
/// </summary>
public class StringToVisConverter : MarkupExtension, IValueConverter
{
    public static readonly StringToVisConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var s = value?.ToString() ?? string.Empty;
        return string.IsNullOrWhiteSpace(s) ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер для определения активного пункта навигации.
/// Точно сравнивает текущий "Section:SubPage" с ожидаемым значением.
/// </summary>
public class SectionActiveConverter : MarkupExtension, IValueConverter
{
    public static readonly SectionActiveConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var current = value?.ToString() ?? string.Empty;
        var expected = parameter?.ToString() ?? string.Empty;

        return string.Equals(current, expected, StringComparison.OrdinalIgnoreCase)
            ? "Active"
            : "Inactive";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: null → Collapsed, не-null → Visible.
/// </summary>
public class NullToVisConverter : MarkupExtension, IValueConverter
{
    public static readonly NullToVisConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value == null ? Visibility.Collapsed : Visibility.Visible;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: null → Visible, не-null → Collapsed.
/// </summary>
public class NullToInverseVisConverter : MarkupExtension, IValueConverter
{
    public static readonly NullToInverseVisConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        => value == null ? Visibility.Visible : Visibility.Collapsed;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер: 0-based индекс + 1 (для нумерации шагов).
/// Принимает AlternationIndex (int) и возвращает (index + 1).
/// </summary>
public class IndexPlusOneConverter : MarkupExtension, IValueConverter
{
    public static readonly IndexPlusOneConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is int i)
            return (i + 1).ToString();
        return "1";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

/// <summary>
/// Конвертер для подсветки выбранной карты.
/// Если имя карты совпадает с выбранной, возвращает цвет акцента.
/// </summary>
public class MapSelectedConverter : MarkupExtension, IValueConverter
{
    public static readonly MapSelectedConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // value — имя текущей карты
        // parameter — SelectedMap.Name (вложенный binding через FallbackValue не работает корректно)
        // Просто возвращаем AccentBrush, если value не null, иначе тёмный цвет
        var name = value?.ToString();
        return string.IsNullOrEmpty(name) ? "#26262A" : "#FF3B3B";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();

    public override object ProvideValue(IServiceProvider serviceProvider) => this;
}

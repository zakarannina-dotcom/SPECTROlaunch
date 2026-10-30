using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Xml.Linq;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис локализации (RU/EN). Подгружает словари из XAML-ресурсов.
/// </summary>
public class LocalizationService : INotifyPropertyChanged
{
    public static readonly string[] SupportedCultures = { "ru-RU", "en-US" };

    private string _currentCulture = "ru-RU";
    private ResourceDictionary? _currentDictionary;

    public event PropertyChangedEventHandler? PropertyChanged;
    public event Action? CultureChanged;

    /// <summary>Текущая культура (например, "ru-RU" или "en-US").</summary>
    public string CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture == value)
                return;
            _currentCulture = value;
            ApplyCulture();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CurrentCulture)));
        }
    }

    /// <summary>Короткое имя языка (RU/EN) для UI.</summary>
    public string LanguageCode => _currentCulture.StartsWith("ru") ? "RU" : "EN";

    public LocalizationService()
    {
        ApplyCulture();
    }

    /// <summary>
    /// Загружает XAML-словарь и применяет его к приложению.
    /// </summary>
    private void ApplyCulture()
    {
        var dictPath = $"/CS2Launcher;component/Resources/Strings.{_currentCulture}.xaml";

        try
        {
            var newDict = new ResourceDictionary
            {
                Source = new Uri(dictPath, UriKind.RelativeOrAbsolute)
            };

            if (_currentDictionary != null)
                Application.Current.Resources.MergedDictionaries.Remove(_currentDictionary);

            Application.Current.Resources.MergedDictionaries.Add(newDict);
            _currentDictionary = newDict;

            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(_currentCulture);
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(_currentCulture);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Localization load failed: {ex.Message}");
        }

        // DynamicResource в XAML автоматически обновляется при смене ResourceDictionary,
        // никаких ручных уведомлений не нужно.
        CultureChanged?.Invoke();
    }

    /// <summary>
    /// Получает локализованную строку по ключу.
    /// </summary>
    public string GetString(string key)
    {
        if (_currentDictionary != null && _currentDictionary.Contains(key))
            return _currentDictionary[key]?.ToString() ?? key;
        return key;
    }

    /// <summary>
    /// Переключает язык на следующий доступный.
    /// </summary>
    public void ToggleLanguage()
    {
        CurrentCulture = _currentCulture == "ru-RU" ? "en-US" : "ru-RU";
    }

    /// <summary>
    /// Загружает последний выбранный язык из настроек.
    /// </summary>
    public void LoadSavedCulture()
    {
        var settingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CS2Launcher");
        var langFile = Path.Combine(settingsDir, "language.txt");

        if (File.Exists(langFile))
        {
            var saved = File.ReadAllText(langFile).Trim();
            if (SupportedCultures.Contains(saved))
                _currentCulture = saved;
        }
        ApplyCulture();
    }

    /// <summary>
    /// Сохраняет выбранный язык.
    /// </summary>
    public void SaveCulture()
    {
        var settingsDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "CS2Launcher");
        Directory.CreateDirectory(settingsDir);
        File.WriteAllText(Path.Combine(settingsDir, "language.txt"), _currentCulture);
    }
}

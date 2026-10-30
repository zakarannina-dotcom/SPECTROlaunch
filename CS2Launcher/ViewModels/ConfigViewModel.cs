using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models;
using CS2Launcher.Services;
using Microsoft.Win32;

namespace CS2Launcher.ViewModels;

/// <summary>
/// ViewModel вкладки управления конфигами CS2.
/// </summary>
public partial class ConfigViewModel : ObservableObject
{
    private readonly ConfigService _configService;

    [ObservableProperty]
    private CS2Config _config;

    [ObservableProperty]
    private string _saveResult = string.Empty;

    [ObservableProperty]
    private string _autoexecPreview = string.Empty;

    [ObservableProperty]
    private string _shareCodeInput = string.Empty;

    /// <summary>Список пресетов профессиональных игроков.</summary>
    public ObservableCollection<ProPlayerPreset> ProPlayers { get; } = new();

    /// <summary>Текст фильтра для поиска игрока.</summary>
    [ObservableProperty]
    private string _playerFilter = string.Empty;

    /// <summary>Отфильтрованный список игроков для отображения.</summary>
    public ObservableCollection<ProPlayerPreset> FilteredPlayers { get; } = new();

    public ICommand SaveCommand { get; }
    public ICommand SaveAutoexecCommand { get; }
    public ICommand GenerateAutoexecCommand { get; }
    public ICommand ResetCommand { get; }
    public ICommand ImportCfgCommand { get; }
    public ICommand ExportCfgCommand { get; }
    public ICommand ImportShareCodeCommand { get; }
    public ICommand ExportShareCodeCommand { get; }
    public ICommand ApplyProPlayerCommand { get; }

    public ConfigViewModel(ConfigService configService, CS2Config config)
    {
        _configService = configService;
        _config = config;

        SaveCommand = new RelayCommand(SaveSettings);
        SaveAutoexecCommand = new RelayCommand(SaveAutoexec);
        GenerateAutoexecCommand = new RelayCommand(GenerateAutoexec);
        ResetCommand = new RelayCommand(Reset);
        ImportCfgCommand = new RelayCommand(ImportCfg);
        ExportCfgCommand = new RelayCommand(ExportCfg);
        ImportShareCodeCommand = new RelayCommand(ImportShareCode);
        ExportShareCodeCommand = new RelayCommand(ExportShareCode);
        ApplyProPlayerCommand = new RelayCommand<ProPlayerPreset>(ApplyProPlayer);

        // Загружаем пресеты про-игроков
        LoadProPlayers();

        Config.PropertyChanged += (_, _) => GenerateAutoexec();
        GenerateAutoexec();
    }

    private void LoadProPlayers()
    {
        ProPlayers.Clear();
        foreach (var p in ProPlayerPresets.GetAll())
            ProPlayers.Add(p);
        ApplyFilter();
    }

    partial void OnPlayerFilterChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        FilteredPlayers.Clear();
        var filter = (PlayerFilter ?? string.Empty).Trim().ToLowerInvariant();

        foreach (var p in ProPlayers)
        {
            if (string.IsNullOrEmpty(filter) ||
                p.PlayerName.ToLowerInvariant().Contains(filter) ||
                p.Team.ToLowerInvariant().Contains(filter) ||
                p.Country.ToLowerInvariant().Contains(filter))
            {
                FilteredPlayers.Add(p);
            }
        }
    }

    private void ApplyProPlayer(ProPlayerPreset? preset)
    {
        if (preset == null)
        {
            SaveResult = "✗ Игрок не выбран";
            return;
        }

        Config.CrosshairSize = preset.Size;
        Config.CrosshairThickness = preset.Thickness;
        Config.CrosshairGap = preset.Gap;
        Config.CrosshairColor = preset.Color;
        Config.CrosshairOutline = preset.Outline;
        Config.CrosshairDot = preset.Dot;
        Config.Sensitivity = preset.Sensitivity;
        ShareCodeInput = preset.CrosshairCode;

        GenerateAutoexec();
        SaveResult = $"✓ Применён пресет: {preset.PlayerName} ({preset.Team}) • {preset.CountryFlag}";
    }

    private void SaveSettings()
    {
        try
        {
            _configService.SaveSettings(Config, new LaunchOptions());
            SaveResult = "✓ Настройки сохранены";
        }
        catch (Exception ex)
        {
            SaveResult = "✗ Ошибка: " + ex.Message;
        }
    }

    private void SaveAutoexec()
    {
        try
        {
            var ok = _configService.SaveAutoexec(Config);
            SaveResult = ok
                ? "✓ autoexec.cfg записан в папку cfg CS2"
                : "✗ Не удалось найти путь CS2. Укажите Steam вручную.";
        }
        catch (Exception ex)
        {
            SaveResult = "✗ Ошибка: " + ex.Message;
        }
    }

    private void GenerateAutoexec()
    {
        AutoexecPreview = Config.GenerateAutoexec();
        Config.AutoexecContent = AutoexecPreview;
    }

    private void Reset()
    {
        Config = new CS2Config();
        GenerateAutoexec();
        SaveResult = "Сброшено к значениям по умолчанию";
    }

    private void ImportCfg()
    {
        var dialog = new OpenFileDialog
        {
            Filter = "CS2 config files|*.cfg|All files|*.*",
            Title = "Импорт конфигурации (.cfg)"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            var content = File.ReadAllText(dialog.FileName);
            Config.ImportFromCfg(content);
            GenerateAutoexec();
            SaveResult = $"✓ Импортировано: {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex)
        {
            SaveResult = "✗ Ошибка импорта: " + ex.Message;
        }
    }

    private void ExportCfg()
    {
        var dialog = new SaveFileDialog
        {
            Filter = "CS2 config files|*.cfg|All files|*.*",
            Title = "Экспорт конфигурации",
            FileName = "autoexec.cfg"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            File.WriteAllText(dialog.FileName, Config.GenerateAutoexec());
            SaveResult = $"✓ Экспортировано: {Path.GetFileName(dialog.FileName)}";
        }
        catch (Exception ex)
        {
            SaveResult = "✗ Ошибка экспорта: " + ex.Message;
        }
    }

    private void ImportShareCode()
    {
        if (string.IsNullOrWhiteSpace(ShareCodeInput))
        {
            SaveResult = "✗ Введите share-код";
            return;
        }

        var ok = Config.ImportFromShareCode(ShareCodeInput);
        if (ok)
        {
            GenerateAutoexec();
            SaveResult = "✓ Share-код импортирован: " + Config.ShareCode;
        }
        else
        {
            SaveResult = "✗ Неверный формат share-кода";
        }
    }

    private void ExportShareCode()
    {
        var code = Config.GenerateShareCode();
        ShareCodeInput = code;
        SaveResult = "✓ Share-код сгенерирован: " + code;
        try
        {
            System.Windows.Clipboard.SetText(code);
            SaveResult += " (скопирован в буфер обмена)";
        }
        catch { /* ignore */ }
    }
}

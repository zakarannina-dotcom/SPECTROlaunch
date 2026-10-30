using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models.ARCRaiders;
using CS2Launcher.Services;
using Microsoft.Win32;

namespace CS2Launcher.ViewModels;

/// <summary>
/// ViewModel вкладки ARC Raiders.
/// </summary>
public partial class ArcRaidersViewModel : ObservableObject
{
    private readonly ArcRaidersService _service;

    [ObservableProperty]
    private bool _isInstalled;

    [ObservableProperty]
    private string _gamePath = string.Empty;

    [ObservableProperty]
    private bool _isRunning;

    [ObservableProperty]
    private bool _isLaunching;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private GraphicsPreset? _selectedPreset;

    /// <summary>Каталог онлайн-модов.</summary>
    public ObservableCollection<OnlineMod> OnlineMods { get; } = new();

    /// <summary>Фильтр по категории модов.</summary>
    [ObservableProperty]
    private string _modFilter = string.Empty;

    /// <summary>Отфильтрованный список модов для отображения.</summary>
    public ObservableCollection<OnlineMod> FilteredOnlineMods { get; } = new();

    public ObservableCollection<GraphicsPreset> Presets { get; } = new();
    public ModCatalog ModCatalog { get; } = new();

    public ICommand LaunchCommand { get; }
    public ICommand CloseGameCommand { get; }
    public ICommand ApplyPresetCommand { get; }
    public ICommand InstallModCommand { get; }
    public ICommand RefreshModsCommand { get; }
    public ICommand OpenModsFolderCommand { get; }
    public ICommand DownloadOnlineModCommand { get; }

    public ArcRaidersViewModel()
    {
        _service = new ArcRaidersService();
        _service.LaunchFailed += msg => StatusMessage = "✗ " + msg;
        _service.GameLaunched += _ => StatusMessage = "✓ Запущено";

        // Заполняем пресеты
        Presets.Add(GraphicsPreset.LowSpec());
        Presets.Add(GraphicsPreset.MediumSpec());
        Presets.Add(GraphicsPreset.HighSpec());
        SelectedPreset = Presets[1]; // Средний по умолчанию

        // Загружаем онлайн-моды
        LoadOnlineMods();

        LaunchCommand = new AsyncRelayCommand(LaunchAsync);
        CloseGameCommand = new RelayCommand(CloseGame);
        ApplyPresetCommand = new RelayCommand<GraphicsPreset>(ApplyPreset);
        InstallModCommand = new RelayCommand(InstallMod);
        RefreshModsCommand = new RelayCommand(RefreshMods);
        OpenModsFolderCommand = new RelayCommand(OpenModsFolder);
        DownloadOnlineModCommand = new AsyncRelayCommand<OnlineMod>(DownloadOnlineModAsync);

        RefreshStatus();
    }

    private void LoadOnlineMods()
    {
        OnlineMods.Clear();
        foreach (var m in OnlineModCatalog.GetPopularMods())
            OnlineMods.Add(m);
        ApplyModFilter();
    }

    partial void OnModFilterChanged(string value) => ApplyModFilter();

    private void ApplyModFilter()
    {
        FilteredOnlineMods.Clear();
        var filter = (ModFilter ?? string.Empty).Trim().ToLowerInvariant();

        foreach (var m in OnlineMods)
        {
            if (string.IsNullOrEmpty(filter) ||
                m.Name.ToLowerInvariant().Contains(filter) ||
                m.Description.ToLowerInvariant().Contains(filter) ||
                m.Category.ToLowerInvariant().Contains(filter) ||
                m.Author.ToLowerInvariant().Contains(filter))
            {
                FilteredOnlineMods.Add(m);
            }
        }
    }

    /// <summary>
    /// Имитация скачивания мода (демо-функция).
    /// В реальном приложении — HttpClient + Stream.CopyToAsync.
    /// </summary>
    private async Task DownloadOnlineModAsync(OnlineMod? mod)
    {
        if (mod == null)
        {
            StatusMessage = "✗ Мод не выбран";
            return;
        }

        if (mod.IsInstalled)
        {
            StatusMessage = $"✓ {mod.Name} уже установлен";
            return;
        }

        if (mod.IsDownloading)
        {
            StatusMessage = "⏳ Уже скачивается...";
            return;
        }

        mod.IsDownloading = true;
        StatusMessage = $"⏳ Скачивание: {mod.Name}...";

        // Имитация прогресса
        for (int i = 0; i <= 100; i += 10)
        {
            mod.DownloadProgress = i;
            await Task.Delay(150);
        }

        mod.IsDownloading = false;
        mod.IsInstalled = true;
        StatusMessage = $"✓ {mod.Name} установлен ({mod.FormattedSize})";

        // В реальности здесь был бы вызов:
        // _service.InstallMod(downloadedFilePath, mod.Name);
        RefreshMods();
    }

    public void RefreshStatus()
    {
        IsInstalled = _service.IsGameInstalled();
        GamePath = _service.GetGamePath() ?? "Не найден";
        IsRunning = _service.IsRunning();

        if (IsInstalled)
        {
            RefreshMods();
        }
    }

    private async Task LaunchAsync()
    {
        if (IsLaunching) return;
        IsLaunching = true;
        StatusMessage = "Запуск...";

        await Task.Run(() => _service.Launch());

        await Task.Delay(2000);
        IsRunning = _service.IsRunning();
        IsLaunching = false;
    }

    private void CloseGame()
    {
        _service.KillRunning();
        IsRunning = false;
        StatusMessage = "Игра принудительно закрыта";
    }

    private void ApplyPreset(GraphicsPreset? preset)
    {
        if (preset == null)
        {
            StatusMessage = "✗ Выберите пресет";
            return;
        }

        SelectedPreset = preset;
        var ok = _service.ApplyGraphicsPreset(preset);
        StatusMessage = ok
            ? $"✓ Применён пресет: {preset.Name}"
            : "✗ Не удалось применить пресет (игра не найдена?)";
    }

    private void InstallMod()
    {
        if (!IsInstalled)
        {
            StatusMessage = "✗ Сначала установите ARC Raiders";
            return;
        }

        var dialog = new OpenFileDialog
        {
            Filter = "Unreal Engine mod|*.pak|All files|*.*",
            Title = "Выберите .pak файл мода",
            Multiselect = true
        };

        if (dialog.ShowDialog() != true)
            return;

        var installed = 0;
        foreach (var file in dialog.FileNames)
        {
            var modName = Path.GetFileNameWithoutExtension(file);
            if (_service.InstallMod(file, modName))
            {
                installed++;
            }
        }

        StatusMessage = installed > 0
            ? $"✓ Установлено модов: {installed}"
            : "✗ Не удалось установить мод";

        RefreshMods();
    }

    private void RefreshMods()
    {
        ModCatalog.Mods.Clear();
        if (!IsInstalled)
        {
            ModCatalog.RefreshStats();
            return;
        }

        var mods = _service.ScanInstalledMods();
        foreach (var mod in mods)
        {
            ModCatalog.Mods.Add(mod);
        }
        ModCatalog.RefreshStats();
    }

    private void OpenModsFolder()
    {
        var modsPath = _service.GetModsPath();
        if (modsPath == null)
        {
            StatusMessage = "✗ Папка модов недоступна";
            return;
        }

        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"\"{modsPath}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            StatusMessage = "✗ " + ex.Message;
        }
    }
}

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models;
using CS2Launcher.Services;

namespace CS2Launcher.ViewModels;

/// <summary>
/// Главная ViewModel, координирует навигацию между разделами (играми)
/// и их подразделами (Запуск / Конфиг / ...).
/// </summary>
public partial class MainViewModel : ObservableObject
{
    private readonly SteamAuthService _authService;
    private readonly SteamApiService _apiService;
    private readonly GameLauncherService _launcherService;
    private readonly ConfigService _configService;
    private readonly UpdateService _updateService;

    /// <summary>
    /// Текущий активный раздел (CS2 / ARC Raiders).
    /// </summary>
    [ObservableProperty]
    private string _currentSection = "CS2";

    /// <summary>
    /// Текущий активный подраздел (Launch / Config / Stats / Mods).
    /// </summary>
    [ObservableProperty]
    private string _currentSubPage = "Launch";

    /// <summary>
    /// Полный путь активной страницы в формате "Section:SubPage" (например, "ARC:Mods").
    /// Используется для точной подсветки активного пункта в навигации.
    /// </summary>
    public string CurrentFullPage => $"{CurrentSection}:{CurrentSubPage}";

    partial void OnCurrentSectionChanged(string value)
        => OnPropertyChanged(nameof(CurrentFullPage));

    partial void OnCurrentSubPageChanged(string value)
        => OnPropertyChanged(nameof(CurrentFullPage));

    [ObservableProperty]
    private ObservableObject? _currentViewModel;

    [ObservableProperty]
    private UserProfile _user = new();

    [ObservableProperty]
    private OverallStats _stats = new();

    [ObservableProperty]
    private bool _isLoggingIn;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    private string _languageCode = "RU";

    // === Состояние обновлений ===
    [ObservableProperty]
    private bool _isCheckingUpdates;

    [ObservableProperty]
    private bool _updateAvailable;

    [ObservableProperty]
    private string _updateVersion = string.Empty;

    [ObservableProperty]
    private string _updateNotes = string.Empty;

    [ObservableProperty]
    private string _updateUrl = string.Empty;

    private GitHubRelease? _latestRelease;

    public LocalizationService Localization { get; }

    // ViewModels CS2
    public LaunchViewModel CS2LaunchViewModel { get; }
    public ConfigViewModel CS2ConfigViewModel { get; }
    public StatsViewModel CS2StatsViewModel { get; }
    public GrenadesViewModel CS2GrenadesViewModel { get; }

    // ViewModels ARC Raiders — каждый со своим типом, чтобы WPF подобрал правильный DataTemplate
    public ArcRaidersLaunchViewModel ArcRaidersLaunchViewModel { get; }
    public ArcRaidersConfigViewModel ArcRaidersConfigViewModel { get; }
    public ArcRaidersModsViewModel ArcRaidersModsViewModel { get; }

    public ICommand NavigateCommand { get; }
    public ICommand LoginCommand { get; }
    public ICommand LogoutCommand { get; }
    public ICommand ToggleLanguageCommand { get; }
    public ICommand CheckUpdatesCommand { get; }
    public ICommand DownloadUpdateCommand { get; }

    public MainViewModel()
    {
        Localization = new LocalizationService();
        Localization.LoadSavedCulture();
        LanguageCode = Localization.LanguageCode;

        _authService = new SteamAuthService();
        _apiService = new SteamApiService(LoadApiKeySafely());
        _launcherService = new GameLauncherService();
        _configService = new ConfigService();
        _updateService = new UpdateService();

        var (config, launchOptions) = _configService.LoadSettings();
        var detectedPath = _launcherService.DetectCS2Path();
        if (!string.IsNullOrEmpty(detectedPath) && string.IsNullOrEmpty(launchOptions.ExecutablePath))
            launchOptions.ExecutablePath = detectedPath;

        // CS2 ViewModels
        CS2LaunchViewModel = new LaunchViewModel(_launcherService, launchOptions);
        CS2ConfigViewModel = new ConfigViewModel(_configService, config);
        CS2StatsViewModel = new StatsViewModel(_apiService);
        CS2GrenadesViewModel = new GrenadesViewModel();

        // ARC Raiders ViewModels — каждый со своим типом для корректного DataTemplate
        ArcRaidersLaunchViewModel = new ArcRaidersLaunchViewModel();
        ArcRaidersConfigViewModel = new ArcRaidersConfigViewModel();
        ArcRaidersModsViewModel = new ArcRaidersModsViewModel();

        NavigateCommand = new RelayCommand<string>(Navigate);
        LoginCommand = new AsyncRelayCommand(LoginAsync);
        LogoutCommand = new RelayCommand(Logout);
        ToggleLanguageCommand = new RelayCommand(ToggleLanguage);
        CheckUpdatesCommand = new AsyncRelayCommand(CheckForUpdatesAsync);
        DownloadUpdateCommand = new RelayCommand(DownloadUpdate);

        _authService.LoginSuccess += OnLoginSuccess;
        _authService.LoginError += OnLoginError;
        _authService.LoginCancelled += OnLoginCancelled;

        Localization.CultureChanged += () =>
        {
            LanguageCode = Localization.LanguageCode;
            OnPropertyChanged(string.Empty);
        };

        // Стартовая страница — запуск CS2
        CurrentSection = "CS2";
        CurrentSubPage = "Launch";
        CurrentViewModel = CS2LaunchViewModel;

        // Авто-проверка обновлений при запуске (без блокировки UI)
        _ = Task.Run(async () =>
        {
            await Task.Delay(2000); // ждём, пока приложение полностью загрузится
            await CheckForUpdatesAsync();
        });
    }

    private async Task CheckForUpdatesAsync()
    {
        if (IsCheckingUpdates) return;
        IsCheckingUpdates = true;
        StatusMessage = "Проверка обновлений...";

        try
        {
            var result = await _updateService.CheckForUpdatesAsync();

            if (result.UpdateAvailable && result.LatestRelease != null)
            {
                _latestRelease = result.LatestRelease;
                UpdateAvailable = true;
                UpdateVersion = result.LatestVersion.ToString();
                UpdateNotes = result.LatestRelease.Body;
                UpdateUrl = result.LatestRelease.HtmlUrl;
                StatusMessage = $"🎉 Доступна новая версия: v{UpdateVersion}!";
            }
            else if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                UpdateAvailable = false;
                StatusMessage = "Обновление: не удалось проверить (нет сети или репозитория)";
            }
            else
            {
                UpdateAvailable = false;
                StatusMessage = $"У вас актуальная версия (v{_updateService.CurrentVersion})";
            }
        }
        finally
        {
            IsCheckingUpdates = false;
        }
    }

    private void DownloadUpdate()
    {
        if (_latestRelease == null) return;

        try
        {
            _updateService.OpenReleasePage(_latestRelease);
            StatusMessage = "Открыта страница загрузки в браузере";
        }
        catch (Exception ex)
        {
            StatusMessage = "✗ " + ex.Message;
        }
    }

    private static string? LoadApiKeySafely()
    {
        try { return new ConfigService().LoadApiKey(); }
        catch { return null; }
    }

    /// <summary>
    /// Навигация: ключ в формате "Section:SubPage" или просто "SubPage" (для текущего раздела).
    /// </summary>
    private void Navigate(string? key)
    {
        if (string.IsNullOrEmpty(key)) return;

        // Формат: "CS2:Launch" или "ARC:Mods" или просто "Launch"
        string section;
        string subPage;

        if (key.Contains(':'))
        {
            var parts = key.Split(':', 2);
            section = parts[0];
            subPage = parts[1];
        }
        else
        {
            section = CurrentSection;
            subPage = key;
        }

        CurrentSection = section;
        CurrentSubPage = subPage;

        CurrentViewModel = (section, subPage) switch
        {
            ("CS2", "Launch") => CS2LaunchViewModel,
            ("CS2", "Config") => CS2ConfigViewModel,
            ("CS2", "Stats") => CS2StatsViewModel,
            ("CS2", "Grenades") => CS2GrenadesViewModel,
            ("ARC", "Launch") => ArcRaidersLaunchViewModel,
            ("ARC", "Config") => ArcRaidersConfigViewModel,
            ("ARC", "Mods") => ArcRaidersModsViewModel,
            _ => CS2LaunchViewModel
        };
    }

    private async Task LoginAsync()
    {
        if (IsLoggingIn) return;
        IsLoggingIn = true;
        StatusMessage = Localization.GetString("StatusAuthenticating");

        try
        {
            await _authService.StartLoginAsync();
        }
        finally
        {
            IsLoggingIn = false;
        }
    }

    private void OnLoginSuccess(string steamId)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(async () =>
        {
            StatusMessage = Localization.GetString("StatusWelcome") + " " + steamId;

            var profile = await _apiService.GetPlayerSummaryAsync(steamId);
            if (profile != null)
            {
                User = profile;
                CS2StatsViewModel.SteamId = steamId;
                await CS2StatsViewModel.LoadStatsAsync();
                Stats = CS2StatsViewModel.Overall;
            }
        });
    }

    private void OnLoginError(string error)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            StatusMessage = $"{Localization.GetString("StatusError")}: {error}";
            IsLoggingIn = false;
        });
    }

    private void OnLoginCancelled()
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            StatusMessage = Localization.GetString("StatusCancelled");
            IsLoggingIn = false;
        });
    }

    private void Logout()
    {
        _authService.Logout();
        User = new UserProfile();
        CS2StatsViewModel.Overall = new OverallStats();
        CS2StatsViewModel.RecentMatches.Clear();
        Stats = new OverallStats();
        StatusMessage = Localization.GetString("StatusLoggedOut");
    }

    private void ToggleLanguage()
    {
        Localization.ToggleLanguage();
        Localization.SaveCulture();
        LanguageCode = Localization.LanguageCode;
        StatusMessage = Localization.GetString("StatusLanguageChanged");
    }
}

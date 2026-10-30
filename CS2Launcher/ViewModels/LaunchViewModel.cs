using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models;
using CS2Launcher.Services;

namespace CS2Launcher.ViewModels;

/// <summary>
/// ViewModel вкладки запуска игры.
/// </summary>
public partial class LaunchViewModel : ObservableObject
{
    private readonly GameLauncherService _launcherService;

    [ObservableProperty]
    private LaunchOptions _options;

    [ObservableProperty]
    private bool _isLaunching;

    [ObservableProperty]
    private string _launchResult = string.Empty;

    [ObservableProperty]
    private bool _isCS2Running;

    public ICommand LaunchCommand { get; }
    public ICommand CloseGameCommand { get; }
    public ICommand BrowseExecutableCommand { get; }

    public LaunchViewModel(GameLauncherService launcherService, LaunchOptions options)
    {
        _launcherService = launcherService;
        _options = options;

        LaunchCommand = new AsyncRelayCommand(LaunchAsync);
        CloseGameCommand = new RelayCommand(CloseGame);
        BrowseExecutableCommand = new RelayCommand(BrowseExecutable);

        _launcherService.GameLaunched += OnGameLaunched;
        _launcherService.LaunchFailed += OnLaunchFailed;

        // Стартовая проверка статуса процесса
        RefreshRunningStatus();
    }

    public void RefreshRunningStatus()
    {
        IsCS2Running = _launcherService.IsCS2Running();
    }

    private async Task LaunchAsync()
    {
        if (IsLaunching) return;
        IsLaunching = true;
        LaunchResult = "...";

        await Task.Run(() =>
        {
            var ok = _launcherService.Launch(Options);
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                LaunchResult = ok ? "✓ Запущено" : "✗ Ошибка";
                if (ok) RefreshRunningStatus();
            });
        });

        IsLaunching = false;
    }

    private void CloseGame()
    {
        _launcherService.KillRunningCS2();
        RefreshRunningStatus();
        LaunchResult = "Принудительно закрыто";
    }

    private void BrowseExecutable()
    {
        var dialog = new Microsoft.Win32.OpenFileDialog
        {
            Filter = "CS2 executable|cs2.exe",
            Title = "Выберите cs2.exe"
        };

        if (dialog.ShowDialog() == true)
        {
            Options.ExecutablePath = dialog.FileName;
        }
    }

    private void OnGameLaunched(string command)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            LaunchResult = "✓ Запущено: " + command;
        });
    }

    private void OnLaunchFailed(string error)
    {
        System.Windows.Application.Current.Dispatcher.Invoke(() =>
        {
            LaunchResult = "✗ " + error;
        });
    }
}

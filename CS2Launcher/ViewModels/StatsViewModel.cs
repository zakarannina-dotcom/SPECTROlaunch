using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models;
using CS2Launcher.Services;

namespace CS2Launcher.ViewModels;

/// <summary>
/// ViewModel вкладки статистики игрока.
/// </summary>
public partial class StatsViewModel : ObservableObject
{
    private readonly SteamApiService _apiService;

    [ObservableProperty]
    private string _steamId = string.Empty;

    [ObservableProperty]
    private OverallStats _overall = new();

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public ObservableCollection<GameStats> RecentMatches { get; } = new();

    public ICommand RefreshCommand { get; }

    public StatsViewModel(SteamApiService apiService)
    {
        _apiService = apiService;
        RefreshCommand = new AsyncRelayCommand(LoadStatsAsync);
    }

    public async Task LoadStatsAsync()
    {
        if (string.IsNullOrEmpty(SteamId))
        {
            // Загружаем демо-данные
            LoadMockData();
            return;
        }

        IsLoading = true;
        ErrorMessage = string.Empty;

        try
        {
            var stats = await _apiService.GetCS2StatsAsync(SteamId);
            if (stats != null)
            {
                Overall = stats;
            }

            // Steam Web API не отдаёт матчи; используем моковые данные
            LoadMockRecentMatches();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void LoadMockData()
    {
        Overall = new OverallStats
        {
            TotalMatches = 247,
            Wins = 158,
            Losses = 89,
            TotalKills = 3124,
            TotalDeaths = 2287,
            HeadshotPercent = 48.3,
            HoursPlayed = 312.5,
            Rank = 12
        };
        LoadMockRecentMatches();
    }

    private void LoadMockRecentMatches()
    {
        RecentMatches.Clear();

        var rnd = new Random(42);
        var maps = new[] { "Mirage", "Inferno", "Dust2", "Nuke", "Ancient", "Anubis", "Vertigo", "Overpass" };
        var modes = new[] { "Competitive", "Premier", "Deathmatch", "Wingman" };

        for (int i = 0; i < 8; i++)
        {
            var kills = rnd.Next(8, 32);
            var deaths = rnd.Next(5, 25);
            var assists = rnd.Next(0, 10);
            var score = kills * 100 + assists * 50 + rnd.Next(0, 100);
            var mvps = rnd.Next(0, 8);
            var winRoll = rnd.NextDouble();

            RecentMatches.Add(new GameStats
            {
                MapName = maps[rnd.Next(maps.Length)],
                GameMode = modes[rnd.Next(modes.Length)],
                MatchDate = DateTime.Now.AddDays(-i).AddHours(-rnd.Next(0, 8)),
                Kills = kills,
                Deaths = deaths,
                Assists = assists,
                MVPs = mvps,
                Score = score,
                Result = winRoll > 0.55 ? MatchResult.Win : (winRoll > 0.3 ? MatchResult.Loss : MatchResult.Draw)
            });
        }
    }
}

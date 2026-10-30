using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models;

/// <summary>
/// Статистика игрока CS2 (модель данных).
/// </summary>
public class GameStats : ObservableObject
{
    private string _mapName = string.Empty;
    private string _gameMode = string.Empty;
    private DateTime _matchDate = DateTime.Now;
    private int _kills;
    private int _deaths;
    private int _assists;
    private int _mvps;
    private int _score;
    private MatchResult _result = MatchResult.Win;

    public string MapName
    {
        get => _mapName;
        set => SetProperty(ref _mapName, value);
    }

    public string GameMode
    {
        get => _gameMode;
        set => SetProperty(ref _gameMode, value);
    }

    public DateTime MatchDate
    {
        get => _matchDate;
        set => SetProperty(ref _matchDate, value);
    }

    public int Kills
    {
        get => _kills;
        set => SetProperty(ref _kills, value);
    }

    public int Deaths
    {
        get => _deaths;
        set => SetProperty(ref _deaths, value);
    }

    public int Assists
    {
        get => _assists;
        set => SetProperty(ref _assists, value);
    }

    public int MVPs
    {
        get => _mvps;
        set => SetProperty(ref _mvps, value);
    }

    public int Score
    {
        get => _score;
        set => SetProperty(ref _score, value);
    }

    public MatchResult Result
    {
        get => _result;
        set => SetProperty(ref _result, value);
    }

    public double KDR => Deaths == 0 ? Kills : Math.Round((double)Kills / Deaths, 2);
}

public enum MatchResult
{
    Win,
    Loss,
    Draw
}

/// <summary>
/// Агрегированная статистика за всё время.
/// </summary>
public class OverallStats : ObservableObject
{
    private int _totalMatches;
    private int _wins;
    private int _losses;
    private int _totalKills;
    private int _totalDeaths;
    private double _headshotPercent;
    private double _hoursPlayed;
    private int _rank;

    public int TotalMatches { get => _totalMatches; set => SetProperty(ref _totalMatches, value); }
    public int Wins { get => _wins; set => SetProperty(ref _wins, value); }
    public int Losses { get => _losses; set => SetProperty(ref _losses, value); }
    public int TotalKills { get => _totalKills; set => SetProperty(ref _totalKills, value); }
    public int TotalDeaths { get => _totalDeaths; set => SetProperty(ref _totalDeaths, value); }
    public double HeadshotPercent { get => _headshotPercent; set => SetProperty(ref _headshotPercent, value); }
    public double HoursPlayed { get => _hoursPlayed; set => SetProperty(ref _hoursPlayed, value); }
    public int Rank { get => _rank; set => SetProperty(ref _rank, value); }

    public double WinRate => TotalMatches == 0 ? 0 : Math.Round((double)Wins / TotalMatches * 100, 1);
    public double OverallKDR => TotalDeaths == 0 ? TotalKills : Math.Round((double)TotalKills / TotalDeaths, 2);
}

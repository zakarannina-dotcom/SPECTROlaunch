using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using CS2Launcher.Models;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис взаимодействия со Steam Web API.
/// Для работы требует API-ключ (см. README.md, раздел "Настройка Steam API").
/// </summary>
public class SteamApiService : IDisposable
{
    private readonly HttpClient _httpClient;
    private string? _apiKey;
    private const string BaseUrl = "https://api.steampowered.com";

    public SteamApiService(string? apiKey = null)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(BaseUrl),
            Timeout = TimeSpan.FromSeconds(15)
        };
    }

    public void SetApiKey(string key) => _apiKey = key;

    public bool HasApiKey => !string.IsNullOrWhiteSpace(_apiKey);

    /// <summary>
    /// Получает базовую информацию о пользователе (имя, аватар, профиль).
    /// </summary>
    public async Task<UserProfile?> GetPlayerSummaryAsync(string steamId, CancellationToken ct = default)
    {
        if (!HasApiKey)
            return GetMockProfile(steamId);

        try
        {
            var url = $"/ISteamUser/GetPlayerSummaries/v2/?key={_apiKey}&steamids={steamId}";
            var response = await _httpClient.GetStringAsync(url, ct);
            var json = JObject.Parse(response);
            var player = json["response"]?["players"]?.FirstOrDefault();

            if (player == null)
                return null;

            return new UserProfile
            {
                SteamId = steamId,
                PersonaName = player["personaname"]?.ToString() ?? "Player",
                AvatarUrl = player["avatarfull"]?.ToString() ?? string.Empty,
                ProfileUrl = player["profileurl"]?.ToString() ?? string.Empty,
                IsLoggedIn = true
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SteamApi error: {ex.Message}");
            return GetMockProfile(steamId);
        }
    }

    /// <summary>
    /// Получает статистику CS2 (appId = 730).
    /// Steam возвращает общие ачивки; для разбора матчей нужны сторонние API (Faceit, Leetify).
    /// </summary>
    public async Task<OverallStats?> GetCS2StatsAsync(string steamId, CancellationToken ct = default)
    {
        if (!HasApiKey)
            return GetMockOverallStats();

        try
        {
            var url = $"/ISteamUserStats/GetUserStatsForGame/v2/?key={_apiKey}&steamid={steamId}&appid=730";
            var response = await _httpClient.GetStringAsync(url, ct);
            var json = JObject.Parse(response);

            var stats = json["playerstats"]?["stats"] as JArray;
            if (stats == null)
                return GetMockOverallStats();

            var dict = stats.ToDictionary(
                s => s["name"]?.ToString() ?? string.Empty,
                s => s["value"]?.ToObject<int>() ?? 0
            );

            var overall = new OverallStats
            {
                TotalKills = dict.GetValueOrDefault("total_kills", 0),
                TotalDeaths = dict.GetValueOrDefault("total_deaths", 0),
                HeadshotPercent = Math.Round(
                    dict.GetValueOrDefault("total_kills_headshot", 0) * 100.0 /
                    Math.Max(1, dict.GetValueOrDefault("total_kills", 1)), 1),
                HoursPlayed = Math.Round(
                    dict.GetValueOrDefault("total_time_played", 0) / 3600.0, 1),
                Rank = 0
            };

            overall.TotalMatches = 0;
            return overall;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"SteamApi error: {ex.Message}");
            return GetMockOverallStats();
        }
    }

    /// <summary>
    /// Моковые данные для демонстрации UI без API-ключа.
    /// </summary>
    private UserProfile GetMockProfile(string steamId) => new()
    {
        SteamId = steamId,
        PersonaName = "Demo Player",
        AvatarUrl = string.Empty,
        ProfileUrl = "https://steamcommunity.com/profiles/" + steamId,
        IsLoggedIn = true
    };

    private OverallStats GetMockOverallStats() => new()
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

    public void Dispose() => _httpClient.Dispose();
}

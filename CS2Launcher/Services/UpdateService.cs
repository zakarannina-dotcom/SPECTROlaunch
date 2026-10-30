using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace CS2Launcher.Services;

/// <summary>
/// Модель релиза с GitHub.
/// </summary>
public class GitHubRelease
{
    public string TagName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string HtmlUrl { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public List<GitHubAsset> Assets { get; set; } = new();
    public bool PreRelease { get; set; }
}

public class GitHubAsset
{
    public string Name { get; set; } = string.Empty;
    public string BrowserDownloadUrl { get; set; } = string.Empty;
    public long Size { get; set; }
    public int DownloadCount { get; set; }
}

/// <summary>
/// Результат проверки обновлений.
/// </summary>
public class UpdateCheckResult
{
    public bool UpdateAvailable { get; set; }
    public GitHubRelease? LatestRelease { get; set; }
    public Version CurrentVersion { get; set; } = new();
    public Version LatestVersion { get; set; } = new();
    public string ErrorMessage { get; set; } = string.Empty;
}

/// <summary>
/// Сервис проверки обновлений через GitHub Releases API.
/// </summary>
public class UpdateService : IDisposable
{
    private readonly HttpClient _httpClient;
    private const string GitHubApiBase = "https://api.github.com/repos";

    /// <summary>
    /// Репозиторий для проверки обновлений.
    /// ВАЖНО: замени на свой GitHub-репозиторий, если будешь публиковать лаунчер.
    /// </summary>
    public string Owner { get; set; } = "your-username";

    public string Repo { get; set; } = "cs2-launcher";

    /// <summary>Текущая версия приложения.</summary>
    public Version CurrentVersion { get; set; } = new Version(1, 0, 0, 0);

    public UpdateService()
    {
        _httpClient = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(15)
        };
        // GitHub API требует User-Agent
        _httpClient.DefaultRequestHeaders.Add("User-Agent", "CS2Launcher-UpdateCheck");
        _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    }

    /// <summary>
    /// Проверяет наличие обновления.
    /// </summary>
    public async Task<UpdateCheckResult> CheckForUpdatesAsync(CancellationToken ct = default)
    {
        var result = new UpdateCheckResult
        {
            CurrentVersion = CurrentVersion
        };

        try
        {
            var url = $"{GitHubApiBase}/{Owner}/{Repo}/releases/latest";
            var response = await _httpClient.GetStringAsync(url, ct);
            var release = ParseRelease(response);

            result.LatestRelease = release;
            result.LatestVersion = ParseVersion(release.TagName);
            result.UpdateAvailable = result.LatestVersion > CurrentVersion;
        }
        catch (Exception ex)
        {
            result.ErrorMessage = ex.Message;
            System.Diagnostics.Debug.WriteLine($"Update check failed: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Открывает страницу релиза в браузере.
    /// </summary>
    public void OpenReleasePage(GitHubRelease release)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = release.HtmlUrl,
                UseShellExecute = true
            });
        }
        catch { /* ignore */ }
    }

    private GitHubRelease ParseRelease(string json)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var release = new GitHubRelease
        {
            TagName = root.TryGetProperty("tag_name", out var tn) ? tn.GetString() ?? "" : "",
            Name = root.TryGetProperty("name", out var n) ? n.GetString() ?? "" : "",
            Body = root.TryGetProperty("body", out var b) ? b.GetString() ?? "" : "",
            HtmlUrl = root.TryGetProperty("html_url", out var u) ? u.GetString() ?? "" : "",
            PreRelease = root.TryGetProperty("prerelease", out var p) && p.GetBoolean()
        };

        if (root.TryGetProperty("published_at", out var pa) &&
            DateTime.TryParse(pa.GetString(), out var date))
        {
            release.PublishedAt = date;
        }

        if (root.TryGetProperty("assets", out var assets) && assets.ValueKind == JsonValueKind.Array)
        {
            foreach (var a in assets.EnumerateArray())
            {
                release.Assets.Add(new GitHubAsset
                {
                    Name = a.TryGetProperty("name", out var an) ? an.GetString() ?? "" : "",
                    BrowserDownloadUrl = a.TryGetProperty("browser_download_url", out var au) ? au.GetString() ?? "" : "",
                    Size = a.TryGetProperty("size", out var s) ? s.GetInt64() : 0,
                    DownloadCount = a.TryGetProperty("download_count", out var dc) ? dc.GetInt32() : 0
                });
            }
        }

        return release;
    }

    private static Version ParseVersion(string tagName)
    {
        // Извлекаем версию из тега "v1.2.3" или "1.2.3"
        var match = Regex.Match(tagName, @"(\d+)\.(\d+)\.(\d+)(?:\.(\d+))?");
        if (!match.Success)
            return new Version(0, 0, 0, 0);

        var major = int.Parse(match.Groups[1].Value);
        var minor = int.Parse(match.Groups[2].Value);
        var build = int.Parse(match.Groups[3].Value);
        var revision = match.Groups[4].Success ? int.Parse(match.Groups[4].Value) : 0;
        return new Version(major, minor, build, revision);
    }

    public void Dispose() => _httpClient.Dispose();
}

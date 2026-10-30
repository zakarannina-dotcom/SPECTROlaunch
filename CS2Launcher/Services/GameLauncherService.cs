using System.Diagnostics;
using System.IO;
using CS2Launcher.Models;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис запуска CS2.
/// Поддерживает два режима: через steam://run/730 (рекомендуется) и прямой запуск cs2.exe.
/// </summary>
public class GameLauncherService
{
    private const int CS2_AppId = 730;
    private const string CS2_ProcessName = "cs2";

    /// <summary>Событие запуска игры.</summary>
    public event Action<string>? GameLaunched;

    /// <summary>Событие ошибки запуска.</summary>
    public event Action<string>? LaunchFailed;

    /// <summary>
    /// Проверяет, запущен ли CS2 в данный момент.
    /// </summary>
    public bool IsCS2Running()
        => Process.GetProcessesByName(CS2_ProcessName).Length > 0;

    /// <summary>
    /// Запускает CS2 с указанными параметрами.
    /// </summary>
    public bool Launch(LaunchOptions options)
    {
        try
        {
            if (IsCS2Running())
            {
                LaunchFailed?.Invoke("CS2 уже запущен. Закройте текущую сессию перед запуском.");
                return false;
            }

            if (!ValidateSteamInstallation())
            {
                LaunchFailed?.Invoke("Steam не найден. Убедитесь, что Steam установлен и запущен.");
                return false;
            }

            var command = options.GetLaunchCommand();
            System.Diagnostics.Debug.WriteLine($"Launching CS2: {command}");

            if (options.UseSteamProtocol)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = command,
                    UseShellExecute = true
                });
            }
            else
            {
                if (string.IsNullOrWhiteSpace(options.ExecutablePath) || !File.Exists(options.ExecutablePath))
                {
                    LaunchFailed?.Invoke("Путь к cs2.exe не указан или файл не найден.");
                    return false;
                }

                var args = options.BuildCommandLine();
                Process.Start(new ProcessStartInfo
                {
                    FileName = options.ExecutablePath,
                    Arguments = args,
                    UseShellExecute = false
                });
            }

            GameLaunched?.Invoke(command);
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke($"Ошибка запуска: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Проверяет наличие Steam на компьютере.
    /// </summary>
    public bool ValidateSteamInstallation()
    {
        var steamPath = GetSteamPath();
        return !string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath);
    }

    /// <summary>
    /// Возвращает путь к установленному Steam.
    /// </summary>
    public string? GetSteamPath()
    {
        // Стандартные пути установки Steam
        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam")
        };

        foreach (var path in candidates)
            if (Directory.Exists(path))
                return path;

        // Реестр Windows
        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            var regPath = key?.GetValue("SteamPath")?.ToString();
            if (!string.IsNullOrEmpty(regPath) && Directory.Exists(regPath))
                return regPath.Replace('/', '\\');
        }
        catch { /* ignore registry errors */ }

        return null;
    }

    /// <summary>
    /// Пытается найти путь установки CS2 (cs2.exe) через libraryfolders.vdf.
    /// </summary>
    public string? DetectCS2Path()
    {
        var steamPath = GetSteamPath();
        if (string.IsNullOrEmpty(steamPath))
            return null;

        var libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(libraryFoldersPath))
            return null;

        try
        {
            // Парсим VDF простым способом: ищем пути вида "path" "..."
            var content = File.ReadAllText(libraryFoldersPath);
            var libraryPaths = new List<string> { steamPath };
            var matches = System.Text.RegularExpressions.Regex.Matches(
                content,
                @"""path""\s+""([^""]+)"""
            );
            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                var p = m.Groups[1].Value.Replace(@"\\", @"\");
                if (Directory.Exists(p))
                    libraryPaths.Add(p);
            }

            foreach (var lib in libraryPaths.Distinct())
            {
                var cs2Exe = Path.Combine(lib, "steamapps", "common", "Counter-Strike Global Offensive",
                    "game", "bin", "win64", "cs2.exe");
                if (File.Exists(cs2Exe))
                    return cs2Exe;
            }
        }
        catch
        {
            // ignore
        }

        return null;
    }

    /// <summary>
    /// Закрывает все процессы CS2 (если запущены).
    /// </summary>
    public void KillRunningCS2()
    {
        foreach (var proc in Process.GetProcessesByName(CS2_ProcessName))
        {
            try
            {
                proc.Kill(entireProcessTree: true);
                proc.WaitForExit(3000);
            }
            catch { /* ignore */ }
            finally { proc.Dispose(); }
        }
    }
}

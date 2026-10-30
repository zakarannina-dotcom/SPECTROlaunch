using System.Diagnostics;
using System.IO;
using CS2Launcher.Models.ARCRaiders;
using Microsoft.Win32;

namespace CS2Launcher.Services;

/// <summary>
/// Сервис для управления ARC Raiders: запуск, конфиги, моды.
/// </summary>
public class ArcRaidersService
{
    /// <summary>Событие запуска игры.</summary>
    public event Action<string>? GameLaunched;

    /// <summary>Событие ошибки запуска.</summary>
    public event Action<string>? LaunchFailed;

    /// <summary>Событие изменения статуса установки.</summary>
    public event Action<bool>? InstallationStatusChanged;

    /// <summary>
    /// Проверяет, запущен ли ARC Raiders.
    /// </summary>
    public bool IsRunning()
        => Process.GetProcessesByName(ARCRaidersInfo.ProcessName).Length > 0;

    /// <summary>
    /// Запускает ARC Raiders через Steam protocol.
    /// </summary>
    public bool Launch()
    {
        try
        {
            if (IsRunning())
            {
                LaunchFailed?.Invoke("ARC Raiders уже запущен.");
                return false;
            }

            if (!IsSteamInstalled())
            {
                LaunchFailed?.Invoke("Steam не найден. Установите Steam и запустите его.");
                return false;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = ARCRaidersInfo.SteamRunUrl,
                UseShellExecute = true
            });

            GameLaunched?.Invoke(ARCRaidersInfo.SteamRunUrl);
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke("Ошибка запуска: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Принудительно закрывает все процессы ARC Raiders.
    /// </summary>
    public void KillRunning()
    {
        foreach (var proc in Process.GetProcessesByName(ARCRaidersInfo.ProcessName))
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

    /// <summary>
    /// Проверяет, установлен ли Steam.
    /// </summary>
    public bool IsSteamInstalled()
    {
        var steamPath = GetSteamPath();
        return !string.IsNullOrEmpty(steamPath) && Directory.Exists(steamPath);
    }

    /// <summary>
    /// Возвращает путь к установленному Steam.
    /// </summary>
    public string? GetSteamPath()
    {
        var candidates = new[]
        {
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Steam"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Steam")
        };

        foreach (var path in candidates)
            if (Directory.Exists(path))
                return path;

        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Valve\Steam");
            var regPath = key?.GetValue("SteamPath")?.ToString();
            if (!string.IsNullOrEmpty(regPath) && Directory.Exists(regPath))
                return regPath.Replace('/', '\\');
        }
        catch { /* ignore */ }

        return null;
    }

    /// <summary>
    /// Проверяет, установлена ли ARC Raiders.
    /// </summary>
    public bool IsGameInstalled()
    {
        var gamePath = GetGamePath();
        if (!string.IsNullOrEmpty(gamePath) && Directory.Exists(gamePath))
        {
            InstallationStatusChanged?.Invoke(true);
            return true;
        }
        InstallationStatusChanged?.Invoke(false);
        return false;
    }

    /// <summary>
    /// Возвращает путь установки ARC Raiders (если найден).
    /// </summary>
    public string? GetGamePath()
    {
        var steamPath = GetSteamPath();
        if (string.IsNullOrEmpty(steamPath))
            return null;

        // Проверяем главную директорию Steam
        var directPath = Path.Combine(steamPath, ARCRaidersInfo.InstallSubPath);
        if (Directory.Exists(directPath))
            return directPath;

        // Парсим libraryfolders.vdf для поиска игры на других дисках
        var libraryFoldersPath = Path.Combine(steamPath, "steamapps", "libraryfolders.vdf");
        if (!File.Exists(libraryFoldersPath))
            return null;

        try
        {
            var content = File.ReadAllText(libraryFoldersPath);
            var matches = System.Text.RegularExpressions.Regex.Matches(
                content,
                @"""path""\s+""([^""]+)"""
            );

            foreach (System.Text.RegularExpressions.Match m in matches)
            {
                var libPath = m.Groups[1].Value.Replace(@"\\", @"\");
                if (Directory.Exists(libPath))
                {
                    var gamePath = Path.Combine(libPath, ARCRaidersInfo.InstallSubPath);
                    if (Directory.Exists(gamePath))
                        return gamePath;
                }
            }
        }
        catch { /* ignore */ }

        return null;
    }

    /// <summary>
    /// Возвращает путь к папке для модов (создаёт если нет).
    /// </summary>
    public string? GetModsPath()
    {
        var gamePath = GetGamePath();
        if (string.IsNullOrEmpty(gamePath))
            return null;

        var modsPath = Path.Combine(gamePath, ARCRaidersInfo.ModsSubPath);
        Directory.CreateDirectory(modsPath);
        return modsPath;
    }

    /// <summary>
    /// Применяет пресет графики: создаёт GameUserSettings.ini.
    /// </summary>
    public bool ApplyGraphicsPreset(GraphicsPreset preset)
    {
        var gamePath = GetGamePath();
        if (string.IsNullOrEmpty(gamePath))
        {
            LaunchFailed?.Invoke("ARC Raiders не найден. Установите игру через Steam.");
            return false;
        }

        try
        {
            // Стандартный путь UE5 для настроек пользователя
            var settingsDir = Path.Combine(gamePath, "ARCGame", "Saved", "Config", "WindowsNoEditor");
            Directory.CreateDirectory(settingsDir);
            var settingsPath = Path.Combine(settingsDir, "GameUserSettings.ini");

            File.WriteAllText(settingsPath, preset.GenerateIniContent());
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke("Ошибка применения пресета: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Устанавливает мод: копирует .pak файл в папку mods.
    /// </summary>
    public bool InstallMod(string pakFilePath, string modName)
    {
        var modsPath = GetModsPath();
        if (modsPath == null)
        {
            LaunchFailed?.Invoke("Не удалось найти папку для модов.");
            return false;
        }

        if (!File.Exists(pakFilePath))
        {
            LaunchFailed?.Invoke("Файл мода не найден: " + pakFilePath);
            return false;
        }

        try
        {
            var destFileName = Path.Combine(modsPath, Path.GetFileName(pakFilePath));
            File.Copy(pakFilePath, destFileName, overwrite: true);
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke("Ошибка установки мода: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Удаляет мод из папки mods.
    /// </summary>
    public bool UninstallMod(TextureMod mod)
    {
        if (string.IsNullOrEmpty(mod.FilePath) || !File.Exists(mod.FilePath))
            return false;

        try
        {
            File.Delete(mod.FilePath);
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke("Ошибка удаления мода: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Сканирует папку mods и возвращает список установленных модов.
    /// </summary>
    public List<TextureMod> ScanInstalledMods()
    {
        var result = new List<TextureMod>();
        var modsPath = GetModsPath();
        if (modsPath == null)
            return result;

        try
        {
            foreach (var file in Directory.EnumerateFiles(modsPath, "*.pak", SearchOption.AllDirectories))
            {
                var fileInfo = new FileInfo(file);
                var mod = new TextureMod
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    FilePath = file,
                    FileSize = fileInfo.Length,
                    IsEnabled = true,
                    Author = "Unknown",
                    Version = "1.0",
                    Category = ModCategory.Texture
                };
                result.Add(mod);
            }
        }
        catch { /* ignore */ }

        return result;
    }

    /// <summary>
    /// Включает/выключает мод (переименовывает .pak в .pak.disabled).
    /// </summary>
    public bool ToggleMod(TextureMod mod, bool enable)
    {
        if (string.IsNullOrEmpty(mod.FilePath))
            return false;

        try
        {
            if (enable)
            {
                var disabledPath = mod.FilePath + ".disabled";
                if (File.Exists(disabledPath))
                {
                    File.Move(disabledPath, mod.FilePath);
                }
            }
            else
            {
                if (File.Exists(mod.FilePath))
                {
                    var disabledPath = mod.FilePath + ".disabled";
                    File.Move(mod.FilePath, disabledPath);
                    mod.FilePath = disabledPath;
                }
            }
            return true;
        }
        catch (Exception ex)
        {
            LaunchFailed?.Invoke("Ошибка переключения мода: " + ex.Message);
            return false;
        }
    }
}

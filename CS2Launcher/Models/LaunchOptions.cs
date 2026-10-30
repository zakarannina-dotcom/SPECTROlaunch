using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models;

/// <summary>
/// Параметры запуска CS2.
/// </summary>
public class LaunchOptions : ObservableObject
{
    private bool _novid = true;
    private bool _nojoy = true;
    private bool _high = true;
    private bool _fullscreen = true;
    private bool _threads = false;
    private int _threadsValue = 4;
    private bool _tickrate = false;
    private int _tickrateValue = 128;
    private bool _console = true;
    private bool _noBrowser = true;
    private string _customArgs = string.Empty;
    private string _executablePath = string.Empty;
    private bool _useSteamProtocol = true;

    /// <summary>Отключает интро-видео.</summary>
    public bool NoVid { get => _novid; set => SetProperty(ref _novid, value); }

    /// <summary>Отключает поддержку геймпада.</summary>
    public bool NoJoy { get => _nojoy; set => SetProperty(ref _nojoy, value); }

    /// <summary>Высокий приоритет процесса.</summary>
    public bool High { get => _high; set => SetProperty(ref _high, value); }

    /// <summary>Полноэкранный режим.</summary>
    public bool Fullscreen { get => _fullscreen; set => SetProperty(ref _fullscreen, value); }

    /// <summary>Указать число потоков.</summary>
    public bool UseThreads { get => _threads; set => SetProperty(ref _threads, value); }

    public int ThreadsValue { get => _threadsValue; set => SetProperty(ref _threadsValue, value); }

    /// <summary>Указать tickrate (для локальных серверов).</summary>
    public bool UseTickrate { get => _tickrate; set => SetProperty(ref _tickrate, value); }

    public int TickrateValue { get => _tickrateValue; set => SetProperty(ref _tickrateValue, value); }

    /// <summary>Включить консоль.</summary>
    public bool Console { get => _console; set => SetProperty(ref _console, value); }

    /// <summary>Отключить встроенный браузер (motd).</summary>
    public bool NoBrowser { get => _noBrowser; set => SetProperty(ref _noBrowser, value); }

    /// <summary>Пользовательские аргументы.</summary>
    public string CustomArgs { get => _customArgs; set => SetProperty(ref _customArgs, value); }

    /// <summary>Путь к cs2.exe (если не используется Steam protocol).</summary>
    public string ExecutablePath
    {
        get => _executablePath;
        set => SetProperty(ref _executablePath, value);
    }

    /// <summary>Запускать через steam://run/730 (рекомендуется).</summary>
    public bool UseSteamProtocol
    {
        get => _useSteamProtocol;
        set => SetProperty(ref _useSteamProtocol, value);
    }

    /// <summary>
    /// Генерирует строку аргументов запуска.
    /// </summary>
    public string BuildCommandLine()
    {
        var args = new List<string>();

        if (NoVid) args.Add("-novid");
        if (NoJoy) args.Add("-nojoy");
        if (High) args.Add("-high");
        if (Fullscreen) args.Add("-fullscreen");
        if (UseThreads) args.Add($"-threads {ThreadsValue}");
        if (UseTickrate) args.Add($"-tickrate {TickrateValue}");
        if (Console) args.Add("-console");
        if (NoBrowser) args.Add("-disablehtmlmotd");

        if (!string.IsNullOrWhiteSpace(CustomArgs))
            args.Add(CustomArgs.Trim());

        return string.Join(" ", args);
    }

    /// <summary>
    /// Полная команда запуска (для steam protocol или прямого exe).
    /// </summary>
    public string GetLaunchCommand()
    {
        var args = BuildCommandLine();
        if (UseSteamProtocol)
            return $"steam://run/730//{args}";
        return $"\"{ExecutablePath}\" {args}";
    }
}

using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models.ARCRaiders;

/// <summary>
/// Информация об игре ARC Raiders.
/// </summary>
public static class ARCRaidersInfo
{
    /// <summary>AppID в Steam (проверь и поправь при необходимости).</summary>
    public const int AppId = 110;

    /// <summary>Название игры.</summary>
    public const string Name = "ARC Raiders";

    /// <summary>Steam protocol URL для запуска.</summary>
    public const string SteamRunUrl = "steam://run/110";

    /// <summary>Имя процесса.</summary>
    public const string ProcessName = "ARCGame";

    /// <summary>Подпапка игры в steamapps/common.</summary>
    public const string InstallSubPath = "steamapps/common/ARC Raiders";

    /// <summary>Подпапка для .pak модов (UE5 стандарт).</summary>
    public const string ModsSubPath = "ARCGame/Content/Paks/~mods";
}

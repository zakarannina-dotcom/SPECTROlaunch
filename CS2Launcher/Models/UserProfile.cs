using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace CS2Launcher.Models;

/// <summary>
/// Профиль пользователя Steam после авторизации через OpenID.
/// </summary>
public class UserProfile : ObservableObject
{
    private string _steamId = string.Empty;
    private string _personaName = string.Empty;
    private string _avatarUrl = string.Empty;
    private string _profileUrl = string.Empty;
    private bool _isLoggedIn;

    public string SteamId
    {
        get => _steamId;
        set => SetProperty(ref _steamId, value);
    }

    public string PersonaName
    {
        get => _personaName;
        set => SetProperty(ref _personaName, value);
    }

    public string AvatarUrl
    {
        get => _avatarUrl;
        set => SetProperty(ref _avatarUrl, value);
    }

    public string ProfileUrl
    {
        get => _profileUrl;
        set => SetProperty(ref _profileUrl, value);
    }

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set => SetProperty(ref _isLoggedIn, value);
    }

    public ObservableCollection<GameStats> RecentMatches { get; } = new();
}

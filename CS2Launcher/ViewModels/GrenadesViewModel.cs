using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CS2Launcher.Models;
using CS2Launcher.Services;

namespace CS2Launcher.ViewModels;

/// <summary>
/// ViewModel вкладки гранат CS2.
/// </summary>
public partial class GrenadesViewModel : ObservableObject
{
    [ObservableProperty]
    private CS2MapInfo? _selectedMap;

    [ObservableProperty]
    private GrenadeLineup? _selectedLineup;

    [ObservableProperty]
    private GrenadeType? _typeFilter;

    [ObservableProperty]
    private LineupSide? _sideFilter;

    [ObservableProperty]
    private string _searchText = string.Empty;

    /// <summary>Все раскидки (полный каталог).</summary>
    public ObservableCollection<GrenadeLineup> AllLineups { get; } = new();

    /// <summary>Отфильтрованный список для отображения.</summary>
    public ObservableCollection<GrenadeLineup> FilteredLineups { get; } = new();

    /// <summary>Список карт.</summary>
    public ObservableCollection<CS2MapInfo> Maps { get; } = new();

    /// <summary>Типы гранат для фильтра.</summary>
    public ObservableCollection<GrenadeType> GrenadeTypes { get; } = new()
    {
        GrenadeType.Smoke, GrenadeType.Flash, GrenadeType.HE, GrenadeType.Molotov
    };

    /// <summary>Стороны для фильтра.</summary>
    public ObservableCollection<LineupSide> Sides { get; } = new()
    {
        LineupSide.Attack, LineupSide.Defense, LineupSide.Both
    };

    public ICommand OpenVideoCommand { get; }
    public ICommand ToggleFavoriteCommand { get; }
    public ICommand SelectMapCommand { get; }
    public ICommand SelectLineupCommand { get; }

    public GrenadesViewModel()
    {
        // Загружаем карты
        foreach (var map in CS2MapInfo.GetActiveDutyMaps())
            Maps.Add(map);

        // Загружаем все раскидки
        foreach (var l in GrenadeCatalog.GetAll())
            AllLineups.Add(l);

        // По умолчанию выбираем Dust II
        SelectedMap = Maps.FirstOrDefault();
        ApplyFilter();

        OpenVideoCommand = new RelayCommand<GrenadeLineup>(OpenVideo);
        ToggleFavoriteCommand = new RelayCommand<GrenadeLineup>(ToggleFavorite);
        SelectMapCommand = new RelayCommand<CS2MapInfo>(map =>
        {
            SelectedMap = map;
        });
        SelectLineupCommand = new RelayCommand<GrenadeLineup>(lineup =>
        {
            SelectedLineup = lineup;
        });
    }

    partial void OnSelectedMapChanged(CS2MapInfo? value) => ApplyFilter();
    partial void OnTypeFilterChanged(GrenadeType? value) => ApplyFilter();
    partial void OnSideFilterChanged(LineupSide? value) => ApplyFilter();
    partial void OnSearchTextChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        FilteredLineups.Clear();
        var search = (SearchText ?? string.Empty).Trim().ToLowerInvariant();

        foreach (var l in AllLineups)
        {
            // Фильтр по карте
            if (SelectedMap != null && l.MapName != SelectedMap.Name)
                continue;

            // Фильтр по типу
            if (TypeFilter.HasValue && l.GrenadeType != TypeFilter.Value)
                continue;

            // Фильтр по стороне
            if (SideFilter.HasValue && l.Side != SideFilter.Value)
                continue;

            // Поиск по тексту
            if (!string.IsNullOrEmpty(search))
            {
                if (!l.Name.ToLowerInvariant().Contains(search) &&
                    !l.Description.ToLowerInvariant().Contains(search) &&
                    !l.StandPosition.ToLowerInvariant().Contains(search) &&
                    !l.Tags.Any(t => t.ToLowerInvariant().Contains(search)))
                    continue;
            }

            FilteredLineups.Add(l);
        }
    }

    private void OpenVideo(GrenadeLineup? lineup)
    {
        if (lineup == null || string.IsNullOrEmpty(lineup.VideoUrl))
            return;

        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = lineup.VideoUrl,
                UseShellExecute = true
            });
        }
        catch { /* ignore */ }
    }

    private void ToggleFavorite(GrenadeLineup? lineup)
    {
        if (lineup != null)
            lineup.IsFavorite = !lineup.IsFavorite;
    }
}

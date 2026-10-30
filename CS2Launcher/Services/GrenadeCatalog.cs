using CS2Launcher.Models;

namespace CS2Launcher.Services;

/// <summary>
/// Каталог раскидок гранат для CS2.
/// Захардкоженные данные по популярным картам.
/// </summary>
public static class GrenadeCatalog
{
    /// <summary>
    /// Возвращает все раскидки гранат (для всех карт).
    /// </summary>
    public static List<GrenadeLineup> GetAll()
    {
        var result = new List<GrenadeLineup>();
        result.AddRange(GetDust2Lineups());
        result.AddRange(GetMirageLineups());
        result.AddRange(GetInfernoLineups());
        result.AddRange(GetNukeLineups());
        result.AddRange(GetOverpassLineups());
        result.AddRange(GetAncientLineups());
        result.AddRange(GetAnubisLineups());
        result.AddRange(GetVertigoLineups());
        result.AddRange(GetTrainLineups());
        return result;
    }

    // ============================================================
    // DUST II
    // ============================================================
    public static List<GrenadeLineup> GetDust2Lineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (CT Spawn)",
                MapName = "de_dust2",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "Встань у ящиков на A site, у стены CT spawn",
                AimLandmark = "Прицел на верхний угол левой стены CT spawn, чуть выше кирпича",
                JumpThrow = false,
                LeftClick = true,
                Description = "Перекрывает обзор CT spawn → A site",
                VideoUrl = "https://youtu.be/example-dust2-a-smoke",
                Steps = { "Зайди на A site с короткого", "Встань вплотную к ящикам у стены", "Прицелься в верхний левый угол стены", "Брось ЛКМ (без прыжка)" },
                Tags = { "A-site", "smoke", "CT-spawn" }
            },
            new()
            {
                Name = "B Site Smoke (Tunnels)",
                MapName = "de_dust2",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "У выхода из upper tunnels",
                AimLandmark = "Прицел на угол над дверью B site",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает обзор с B на plat",
                Steps = { "Зайди в upper tunnels", "Встань у левой стены", "Прицелься на угол над дверью", "Брось jumpthrow" },
                Tags = { "B-site", "smoke", "tunnels" }
            },
            new()
            {
                Name = "Mid Flash (Doors)",
                MapName = "de_dust2",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "У T spawn, у ящика",
                AimLandmark = "Прицел над mid doors, чуть правее центра",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на mid / catwalk",
                Steps = { "Встань у ящика T spawn", "Прицелься над mid doors", "Брось ЛКМ", "Беги на catwalk" },
                Tags = { "mid", "flash" }
            },
            new()
            {
                Name = "A Long Molotov (CT)",
                MapName = "de_dust2",
                GrenadeType = GrenadeType.Molotov,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "У CT spawn, у входа на A long",
                AimLandmark = "Прицел на угол A long doors",
                JumpThrow = false,
                LeftClick = true,
                Description = "Сжигает T при пуше с A long",
                Steps = { "Займи позицию у CT spawn", "Прицелься на угол A long", "Брось молотов", "Займи позицию" },
                Tags = { "A-long", "molotov", "defense" }
            },
            new()
            {
                Name = "B Site HE (Plat)",
                MapName = "de_dust2",
                GrenadeType = GrenadeType.HE,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "B site, у ящиков plat",
                AimLandmark = "Прицел на угол над дверью",
                JumpThrow = false,
                LeftClick = true,
                Description = "Дамажит T при пуше из tunnels",
                Steps = { "Займи позицию у plat", "Прицелься на угол двери", "Брось HE", "Беги за ящики" },
                Tags = { "B-site", "HE", "defense" }
            }
        };
    }

    // ============================================================
    // MIRAGE
    // ============================================================
    public static List<GrenadeLineup> GetMirageLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Stairs)",
                MapName = "de_mirage",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "У T-ramp, в углу",
                AimLandmark = "Прицел на верхний угол над stairs, у листвы",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает CT spawn → A site",
                VideoUrl = "https://youtu.be/example-mirage-a-smoke",
                Steps = { "Зайди к T-ramp", "Встань в углу у стены", "Прицелься на угол над stairs", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "stairs" }
            },
            new()
            {
                Name = "Mid Smoke (Top Mid)",
                MapName = "de_mirage",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Hard,
                StandPosition = "У T spawn, у правой стены",
                AimLandmark = "Прицел на угол крыши топ mid",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает обзор window → mid",
                Steps = { "Встань у T spawn правой стены", "Прицелься на угол крыши топ mid", "Брось jumpthrow", "Беги под лавку" },
                Tags = { "mid", "smoke", "top-mid" }
            },
            new()
            {
                Name = "B Site Flash (Window)",
                MapName = "de_mirage",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "У apartments, у выхода на B",
                AimLandmark = "Прицел над window, чуть выше",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на B site",
                Steps = { "Зайди в apartments", "У выхода на B прицелься над окном", "Брось ЛКМ", "Выходи на B" },
                Tags = { "B-site", "flash", "apartments" }
            },
            new()
            {
                Name = "A Site Molotov (Firebox)",
                MapName = "de_mirage",
                GrenadeType = GrenadeType.Molotov,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "A site, у входа",
                AimLandmark = "Прицел на firebox (угол ящиков)",
                JumpThrow = false,
                LeftClick = true,
                Description = "Выкуривает CT из firebox",
                Steps = { "Зайди на A site", "Прицелься на firebox", "Брось молотов", "Бери плент" },
                Tags = { "A-site", "molotov", "firebox" }
            },
            new()
            {
                Name = "Jungle Smoke (CT)",
                MapName = "de_mirage",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "У CT spawn, в углу",
                AimLandmark = "Прицел на угол jungle",
                JumpThrow = false,
                LeftClick = true,
                Description = "Перекрывает jungle для ретейка",
                Steps = { "Встань в углу CT spawn", "Прицелься на угол jungle", "Брось ЛКМ" },
                Tags = { "jungle", "smoke", "defense" }
            }
        };
    }

    // ============================================================
    // INFERNO
    // ============================================================
    public static List<GrenadeLineup> GetInfernoLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "Banana Smoke (CT)",
                MapName = "de_inferno",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "CT spawn, у ящиков",
                AimLandmark = "Прицел на верхний угол banana",
                JumpThrow = false,
                LeftClick = true,
                Description = "Перекрывает banana для дефа B",
                VideoUrl = "https://youtu.be/example-inferno-banana",
                Steps = { "Встань у ящиков CT spawn", "Прицелься на угол banana", "Брось ЛКМ" },
                Tags = { "banana", "smoke", "defense", "B" }
            },
            new()
            {
                Name = "A Site Smoke (Graveyard)",
                MapName = "de_inferno",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Hard,
                StandPosition = "У T spawn, у стены",
                AimLandmark = "Прицел на крышу над graveyard",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает CT spawn → A",
                Steps = { "Встань у стены T spawn", "Прицелься на крышу над graveyard", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "graveyard" }
            },
            new()
            {
                Name = "Boiler Flash (T)",
                MapName = "de_inferno",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "У выхода из apartments",
                AimLandmark = "Прицел над boiler",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на short",
                Steps = { "Зайди в apartments", "У выхода прицелься над boiler", "Брось ЛКМ", "Выходи" },
                Tags = { "apartments", "flash", "boiler" }
            },
            new()
            {
                Name = "Pit Molotov (CT)",
                MapName = "de_inferno",
                GrenadeType = GrenadeType.Molotov,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "Pit, у стены",
                AimLandmark = "Прицел на угол entrance",
                JumpThrow = false,
                LeftClick = true,
                Description = "Сжигает T при пуше на A",
                Steps = { "Займи pit", "Прицелься на угол входа", "Брось молотов" },
                Tags = { "pit", "molotov", "defense", "A" }
            }
        };
    }

    // ============================================================
    // NUKE
    // ============================================================
    public static List<GrenadeLineup> GetNukeLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "Outside Smoke (Hangar)",
                MapName = "de_nuke",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "T spawn, у контейнера",
                AimLandmark = "Прицел на угол hangar",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает outside для перехода",
                VideoUrl = "https://youtu.be/example-nuke-outside",
                Steps = { "Встань у контейнера T spawn", "Прицелься на угол hangar", "Брось jumpthrow" },
                Tags = { "outside", "smoke", "hangar" }
            },
            new()
            {
                Name = "Ramp HE (CT)",
                MapName = "de_nuke",
                GrenadeType = GrenadeType.HE,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "Ramp, у ящиков",
                AimLandmark = "Прицел на угол ramp entrance",
                JumpThrow = false,
                LeftClick = true,
                Description = "Дамажит T при пуше ramp",
                Steps = { "Займи позицию у ramp", "Прицелься на угол входа", "Брось HE" },
                Tags = { "ramp", "HE", "defense" }
            },
            new()
            {
                Name = "Heaven Smoke (CT)",
                MapName = "de_nuke",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Hard,
                StandPosition = "Heaven, у перил",
                AimLandmark = "Прицел на угол над B site",
                JumpThrow = false,
                LeftClick = true,
                Description = "Перекрывает обзор на B site сверху",
                Steps = { "Поднимись на heaven", "Встань у перил", "Прицелься на угол над B", "Брось ЛКМ" },
                Tags = { "heaven", "smoke", "defense", "B" }
            }
        };
    }

    // ============================================================
    // OVERPASS
    // ============================================================
    public static List<GrenadeLineup> GetOverpassLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Bank)",
                MapName = "de_overpass",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "У bank, в углу",
                AimLandmark = "Прицел на угол над A site",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает bank → A site",
                Steps = { "Зайди к bank", "Встань в углу", "Прицелься на угол над A", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "bank" }
            },
            new()
            {
                Name = "B Site Flash (Water)",
                MapName = "de_overpass",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "Water, у выхода",
                AimLandmark = "Прицел над B site",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на B site",
                Steps = { "Зайди в water", "У выхода прицелься над B", "Брось ЛКМ", "Беги на B" },
                Tags = { "B-site", "flash", "water" }
            },
            new()
            {
                Name = "Connector Molotov (CT)",
                MapName = "de_overpass",
                GrenadeType = GrenadeType.Molotov,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "Connector, у стены",
                AimLandmark = "Прицел на угол входа",
                JumpThrow = false,
                LeftClick = true,
                Description = "Сжигает T в connector",
                Steps = { "Займи connector", "Прицелься на угол входа", "Брось молотов" },
                Tags = { "connector", "molotov", "defense" }
            }
        };
    }

    // ============================================================
    // ANCIENT
    // ============================================================
    public static List<GrenadeLineup> GetAncientLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Donut)",
                MapName = "de_ancient",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Hard,
                StandPosition = "T spawn, у колонны",
                AimLandmark = "Прицел на угол donut",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает donut → A",
                Steps = { "Встань у колонны T spawn", "Прицелься на угол donut", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "donut" }
            },
            new()
            {
                Name = "B Site Flash (Cave)",
                MapName = "de_ancient",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "Cave, у выхода",
                AimLandmark = "Прицел над B site",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на B",
                Steps = { "Зайди в cave", "Прицелься над B", "Брось ЛКМ" },
                Tags = { "B-site", "flash", "cave" }
            }
        };
    }

    // ============================================================
    // ANUBIS
    // ============================================================
    public static List<GrenadeLineup> GetAnubisLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Chest)",
                MapName = "de_anubis",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "T spawn, у ящиков",
                AimLandmark = "Прицел на угол над A site",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает chest → A",
                Steps = { "Встань у ящиков T spawn", "Прицелься на угол над A", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "chest" }
            },
            new()
            {
                Name = "B Site Molotov (CT)",
                MapName = "de_anubis",
                GrenadeType = GrenadeType.Molotov,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "B site, у входа",
                AimLandmark = "Прицел на угол entrance",
                JumpThrow = false,
                LeftClick = true,
                Description = "Сжигает T при пуше B",
                Steps = { "Займи B site", "Прицелься на угол входа", "Брось молотов" },
                Tags = { "B-site", "molotov", "defense" }
            }
        };
    }

    // ============================================================
    // VERTIGO
    // ============================================================
    public static List<GrenadeLineup> GetVertigoLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Ramp)",
                MapName = "de_vertigo",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Hard,
                StandPosition = "Ramp, у перил",
                AimLandmark = "Прицел на угол над A site",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает ramp → A",
                Steps = { "Поднимись по ramp", "Встань у перил", "Прицелься на угол над A", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "ramp" }
            },
            new()
            {
                Name = "B Site Flash (Mid)",
                MapName = "de_vertigo",
                GrenadeType = GrenadeType.Flash,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "Mid, у выхода",
                AimLandmark = "Прицел над B site",
                JumpThrow = false,
                LeftClick = true,
                Description = "Ослепляет CT на B",
                Steps = { "Зайди в mid", "Прицелься над B", "Брось ЛКМ" },
                Tags = { "B-site", "flash", "mid" }
            }
        };
    }

    // ============================================================
    // TRAIN (вернулась в CS2)
    // ============================================================
    public static List<GrenadeLineup> GetTrainLineups()
    {
        return new List<GrenadeLineup>
        {
            new()
            {
                Name = "A Site Smoke (Popdog)",
                MapName = "de_train",
                GrenadeType = GrenadeType.Smoke,
                Side = LineupSide.Attack,
                Difficulty = LineupDifficulty.Medium,
                StandPosition = "Popdog, у стены",
                AimLandmark = "Прицел на угол над A site",
                JumpThrow = true,
                LeftClick = true,
                Description = "Перекрывает popdog → A",
                Steps = { "Зайди к popdog", "Встань у стены", "Прицелься на угол над A", "Брось jumpthrow" },
                Tags = { "A-site", "smoke", "popdog" }
            },
            new()
            {
                Name = "B Site HE (CT)",
                MapName = "de_train",
                GrenadeType = GrenadeType.HE,
                Side = LineupSide.Defense,
                Difficulty = LineupDifficulty.Easy,
                StandPosition = "B site, у ящиков",
                AimLandmark = "Прицел на угол entrance",
                JumpThrow = false,
                LeftClick = true,
                Description = "Дамажит T при пуше B",
                Steps = { "Займи позицию на B", "Прицелься на угол входа", "Брось HE" },
                Tags = { "B-site", "HE", "defense" }
            }
        };
    }
}

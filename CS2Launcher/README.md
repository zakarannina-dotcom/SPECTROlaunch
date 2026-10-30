# CS2 Launcher — WPF/C# приложение

Кастомный лаунчер для Counter-Strike 2 в стиле Faceit с тёмной красно-чёрной палитрой, двуязычным интерфейсом (RU/EN) и поддержкой авторизации через Steam OpenID.

## Возможности

- **Запуск игры** с настраиваемыми параметрами (`-novid`, `-high`, `-tickrate`, `-threads` и др.)
- **Управление конфигами** — генерация `autoexec.cfg` с настройками прицела, мыши и графики
- **Статистика игрока** — общая статистика и список недавних матчей
- **Steam OpenID авторизация** — вход через Steam с получением профиля и SteamID
- **Двуязычный интерфейс** — переключение между русским и английским
- **Детектор установки** — автоматический поиск `cs2.exe` и путей Steam

## Требования

- **Windows 10/11** (WPF — это Windows-only технология)
- **.NET 8 SDK** — [скачать с microsoft.com](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** (17.8+) с компонентом **".NET desktop development"**
  *или* **VS Code** с расширением **C# Dev Kit**
- **Steam** (установленный и залогиненный)
- **Counter-Strike 2** (бесплатно в Steam, AppID 730)

## Структура проекта

```
CS2Launcher/
├── CS2Launcher.csproj          # .NET 8 WPF-проект
├── app.manifest                # Манифест (DPI awareness, admin rights)
├── App.xaml / App.xaml.cs      # Точка входа WPF
├── MainWindow.xaml             # Главное окно (сайдбар + навигация + titlebar)
├── Models/
│   ├── UserProfile.cs          # Модель пользователя Steam (ObservableObject)
│   ├── GameStats.cs            # Статистика (матч + общая)
│   ├── CS2Config.cs            # Конфиг CS2 (прицел, мышь, графика)
│   └── LaunchOptions.cs        # Параметры запуска игры
├── Services/
│   ├── SteamAuthService.cs     # OpenID 2.0 flow с HttpListener
│   ├── SteamApiService.cs      # Steam Web API (профиль, статистика)
│   ├── GameLauncherService.cs  # Запуск CS2 + детектор путей
│   ├── ConfigService.cs        # Сохранение/чтение настроек и autoexec.cfg
│   └── LocalizationService.cs  # Переключение RU/EN
├── ViewModels/                 # MVVM (CommunityToolkit.Mvvm)
│   ├── MainViewModel.cs        # Главная VM, навигация, авторизация
│   ├── LaunchViewModel.cs      # Вкладка запуска
│   ├── ConfigViewModel.cs     # Вкладка конфига
│   ├── StatsViewModel.cs       # Вкладка статистики
│   └── Converters.cs          # ValueConverter'ы (Bool→Vis, Result→Color и т.д.)
├── Views/
│   ├── LaunchView.xaml         # UI вкладки запуска
│   ├── ConfigView.xaml         # UI вкладки конфига (с предпросмотром прицела)
│   └── StatsView.xaml          # UI вкладки статистики
├── Resources/
│   ├── Styles.xaml             # Палитра Faceit-like + стили контролов
│   ├── Strings.ru-RU.xaml      # Русская локализация
│   └── Strings.en-US.xaml      # Английская локализация
└── README.md                   # Этот файл
```

## Сборка

### Вариант 1: Visual Studio 2022

1. Открой папку `CS2Launcher/` (или файл `CS2Launcher.csproj`) в Visual Studio 2022.
2. Дождись восстановления NuGet-пакетов (Tools → NuGet Package Manager → Restore).
3. Выбери конфигурацию **Debug** или **Release**, платформа **x64** или **Any CPU**.
4. Нажми **F5** (с отладкой) или **Ctrl+F5** (без отладки) для запуска.
5. Для сборки инсталлятора: Build → Build Solution. Исполняемый файл появится в
   `bin/Release/net8.0-windows/CS2Launcher.exe`.

### Вариант 2: .NET CLI (командная строка)

```powershell
cd CS2Launcher

# Очистить предыдущую сборку (если была)
dotnet clean

# Восстановить пакеты
dotnet restore

# Сборка Debug
dotnet build

# Сборка Release (оптимизированная)
dotnet build -c Release

# Запуск
dotnet run
```

> **Если пересобираешь проект после правок**: всегда выполняй `dotnet clean` перед `dotnet build`,
> иначе Visual Studio может не подхватить изменения в `.csproj`.

> **При ошибке NETSDK1022** (дублирование .xaml файлов): .NET 8 Windows Desktop SDK
> автоматически включает все `.xaml` как `Page`, а `App.xaml` как `ApplicationDefinition`.
> Не дублируй их вручную в `.csproj` — это уже исправлено в текущей версии проекта.

Готовый `CS2Launcher.exe` появится в `bin/Release/net8.0-windows/`.

## Настройка Steam API (опционально, но рекомендуется)

Чтобы получить реальные данные профиля (имя, аватар) и статистику CS2, нужен Steam Web API-ключ.

### 1. Получите API-ключ

1. Откройте [https://steamcommunity.com/dev/apikey](https://steamcommunity.com/dev/apikey)
2. Войдите в свой Steam-аккаунт.
3. Введите имя домена (например, `localhost`) и зарегистрируйте ключ.
4. Скопируйте 32-символьный API-ключ.

### 2. Сохраните ключ в лаунчере

Приложение ищет ключ в файле:

```
%APPDATA%\CS2Launcher\steam_api_key.txt
```

Создай этот файл вручную и вставь свой ключ (одной строкой, без переносов):

```powershell
notepad "%APPDATA%\CS2Launcher\steam_api_key.txt"
```

Вставь ключ и сохрани файл. После перезапуска лаунчер автоматически подхватит ключ.

> **Без API-ключа** лаунчер по-прежнему работает, но использует демонстрационные данные
> профиля ("Demo Player") и моковую статистику.

## Настройка Steam OpenID (требуется для входа в Steam)

Лаунчер использует локальный HTTP-сервер на порту **8080** для приёма callback'а OpenID.

### Требования

1. **Порт 8080** должен быть свободен (проверь: `netstat -an | findstr :8080`).
2. Если порт занят другим приложением, измени константу `CallbackPort`
   в `Services/SteamAuthService.cs` (например, на 8484).
3. **Брандмауэр Windows** может запросить разрешение — разреши для приватных сетей.

### Поток авторизации

1. Пользователь нажимает "Войти через Steam".
2. Запускается локальный HttpListener на `http://localhost:8080/`.
3. Открывается браузер с страницей Steam OpenID.
4. После подтверждения Steam редиректит на `http://localhost:8080/auth/steam/callback`
   с параметрами, включая `openid.identity = https://steamcommunity.com/openid/id/{steamid}`.
5. Лаунчер парсит SteamID64 и закрывает HttpListener.

> **Важно для продакшена**: текущая реализация доверяет callback-редиректу.
> Для полноценной серверной валидации нужно отправлять POST-запрос в Steam
> с `openid.mode=check_authentication` и проверять ответ `is_valid:true`.
> Это можно добавить в метод `ValidateAndExtractSteamId`.

## Использование

1. **Запусти лаунчер** (`CS2Launcher.exe`).
2. **Войди в Steam** — кнопка "Войти через Steam" в левой панели. Откроется браузер,
   подтвердите вход. После возврата в лаунчер отобразится профиль.
3. **Вкладка "Запуск"** — настрой параметры запуска (галочки + кастомные аргументы),
   нажми "ИГРАТЬ". Статус CS2 отображается в виде зелёной точки.
4. **Вкладка "Конфиг"** — настрой прицел (size, thickness, gap, color, dot),
   чувствительность мыши, FOV. Предпросмотр прицела обновляется в реальном времени.
   Нажми "Записать autoexec.cfg" — файл появится в
   `Steam/steamapps/common/Counter-Strike Global Offensive/game/csgo/cfg/autoexec.cfg`.
5. **Вкладка "Статистика"** — общая статистика (матчи, K/D, винрейт, часы) и
   список недавних матчей. Кнопка "Обновить" перезагружает данные из Steam API.
6. **Переключение языка** — кнопка "RU/EN" внизу левой панели. Выбор сохраняется
   в `%APPDATA%\CS2Launcher\language.txt`.

## Известные ограничения

- **WPF-специфично**: работает только на Windows.
- **Steam Web API не отдаёт матчи** CS2 — данные о недавних матчах приходят из моковых
  источников. Для реальных матчей используй сторонние API: **Faceit API** ([developers.faceit.com](https://developers.faceit.com)),
  **Leetify API** или **Tracker.gg API**.
- **OpenID без server-side валидации** — доверяет callback-редиректу. Для публичного
  релиза добавь проверку `check_authentication`.
- **Несовместимость с античитом**: лаунчер не взаимодействует с VAC или Trusted Mode
  CS2. Все параметры запуска стандартные и не нарушают правила Valve.

## Технологии

| Компонент | Библиотека / Технология |
|-----------|------------------------|
| UI Framework | WPF (.NET 8) |
| MVVM | CommunityToolkit.Mvvm 8.2 |
| JSON | Newtonsoft.Json 13.0 + System.Text.Json |
| HTTP | System.Net.Http + HttpListener |
| Localization | Custom XAML ResourceDictionary + MarkupExtension |
| DI | Microsoft.Extensions.DependencyInjection |

## Лицензия и дисклеймер

Этот проект — **неофициальный** и не связан с Valve Corporation.
"Counter-Strike 2", "Steam" и связанные торговые марки принадлежат Valve Corporation.
Используй на свой страх и риск. Автор не несёт ответственности за возможные
ограничения Steam-аккаунта при использовании сторонних инструментов.

## Частые проблемы

### "Порт 8080 занят"
- Измени `CallbackPort` в `Services/SteamAuthService.cs` (например, на `8484`).
- Обнови `ReturnUrl` и `Realm` на новый порт.
- Перезапусти приложение.

### "Steam не найден" при запуске
- Убедись, что Steam установлен в стандартную директорию (`C:\Program Files (x86)\Steam`).
- Если установлен в нестандартную директорию, добавь путь в реестр:
  `HKEY_CURRENT_USER\Software\Valve\Steam\SteamPath`.

### "Статистика пустая / показывает Demo Player"
- Не настроен Steam API-ключ (см. раздел "Настройка Steam API").
- Приватный профиль Steam: открой [настройки приватности](https://steamcommunity.com/my/edit/settings)
  и установи "Game details" → Public.

### "cs2.exe не найден"
- Открой Steam и проверь, что CS2 действительно установлена.
- На вкладке "Запуск" отключи чекбокс "Запускать через Steam" и укажи путь к
  `cs2.exe` вручную (кнопка "Обзор...").

## Контакты и развитие

Проект легко расширяется. Возможные дополнения:
- Интеграция с **Faceit API** для реальной статистики матчей
- **Crosshair-импорт/экспорт** (share codes)
- **Bhop trainer**, **movement maps launcher**
- **Авто-обновление** конфигов через cloud-синхронизацию
- **Auto-launch при старте Windows**

Удачной игры! 🎮

using System.Windows;

namespace CS2Launcher;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Локализация в XAML использует DynamicResource — строки обновляются
        // автоматически при смене ResourceDictionary в LocalizationService.
        // Никаких ручных уведомлений не требуется.
    }
}

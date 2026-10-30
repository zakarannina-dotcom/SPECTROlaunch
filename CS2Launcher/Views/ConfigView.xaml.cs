using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using CS2Launcher.ViewModels;
using CS2Launcher.Models;

namespace CS2Launcher.Views;

public partial class ConfigView : UserControl
{
    public ConfigView()
    {
        InitializeComponent();
        DataContextChanged += (_, _) =>
        {
            if (DataContext is ConfigViewModel vm)
            {
                vm.Config.PropertyChanged += (_, _) => UpdateCrosshairPositions();
            }
        };
        Loaded += (_, _) => UpdateCrosshairPositions();
    }

    /// <summary>
    /// Пересчитывает позиции 4 линий прицела на основе gap, size, thickness.
    /// Canvas 200x200, центр в (100, 100).
    /// </summary>
    private void UpdateCrosshairPositions()
    {
        if (DataContext is not ConfigViewModel vm)
            return;

        var cfg = vm.Config;
        const double center = 100.0;

        // gap влияет на расстояние от центра до линии
        // в CS2 gap ~ -2 пикселя за единицу, домножим
        var gapOffset = cfg.CrosshairGap * 2.0;
        var thickness = cfg.CrosshairThickness;
        var size = cfg.CrosshairSize;

        // Толщина линии = thickness, размещаем по центру
        var thicknessOffset = thickness / 2.0;

        // ВЕРХНЯЯ ЛИНИЯ: x = center - thickness/2, y = center - gapOffset - size
        Canvas.SetLeft(LineTop, center - thicknessOffset);
        Canvas.SetTop(LineTop, center - gapOffset - size);

        // НИЖНЯЯ ЛИНИЯ: x = center - thickness/2, y = center + gapOffset
        Canvas.SetLeft(LineBottom, center - thicknessOffset);
        Canvas.SetTop(LineBottom, center + gapOffset);

        // ЛЕВАЯ ЛИНИЯ: x = center - gapOffset - size, y = center - thickness/2
        Canvas.SetLeft(LineLeft, center - gapOffset - size);
        Canvas.SetTop(LineLeft, center - thicknessOffset);

        // ПРАВАЯ ЛИНИЯ: x = center + gapOffset, y = center - thickness/2
        Canvas.SetLeft(LineRight, center + gapOffset);
        Canvas.SetTop(LineRight, center - thicknessOffset);

        // ТОЧКА: в центре
        Canvas.SetLeft(Dot, center - thicknessOffset);
        Canvas.SetTop(Dot, center - thicknessOffset);
    }

    /// <summary>
    /// Обработчик клика по цветному образцу.
    /// </summary>
    private void ColorSwatch_Click(object sender, MouseButtonEventArgs e)
    {
        if (sender is Rectangle rect && rect.Tag is string colorHex)
        {
            // Сбрасываем обводку у всех образцов
            var defaultBrush = (Brush)FindResource("BorderLightBrush");
            var parent = rect.Parent;
            if (parent is Panel panel)
            {
                foreach (var child in panel.Children)
                {
                    if (child is Rectangle r)
                    {
                        r.Stroke = defaultBrush;
                        r.StrokeThickness = 1;
                    }
                }
            }
            rect.Stroke = Brushes.White;
            rect.StrokeThickness = 2;

            // Обновляем ViewModel
            if (DataContext is ConfigViewModel vm)
            {
                vm.Config.CrosshairColor = colorHex;
            }

            UpdateCrosshairPositions();
        }
    }
}

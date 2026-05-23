using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AlSa3d.Desktop.Services;

/// <summary>
/// خدمة الإشعارات - Notification Service
/// </summary>
public class NotificationService : INotificationService
{
    private Grid? _notificationContainer;
    
    public void SetNotificationContainer(Grid container)
    {
        _notificationContainer = container;
    }

    public void ShowSuccess(string message)
    {
        ShowNotification(message, "#10B981", "✓"); // أخضر
    }

    public void ShowError(string message)
    {
        ShowNotification(message, "#EF4444", "✗"); // أحمر
    }

    public void ShowWarning(string message)
    {
        ShowNotification(message, "#F59E0B", "⚠"); // برتقالي
    }

    public void ShowInfo(string message)
    {
        ShowNotification(message, "#3B82F6", "ℹ"); // أزرق
    }

    private void ShowNotification(string message, string colorHex, string icon)
    {
        if (_notificationContainer == null)
        {
            MessageBox.Show(message, "إشعار", MessageBoxButton.OK, GetImageFromColor(colorHex));
            return;
        }

        // إنشاء عنصر الإشعار
        var border = new Border
        {
            Background = (new BrushConverter().ConvertFromString(colorHex) as Brush) ?? Brushes.White,
            CornerRadius = new CornerRadius(8),
            Padding = new Thickness(15),
            Margin = new Thickness(5),
            Width = 350,
            HorizontalAlignment = HorizontalAlignment.Right
        };

        var grid = new Grid();
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

        var iconText = new TextBlock
        {
            Text = icon,
            FontSize = 20,
            FontWeight = FontWeights.Bold,
            Foreground = Brushes.White,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 10, 0)
        };

        var messageText = new TextBlock
        {
            Text = message,
            FontSize = 14,
            Foreground = Brushes.White,
            TextWrapping = TextWrapping.Wrap,
            VerticalAlignment = VerticalAlignment.Center
        };

        Grid.SetColumn(iconText, 0);
        Grid.SetColumn(messageText, 1);

        grid.Children.Add(iconText);
        grid.Children.Add(messageText);
        border.Child = grid;

        _notificationContainer.Children.Add(border);

        // إزالة الإشعار بعد 4 ثواني
        var dispatcherTimer = new System.Windows.Threading.DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(4)
        };
        
        dispatcherTimer.Tick += (s, e) =>
        {
            dispatcherTimer.Stop();
            _notificationContainer.Children.Remove(border);
        };
        
        dispatcherTimer.Start();
    }

    private MessageBoxImage GetImageFromColor(string colorHex)
    {
        return colorHex switch
        {
            "#10B981" => MessageBoxImage.Information,
            "#EF4444" => MessageBoxImage.Error,
            "#F59E0B" => MessageBoxImage.Warning,
            _ => MessageBoxImage.Information
        };
    }
}

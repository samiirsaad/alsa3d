using System.Windows;

namespace AlSa3d.Desktop.Services;

/// <summary>
/// خدمة النوافذ المنبثقة والحوارات - Dialog Service
/// </summary>
public class DialogService : IDialogService
{
    public MessageBoxResult ShowMessage(string message, string title = "تنبيه", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information)
    {
        return MessageBox.Show(message, title, buttons, image);
    }

    public bool ShowConfirm(string message, string title = "تأكيد")
    {
        var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
        return result == MessageBoxResult.Yes;
    }

    public T? ShowDialog<T>(string title = "", int width = 400, int height = 300) where T : Window
    {
        try
        {
            var window = Activator.CreateInstance<T>();
            
            if (!string.IsNullOrEmpty(title))
            {
                window.Title = title;
            }
            
            window.Width = width;
            window.Height = height;
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            
            var result = window.ShowDialog();
            
            if (result == true)
            {
                return window;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خطأ في فتح النافذة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        
        return null;
    }
}

using System.Windows;

namespace AlSa3d.Desktop.Services;

/// <summary>
/// خدمة التنقل بين الشاشات
/// </summary>
public interface INavigationService
{
    void NavigateTo<TViewModel>() where TViewModel : class;
    void GoBack();
    void GoHome();
}

/// <summary>
/// خدمة النوافذ المنبثقة والحوارات
/// </summary>
public interface IDialogService
{
    MessageBoxResult ShowMessage(string message, string title = "تنبيه", MessageBoxButton buttons = MessageBoxButton.OK, MessageBoxImage image = MessageBoxImage.Information);
    bool ShowConfirm(string message, string title = "تأكيد");
    T? ShowDialog<T>(string title = "", int width = 400, int height = 300) where T : Window;
}

/// <summary>
/// خدمة الإشعارات
/// </summary>
public interface INotificationService
{
    void ShowSuccess(string message);
    void ShowError(string message);
    void ShowWarning(string message);
    void ShowInfo(string message);
}

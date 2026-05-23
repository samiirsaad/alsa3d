using System.Windows;

namespace AlSa3d.App.Services
{
    public interface IDialogService
    {
        void ShowSuccess(string message);
        void ShowError(string message);
        void ShowWarning(string message);
        bool ShowConfirm(string message, string title = "تأكيد");
    }

    public class DialogService : IDialogService
    {
        public void ShowSuccess(string message)
        {
            MessageBox.Show(message, "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowError(string message)
        {
            MessageBox.Show(message, "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowWarning(string message)
        {
            MessageBox.Show(message, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
        }

        public bool ShowConfirm(string message, string title = "تأكيد")
        {
            var result = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return result == MessageBoxResult.Yes;
        }
    }
}

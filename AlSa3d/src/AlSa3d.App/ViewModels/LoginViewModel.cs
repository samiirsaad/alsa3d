using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;

namespace AlSa3d.App.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _errorMessage = "";

        public LoginViewModel(IUserService userService, IDialogService dialogService)
        {
            _userService = userService;
            _dialogService = dialogService;
        }

        [RelayCommand]
        private async Task Login(PasswordBox passwordBox)
        {
            if (string.IsNullOrWhiteSpace(Username))
            {
                ErrorMessage = "يرجى إدخال اسم المستخدم";
                return;
            }

            var password = passwordBox.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "يرجى إدخال كلمة المرور";
                return;
            }

            try
            {
                var result = await _userService.LoginAsync(Username, password);
                
                if (result.Success)
                {
                    _dialogService.ShowSuccess($"مرحباً بك يا {result.Data?.UserName}");
                    
                    // فتح النافذة الرئيسية
                    var mainWindow = App.ServiceProvider.GetService<MainWindow>();
                    if (mainWindow != null)
                    {
                        mainWindow.Show();
                        var currentWindow = Application.Current.MainWindow;
                        currentWindow?.Close();
                        Application.Current.MainWindow = mainWindow;
                    }
                }
                else
                {
                    ErrorMessage = result.Message ?? "اسم المستخدم أو كلمة المرور غير صحيحة";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "حدث خطأ أثناء تسجيل الدخول: " + ex.Message;
                _dialogService.ShowError(ErrorMessage);
            }
        }
    }
}

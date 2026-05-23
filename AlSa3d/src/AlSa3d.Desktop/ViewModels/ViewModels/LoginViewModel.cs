using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using AlSa3d.Services.Interfaces;
using AlSa3d.Desktop.Views;

namespace AlSa3d.Desktop.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IUserService _userService;

        [ObservableProperty]
        private string _username = "";

        [ObservableProperty]
        private string _errorMessage = "";

        public LoginViewModel(IUserService userService)
        {
            _userService = userService;
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
            }
        }
    }
}

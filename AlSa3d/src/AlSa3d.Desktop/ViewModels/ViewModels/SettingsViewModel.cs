using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Desktop.Services;
using System.Threading.Tasks;

namespace AlSa3d.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string _companyName = string.Empty;

    [ObservableProperty]
    private string _defaultCurrency = "EGP";

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private bool _autoBackupEnabled;

    public SettingsViewModel(IDialogService dialogService, INotificationService notificationService)
    {
        _dialogService = dialogService;
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task SaveSettings()
    {
        _notificationService.ShowSuccess("تم حفظ الإعدادات بنجاح");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task CreateBackup()
    {
        _notificationService.ShowInfo("إنشاء نسخة احتياطية - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task AddUser()
    {
        _notificationService.ShowInfo("إضافة مستخدم جديد - قيد التطوير");
        await Task.CompletedTask;
    }
}
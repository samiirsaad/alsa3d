using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AlSa3d.Desktop.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _companyName = string.Empty;

    [ObservableProperty]
    private string _defaultCurrency = "EGP";

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private bool _autoBackupEnabled;

    [RelayCommand]
    private async Task SaveSettings() => await Task.CompletedTask;

    [RelayCommand]
    private async Task CreateBackup() => await Task.CompletedTask;

    [RelayCommand]
    private async Task AddUser() => await Task.CompletedTask;
}

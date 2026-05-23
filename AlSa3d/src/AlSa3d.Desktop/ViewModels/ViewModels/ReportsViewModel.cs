using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Desktop.Services;
using System.Threading.Tasks;

namespace AlSa3d.Desktop.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string _selectedReportType = "sales";

    public ReportsViewModel(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [RelayCommand]
    private async Task GenerateReport()
    {
        _notificationService.ShowInfo("توليد التقرير - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ExportToPdf()
    {
        _notificationService.ShowInfo("تصدير PDF - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ExportToExcel()
    {
        _notificationService.ShowInfo("تصدير Excel - قيد التطوير");
        await Task.CompletedTask;
    }
}
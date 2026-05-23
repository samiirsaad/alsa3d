using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Threading.Tasks;

namespace AlSa3d.Desktop.ViewModels;

public partial class ReportsViewModel : ObservableObject
{
    [ObservableProperty]
    private string _selectedReportType = "sales";

    [RelayCommand]
    private async Task GenerateReport() => await Task.CompletedTask;

    [RelayCommand]
    private async Task ExportToPdf() => await Task.CompletedTask;

    [RelayCommand]
    private async Task ExportToExcel() => await Task.CompletedTask;
}

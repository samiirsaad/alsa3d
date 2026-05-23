using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Services.Interfaces;
using AlSa3d.Core.Entities;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlSa3d.App.ViewModels;

public partial class InvoiceViewModel : ObservableObject
{
    private readonly IInvoiceService _invoiceService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Invoice? _selectedInvoice;

    [ObservableProperty]
    private ObservableCollection<Invoice> _invoices = new();

    [ObservableProperty]
    private ObservableCollection<InvoiceStatus> _invoiceStatuses = new();

    [ObservableProperty]
    private InvoiceStatus? _selectedStatus;

    public InvoiceViewModel(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
        LoadInvoicesCommand.Execute(null);
        LoadStatuses();
    }

    private void LoadStatuses()
    {
        InvoiceStatuses = new ObservableCollection<InvoiceStatus>
        {
            new InvoiceStatus { Id = 1, Name = "الكل" },
            new InvoiceStatus { Id = 2, Name = "مدفوعة" },
            new InvoiceStatus { Id = 3, Name = "غير مدفوعة" },
            new InvoiceStatus { Id = 4, Name = "مدفوعة جزئياً" }
        };
        SelectedStatus = InvoiceStatuses[0];
    }

    [RelayCommand]
    private async Task LoadInvoices()
    {
        var result = await _invoiceService.GetAllInvoicesAsync();
        if (result.IsSuccess)
        {
            Invoices = new ObservableCollection<Invoice>(result.Data);
        }
    }

    [RelayCommand]
    private async Task SearchInvoices()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadInvoicesCommand.ExecuteAsync(null);
            return;
        }

        var result = await _invoiceService.SearchInvoicesAsync(SearchText);
        if (result.IsSuccess)
        {
            Invoices = new ObservableCollection<Invoice>(result.Data);
        }
    }

    [RelayCommand]
    private async Task CreateNewInvoice()
    {
        // Open dialog for new invoice
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ViewInvoice(Invoice? invoice)
    {
        if (invoice == null) return;
        // Show invoice details
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task PrintInvoice(Invoice? invoice)
    {
        if (invoice == null) return;
        // Print invoice
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteInvoice(Invoice? invoice)
    {
        if (invoice == null) return;
        
        var result = await _invoiceService.DeleteInvoiceAsync(invoice.Id);
        if (result.IsSuccess)
        {
            await LoadInvoicesCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private async Task ShowReports()
    {
        // Navigate to reports
        await Task.CompletedTask;
    }
}

public class InvoiceStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

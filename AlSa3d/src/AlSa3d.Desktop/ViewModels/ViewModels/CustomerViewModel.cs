using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
using AlSa3d.Core.Entities;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace AlSa3d.Desktop.ViewModels;

public partial class CustomerViewModel : ObservableObject
{
    private readonly ICustomerService _customerService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private Customer? _selectedCustomer;

    [ObservableProperty]
    private ObservableCollection<Customer> _customers = new();

    public CustomerViewModel(ICustomerService customerService)
    {
        _customerService = customerService;
        LoadCustomersCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadCustomers()
    {
        var result = await _customerService.GetAllCustomersAsync();
        if (result.Success)
        {
            Customers = new ObservableCollection<Customer>(result.Data);
        }
    }

    [RelayCommand]
    private async Task Search()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadCustomersCommand.ExecuteAsync(null);
            return;
        }

        var result = await _customerService.SearchCustomersAsync(SearchText);
        if (result.Success)
        {
            Customers = new ObservableCollection<Customer>(result.Data);
        }
    }

    [RelayCommand]
    private async Task ResetSearch()
    {
        SearchText = string.Empty;
        await LoadCustomersCommand.ExecuteAsync(null);
    }

    [RelayCommand]
    private async Task AddNewCustomer()
    {
        // Open dialog for new customer
        // Implementation depends on DialogService
    }

    [RelayCommand]
    private async Task EditCustomer(Customer customer)
    {
        if (customer == null) return;
        // Open dialog for editing
    }

    [RelayCommand]
    private async Task DeleteCustomer(Customer customer)
    {
        if (customer == null) return;
        
        var result = await _customerService.DeleteCustomerAsync(customer.Id);
        if (result.Success)
        {
            await LoadCustomersCommand.ExecuteAsync(null);
        }
    }

    [RelayCommand]
    private async Task ExportToExcel()
    {
        // Export functionality
        await Task.CompletedTask;
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlSa3d.App.ViewModels;

public partial class FinancialViewModel : ObservableObject
{
    private readonly IFinancialService _financialService;

    [ObservableProperty]
    private ObservableCollection<Transaction> _transactions = new();

    public FinancialViewModel(IFinancialService financialService)
    {
        _financialService = financialService;
        LoadTransactionsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTransactions()
    {
        var result = await _financialService.GetAllTransactionsAsync();
        if (result.IsSuccess)
            Transactions = new ObservableCollection<Transaction>(result.Data);
    }

    [RelayCommand]
    private async Task AddNewTransaction() => await Task.CompletedTask;

    [RelayCommand]
    private async Task ShowBankStatement() => await Task.CompletedTask;
}

public class Transaction
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EntityTransaction = AlSa3d.Core.Entities.Transaction;

namespace AlSa3d.Desktop.ViewModels;

public class TransactionDisplayModel
{
    public int Id { get; set; }
    public string ReferenceNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string CurrencyName { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

public partial class FinancialViewModel : ObservableObject
{
    private readonly IFinancialService _financialService;

    [ObservableProperty]
    private ObservableCollection<TransactionDisplayModel> _transactions = new();

    public FinancialViewModel(IFinancialService financialService)
    {
        _financialService = financialService;
        LoadTransactionsCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadTransactions()
    {
        var result = await _financialService.GetAccountTransactionsAsync(0);
        if (result.Success)
            Transactions = new ObservableCollection<TransactionDisplayModel>(
                result.Data.Select(t => new TransactionDisplayModel
                {
                    Id = t.Id,
                    ReferenceNumber = t.Reference ?? "",
                    Type = t.Type,
                    Amount = t.Amount,
                    CurrencyName = "جنيه",
                    Date = t.Date,
                    StatusName = "مؤكدة"
                }));
    }

    [RelayCommand]
    private async Task AddNewTransaction() => await Task.CompletedTask;

    [RelayCommand]
    private async Task ShowBankStatement() => await Task.CompletedTask;
}

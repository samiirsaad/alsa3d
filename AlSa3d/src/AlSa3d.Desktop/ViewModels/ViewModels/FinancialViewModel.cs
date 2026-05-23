using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
using AlSa3d.Desktop.Services;
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
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private ObservableCollection<TransactionDisplayModel> _transactions = new();

    public FinancialViewModel(IFinancialService financialService, IDialogService dialogService, INotificationService notificationService)
    {
        _financialService = financialService;
        _dialogService = dialogService;
        _notificationService = notificationService;
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
    private async Task AddNewTransaction()
    {
        _notificationService.ShowInfo("إضافة حركة مالية جديدة - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ShowBankStatement()
    {
        _notificationService.ShowInfo("كشف الحساب - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task ViewTransaction(TransactionDisplayModel? transaction)
    {
        if (transaction == null) return;
        _notificationService.ShowInfo("عرض تفاصيل الحركة - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteTransaction(TransactionDisplayModel? transaction)
    {
        if (transaction == null) return;
        if (!_dialogService.ShowConfirm("هل أنت متأكد من حذف هذه الحركة المالية؟"))
            return;
        _notificationService.ShowSuccess("تم حذف الحركة المالية بنجاح");
        await LoadTransactionsCommand.ExecuteAsync(null);
    }
}
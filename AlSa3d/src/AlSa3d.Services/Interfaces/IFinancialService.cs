using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface IFinancialService
{
    Task<Result<IEnumerable<Bank>>> GetAllBanksAsync();
    Task<Result<Bank>> CreateBankAsync(CreateBankDto dto);
    Task<Result<Account>> CreateAccountAsync(CreateAccountDto dto);
    Task<Result<Check>> CreateCheckAsync(CreateCheckDto dto);
    Task<Result<Check>> DepositCheckAsync(int checkId, int accountId);
    Task<Result<Check>> BounceCheckAsync(int checkId, string reason);
    Task<Result<Transaction>> CreateTransactionAsync(CreateTransactionDto dto);
    Task<Result<IEnumerable<Transaction>>> GetAccountTransactionsAsync(int accountId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Result<ExchangeRate>> SetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, decimal rate);
    Task<Result<decimal>> ConvertCurrencyAsync(decimal amount, int fromCurrencyId, int toCurrencyId);
}

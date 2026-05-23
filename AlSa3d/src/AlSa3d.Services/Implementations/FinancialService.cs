using AlSa3d.Core;
using AlSa3d.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;

namespace AlSa3d.Services.Implementations
{
    public class FinancialService : IFinancialService
    {
        private readonly IRepository<Bank> _bankRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<Check> _checkRepository;
        private readonly IRepository<Transaction> _transactionRepository;
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IRepository<ExchangeRate> _exchangeRateRepository;

        public FinancialService(
            IRepository<Bank> bankRepository,
            IRepository<Account> accountRepository,
            IRepository<Check> checkRepository,
            IRepository<Transaction> transactionRepository,
            IRepository<Currency> currencyRepository,
            IRepository<ExchangeRate> exchangeRateRepository)
        {
            _bankRepository = bankRepository;
            _accountRepository = accountRepository;
            _checkRepository = checkRepository;
            _transactionRepository = transactionRepository;
            _currencyRepository = currencyRepository;
            _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<Result<IEnumerable<Bank>>> GetAllBanksAsync()
        {
            try
            {
                var banks = await _bankRepository.GetAllAsync(b => b.Accounts);
                return Result.Ok(banks.Where(b => !b.IsDeleted).OrderBy((b => b.Name)).AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Bank>>($"فشل في جلب البنوك: {ex.Message}");
            }
        }

        public async Task<Result<Bank>> CreateBankAsync(CreateBankDto dto)
        {
            try
            {
                var bank = new Bank
                {
                    Name = dto.Name,
                    Code = dto.Code,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _bankRepository.AddAsync(bank);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Bank>($"فشل في إضافة البنك: {ex.Message}");
            }
        }

        public async Task<Result<Account>> CreateAccountAsync(CreateAccountDto dto)
        {
            try
            {
                var account = new Account
                {
                    BankId = dto.BankId,
                    AccountNumber = dto.AccountNumber,
                    AccountName = dto.AccountName,
                    AccountType = dto.AccountType,
                    CurrencyId = dto.CurrencyId ?? 1, // EGP default
                    Balance = dto.InitialBalance ?? 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _accountRepository.AddAsync(account);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Account>($"فشل في إضافة الحساب: {ex.Message}");
            }
        }

        public async Task<Result<Check>> CreateCheckAsync(CreateCheckDto dto)
        {
            try
            {
                var check = new Check
                {
                    CheckNumber = dto.CheckNumber,
                    AccountId = dto.AccountId,
                    Amount = dto.Amount,
                    CurrencyId = dto.CurrencyId ?? 1,
                    IssueDate = dto.IssueDate ?? DateTime.Now,
                    DueDate = dto.DueDate,
                    PayeeName = dto.PayeeName,
                    Notes = dto.Notes,
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                var result = await _checkRepository.AddAsync(check);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Check>($"فشل في إضافة الشيك: {ex.Message}");
            }
        }

        public async Task<Result<Check>> DepositCheckAsync(int checkId, int accountId)
        {
            try
            {
                var check = await _checkRepository.GetByIdAsync(checkId);
                if (check == null)
                    return Result.Failure<Check>("الشيك غير موجود");

                check.Status = "Deposited";
                check.DepositedAt = DateTime.Now;
                check.DepositedAccountId = accountId;

                // إنشاء معاملة
                var transaction = new Transaction
                {
                    AccountId = accountId,
                    Type = "Credit",
                    Amount = check.Amount,
                    CurrencyId = check.CurrencyId,
                    Description = $"إيداع شيك رقم {check.CheckNumber}",
                    ReferenceType = "Check",
                    ReferenceId = checkId,
                    Date = DateTime.Now
                };

                await _transactionRepository.AddAsync(transaction);
                var result = await _checkRepository.UpdateAsync(check);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Check>($"فشل في إيداع الشيك: {ex.Message}");
            }
        }

        public async Task<Result<Check>> BounceCheckAsync(int checkId, string reason)
        {
            try
            {
                var check = await _checkRepository.GetByIdAsync(checkId);
                if (check == null)
                    return Result.Failure<Check>("الشيك غير موجود");

                check.Status = "Bounced";
                check.BouncedAt = DateTime.Now;
                check.BounceReason = reason;

                var result = await _checkRepository.UpdateAsync(check);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Check>($"فشل في تسجيل ارتداد الشيك: {ex.Message}");
            }
        }

        public async Task<Result<Transaction>> CreateTransactionAsync(CreateTransactionDto dto)
        {
            try
            {
                var transaction = new Transaction
                {
                    AccountId = dto.AccountId,
                    Type = dto.Type,
                    Amount = dto.Amount,
                    CurrencyId = dto.CurrencyId ?? 1,
                    Description = dto.Description,
                    ReferenceType = dto.ReferenceType,
                    ReferenceId = dto.ReferenceId,
                    Date = dto.Date ?? DateTime.Now
                };

                // تحديث رصيد الحساب
                var account = await _accountRepository.GetByIdAsync(dto.AccountId);
                if (account == null)
                    return Result.Failure<Transaction>("الحساب غير موجود");

                if (dto.Type == "Credit")
                    account.Balance += dto.Amount;
                else
                    account.Balance -= dto.Amount;

                await _accountRepository.UpdateAsync(account);
                var result = await _transactionRepository.AddAsync(transaction);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Transaction>($"فشل في إضافة المعاملة: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Transaction>>> GetAccountTransactionsAsync(int accountId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var transactions = await _transactionRepository.GetAllAsync(t => t.AccountId == accountId);

                if (fromDate.HasValue)
                    transactions = transactions.Where(t => t.Date >= fromDate.Value.Date);

                if (toDate.HasValue)
                    transactions = transactions.Where(t => t.Date <= toDate.Value.Date);

                return Result.Ok(transactions.OrderByDescending(t => t.Date).AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Transaction>>($"فشل في جلب معاملات الحساب: {ex.Message}");
            }
        }

        public async Task<Result<ExchangeRate>> SetExchangeRateAsync(int fromCurrencyId, int toCurrencyId, decimal rate)
        {
            try
            {
                var existingRate = await _exchangeRateRepository.GetAsync(e => 
                    e.FromCurrencyId == fromCurrencyId && 
                    e.ToCurrencyId == toCurrencyId);

                if (existingRate != null)
                {
                    existingRate.Rate = rate;
                    existingRate.UpdatedAt = DateTime.Now;
                    var result = await _exchangeRateRepository.UpdateAsync(existingRate);
                    return result;
                }

                var exchangeRate = new ExchangeRate
                {
                    FromCurrencyId = fromCurrencyId,
                    ToCurrencyId = toCurrencyId,
                    Rate = rate,
                    Date = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                var result2 = await _exchangeRateRepository.AddAsync(exchangeRate);
                return result2;
            }
            catch (Exception ex)
            {
                return Result.Failure<ExchangeRate>($"فشل في تعيين سعر الصرف: {ex.Message}");
            }
        }

        public async Task<Result<decimal>> ConvertCurrencyAsync(decimal amount, int fromCurrencyId, int toCurrencyId)
        {
            try
            {
                if (fromCurrencyId == toCurrencyId)
                    return Result.Ok(amount);

                var rate = await _exchangeRateRepository.GetAsync(e => 
                    e.FromCurrencyId == fromCurrencyId && 
                    e.ToCurrencyId == toCurrencyId);

                if (rate == null)
                    return Result.Failure<decimal>("سعر الصرف غير متوفر");

                return Result.Ok(amount * rate.Rate);
            }
            catch (Exception ex)
            {
                return Result.Failure<decimal>($"فشل في تحويل العملة: {ex.Message}");
            }
        }
    }
}

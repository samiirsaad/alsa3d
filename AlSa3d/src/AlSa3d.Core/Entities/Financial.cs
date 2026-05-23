using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Bank : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}

public class Account : BaseEntity
{
    public int BankId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Checking";
    public decimal Balance { get; set; }
    public int CurrencyId { get; set; } = 1;
    public bool IsActive { get; set; } = true;

    public virtual Bank? Bank { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Check> Checks { get; set; } = new List<Check>();
}

public class Check : BaseEntity
{
    public int AccountId { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; } = 1;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? PayeeName { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime? DepositedAt { get; set; }
    public int? DepositedAccountId { get; set; }
    public DateTime? BouncedAt { get; set; }
    public string? BounceReason { get; set; }

    public virtual Account? Account { get; set; }
}

public class Transaction : BaseEntity
{
    public int AccountId { get; set; }
    public string Type { get; set; } = "Deposit";
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; } = 1;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public DateTime Date { get; set; } = DateTime.Now;
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public decimal BalanceAfter { get; set; }

    public virtual Account? Account { get; set; }
}

public class Currency : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; } = 1;
    public bool IsBase { get; set; }
    public bool IsActive { get; set; } = true;
}

public class ExchangeRate : BaseEntity
{
    public int FromCurrencyId { get; set; }
    public int ToCurrencyId { get; set; }
    public decimal Rate { get; set; }
    public DateTime EffectiveDate { get; set; } = DateTime.Now;
    public DateTime Date { get; set; } = DateTime.Now;

    public virtual Currency? FromCurrency { get; set; }
    public virtual Currency? ToCurrency { get; set; }
}

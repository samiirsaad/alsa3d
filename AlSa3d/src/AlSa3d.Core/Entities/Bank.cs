using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Bank : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    
    public virtual ICollection<BankAccount> Accounts { get; set; } = new List<BankAccount>();
}

public class BankAccount : BaseEntity
{
    public int BankId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string? IBAN { get; set; }
    public decimal Balance { get; set; } = 0;
    public CurrencyType Currency { get; set; } = CurrencyType.EGP;
    public bool IsActive { get; set; } = true;
    
    public virtual Bank? Bank { get; set; }
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public virtual ICollection<Check> Checks { get; set; } = new List<Check>();
}

public class Check : BaseEntity
{
    public int AccountId { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; } = 0;
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Payee { get; set; } // المستفيد
    public CheckStatus Status { get; set; } = CheckStatus.Pending;
    public string? Notes { get; set; }
    
    public virtual BankAccount? Account { get; set; }
}

public class Transaction : BaseEntity
{
    public int AccountId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; } = 0;
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    public string? Description { get; set; }
    public string? Reference { get; set; }
    public decimal BalanceAfter { get; set; } = 0;
    
    public virtual BankAccount? Account { get; set; }
}

public enum CurrencyType
{
    EGP = 0, // جنيه مصري
    USD = 1, // دولار أمريكي
    EUR = 2, // يورو
    SAR = 3, // ريال سعودي
    AED = 4  // درهم إماراتي
}

public enum CheckStatus
{
    Pending = 0,    // قيد الانتظار
    Cashed = 1,     // تم الصرف
    Deposited = 2,  // تم الإيداع
    Bounced = 3,    // مرتجع
    Cancelled = 4   // ملغى
}

public enum TransactionType
{
    Deposit = 0,    // إيداع
    Withdrawal = 1, // سحب
    Transfer = 2    // تحويل
}

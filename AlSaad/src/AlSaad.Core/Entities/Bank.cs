using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// البنك
/// </summary>
public class Bank : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? SwiftCode { get; set; }
    
    // حالة البنك
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ICollection<BankAccount>? Accounts { get; set; }
}

/// <summary>
/// حساب بنكي
/// </summary>
public class BankAccount : BaseEntity
{
    public string AccountName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string? IBAN { get; set; }
    
    // البنك
    public int BankId { get; set; }
    public virtual Bank? Bank { get; set; }
    
    // العملة
    public int CurrencyId { get; set; }
    public virtual Currency? Currency { get; set; }
    
    // الرصيد
    public decimal CurrentBalance { get; set; }
    public decimal? MinimumBalance { get; set; }
    
    // حالة الحساب
    public bool IsActive { get; set; } = true;
    public bool IsMain { get; set; } = false;
    
    // ملاحظات
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<Check>? Checks { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
}

/// <summary>
/// شيك
/// </summary>
public class Check : BaseEntity
{
    public string CheckNumber { get; set; } = string.Empty;
    
    // نوع الشيك
    public CheckType Type { get; set; } // وارد، صادر
    
    // الحالة
    public CheckStatus Status { get; set; } = CheckStatus.Pending;
    
    // المبلغ والعملة
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public virtual Currency? Currency { get; set; }
    
    // الحساب البنكي
    public int BankAccountId { get; set; }
    public virtual BankAccount? BankAccount { get; set; }
    
    // التاريخ
    public DateTime IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? DepositDate { get; set; }
    
    // الطرف الآخر
    public string? PayeeName { get; set; } // للمستفيد
    public string? PayerName { get; set; } // من الدفع
    
    // مرجع الشيك
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    
    // ملاحظات
    public string? Notes { get; set; }
    public string? RejectionReason { get; set; }
    
    // تواريخ المتابعة
    public DateTime? ReturnedDate { get; set; }
    public DateTime? ClearedDate { get; set; }
}

/// <summary>
/// حركة مالية
/// </summary>
public class Transaction : BaseEntity
{
    public string TransactionNumber { get; set; } = string.Empty;
    
    // نوع الحركة
    public TransactionType Type { get; set; } // إيداع، سحب، تحويل
    
    // المبلغ والعملة
    public decimal Amount { get; set; }
    public int CurrencyId { get; set; }
    public virtual Currency? Currency { get; set; }
    
    // الحساب
    public int BankAccountId { get; set; }
    public virtual BankAccount? BankAccount { get; set; }
    
    // التاريخ
    public DateTime TransactionDate { get; set; } = DateTime.Now;
    
    // وصف الحركة
    public string Description { get; set; } = string.Empty;
    
    // مرجع الحركة
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    
    // الرصيد بعد الحركة
    public decimal BalanceAfter { get; set; }
    
    // ملاحظات
    public string? Notes { get; set; }
}

/// <summary>
/// عملة
/// </summary>
public class Currency : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty; // EGP, USD, EUR
    public string Symbol { get; set; } = string.Empty; // ج.م, $, €
    
    // سعر الصرف مقابل الجنيه
    public decimal ExchangeRate { get; set; } = 1.0m;
    
    // حالة العملة
    public bool IsActive { get; set; } = true;
    public bool IsDefault { get; set; } = false;
    
    // خصائص التنقل
    public virtual ICollection<BankAccount>? BankAccounts { get; set; }
    public virtual ICollection<Check>? Checks { get; set; }
    public virtual ICollection<Transaction>? Transactions { get; set; }
    public virtual ICollection<ExchangeRate>? ExchangeRates { get; set; }
}

/// <summary>
/// سعر صرف
/// </summary>
public class ExchangeRate : BaseEntity
{
    public int CurrencyId { get; set; }
    public virtual Currency? Currency { get; set; }
    
    public decimal Rate { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    
    public string? Source { get; set; } // مصدر سعر الصرف
    public string? Notes { get; set; }
}

public enum CheckType
{
    Received,  // شيك وارد (لصالحنا)
    Paid       // شيك صادر (علينا)
}

public enum CheckStatus
{
    Pending,      // تحت التحصيل
    Cleared,      // تم الصرف
    Returned,     // مرتجع
    Cancelled,    // ملغى
    PostDated     // مؤجل
}

public enum TransactionType
{
    Deposit,   // إيداع
    Withdrawal,// سحب
    Transfer,  // تحويل
    Adjustment // تسوية
}

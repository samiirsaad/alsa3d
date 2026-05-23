using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public int CustomerId { get; set; }
    public decimal SubTotal { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal TaxRate { get; set; } = 14; // 14% VAT
    public decimal TaxAmount { get; set; } = 0;
    public decimal Total { get; set; } = 0;
    public decimal PaidAmount { get; set; } = 0;
    public decimal Remaining { get; set; } = 0;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }
    
    // Navigation Properties
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public decimal Total { get; set; } = 0;
    
    // Navigation Properties
    public virtual Invoice? Invoice { get; set; }
    public virtual Product? Product { get; set; }
}

public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; } = 0;
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? Reference { get; set; } // Check number, Transaction ID, etc.
    public string? Notes { get; set; }
    
    public virtual Invoice? Invoice { get; set; }
}

public enum InvoiceStatus
{
    Draft = 0,
    Pending = 1,
    Paid = 2,
    Cancelled = 3
}

public enum PaymentMethod
{
    Cash = 0,
    Check = 1,
    BankTransfer = 2,
    CreditCard = 3
}

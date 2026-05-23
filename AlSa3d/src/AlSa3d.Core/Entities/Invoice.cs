using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceType Type { get; set; } = InvoiceType.Sales;
    public DateTime InvoiceDate { get; set; } = DateTime.Now;
    public DateTime Date { get; set; } = DateTime.Now;
    public DateTime? DueDate { get; set; }
    public int CustomerId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; } = 14;
    public decimal TaxAmount { get; set; }
    public decimal Total { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal Remaining { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public string? Notes { get; set; }
    public int? CreatedByUserId { get; set; }
    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    public virtual Customer? Customer { get; set; }
    public new virtual User? CreatedBy { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal TaxRate { get; set; } = 14;
    public decimal Total { get; set; }
    public int WarehouseId { get; set; } = 1;

    public virtual Invoice? Invoice { get; set; }
    public virtual Product? Product { get; set; }
}

public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public string Method { get; set; } = "Cash";
    public string? Reference { get; set; }
    public string? Notes { get; set; }

    public virtual Invoice? Invoice { get; set; }
}

public class Return : BaseEntity
{
    public string ReturnNumber { get; set; } = string.Empty;
    public int InvoiceId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public string Reason { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public ReturnStatus Status { get; set; } = ReturnStatus.Pending;
    public int? CreatedByUserId { get; set; }

    public virtual Invoice? Invoice { get; set; }
    public new virtual User? CreatedBy { get; set; }
    public virtual ICollection<ReturnItem> Items { get; set; } = new List<ReturnItem>();
}

public class ReturnItem : BaseEntity
{
    public int ReturnId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal Total { get; set; }
    public string? Reason { get; set; }

    public virtual Return? Return { get; set; }
    public virtual Product? Product { get; set; }
}

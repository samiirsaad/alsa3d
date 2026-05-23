using System;
using AlSaad.Core.Common;
using AlSaad.Core.Enums;

namespace AlSaad.Core.Entities;

/// <summary>
/// الفاتورة
/// </summary>
public class Invoice : BaseEntity
{
    public Invoice()
    {
        InvoiceNumber = string.Empty;
        Items = new HashSet<InvoiceItem>();
        Payments = new HashSet<Payment>();
    }
    
    public string InvoiceNumber { get; set; }
    public InvoiceType Type { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    
    // معلومات العميل
    public int CustomerId { get; set; }
    
    // القيم المالية
    public decimal SubTotal { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    
    // الحالة
    public InvoiceStatus Status { get; set; }
    
    // معلومات إضافية
    public string? Notes { get; set; }
    public string? InternalNotes { get; set; }
    public int? WarehouseId { get; set; }
    public int? SalesmanId { get; set; }
    
    // إعدادات الطباعة
    public bool IsPrinted { get; set; }
    public DateTime? PrintedAt { get; set; }
    
    // خصائص التنقل
    public virtual Customer? Customer { get; set; }
    public virtual ICollection<InvoiceItem> Items { get; set; }
    public virtual ICollection<Payment> Payments { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual Employee? Salesman { get; set; }
}

/// <summary>
/// صنف الفاتورة
/// </summary>
public class InvoiceItem : BaseEntity
{
    public int InvoiceId { get; set; }
    public int ProductId { get; set; }
    
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = "قطعة";
    
    public decimal UnitPrice { get; set; }
    public decimal Discount { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    
    public decimal SubTotal { get; set; }
    public decimal Total { get; set; }
    
    public string? Notes { get; set; }
    public int? SerialNumber { get; set; }
    
    // خصائص التنقل
    public virtual Invoice? Invoice { get; set; }
    public virtual Product? Product { get; set; }
}

/// <summary>
/// المنتج/الصنف
/// </summary>
public class Product : BaseEntity
{
    public Product()
    {
        ProductCode = string.Empty;
        Name = string.Empty;
    }
    
    public string ProductCode { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public string? AlternativeBarcode { get; set; }
    
    // التصنيف
    public int? CategoryId { get; set; }
    
    // الوحدات
    public string Unit { get; set; } = "قطعة";
    public decimal PurchaseUnit { get; set; } = 1;
    public decimal SalesUnit { get; set; } = 1;
    
    // الأسعار
    public decimal CostPrice { get; set; }
    public decimal WholesalePrice { get; set; }
    public decimal RetailPrice { get; set; }
    public decimal MinimumPrice { get; set; }
    
    // المخزون
    public decimal CurrentStock { get; set; }
    public decimal MinimumStock { get; set; }
    public decimal MaximumStock { get; set; }
    public int? WarehouseId { get; set; }
    
    // الضريبة
    public bool IsTaxable { get; set; } = true;
    public decimal TaxPercent { get; set; } = 14;
    
    // الحالة
    public bool IsActive { get; set; } = true;
    public bool IsService { get; set; } = false;
    
    // صور المنتج
    public string? ImagePath { get; set; }
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual ProductCategory? Category { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
    public virtual ICollection<StockMovement>? StockMovements { get; set; }
}

/// <summary>
/// تصنيف المنتجات
/// </summary>
public class ProductCategory : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImagePath { get; set; }
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ProductCategory? Parent { get; set; }
    public virtual ICollection<ProductCategory>? Children { get; set; }
    public virtual ICollection<Product>? Products { get; set; }
}

/// <summary>
/// المخزن
/// </summary>
public class Warehouse : BaseEntity
{
    public string Name { get; set; }
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsMain { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<Product>? Products { get; set; }
    public virtual ICollection<StockMovement>? StockMovements { get; set; }
}

/// <summary>
/// حركة المخزن
/// </summary>
public class StockMovement : BaseEntity
{
    public int ProductId { get; set; }
    public int WarehouseId { get; set; }
    
    public MovementType Type { get; set; }
    public decimal Quantity { get; set; }
    public decimal QuantityBefore { get; set; }
    public decimal QuantityAfter { get; set; }
    
    public string? ReferenceType { get; set; } // Invoice, Return, Adjustment
    public int? ReferenceId { get; set; }
    
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual Product? Product { get; set; }
    public virtual Warehouse? Warehouse { get; set; }
}

/// <summary>
/// نوع حركة المخزن
/// </summary>
public enum MovementType
{
    In = 0,   // وارد
    Out = 1,  // صادر
    Adjust = 2, // تسوية
    Transfer = 3 // تحويل
}

/// <summary>
/// السداد/الدفع
/// </summary>
public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    
    public PaymentMethod Method { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaymentDate { get; set; }
    
    // معلومات الشيك (إذا كان الدفع بشيك)
    public string? CheckNumber { get; set; }
    public string? BankName { get; set; }
    public DateTime? CheckDueDate { get; set; }
    
    public string? Notes { get; set; }
    public int? UserId { get; set; }
    
    // خصائص التنقل
    public virtual Invoice? Invoice { get; set; }
}

/// <summary>
/// طريقة الدفع
/// </summary>
public enum PaymentMethod
{
    Cash = 0,      // نقدي
    Check = 1,     // شيك
    BankTransfer = 2, // تحويل بنكي
    CreditCard = 3, // بطاقة ائتمان
    VodafoneCash = 4, // فودافون كاش
    Other = 5      // أخرى
}

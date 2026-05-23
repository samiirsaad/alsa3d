namespace AlSaad.Core.Enums;

/// <summary>
/// حالة الفاتورة
/// </summary>
public enum InvoiceStatus
{
    /// <summary>
    /// مسودة
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// مؤكدة
    /// </summary>
    Confirmed = 1,
    
    /// <summary>
    /// مدفوعة جزئياً
    /// </summary>
    PartiallyPaid = 2,
    
    /// <summary>
    /// مدفوعة بالكامل
    /// </summary>
    Paid = 3,
    
    /// <summary>
    /// ملغاة
    /// </summary>
    Cancelled = 4,
    
    /// <summary>
    /// مرتجعة
    /// </summary>
    Returned = 5
}

/// <summary>
/// نوع الفاتورة
/// </summary>
public enum InvoiceType
{
    /// <summary>
    /// فاتورة مبيعات
    /// </summary>
    Sales = 0,
    
    /// <summary>
    /// فاتورة مشتريات
    /// </summary>
    Purchase = 1,
    
    /// <summary>
    /// مرتجع مبيعات
    /// </summary>
    SalesReturn = 2,
    
    /// <summary>
    /// مرتجع مشتريات
    /// </summary>
    PurchaseReturn = 3
}

/// <summary>
/// نوع العميل
/// </summary>
public enum CustomerType
{
    /// <summary>
    /// عميل نقدي
    /// </summary>
    Cash = 0,
    
    /// <summary>
    /// عميل آجل
    /// </summary>
    Credit = 1,
    
    /// <summary>
    /// شركة
    /// </summary>
    Company = 2,
    
    /// <summary>
    /// فرد
    /// </summary>
    Individual = 3
}

/// <summary>
/// حالة الشيك
/// </summary>
public enum CheckStatus
{
    /// <summary>
    /// تحت التحصيل
    /// </summary>
    Pending = 0,
    
    /// <summary>
    /// تم الصرف
    /// </summary>
    Cashed = 1,
    
    /// <summary>
    /// تم الإرجاع
    /// </summary>
    Returned = 2,
    
    /// <summary>
    /// ملغى
    /// </summary>
    Cancelled = 3
}

/// <summary>
/// نوع الحركة
/// </summary>
public enum TransactionType
{
    /// <summary>
    /// قيد يدوي
    /// </summary>
    Manual = 0,
    
    /// <summary>
    /// من فاتورة
    /// </summary>
    Invoice = 1,
    
    /// <summary>
    /// من شيك
    /// </summary>
    Check = 2,
    
    /// <summary>
    /// من سند قبض
    /// </summary>
    Receipt = 3,
    
    /// <summary>
    /// من سند صرف
    /// </summary>
    Payment = 4
}

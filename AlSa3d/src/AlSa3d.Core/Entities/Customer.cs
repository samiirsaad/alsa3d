using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

/// <summary>
/// كيان العميل - يمثل عميل في النظام
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// اسم العميل
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// نوع العميل (فرد / شركة)
    /// </summary>
    public string Type { get; set; } = "Individual";

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// رقم الضريبة
    /// </summary>
    public string? TaxNumber { get; set; }

    /// <summary>
    /// رقم الهوية / البطاقة الشخصية
    /// </summary>
    public string? NationalId { get; set; }

    /// <summary>
    /// العنوان الرئيسي
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// المدينة
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// المحافظة
    /// </summary>
    public string? Governorate { get; set; }

    /// <summary>
    /// الرمز البريدي
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// الدولة (القيمة الافتراضية: مصر)
    /// </summary>
    public string? Country { get; set; } = "مصر";

    /// <summary>
    /// ملاحظات إضافية
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// الرصيد المالي للعميل
    /// </summary>
    public decimal Balance { get; set; } = 0;

    /// <summary>
    /// هل العميل نشط
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// تاريخ الحذف (soft delete)
    /// </summary>
    public DateTime? DeletedAt { get; set; }

    /// <summary>
    /// قائمة العناوين للعميل
    /// </summary>
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    /// <summary>
    /// قائمة جهات الاتصال للعميل
    /// </summary>
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    /// <summary>
    /// قائمة الفواتير للعميل
    /// </summary>
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

/// <summary>
/// كيان عنوان العميل
/// </summary>
public class Address : BaseEntity
{
    /// <summary>
    /// معرف العميل
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// نوع العنوان (منزل / مكتب / أخرى)
    /// </summary>
    public string AddressType { get; set; } = "Home";

    /// <summary>
    /// الشارع والعنوان التفصيلي
    /// </summary>
    public string Street { get; set; } = string.Empty;

    /// <summary>
    /// المدينة
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// المحافظة
    /// </summary>
    public string Governorate { get; set; } = string.Empty;

    /// <summary>
    /// الرمز البريدي
    /// </summary>
    public string? PostalCode { get; set; }

    /// <summary>
    /// الدولة
    /// </summary>
    public string? Country { get; set; } = "مصر";

    /// <summary>
    /// هل هذا العنوان الافتراضي
    /// </summary>
    public bool IsDefault { get; set; }

    /// <summary>
    /// العلاقة مع العميل
    /// </summary>
    public virtual Customer? Customer { get; set; }
}

/// <summary>
/// كيان جهة الاتصال بالعميل
/// </summary>
public class Contact : BaseEntity
{
    /// <summary>
    /// معرف العميل
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// اسم جهة الاتصال
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// الوظيفة / المسمى الوظيفي
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// رقم الهاتف
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// البريد الإلكتروني
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// هل هذه جهة الاتصال الأساسية
    /// </summary>
    public bool IsPrimary { get; set; }

    /// <summary>
    /// العلاقة مع العميل
    /// </summary>
    public virtual Customer? Customer { get; set; }
}

using System;
using AlSaad.Core.Common;
using AlSaad.Core.Enums;

namespace AlSaad.Core.Entities;

/// <summary>
/// العميل
/// </summary>
public class Customer : BaseEntity
{
    public string CustomerCode { get; set; }
    public string Name { get; set; }
    public string? CompanyName { get; set; }
    public CustomerType Type { get; set; }
    
    // معلومات الاتصال
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Fax { get; set; }
    
    // العنوان
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public string? Country { get; set; } = "مصر";
    public string? PostalCode { get; set; }
    
    // المعلومات الضريبية
    public string? TaxNumber { get; set; }
    public string? NationalId { get; set; }
    
    // الحدود الائتمانية
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; }
    
    // إعدادات إضافية
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual ICollection<Address>? Addresses { get; set; }
    public virtual ICollection<Invoice>? Invoices { get; set; }
    public virtual ICollection<CustomerContact>? Contacts { get; set; }
}

/// <summary>
/// عنوان العميل
/// </summary>
public class Address : BaseEntity
{
    public string AddressName { get; set; }
    public int CustomerId { get; set; }
    public string? Street { get; set; }
    public string? Building { get; set; }
    public string? Floor { get; set; }
    public string? Apartment { get; set; }
    public string? Landmark { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public string? PostalCode { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public bool IsDefault { get; set; }
    
    // خصائص التنقل
    public virtual Customer? Customer { get; set; }
}

/// <summary>
/// جهة اتصال للعميل
/// </summary>
public class CustomerContact : BaseEntity
{
    public string Name { get; set; }
    public int CustomerId { get; set; }
    public string? Position { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
    
    // خصائص التنقل
    public virtual Customer? Customer { get; set; }
}

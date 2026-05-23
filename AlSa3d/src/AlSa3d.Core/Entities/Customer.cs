using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; } = "مصر";
    public decimal Balance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Address : BaseEntity
{
    public int CustomerId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public bool IsPrimary { get; set; } = false;
    
    public virtual Customer? Customer { get; set; }
}

public class Contact : BaseEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Position { get; set; }
    
    public virtual Customer? Customer { get; set; }
}

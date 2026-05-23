using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Customer : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = "Individual";
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? TaxNumber { get; set; }
    public string? NationalId { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Governorate { get; set; }
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "مصر";
    public string? Notes { get; set; }
    public decimal Balance { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();
    public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}

public class Address : BaseEntity
{
    public int CustomerId { get; set; }
    public string AddressType { get; set; } = "Home";
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "مصر";
    public bool IsDefault { get; set; }

    public virtual Customer? Customer { get; set; }
}

public class Contact : BaseEntity
{
    public int CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Title { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }

    public virtual Customer? Customer { get; set; }
}

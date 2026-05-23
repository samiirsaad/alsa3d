namespace AlSa3d.Core.DTOs;

public class CreateCustomerDto
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
    public string? Notes { get; set; }
    public List<CreateAddressDto>? Addresses { get; set; }
    public List<CreateContactDto>? Contacts { get; set; }
}

public class UpdateCustomerDto
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
    public string? Notes { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateAddressDto
{
    public string AddressType { get; set; } = "Home";
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public string? Country { get; set; } = "مصر";
    public bool IsDefault { get; set; }
}

public class CreateContactDto
{
    public string Name { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsPrimary { get; set; }
}

public class CustomerStatisticsDto
{
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int TotalInvoices { get; set; }
    public decimal TotalPurchases { get; set; }
    public decimal TotalReturns { get; set; }
    public decimal CurrentBalance { get; set; }
    public DateTime? LastPurchaseDate { get; set; }
    public decimal AverageMonthlyPurchases { get; set; }
}

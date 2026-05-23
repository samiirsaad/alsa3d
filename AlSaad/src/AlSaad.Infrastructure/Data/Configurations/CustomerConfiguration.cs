using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Domain.Entities;

namespace AlSaad.Infrastructure.Data.Configurations;

/// <summary>
/// إعدادات جدول العملاء
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.TaxNumber)
            .HasMaxLength(50);

        builder.Property(c => c.CommercialRecord)
            .HasMaxLength(100);

        builder.Property(c => c.NationalId)
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.Mobile)
            .HasMaxLength(20);

        builder.Property(c => c.Fax)
            .HasMaxLength(20);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.CreditLimit)
            .HasDefaultValue(0);

        builder.Property(c => c.Balance)
            .HasDefaultValue(0);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(c => c.TaxNumber);
        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.Email);

        // علاقات
        builder.HasMany(c => c.Addresses)
            .WithOne(a => a.Customer)
            .HasForeignKey(a => a.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Contacts)
            .WithOne(co => co.Customer)
            .HasForeignKey(co => co.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Customer)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// إعدادات جدول عناوين العملاء
/// </summary>
public class CustomerAddressConfiguration : IEntityTypeConfiguration<CustomerAddress>
{
    public void Configure(EntityTypeBuilder<CustomerAddress> builder)
    {
        builder.ToTable("CustomerAddresses");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.AddressType)
            .HasMaxLength(50);

        builder.Property(a => a.City)
            .HasMaxLength(100);

        builder.Property(a => a.District)
            .HasMaxLength(100);

        builder.Property(a => a.Street)
            .HasMaxLength(200);

        builder.Property(a => a.BuildingNumber)
            .HasMaxLength(20);

        builder.Property(a => a.Floor)
            .HasMaxLength(20);

        builder.Property(a => a.Apartment)
            .HasMaxLength(20);

        builder.Property(a => a.PostalCode)
            .HasMaxLength(10);

        builder.Property(a => a.IsDefault)
            .HasDefaultValue(false);
    }
}

/// <summary>
/// إعدادات جدول جهات اتصال العملاء
/// </summary>
public class CustomerContactConfiguration : IEntityTypeConfiguration<CustomerContact>
{
    public void Configure(EntityTypeBuilder<CustomerContact> builder)
    {
        builder.ToTable("CustomerContacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Position)
            .HasMaxLength(100);

        builder.Property(c => c.Phone)
            .HasMaxLength(20);

        builder.Property(c => c.Mobile)
            .HasMaxLength(20);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.IsDefault)
            .HasDefaultValue(false);
    }
}

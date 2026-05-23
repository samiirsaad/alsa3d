using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Domain.Entities;
using AlSaad.Domain.Enums;

namespace AlSaad.Infrastructure.Data.Configurations;

/// <summary>
/// إعدادات جدول الفواتير
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.InvoiceDate)
            .IsRequired();

        builder.Property(i => i.InvoiceType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(i => i.SubTotal)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.DiscountAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TaxAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.PaidAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.RemainingAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Notes)
            .HasMaxLength(1000);

        builder.Property(i => i.PaymentMethod)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.HasIndex(i => i.InvoiceNumber)
            .IsUnique();

        builder.HasIndex(i => i.InvoiceDate);

        builder.HasIndex(i => i.CustomerId);

        // علاقات
        builder.HasOne(i => i.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(i => i.Items)
            .WithOne(it => it.Invoice)
            .HasForeignKey(it => it.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(i => i.Warehouse)
            .WithMany(w => w.Invoices)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// إعدادات جدول أصناف الفاتورة
/// </summary>
public class InvoiceItemConfiguration : IEntityTypeConfiguration<InvoiceItem>
{
    public void Configure(EntityTypeBuilder<InvoiceItem> builder)
    {
        builder.ToTable("InvoiceItems");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.ProductName)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(i => i.Quantity)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(i => i.UnitPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.DiscountPercent)
            .HasDefaultValue(0)
            .HasColumnType("decimal(5,2)");

        builder.Property(i => i.DiscountAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TaxPercent)
            .HasDefaultValue(0)
            .HasColumnType("decimal(5,2)");

        builder.Property(i => i.TaxAmount)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.TotalAmount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(i => i.Notes)
            .HasMaxLength(500);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.InvoiceItems)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(i => i.Warehouse)
            .WithMany()
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// إعدادات جدول المنتجات
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.NameAr)
            .IsRequired()
            .HasMaxLength(300);

        builder.Property(p => p.Code)
            .HasMaxLength(100);

        builder.Property(p => p.Barcode)
            .HasMaxLength(100);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.Unit)
            .HasMaxLength(50);

        builder.Property(p => p.CostPrice)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.SellingPrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(p => p.MinStockLevel)
            .HasDefaultValue(0);

        builder.Property(p => p.MaxStockLevel)
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(p => p.Code)
            .IsUnique();

        builder.HasIndex(p => p.Barcode);

        builder.HasMany(p => p.InvoiceItems)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// إعدادات جدول المخازن
/// </summary>
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(w => w.Code)
            .HasMaxLength(50);

        builder.Property(w => w.Address)
            .HasMaxLength(500);

        builder.Property(w => w.Phone)
            .HasMaxLength(20);

        builder.Property(w => w.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(w => w.Code)
            .IsUnique();

        builder.HasMany(w => w.Invoices)
            .WithOne(i => i.Warehouse)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(w => w.StockMovements)
            .WithOne(s => s.Warehouse)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

/// <summary>
/// إعدادات جدول حركات المخزن
/// </summary>
public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.MovementType)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(s => s.Quantity)
            .IsRequired()
            .HasColumnType("decimal(18,3)");

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.Property(s => s.ReferenceNumber)
            .HasMaxLength(100);

        builder.HasIndex(s => s.ProductId);

        builder.HasIndex(s => s.WarehouseId);

        builder.HasIndex(s => s.MovementDate);

        builder.HasOne(s => s.Product)
            .WithMany(p => p.StockMovements)
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Warehouse)
            .WithMany(w => w.StockMovements)
            .HasForeignKey(s => s.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.Invoice)
            .WithMany()
            .HasForeignKey(s => s.InvoiceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

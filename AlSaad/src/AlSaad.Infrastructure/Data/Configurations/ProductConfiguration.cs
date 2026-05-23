using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Core.Entities;

namespace AlSaad.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.ProductCode)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();
        
        builder.Property(p => p.Description)
            .HasMaxLength(1000);
        
        builder.Property(p => p.Unit)
            .HasMaxLength(50)
            .HasDefaultValue("قطعة");
        
        builder.Property(p => p.CostPrice)
            .HasColumnType("decimal(18,4)")
            .HasDefaultValue(0);
        
        builder.Property(p => p.PurchasePrice)
            .HasColumnType("decimal(18,4)")
            .HasDefaultValue(0);
        
        builder.Property(p => p.SellingPrice)
            .HasColumnType("decimal(18,4)")
            .IsRequired();
        
        builder.Property(p => p.TaxRate)
            .HasColumnType("decimal(5,2)")
            .HasDefaultValue(14.0m);
        
        builder.HasIndex(p => p.ProductCode)
            .IsUnique();
        
        builder.HasIndex(p => p.Barcode)
            .IsUnique(false);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(p => p.InvoiceItems)
            .WithOne(i => i.Product)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

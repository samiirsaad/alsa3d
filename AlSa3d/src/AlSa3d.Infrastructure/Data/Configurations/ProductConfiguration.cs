using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSa3d.Core.Entities;

namespace AlSa3d.Infrastructure.Data.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Barcode).HasMaxLength(50);
        builder.HasIndex(p => p.Barcode).IsUnique();
        
        builder.Property(p => p.CostPrice).HasPrecision(18, 2);
        builder.Property(p => p.SellingPrice).HasPrecision(18, 2);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name).IsRequired().HasMaxLength(100);
        
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name).IsRequired().HasMaxLength(100);
        builder.Property(w => w.Phone).HasMaxLength(20);
    }
}

public class ProductWarehouseConfiguration : IEntityTypeConfiguration<ProductWarehouse>
{
    public void Configure(EntityTypeBuilder<ProductWarehouse> builder)
    {
        builder.ToTable("ProductWarehouses");
        
        builder.HasKey(pw => pw.Id);
        
        builder.Property(pw => pw.Quantity).HasPrecision(18, 3);
        builder.Property(pw => pw.ReservedQuantity).HasPrecision(18, 3);
        
        builder.HasIndex(pw => new { pw.ProductId, pw.WarehouseId }).IsUnique();
        
        builder.HasOne(pw => pw.Product)
            .WithMany(p => p.ProductWarehouses)
            .HasForeignKey(pw => pw.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasOne(pw => pw.Warehouse)
            .WithMany(w => w.ProductWarehouses)
            .HasForeignKey(pw => pw.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Core.Entities;

namespace AlSaad.Infrastructure.Data.Configurations;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name)
            .HasMaxLength(100)
            .IsRequired();
        
        builder.Property(w => w.Code)
            .HasMaxLength(50);
        
        builder.Property(w => w.Address)
            .HasMaxLength(500);
        
        builder.HasOne(w => w.Manager)
            .WithMany()
            .HasForeignKey(w => w.ManagerId)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasMany(w => w.WarehouseProducts)
            .WithOne(wp => wp.Warehouse)
            .HasForeignKey(wp => wp.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class WarehouseProductConfiguration : IEntityTypeConfiguration<WarehouseProduct>
{
    public void Configure(EntityTypeBuilder<WarehouseProduct> builder)
    {
        builder.ToTable("WarehouseProducts");
        
        builder.HasKey(wp => wp.Id);
        
        builder.HasIndex(wp => new { wp.WarehouseId, wp.ProductId })
            .IsUnique();
        
        builder.HasOne(wp => wp.Warehouse)
            .WithMany(w => w.WarehouseProducts)
            .HasForeignKey(wp => wp.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(wp => wp.Product)
            .WithMany(p => p.WarehouseProducts)
            .HasForeignKey(wp => wp.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        builder.ToTable("StockMovements");
        
        builder.HasKey(sm => sm.Id);
        
        builder.Property(sm => sm.MovementType)
            .HasMaxLength(50)
            .IsRequired();
        
        builder.Property(sm => sm.ReferenceType)
            .HasMaxLength(50);
        
        builder.HasOne(sm => sm.Warehouse)
            .WithMany(w => w.StockMovements)
            .HasForeignKey(sm => sm.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(sm => sm.Product)
            .WithMany()
            .HasForeignKey(sm => sm.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(sm => sm.User)
            .WithMany()
            .HasForeignKey(sm => sm.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

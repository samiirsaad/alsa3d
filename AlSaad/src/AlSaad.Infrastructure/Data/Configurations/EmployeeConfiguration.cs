using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using AlSaad.Domain.Entities;

namespace AlSaad.Infrastructure.Data.Configurations;

/// <summary>
/// إعدادات جدول الموظفين
/// </summary>
public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employees");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Code)
            .HasMaxLength(50);

        builder.Property(e => e.NationalId)
            .HasMaxLength(20);

        builder.Property(e => e.Email)
            .HasMaxLength(200);

        builder.Property(e => e.Phone)
            .HasMaxLength(20);

        builder.Property(e => e.Mobile)
            .HasMaxLength(20);

        builder.Property(e => e.Address)
            .HasMaxLength(500);

        builder.Property(e => e.HireDate);

        builder.Property(e => e.Salary)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(e => e.Code)
            .IsUnique();

        builder.HasIndex(e => e.NationalId);

        builder.HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(e => e.Salaries)
            .WithOne(s => s.Employee)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Attendances)
            .WithOne(a => a.Employee)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// إعدادات جدول الأقسام
/// </summary>
public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("Departments");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.NameAr)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Code)
            .HasMaxLength(50);

        builder.Property(d => d.Description)
            .HasMaxLength(500);

        builder.HasIndex(d => d.Code)
            .IsUnique();

        builder.HasMany(d => d.Employees)
            .WithOne(e => e.Department)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

/// <summary>
/// إعدادات جدول الرواتب
/// </summary>
public class SalaryConfiguration : IEntityTypeConfiguration<Salary>
{
    public void Configure(EntityTypeBuilder<Salary> builder)
    {
        builder.ToTable("Salaries");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Month)
            .IsRequired();

        builder.Property(s => s.Year)
            .IsRequired();

        builder.Property(s => s.BasicSalary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Allowances)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Deductions)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.Bonus)
            .HasDefaultValue(0)
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.NetSalary)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(s => s.IsPaid)
            .HasDefaultValue(false);

        builder.Property(s => s.PaymentDate);

        builder.Property(s => s.Notes)
            .HasMaxLength(500);

        builder.HasIndex(s => s.EmployeeId);

        builder.HasIndex(s => new { s.EmployeeId, s.Month, s.Year })
            .IsUnique();

        builder.HasOne(s => s.Employee)
            .WithMany(e => e.Salaries)
            .HasForeignKey(s => s.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

/// <summary>
/// إعدادات جدول الحضور والانصراف
/// </summary>
public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
{
    public void Configure(EntityTypeBuilder<Attendance> builder)
    {
        builder.ToTable("Attendances");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Date)
            .IsRequired();

        builder.Property(a => a.CheckIn);

        builder.Property(a => a.CheckOut);

        builder.Property(a => a.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        builder.Property(a => a.LateMinutes)
            .HasDefaultValue(0);

        builder.Property(a => a.EarlyLeaveMinutes)
            .HasDefaultValue(0);

        builder.Property(a => a.Notes)
            .HasMaxLength(500);

        builder.HasIndex(a => a.EmployeeId);

        builder.HasIndex(a => a.Date);

        builder.HasIndex(a => new { a.EmployeeId, a.Date })
            .IsUnique();

        builder.HasOne(a => a.Employee)
            .WithMany(e => e.Attendances)
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

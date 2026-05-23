using System;
using AlSaad.Core.Common;

namespace AlSaad.Core.Entities;

/// <summary>
/// الموظف
/// </summary>
public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; }
    public string Name { get; set; }
    public string? FullName { get; set; }
    
    // معلومات الاتصال
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    
    // المعلومات الشخصية
    public DateTime? BirthDate { get; set; }
    public string? NationalId { get; set; }
    public string? Gender { get; set; }
    public string? MaritalStatus { get; set; }
    
    // الوظيفة
    public int? DepartmentId { get; set; }
    public int? PositionId { get; set; }
    public DateTime HireDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    // الراتب
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal TotalSalary => BasicSalary + HousingAllowance + TransportationAllowance + OtherAllowances;
    
    // الحالة
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    
    // صورة الموظف
    public string? ImagePath { get; set; }
    
    // خصائص التنقل
    public virtual Department? Department { get; set; }
    public virtual Position? Position { get; set; }
    public virtual ICollection<Salary>? Salaries { get; set; }
    public virtual ICollection<Attendance>? Attendances { get; set; }
}

/// <summary>
/// القسم
/// </summary>
public class Department : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual Department? Parent { get; set; }
    public virtual ICollection<Department>? Children { get; set; }
    public virtual ICollection<Employee>? Employees { get; set; }
}

/// <summary>
/// الوظيفة/المسمى الوظيفي
/// </summary>
public class Position : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }
    public decimal? MinSalary { get; set; }
    public decimal? MaxSalary { get; set; }
    public bool IsActive { get; set; } = true;
    
    // خصائص التنقل
    public virtual ICollection<Employee>? Employees { get; set; }
}

/// <summary>
/// الراتب الشهري
/// </summary>
public class Salary : BaseEntity
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    
    // المستحقات
    public decimal BasicSalary { get; set; }
    public decimal HousingAllowance { get; set; }
    public decimal TransportationAllowance { get; set; }
    public decimal OtherAllowances { get; set; }
    public decimal Overtime { get; set; }
    public decimal Bonus { get; set; }
    public decimal TotalEarnings => BasicSalary + HousingAllowance + TransportationAllowance + 
                                    OtherAllowances + Overtime + Bonus;
    
    // الخصومات
    public decimal SocialInsurance { get; set; }
    public decimal Loan { get; set; }
    public decimal Absence { get; set; }
    public decimal OtherDeductions { get; set; }
    public decimal TotalDeductions => SocialInsurance + Loan + Absence + OtherDeductions;
    
    // الصافي
    public decimal NetSalary => TotalEarnings - TotalDeductions;
    
    // حالة الصرف
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual Employee? Employee { get; set; }
    public virtual ICollection<SalaryDeduction>? Deductions { get; set; }
    public virtual ICollection<SalaryAddition>? Additions { get; set; }
}

/// <summary>
/// خصم إضافي على الراتب
/// </summary>
public class SalaryDeduction : BaseEntity
{
    public int SalaryId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual Salary? Salary { get; set; }
}

/// <summary>
/// إضافة إضافية على الراتب
/// </summary>
public class SalaryAddition : BaseEntity
{
    public int SalaryId { get; set; }
    public string Name { get; set; }
    public decimal Amount { get; set; }
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual Salary? Salary { get; set; }
}

/// <summary>
/// الحضور والانصراف
/// </summary>
public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    
    public AttendanceStatus Status { get; set; }
    public decimal LateMinutes { get; set; }
    public decimal EarlyLeaveMinutes { get; set; }
    public decimal OvertimeHours { get; set; }
    
    public string? Notes { get; set; }
    
    // خصائص التنقل
    public virtual Employee? Employee { get; set; }
}

/// <summary>
/// حالة الحضور
/// </summary>
public enum AttendanceStatus
{
    Present = 0,      // حاضر
    Absent = 1,       // غائب
    Late = 2,         // متأخر
    EarlyLeave = 3,   // انصراف مبكر
    Vacation = 4,     // إجازة
    SickLeave = 5,    // إجازة مرضية
    Excused = 6       // بعذر
}

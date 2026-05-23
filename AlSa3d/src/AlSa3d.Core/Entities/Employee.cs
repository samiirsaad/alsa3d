using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Employee : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? NationalId { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Now;
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? DeletedAt { get; set; }

    public virtual Department? Department { get; set; }
    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentDepartmentId { get; set; }
    public int? ManagerId { get; set; }

    public virtual Department? ParentDepartment { get; set; }
    public virtual ICollection<Department> SubDepartments { get; set; } = new List<Department>();
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Salary : BaseEntity
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal Overtime { get; set; }
    public decimal AbsenceDays { get; set; }
    public decimal Bonus { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal NetSalary { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidDate { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = "Pending";

    public virtual Employee? Employee { get; set; }
}

public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string Type { get; set; } = "CheckIn";
    public DateTime Time { get; set; } = DateTime.Now;
    public string Status { get; set; } = "Present";
    public string? Notes { get; set; }

    public virtual Employee? Employee { get; set; }
}

using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Employee : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime HireDate { get; set; } = DateTime.Now;
    public decimal BasicSalary { get; set; } = 0;
    public int DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation Properties
    public virtual Department? Department { get; set; }
    public virtual ICollection<Salary> Salaries { get; set; } = new List<Salary>();
    public virtual ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentDepartmentId { get; set; }
    
    public virtual Department? ParentDepartment { get; set; }
    public virtual ICollection<Department> SubDepartments { get; set; } = new List<Department>();
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}

public class Salary : BaseEntity
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BasicSalary { get; set; } = 0;
    public decimal Allowances { get; set; } = 0;
    public decimal Deductions { get; set; } = 0;
    public decimal NetSalary { get; set; } = 0;
    public bool IsPaid { get; set; } = false;
    public DateTime? PaidDate { get; set; }
    
    public virtual Employee? Employee { get; set; }
}

public class Attendance : BaseEntity
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; } = DateTime.Now;
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present;
    public string? Notes { get; set; }
    
    public virtual Employee? Employee { get; set; }
}

public enum AttendanceStatus
{
    Present = 0,
    Absent = 1,
    Late = 2,
    HalfDay = 3,
    Vacation = 4
}

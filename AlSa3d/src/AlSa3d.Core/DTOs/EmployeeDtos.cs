namespace AlSa3d.Core.DTOs;

public class CreateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int DepartmentId { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal? Allowances { get; set; }
    public decimal? Deductions { get; set; }
    public DateTime? HireDate { get; set; }
}

public class UpdateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string? JobTitle { get; set; }
    public string? NationalId { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public int DepartmentId { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal? Allowances { get; set; }
    public decimal? Deductions { get; set; }
    public bool? IsActive { get; set; }
}

public class CreateDepartmentDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ManagerId { get; set; }
}

public class SalaryDto
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal Deductions { get; set; }
    public decimal NetSalary { get; set; }
}

public class AttendanceDto
{
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public TimeSpan? CheckIn { get; set; }
    public TimeSpan? CheckOut { get; set; }
    public string Status { get; set; } = "Present";
}

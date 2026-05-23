using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface IEmployeeService
{
    Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync();
    Task<Result<Employee>> GetEmployeeByIdAsync(int id);
    Task<Result<Employee>> CreateEmployeeAsync(CreateEmployeeDto dto);
    Task<Result<Employee>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto);
    Task<Result<bool>> DeleteEmployeeAsync(int id);
    Task<Result<Department>> CreateDepartmentAsync(CreateDepartmentDto dto);
    Task<Result<Salary>> ProcessSalaryAsync(int employeeId, int month, int year);
    Task<Result<Attendance>> RecordAttendanceAsync(int employeeId, AttendanceType type, DateTime date);
    Task<Result<IEnumerable<Attendance>>> GetAttendanceByEmployeeAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null);
}

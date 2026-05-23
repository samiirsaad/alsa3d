using AlSa3d.Core;
using AlSa3d.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;

namespace AlSa3d.Services.Implementations
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Department> _departmentRepository;
        private readonly IRepository<Salary> _salaryRepository;
        private readonly IRepository<Attendance> _attendanceRepository;

        public EmployeeService(
            IRepository<Employee> employeeRepository,
            IRepository<Department> departmentRepository,
            IRepository<Salary> salaryRepository,
            IRepository<Attendance> attendanceRepository)
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _salaryRepository = salaryRepository;
            _attendanceRepository = attendanceRepository;
        }

        public async Task<Result<IEnumerable<Employee>>> GetAllEmployeesAsync()
        {
            try
            {
                var employees = await _employeeRepository.GetAllAsync(e => e.Department);
                return Result.Ok(employees.Where(e => !e.IsDeleted).OrderBy((e => e.Name)).AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Employee>>($"فشل في جلب الموظفين: {ex.Message}");
            }
        }

        public async Task<Result<Employee>> GetEmployeeByIdAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id, e => e.Department);
                if (employee == null || employee.IsDeleted)
                    return Result.Failure<Employee>("الموظف غير موجود");

                return Result.Ok(employee);
            }
            catch (Exception ex)
            {
                return Result.Failure<Employee>($"فشل في جلب الموظف: {ex.Message}");
            }
        }

        public async Task<Result<Employee>> CreateEmployeeAsync(CreateEmployeeDto dto)
        {
            try
            {
                var existing = await _employeeRepository.GetAsync(e => e.NationalId == dto.NationalId && !e.IsDeleted);
                if (existing != null)
                    return Result.Failure<Employee>("الرقم القومي مسجل بالفعل");

                var employee = new Employee
                {
                    Name = dto.Name,
                    NationalId = dto.NationalId,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Address = dto.Address,
                    DepartmentId = dto.DepartmentId,
                    JobTitle = dto.JobTitle,
                    HireDate = dto.HireDate ?? DateTime.Now,
                    BasicSalary = dto.BasicSalary,
                    Allowances = dto.Allowances ?? 0,
                    Deductions = dto.Deductions ?? 0,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                };

                var result = await _employeeRepository.AddAsync(employee);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Employee>($"فشل في إضافة الموظف: {ex.Message}");
            }
        }

        public async Task<Result<Employee>> UpdateEmployeeAsync(int id, UpdateEmployeeDto dto)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null || employee.IsDeleted)
                    return Result.Failure<Employee>("الموظف غير موجود");

                employee.Name = dto.Name;
                employee.NationalId = dto.NationalId;
                employee.Phone = dto.Phone;
                employee.Email = dto.Email;
                employee.Address = dto.Address;
                employee.DepartmentId = dto.DepartmentId;
                employee.JobTitle = dto.JobTitle;
                employee.BasicSalary = dto.BasicSalary;
                employee.Allowances = dto.Allowances ?? employee.Allowances;
                employee.Deductions = dto.Deductions ?? employee.Deductions;
                employee.IsActive = dto.IsActive ?? employee.IsActive;
                employee.UpdatedAt = DateTime.Now;

                var result = await _employeeRepository.UpdateAsync(employee);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Employee>($"فشل في تحديث الموظف: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _employeeRepository.GetByIdAsync(id);
                if (employee == null || employee.IsDeleted)
                    return Result.Failure<bool>("الموظف غير موجود");

                employee.IsDeleted = true;
                employee.DeletedAt = DateTime.Now;

                var updateResult = await _employeeRepository.UpdateAsync(employee);
                return Result.Ok(updateResult.Success);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في حذف الموظف: {ex.Message}");
            }
        }

        public async Task<Result<Department>> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            try
            {
                var department = new Department
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    ManagerId = dto.ManagerId,
                    CreatedAt = DateTime.Now
                };

                var result = await _departmentRepository.AddAsync(department);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Department>($"فشل في إضافة القسم: {ex.Message}");
            }
        }

        public async Task<Result<Salary>> ProcessSalaryAsync(int employeeId, int month, int year)
        {
            try
            {
                var employee = await _employeeRepository.GetAsync(e => e.Id == employeeId && !e.IsDeleted);
                if (employee == null)
                    return Result.Failure<Salary>("الموظف غير موجود");

                var existingSalary = await _salaryRepository.GetAsync(s => s.EmployeeId == employeeId && s.Month == month && s.Year == year);
                if (existingSalary != null)
                    return Result.Failure<Salary>("الراتب لهذا الشهر تم معالجته بالفعل");

                var salary = new Salary
                {
                    EmployeeId = employeeId,
                    Month = month,
                    Year = year,
                    BasicSalary = employee.BasicSalary,
                    Allowances = employee.Allowances,
                    Deductions = employee.Deductions,
                    Overtime = 0,
                    AbsenceDays = 0,
                    Bonus = 0,
                    Notes = $"راتب شهر {month}/{year}",
                    Status = "Pending",
                    CreatedAt = DateTime.Now
                };

                salary.GrossSalary = salary.BasicSalary + salary.Allowances + salary.Overtime + salary.Bonus;
                salary.NetSalary = salary.GrossSalary - salary.Deductions;

                var result = await _salaryRepository.AddAsync(salary);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Salary>($"فشل في معالجة الراتب: {ex.Message}");
            }
        }

        public async Task<Result<Attendance>> RecordAttendanceAsync(int employeeId, AttendanceType type, DateTime date)
        {
            try
            {
                var attendance = new Attendance
                {
                    EmployeeId = employeeId,
                    Date = date.Date,
                    Type = type.ToString(),
                    Time = DateTime.Now,
                    Notes = type == AttendanceType.CheckIn ? "تسجيل دخول" : "تسجيل خروج"
                };

                var result = await _attendanceRepository.AddAsync(attendance);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Attendance>($"فشل في تسجيل الحضور: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Attendance>>> GetAttendanceByEmployeeAsync(int employeeId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var attendances = await _attendanceRepository.GetAllAsync(a => a.EmployeeId == employeeId);
                
                if (fromDate.HasValue)
                    attendances = attendances.Where(a => a.Date >= fromDate.Value.Date);
                
                if (toDate.HasValue)
                    attendances = attendances.Where(a => a.Date <= toDate.Value.Date);

                return Result.Ok(attendances.OrderByDescending(a => a.Date).AsEnumerable());
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Attendance>>($"فشل في جلب الحضور: {ex.Message}");
            }
        }
    }
}

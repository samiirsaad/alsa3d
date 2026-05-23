using AlSa3d.Core.Entities;
using AlSa3d.Core.Entities.EmployeeModule;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AlSa3d.Services.Interfaces
{
    /// <summary>
    /// واجهة خدمة الموظفين والرواتب
    /// </summary>
    public interface IEmployeeService
    {
        #region Employee Operations

        /// <summary>
        /// إنشاء موظف جديد
        /// </summary>
        Task<Result<Employee>> CreateEmployeeAsync(Employee employee);

        /// <summary>
        /// تحديث بيانات موظف
        /// </summary>
        Task<Result<Employee>> UpdateEmployeeAsync(Employee employee);

        /// <summary>
        /// حذف موظف (Soft Delete)
        /// </summary>
        Task<Result<bool>> DeleteEmployeeAsync(Guid employeeId);

        /// <summary>
        /// الحصول على موظف بالمعرف
        /// </summary>
        Task<Result<Employee>> GetEmployeeByIdAsync(Guid employeeId);

        /// <summary>
        /// الحصول على جميع الموظفين
        /// </summary>
        Task<Result<List<Employee>>> GetAllEmployeesAsync();

        /// <summary>
        /// البحث عن موظف بالاسم
        /// </summary>
        Task<Result<List<Employee>>> SearchByNameAsync(string name);

        /// <summary>
        /// الحصول على موظف بالكود
        /// </summary>
        Task<Result<Employee>> GetEmployeeByCodeAsync(string employeeCode);

        /// <summary>
        /// تعيين موظف في قسم
        /// </summary>
        Task<Result<bool>> AssignToDepartmentAsync(Guid employeeId, Guid departmentId);

        /// <summary>
        /// ترقية موظف
        /// </summary>
        Task<Result<bool>> PromoteEmployeeAsync(Guid employeeId, string newJobTitle, decimal? newSalary = null);

        /// <summary>
        /// إنهاء خدمة موظف
        /// </summary>
        Task<Result<bool>> TerminateEmployeeAsync(Guid employeeId, DateTime terminationDate, string reason);

        #endregion

        #region Department Operations

        /// <summary>
        /// إنشاء قسم جديد
        /// </summary>
        Task<Result<Department>> CreateDepartmentAsync(Department department);

        /// <summary>
        /// تحديث قسم
        /// </summary>
        Task<Result<Department>> UpdateDepartmentAsync(Department department);

        /// <summary>
        /// حذف قسم
        /// </summary>
        Task<Result<bool>> DeleteDepartmentAsync(Guid departmentId);

        /// <summary>
        /// الحصول على جميع الأقسام
        /// </summary>
        Task<Result<List<Department>>> GetAllDepartmentsAsync();

        /// <summary>
        /// الحصول على موظفي القسم
        /// </summary>
        Task<Result<List<Employee>>> GetDepartmentEmployeesAsync(Guid departmentId);

        #endregion

        #region Salary Operations

        /// <summary>
        /// إنشاء سجل راتب للموظف
        /// </summary>
        Task<Result<SalaryRecord>> CreateSalaryRecordAsync(SalaryRecord salaryRecord);

        /// <summary>
        /// حساب راتب الموظف لشهر معين
        /// </summary>
        Task<Result<SalaryRecord>> CalculateSalaryAsync(Guid employeeId, int year, int month);

        /// <summary>
        /// تحديث سجل الراتب
        /// </summary>
        Task<Result<SalaryRecord>> UpdateSalaryRecordAsync(SalaryRecord salaryRecord);

        /// <summary>
        /// اعتماد الراتب
        /// </summary>
        Task<Result<bool>> ApproveSalaryAsync(Guid salaryRecordId);

        /// <summary>
        /// صرف الراتب
        /// </summary>
        Task<Result<bool>> PaySalaryAsync(Guid salaryRecordId, string paymentMethod, string referenceNumber);

        /// <summary>
        /// الحصول على سجلات راتب الموظف
        /// </summary>
        Task<Result<List<SalaryRecord>>> GetEmployeeSalaryRecordsAsync(Guid employeeId, int? year = null);

        /// <summary>
        /// الحصول على رواتب قسم معين
        /// </summary>
        Task<Result<List<SalaryRecord>>> GetDepartmentSalariesAsync(Guid departmentId, int year, int month);

        #endregion

        #region Attendance Operations

        /// <summary>
        /// تسجيل حضور موظف
        /// </summary>
        Task<Result<Attendance>> RecordAttendanceAsync(Attendance attendance);

        /// <summary>
        /// تسجيل انصراف موظف
        /// </summary>
        Task<Result<Attendance>> RecordDepartureAsync(Guid employeeId, DateTime departureTime);

        /// <summary>
        /// تحديث سجل الحضور
        /// </summary>
        Task<Result<Attendance>> UpdateAttendanceAsync(Attendance attendance);

        /// <summary>
        /// الحصول على حضور الموظف
        /// </summary>
        Task<Result<List<Attendance>>> GetEmployeeAttendanceAsync(Guid employeeId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// تقرير الحضور الشهري
        /// </summary>
        Task<Result<MonthlyAttendanceReport>> GetMonthlyAttendanceReportAsync(Guid employeeId, int year, int month);

        #endregion

        #region Leave Operations

        /// <summary>
        /// تقديم طلب إجازة
        /// </summary>
        Task<Result<LeaveRequest>> SubmitLeaveRequestAsync(LeaveRequest leaveRequest);

        /// <summary>
        /// الموافقة على الإجازة
        /// </summary>
        Task<Result<bool>> ApproveLeaveRequestAsync(Guid leaveRequestId, string approvedBy);

        /// <summary>
        /// رفض الإجازة
        /// </summary>
        Task<Result<bool>> RejectLeaveRequestAsync(Guid leaveRequestId, string rejectedBy, string reason);

        /// <summary>
        /// إلغاء الإجازة
        /// </summary>
        Task<Result<bool>> CancelLeaveRequestAsync(Guid leaveRequestId);

        /// <summary>
        /// الحصول على طلبات إجازة الموظف
        /// </summary>
        Task<Result<List<LeaveRequest>>> GetEmployeeLeaveRequestsAsync(Guid employeeId, int? year = null);

        /// <summary>
        /// الحصول على رصيد الإجازات
        /// </summary>
        Task<Result<LeaveBalance>> GetLeaveBalanceAsync(Guid employeeId);

        #endregion

        #region Reports

        /// <summary>
        /// تقرير الرواتب الشهري
        /// </summary>
        Task<Result<MonthlySalaryReport>> GetMonthlySalaryReportAsync(int year, int month);

        /// <summary>
        /// تقرير تفصيلي للموظف
        /// </summary>
        Task<Result<EmployeeDetailedReport>> GetEmployeeDetailedReportAsync(Guid employeeId);

        #endregion
    }

    #region Report Models

    public class MonthlySalaryReport
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalSalaries { get; set; }
        public decimal TotalAdditions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal NetTotal { get; set; }
        public List<SalarySummary> SalarySummaries { get; set; } = new();
    }

    public class SalarySummary
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Department { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal Additions { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }
        public bool IsPaid { get; set; }
    }

    public class MonthlyAttendanceReport
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int WorkingDays { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
        public int LeaveDays { get; set; }
        public TimeSpan TotalLateMinutes { get; set; }
        public List<DailyAttendance> DailyRecords { get; set; } = new();
    }

    public class DailyAttendance
    {
        public DateTime Date { get; set; }
        public TimeSpan? CheckIn { get; set; }
        public TimeSpan? CheckOut { get; set; }
        public string Status { get; set; }
        public TimeSpan? LateMinutes { get; set; }
    }

    public class EmployeeDetailedReport
    {
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public DateTime HireDate { get; set; }
        public decimal CurrentSalary { get; set; }
        public string EmploymentStatus { get; set; }
        public int TotalLeaveDays { get; set; }
        public int UsedLeaveDays { get; set; }
        public int RemainingLeaveDays { get; set; }
        public decimal TotalSalariesReceived { get; set; }
        public List<SalarySummary> RecentSalaries { get; set; } = new();
        public List<AttendanceSummary> AttendanceSummary { get; set; } = new();
    }

    public class AttendanceSummary
    {
        public string Month { get; set; }
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LateDays { get; set; }
    }

    #endregion
}

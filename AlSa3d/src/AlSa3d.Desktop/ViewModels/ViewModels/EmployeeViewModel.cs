using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
using AlSa3d.Desktop.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using EntityEmployee = AlSa3d.Core.Entities.Employee;

namespace AlSa3d.Desktop.ViewModels;

public class EmployeeDisplayModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public DateTime HireDate { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

public partial class EmployeeViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EmployeeDisplayModel> _employees = new();

    public EmployeeViewModel(IEmployeeService employeeService, IDialogService dialogService, INotificationService notificationService)
    {
        _employeeService = employeeService;
        _dialogService = dialogService;
        _notificationService = notificationService;
        LoadEmployeesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadEmployees()
    {
        var result = await _employeeService.GetAllEmployeesAsync();
        if (result.Success)
            Employees = new ObservableCollection<EmployeeDisplayModel>(
                result.Data.Select(e => new EmployeeDisplayModel
                {
                    Id = e.Id,
                    Name = e.Name,
                    DepartmentName = e.Department?.Name ?? "",
                    JobTitle = e.JobTitle ?? "",
                    BasicSalary = e.BasicSalary,
                    HireDate = e.HireDate,
                    StatusName = e.IsActive ? "نشط" : "غير نشط"
                }));
    }

    [RelayCommand]
    private async Task AddNewEmployee()
    {
        _notificationService.ShowInfo("إضافة موظف جديد - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task PaySalaries()
    {
        _notificationService.ShowInfo("صرف الرواتب - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task EditEmployee(EmployeeDisplayModel? employee)
    {
        if (employee == null) return;
        _notificationService.ShowInfo("تعديل الموظف - قيد التطوير");
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteEmployee(EmployeeDisplayModel? employee)
    {
        if (employee == null) return;
        if (!_dialogService.ShowConfirm($"هل أنت متأكد من حذف الموظف '{employee.Name}'؟"))
            return;
        var result = await _employeeService.DeleteEmployeeAsync(employee.Id);
        if (result.Success)
        {
            _notificationService.ShowSuccess("تم حذف الموظف بنجاح");
            await LoadEmployeesCommand.ExecuteAsync(null);
        }
        else
            _dialogService.ShowMessage(result.Message ?? "فشل حذف الموظف", "خطأ");
    }
}
using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Services.Interfaces;
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

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<EmployeeDisplayModel> _employees = new();

    public EmployeeViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
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
    private async Task AddNewEmployee() => await Task.CompletedTask;

    [RelayCommand]
    private async Task PaySalaries() => await Task.CompletedTask;

    [RelayCommand]
    private async Task EditEmployee(EmployeeDisplayModel? employee)
    {
        if (employee == null) return;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteEmployee(EmployeeDisplayModel? employee)
    {
        if (employee == null) return;
        var result = await _employeeService.DeleteEmployeeAsync(employee.Id);
        if (result.Success)
            await LoadEmployeesCommand.ExecuteAsync(null);
    }
}

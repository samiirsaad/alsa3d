using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AlSa3d.Core.Services.Interfaces;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AlSa3d.App.ViewModels;

public partial class EmployeeViewModel : ObservableObject
{
    private readonly IEmployeeService _employeeService;

    [ObservableProperty]
    private string _searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Employee> _employees = new();

    public EmployeeViewModel(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
        LoadEmployeesCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadEmployees()
    {
        var result = await _employeeService.GetAllEmployeesAsync();
        if (result.IsSuccess)
            Employees = new ObservableCollection<Employee>(result.Data);
    }

    [RelayCommand]
    private async Task AddNewEmployee() => await Task.CompletedTask;

    [RelayCommand]
    private async Task PaySalaries() => await Task.CompletedTask;

    [RelayCommand]
    private async Task EditEmployee(Employee? employee)
    {
        if (employee == null) return;
        await Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DeleteEmployee(Employee? employee)
    {
        if (employee == null) return;
        var result = await _employeeService.DeleteEmployeeAsync(employee.Id);
        if (result.IsSuccess)
            await LoadEmployeesCommand.ExecuteAsync(null);
    }
}

public class Employee
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string JobTitle { get; set; } = string.Empty;
    public decimal BasicSalary { get; set; }
    public DateTime HireDate { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

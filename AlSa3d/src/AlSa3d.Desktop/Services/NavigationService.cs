using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using AlSa3d.Desktop.ViewModels;
using AlSa3d.Desktop.Views;

namespace AlSa3d.Desktop.Services;

public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
    private readonly Stack<Type> _history = new();

    public NavigationService()
    {
        RegisterMapping<LoginViewModel, LoginView>();
        RegisterMapping<DashboardViewModel, DashboardView>();
        RegisterMapping<CustomerViewModel, CustomerView>();
        RegisterMapping<InvoiceViewModel, InvoiceView>();
        RegisterMapping<ProductViewModel, ProductView>();
        RegisterMapping<EmployeeViewModel, EmployeeView>();
        RegisterMapping<FinancialViewModel, FinancialView>();
        RegisterMapping<ReportsViewModel, ReportsView>();
        RegisterMapping<SettingsViewModel, SettingsView>();
    }

    private void RegisterMapping<TViewModel, TView>()
        where TViewModel : class
        where TView : UserControl, new()
    {
        _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
    }

    public UserControl CreateView<TViewModel>() where TViewModel : class
    {
        var vmType = typeof(TViewModel);
        _history.Push(vmType);
        return CreateViewForType(vmType);
    }

    public UserControl? GoBack()
    {
        if (_history.Count <= 1) return null;
        _history.Pop();
        var prev = _history.Peek();
        return CreateViewForType(prev);
    }

    public UserControl GoHome()
    {
        _history.Clear();
        _history.Push(typeof(DashboardViewModel));
        return CreateViewForType(typeof(DashboardViewModel));
    }

    private UserControl CreateViewForType(Type viewModelType)
    {
        if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
            throw new InvalidOperationException($"لم يتم العثور على View لـ {viewModelType.Name}");

        var viewModel = App.ServiceProvider.GetService(viewModelType);
        var newView = (UserControl)Activator.CreateInstance(viewType)!;
        newView.DataContext = viewModel;
        return newView;
    }
}

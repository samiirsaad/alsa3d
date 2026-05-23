using System;
using System.Collections.Generic;
using System.Windows;
using AlSa3d.Desktop.ViewModels;

namespace AlSa3d.Desktop.Services;

/// <summary>
/// خدمة التنقل بين الشاشات - Navigation Service
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Dictionary<Type, Type> _viewModelToViewMap = new();
    private readonly Stack<Type> _navigationHistory = new();
    private Window? _mainWindow;
    private ContentControl? _contentArea;

    public NavigationService()
    {
        // تسجيل خرائط ViewModel إلى View
        RegisterViewModelToView<LoginViewModel, LoginView>();
        RegisterViewModelToView<DashboardViewModel, DashboardView>();
        RegisterViewModelToView<CustomerViewModel, CustomerView>();
        RegisterViewModelToView<InvoiceViewModel, InvoiceView>();
        RegisterViewModelToView<ProductViewModel, ProductView>();
        RegisterViewModelToView<EmployeeViewModel, EmployeeView>();
        RegisterViewModelToView<FinancialViewModel, FinancialView>();
        RegisterViewModelToView<ReportsViewModel, ReportsView>();
        RegisterViewModelToView<SettingsViewModel, SettingsView>();
    }

    private void RegisterViewModelToView<TViewModel, TView>()
        where TViewModel : class
        where TView : UserControl, new()
    {
        _viewModelToViewMap[typeof(TViewModel)] = typeof(TView);
    }

    public void SetMainWindow(Window mainWindow)
    {
        _mainWindow = mainWindow;
        
        // البحث عن ContentControl في MainWindow
        if (mainWindow is MainWindow mw)
        {
            _contentArea = mw.FindName("MainContent") as ContentControl;
        }
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        if (!_viewModelToViewMap.TryGetValue(typeof(TViewModel), out var viewType))
        {
            throw new InvalidOperationException($"لم يتم العثور على View لـ {typeof(TViewModel).Name}");
        }

        // حفظ التاريخ
        if (_contentArea?.Content != null)
        {
            var currentType = _contentArea.Content.GetType();
            _navigationHistory.Push(currentType);
        }

        // إنشاء العرض الجديد
        var newView = (UserControl)Activator.CreateInstance(viewType)!;
        
        if (_contentArea != null)
        {
            _contentArea.Content = newView;
        }
    }

    public void GoBack()
    {
        if (_navigationHistory.Count > 0 && _contentArea != null)
        {
            var previousType = _navigationHistory.Pop();
            
            if (_viewModelToViewMap.ContainsValue(previousType))
            {
                var viewModelType = _viewModelToViewMap.First(x => x.Value == previousType).Key;
                NavigateToGeneric(viewModelType);
            }
        }
    }

    public void GoHome()
    {
        _navigationHistory.Clear();
        NavigateTo<DashboardViewModel>();
    }

    private void NavigateToGeneric(Type viewModelType)
    {
        if (!_viewModelToViewMap.TryGetValue(viewModelType, out var viewType))
        {
            return;
        }

        var newView = (UserControl)Activator.CreateInstance(viewType)!;
        
        if (_contentArea != null)
        {
            _contentArea.Content = newView;
        }
    }
}

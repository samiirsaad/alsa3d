# 🏛️ بنية معمارية Al-Sa3d

## نظرة عامة على البنية المعمارية

يعتمد مشروع **Al-Sa3d** على **Clean Architecture** مع تطبيق نمط **MVVM** لواجهات المستخدم.

---

## 📐 الطبقات المعمارية

```
┌─────────────────────────────────────────────────────────┐
│                   Presentation Layer                     │
│                    (AlSaad.UI - WPF)                     │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │   Views     │  │ ViewModels  │  │  Controls   │     │
│  │  (XAML)     │◄─┤   (MVVM)    │  │  (Custom)   │     │
│  └─────────────┘  └──────┬──────┘  └─────────────┘     │
│                          │                               │
└──────────────────────────┼───────────────────────────────┘
                           │
┌──────────────────────────▼───────────────────────────────┐
│                   Application Layer                       │
│                  (AlSaad.Core.Services)                   │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │   Invoice   │  │  Customer   │  │   Salary    │     │
│  │   Service   │  │   Service   │  │   Service   │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
│                                                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │   Product   │  │    Bank     │  │    Auth     │     │
│  │   Service   │  │   Service   │  │   Service   │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
└──────────────────────────┬───────────────────────────────┘
                           │
┌──────────────────────────▼───────────────────────────────┐
│                    Domain Layer                           │
│                   (AlSaad.Core.Entities)                  │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │  Entities   │  │ Interfaces  │  │   Enums     │     │
│  │   (Models)  │  │  (Contracts)│  │            │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
│                                                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │ Exceptions  │  │  Events     │  │  Specifications│   │
│  │            │  │            │  │              │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
└──────────────────────────┬───────────────────────────────┘
                           │
┌──────────────────────────▼───────────────────────────────┐
│                 Infrastructure Layer                      │
│                   (AlSaad.Data & Reports)                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │Repository   │  │    EF Core  │  │   Reports   │     │
│  │  Pattern    │  │   Context   │  │  Generator  │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
│                                                          │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │    Unit     │  │ Migrations  │  │   Barcodes  │     │
│  │   of Work   │  │            │  │            │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
└──────────────────────────┬───────────────────────────────┘
                           │
┌──────────────────────────▼───────────────────────────────┐
│                    Data Layer                             │
│                  (SQL Server Database)                    │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐     │
│  │   Tables    │  │   Views     │  │  Procedures │     │
│  │            │  │            │  │            │     │
│  └─────────────┘  └─────────────┘  └─────────────┘     │
└─────────────────────────────────────────────────────────┘
```

---

## 🎯 مبادئ التصميم

### 1. فصل الاهتمامات (Separation of Concerns)
```csharp
// ❌ خطأ: كل شيء في مكان واحد
public class InvoiceController {
    public void SaveInvoice() {
        // UI logic
        // Business logic
        // Database logic
    }
}

// ✅ صحيح: كل طبقة مسؤولة عن شيء
public class InvoiceViewModel { /* UI Logic */ }
public class InvoiceService { /* Business Logic */ }
public class InvoiceRepository { /* Data Access */ }
```

### 2. الاعتماد على التجريد (Dependency Inversion)
```csharp
// الاعتماد على الـ Interface وليس التطبيق
public interface IInvoiceRepository {
    Task<Invoice> GetByIdAsync(int id);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<Invoice> CreateAsync(Invoice invoice);
}

public class InvoiceService {
    private readonly IInvoiceRepository _repository;
    
    public InvoiceService(IInvoiceRepository repository) {
        _repository = repository; // Dependency Injection
    }
}
```

### 3. نمط Repository
```csharp
public interface IRepository<T> where T : BaseEntity {
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}

public class EfRepository<T> : IRepository<T> where T : BaseEntity {
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public EfRepository(AppDbContext context) {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    // Implementation...
}
```

### 4. نمط Unit of Work
```csharp
public interface IUnitOfWork : IDisposable {
    IInvoiceRepository Invoices { get; }
    ICustomerRepository Customers { get; }
    IProductRepository Products { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

---

## 📦 هيكل الحل (Solution Structure)

```
AlSaad.sln
│
├── src/
│   │
│   ├── AlSaad.Core/                    # Class Library
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── Invoice.cs
│   │   │   ├── InvoiceItem.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Product.cs
│   │   │   ├── Employee.cs
│   │   │   └── ...
│   │   │
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IInvoiceRepository.cs
│   │   │   │   ├── ICustomerRepository.cs
│   │   │   │   └── ...
│   │   │   ├── Services/
│   │   │   │   ├── IInvoiceService.cs
│   │   │   │   ├── ICustomerService.cs
│   │   │   │   └── ...
│   │   │   └── IUnitOfWork.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── InvoiceService.cs
│   │   │   ├── CustomerService.cs
│   │   │   ├── AuthService.cs
│   │   │   └── ...
│   │   │
│   │   ├── Common/
│   │   │   ├── Result.cs
│   │   │   ├── PagedList.cs
│   │   │   ├── AuditInfo.cs
│   │   │   └── ...
│   │   │
│   │   └── Exceptions/
│   │       ├── BusinessException.cs
│   │       ├── NotFoundException.cs
│   │       └── ValidationException.cs
│   │
│   ├── AlSaad.Data/                    # Class Library
│   │   ├── Context/
│   │   │   └── AppDbContext.cs
│   │   │
│   │   ├── Configurations/
│   │   │   ├── InvoiceConfiguration.cs
│   │   │   ├── CustomerConfiguration.cs
│   │   │   └── ...
│   │   │
│   │   ├── Repositories/
│   │   │   ├── InvoiceRepository.cs
│   │   │   ├── CustomerRepository.cs
│   │   │   └── ...
│   │   │
│   │   ├── UnitOfWork/
│   │   │   └── UnitOfWork.cs
│   │   │
│   │   └── Migrations/
│   │       └── [Auto-generated]
│   │
│   ├── AlSaad.UI/                      # WPF Application
│   │   ├── App.xaml(.cs)
│   │   ├── MainWindow.xaml(.cs)
│   │   │
│   │   ├── Views/
│   │   │   ├── LoginView.xaml
│   │   │   ├── DashboardView.xaml
│   │   │   ├── InvoiceView.xaml
│   │   │   ├── CustomerView.xaml
│   │   │   └── ...
│   │   │
│   │   ├── ViewModels/
│   │   │   ├── BaseViewModel.cs
│   │   │   ├── LoginViewModel.cs
│   │   │   ├── DashboardViewModel.cs
│   │   │   ├── InvoiceViewModel.cs
│   │   │   └── ...
│   │   │
│   │   ├── Controls/
│   │   │   ├── CustomTextBox.xaml
│   │   │   ├── CustomButton.xaml
│   │   │   ├── DataGridHelper.xaml
│   │   │   └── ...
│   │   │
│   │   ├── Converters/
│   │   │   ├── BoolToVisibilityConverter.cs
│   │   │   ├── DateToStringConverter.cs
│   │   │   └── ...
│   │   │
│   │   ├── Resources/
│   │   │   ├── Styles/
│   │   │   │   ├── Colors.xaml
│   │   │   │   ├── Buttons.xaml
│   │   │   │   └── ...
│   │   │   └── Templates/
│   │   │       └── ...
│   │   │
│   │   └── Helpers/
│   │       ├── DialogHelper.cs
│   │       ├── NavigationHelper.cs
│   │       └── ...
│   │
│   └── AlSaad.Reports/                 # Class Library
│       ├── Designs/
│       │   ├── InvoiceReport.Designer.cs
│       │   ├── CustomerReport.Designer.cs
│       │   └── ...
│       │
│       ├── Generators/
│       │   ├── ReportGenerator.cs
│       │   ├── PdfGenerator.cs
│       │   └── ExcelGenerator.cs
│       │
│       ├── Export/
│       │   ├── PdfExporter.cs
│       │   ├── ExcelExporter.cs
│       │   └── WordExporter.cs
│       │
│       └── Barcodes/
│           ├── BarcodeGenerator.cs
│           └── QrCodeGenerator.cs
│
├── tests/
│   ├── AlSaad.Core.Tests/
│   ├── AlSaad.Data.Tests/
│   └── AlSaad.UI.Tests/
│
└── AlSaad.Installer/                   # Setup Project
    └── [Installer Files]
```

---

## 🔗 تدفق البيانات (Data Flow)

### مثال: إنشاء فاتورة جديدة

```
┌─────────────┐
│   User      │ يقوم بإنشاء فاتورة جديدة
└──────┬──────┘
       │
       ▼
┌─────────────┐
│  InvoiceView│ (WPF Window)
│  (XAML)     │ يعرض نموذج الفاتورة
└──────┬──────┘
       │ Binding
       ▼
┌─────────────┐
│InvoiceVM    │ (ViewModel)
│             │ • يستقبل البيانات
│             │ • يتحقق من الصحة
│             │ • يستدعي الخدمة
└──────┬──────┘
       │ Call
       ▼
┌─────────────┐
│InvoiceSvc   │ (Service Layer)
│             │ • يطبق قواعد العمل
│             │ • يحسب الإجماليات
│             │ • يتحقق من المخزون
└──────┬──────┘
       │ Request
       ▼
┌─────────────┐
│UnitOfWork   │ (UoW Pattern)
│             │ • يبدأ معاملة
│             │ • ينسق بين repositories
└──────┬──────┘
       │ Call
       ▼
┌─────────────┐
│InvoiceRepo  │ (Repository)
│             │ • يجهز البيانات
│             │ • يستدعي EF Core
└──────┬──────┘
       │ SQL
       ▼
┌─────────────┐
│  SQL Server │
│  Database   │ • يحفظ البيانات
│             │ • يرجع ID
└──────┬──────┘
       │
       ▼
[Return Path - نفس المسار بالعكس]
       │
       ▼
┌─────────────┐
│   User      │ يرى رسالة النجاح
└─────────────┘
```

---

## 💉 حقن التبعيات (Dependency Injection)

### تسجيل الخدمات في Program.cs / App.xaml.cs

```csharp
public partial class App : Application
{
    private readonly IHost _host;
    
    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                // Database Context
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(
                        context.Configuration.GetConnectionString("DefaultConnection")));
                
                // Repositories
                services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
                services.AddScoped<IInvoiceRepository, InvoiceRepository>();
                services.AddScoped<ICustomerRepository, CustomerRepository>();
                services.AddScoped<IProductRepository, ProductRepository>();
                
                // Unit of Work
                services.AddScoped<IUnitOfWork, UnitOfWork>();
                
                // Services
                services.AddScoped<IInvoiceService, InvoiceService>();
                services.AddScoped<ICustomerService, CustomerService>();
                services.AddScoped<IAuthService, AuthService>();
                
                // ViewModels
                services.AddTransient<LoginViewModel>();
                services.AddTransient<DashboardViewModel>();
                services.AddTransient<InvoiceViewModel>();
                
                // Views
                services.AddTransient<MainWindow>();
            })
            .Build();
    }
    
    protected override async void OnStartup(StartupEventArgs e)
    {
        await _host.StartAsync();
        
        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
        
        base.OnStartup(e);
    }
}
```

---

## 🔄 نمط MVVM المفصل

### ViewModel Base

```csharp
public abstract class BaseViewModel : ObservableObject
{
    protected readonly INavigationService _navigationService;
    protected readonly IDialogService _dialogService;
    
    private bool _isBusy;
    private string _title;
    
    public bool IsBusy
    {
        get => _isBusy;
        set => SetProperty(ref _isBusy, value);
    }
    
    public string Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }
    
    public ICommand NavigateCommand { get; }
    
    protected BaseViewModel(
        INavigationService navigationService,
        IDialogService dialogService)
    {
        _navigationService = navigationService;
        _dialogService = dialogService;
    }
}
```

### مثال: InvoiceViewModel

```csharp
public partial class InvoiceViewModel : BaseViewModel
{
    private readonly IInvoiceService _invoiceService;
    
    [ObservableProperty]
    private ObservableCollection<InvoiceItemViewModel> _items;
    
    [ObservableProperty]
    private Customer _selectedCustomer;
    
    [ObservableProperty]
    private decimal _totalAmount;
    
    [ObservableProperty]
    private bool _canSave;
    
    public IAsyncRelayCommand SaveCommand { get; }
    public IRelayCommand AddItemCommand { get; }
    public IRelayCommand DeleteItemCommand { get; }
    
    public InvoiceViewModel(
        IInvoiceService invoiceService,
        INavigationService navigationService,
        IDialogService dialogService)
        : base(navigationService, dialogService)
    {
        _invoiceService = invoiceService;
        
        Items = new ObservableCollection<InvoiceItemViewModel>();
        
        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        AddItemCommand = new RelayCommand(AddItem);
        DeleteItemCommand = new RelayCommand<InvoiceItemViewModel>(DeleteItem);
        
        Items.CollectionChanged += (s, e) => 
        {
            CalculateTotal();
            (SaveCommand as AsyncRelayCommand)?.NotifyCanExecuteChanged();
        };
    }
    
    private bool CanSave() => !IsBusy && Items.Any() && SelectedCustomer != null;
    
    private async Task SaveAsync()
    {
        try
        {
            IsBusy = true;
            
            var invoice = new Invoice
            {
                CustomerId = SelectedCustomer.Id,
                Date = DateTime.Now,
                Items = Items.Select(i => i.ToEntity()).ToList()
            };
            
            var result = await _invoiceService.CreateInvoiceAsync(invoice);
            
            if (result.Success)
            {
                await _dialogService.ShowMessageAsync(
                    "تم حفظ الفاتورة بنجاح", 
                    "نجاح");
                    
                await _navigationService.NavigateAsync<DashboardViewModel>();
            }
            else
            {
                await _dialogService.ShowErrorAsync(result.Message);
            }
        }
        catch (Exception ex)
        {
            await _dialogService.ShowErrorAsync($"حدث خطأ: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    
    private void AddItem()
    {
        Items.Add(new InvoiceItemViewModel());
    }
    
    private void DeleteItem(InvoiceItemViewModel item)
    {
        if (item != null)
            Items.Remove(item);
    }
    
    private void CalculateTotal()
    {
        TotalAmount = Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
```

---

## 📊 كائنات قاعدة البيانات

### Entity Example

```csharp
public class Invoice : BaseEntity
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; }
    public DateTime Date { get; set; }
    public int CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal NetAmount { get; set; }
    public InvoiceStatus Status { get; set; }
    public string Notes { get; set; }
    
    // Navigation Properties
    public Customer Customer { get; set; }
    public ICollection<InvoiceItem> Items { get; set; }
    public ICollection<Payment> Payments { get; set; }
    
    // Audit Properties
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? ModifiedBy { get; set; }
    public DateTime? ModifiedAt { get; set; }
}
```

### EF Core Configuration

```csharp
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.ToTable("Invoices");
        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.InvoiceNumber)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(x => x.TotalAmount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.HasIndex(x => x.InvoiceNumber)
            .IsUnique();
        
        builder.HasIndex(x => x.Date);
        
        builder.HasIndex(x => x.CustomerId);
        
        builder.HasOne(x => x.Customer)
            .WithMany(c => c.Invoices)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasMany(x => x.Items)
            .WithOne(i => i.Invoice)
            .HasForeignKey(i => i.InvoiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
```

---

## 🎨 تصميم الشاشات

### MainWindow.xaml

```xml
<Window x:Class="AlSaad.UI.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:md="http://materialdesigninxaml.net/winfx/xaml/themes"
        Title="Al-Sa3d - نظام المحاسبة المتكامل"
        Height="800" Width="1200"
        WindowState="Maximized"
        FlowDirection="RightToLeft">
    
    <md:DialogHost>
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="250"/>
                <ColumnDefinition Width="*"/>
            </Grid.ColumnDefinitions>
            
            <!-- القائمة الجانبية -->
            <md:ColorZone Grid.Column="0" Mode="PrimaryMid">
                <DockPanel>
                    <!-- الشعار -->
                    <StackPanel DockPanel.Dock="Top" Margin="20">
                        <Image Source="/Assets/logo.png" Height="80"/>
                        <TextBlock Text="Al-Sa3d" 
                                   Style="{StaticResource MaterialDesignHeadline6Typography}"
                                   HorizontalAlignment="Center"
                                   Margin="0,10,0,0"/>
                    </StackPanel>
                    
                    <!-- عناصر القائمة -->
                    <ListBox Style="{StaticResource MaterialDesignNavigationListBox}"
                             ItemsSource="{Binding MenuItems}">
                        <ListBox.ItemTemplate>
                            <DataTemplate>
                                <DockPanel Margin="10">
                                    <md:PackIcon Kind="{Binding Icon}" 
                                                 Margin="0,0,10,0"/>
                                    <TextBlock Text="{Binding Title}"
                                               VerticalAlignment="Center"/>
                                </DockPanel>
                            </DataTemplate>
                        </ListBox.ItemTemplate>
                    </ListBox>
                </DockPanel>
            </md:ColorZone>
            
            <!-- منطقة المحتوى -->
            <ContentControl Grid.Column="1" 
                            Content="{Binding CurrentViewModel}"/>
        </Grid>
    </md:DialogHost>
</Window>
```

---

## 🔒 الأمان

### Authentication Service

```csharp
public interface IAuthService
{
    Task<AuthResult> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    User GetCurrentUser();
    bool HasPermission(string permission);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    
    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        var user = await _userRepository.GetByUsernameAsync(username);
        
        if (user == null || !_passwordHasher.Verify(password, user.PasswordHash))
        {
            return AuthResult.Failure("اسم المستخدم أو كلمة المرور غير صحيحة");
        }
        
        if (!user.IsActive)
        {
            return AuthResult.Failure("الحساب غير مفعل");
        }
        
        var token = _tokenService.GenerateToken(user);
        
        await _userRepository.UpdateLastLoginAsync(user.Id);
        
        return AuthResult.Success(user, token);
    }
}
```

---

## 📝 الخلاصة

هذه البنية المعمارية توفر:

✅ **قابلية الصيانة**: كود منظم وسهل الفهم  
✅ **القابلية للاختبار**: كل طبقة قابلة للاختبار بشكل منفصل  
✅ **المرونة**: سهولة إضافة ميزات جديدة  
✅ **الأداء**: تحسين استعلامات قاعدة البيانات  
✅ **الأمان**: حماية البيانات والصلاحيات  
✅ **التوسع**: دعم عدد كبير من المستخدمين  

**الخطوة التالية**: البدء في تنفيذ الكيانات الأساسية! 🚀

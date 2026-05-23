# 💡 أمثلة الكود المحسّن - Al-Sa3d

دليل سريع لاستخدام الميزات الجديدة في المشروع

---

## 🔹 1. استخدام Result Pattern المحسّن

### قبل (القديم):
```csharp
public async Task<Result<User>> LoginAsync(string username, string password)
{
    try
    {
        // ...code
        return Result.Ok(user);
    }
    catch (Exception ex)
    {
        return Result.Failure<User>($"خطأ: {ex.Message}");
    }
}
```

### بعد (الجديد):
```csharp
public async Task<Result<User>> LoginAsync(string username, string password)
{
    try
    {
        _logger.LogInformation("📝 محاولة تسجيل دخول: {username}", username);
        
        // Validate input
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return Result.Failure<User>("اسم المستخدم وكلمة المرور مطلوبة");
        
        var user = await _userRepository.GetAsync(u => u.Username == username);
        
        if (user == null)
        {
            _logger.LogWarning("⚠️ محاولة تسجيل دخول باسم مستخدم غير موجود");
            return Result.Failure<User>("اسم المستخدم غير موجود");
        }
        
        // ...more checks
        
        _logger.LogInformation("✅ تسجيل دخول ناجح: {username}", username);
        return Result.Ok(user);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "❌ خطأ في تسجيل الدخول");
        // الآن نمرر الـ exception أيضاً
        return Result.Failure<User>("فشل في تسجيل الدخول", ex);
    }
}
```

### الفائدة:
```csharp
// الآن يمكنك الوصول للـ exception في الـ UI
var result = await _userService.LoginAsync(username, password);

if (result.IsFailed)
{
    // استخدام IsFailed بدل !result.IsSuccess
    MessageBox.Show(result.Message);
    
    // إذا أردت الـ exception
    if (result.Exception != null)
    {
        _logger.LogError(result.Exception, "Error details");
    }
}

// أو multiple errors
var validationResult = Result.Failure<User>(
    "اسم المستخدم قصير جداً",
    "كلمة المرور ضعيفة جداً",
    "الحساب معطل"
);
// validationResult.Errors سيحتوي على جميع الأخطاء
```

---

## 🔹 2. استخدام Logging

### قبل:
```csharp
// بدون logging
var customers = await _customerService.GetAllCustomersAsync();
```

### بعد:
```csharp
private readonly ILogger<CustomerViewModel> _logger;

// في الـ constructor
public CustomerViewModel(
    ICustomerService customerService,
    ILogger<CustomerViewModel> logger)
{
    _customerService = customerService;
    _logger = logger;
}

// في الـ methods
[RelayCommand]
private async Task LoadCustomers()
{
    try
    {
        IsLoading = true;
        ErrorMessage = null;
        
        _logger.LogInformation("📖 جاري تحميل قائمة العملاء...");
        
        var result = await _customerService.GetAllCustomersAsync();
        
        if (result.IsSuccess)
        {
            Customers = new ObservableCollection<Customer>(result.Data);
            _logger.LogInformation("✅ تم تحميل {count} عميل", Customers.Count);
        }
        else
        {
            ErrorMessage = result.Message;
            _logger.LogWarning("⚠️ فشل تحميل العملاء: {message}", result.Message);
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = "حدث خطأ عند تحميل العملاء";
        _logger.LogError(ex, "❌ خطأ في تحميل العملاء");
    }
    finally
    {
        IsLoading = false;
    }
}
```

### مستويات Logging:
```csharp
_logger.LogDebug("🐛 معلومة تفصيلية للتطوير");
_logger.LogInformation("ℹ️ معلومة عادية");
_logger.LogWarning("⚠️ تحذير - قد يكون هناك مشكلة");
_logger.LogError(ex, "❌ خطأ خطير");
_logger.LogCritical(ex, "🔴 خطأ حرج - التطبيق قد لا يعمل");
```

---

## 🔹 3. استخدام Validation على DTOs

### قبل:
```csharp
public class RegisterUserDto
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}

// بدون validation - قد تمرر بيانات خاطئة
```

### بعد:
```csharp
using System.ComponentModel.DataAnnotations;

public class RegisterUserDto
{
    [Required(ErrorMessage = "اسم المستخدم مطلوب")]
    [StringLength(50, MinimumLength = 3, 
        ErrorMessage = "اسم المستخدم يجب أن يكون بين 3 و 50 حرف")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "كلمة المرور مطلوبة")]
    [StringLength(100, MinimumLength = 6, 
        ErrorMessage = "كلمة المرور يجب أن تكون على الأقل 6 أحرف")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "الاسم الكامل مطلوب")]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
    public string? Email { get; set; }
}
```

### الاستخدام:
```csharp
public async Task<Result<User>> RegisterAsync(RegisterUserDto dto)
{
    // Validate using DataAnnotations
    var context = new ValidationContext(dto);
    var results = new List<ValidationResult>();
    
    if (!Validator.TryValidateObject(dto, context, results, true))
    {
        var errors = results.Select(r => r.ErrorMessage).ToArray();
        return Result.Failure<User>(errors);
    }
    
    // ... register logic
}
```

---

## 🔹 4. معالجة الأخطاء في ViewModels

### قبل:
```csharp
[RelayCommand]
private async Task DeleteCustomer(Customer customer)
{
    var result = await _customerService.DeleteCustomerAsync(customer.Id);
    if (result.IsSuccess)
    {
        await LoadCustomersCommand.ExecuteAsync(null);
    }
    // وماذا إذا فشل؟ لا يوجد تعامل!
}
```

### بعد:
```csharp
[ObservableProperty]
private string? _errorMessage;

[ObservableProperty]
private bool _isLoading = false;

[RelayCommand]
private async Task DeleteCustomer(Customer? customer)
{
    try
    {
        if (customer == null)
        {
            ErrorMessage = "يجب اختيار عميل أولاً";
            return;
        }

        IsLoading = true;
        ErrorMessage = null;
        _logger.LogInformation("🗑️ حذف العميل: {customerId}", customer.Id);
        
        var result = await _customerService.DeleteCustomerAsync(customer.Id);
        
        if (result.IsSuccess)
        {
            await LoadCustomersCommand.ExecuteAsync(null);
            _logger.LogInformation("✅ تم حذف العميل: {customerId}", customer.Id);
        }
        else
        {
            ErrorMessage = result.Message;
            _logger.LogWarning("⚠️ فشل حذف العميل: {message}", result.Message);
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = "فشل في حذف العميل";
        _logger.LogError(ex, "❌ خطأ في حذف العميل: {customerId}", customer?.Id);
    }
    finally
    {
        IsLoading = false;
    }
}
```

### الفائدة:
```xaml
<!-- في XAML يمكن عرض الأخطاء للمستخدم -->
<TextBlock Text="{Binding ErrorMessage}" 
           Foreground="Red" 
           Visibility="{Binding ErrorMessage, Converter=...}"/>

<!-- وإظهار Loading indicator -->
<ProgressRing IsActive="{Binding IsLoading}" />
```

---

## 🔹 5. XML Documentation للـ IntelliSense

### قبل:
```csharp
public async Task<Result<Customer>> GetCustomerByIdAsync(int id)
{
    // بدون شرح - المطورون لا يعرفون ماذا يفعل
    return await _customerService.GetCustomerByIdAsync(id);
}
```

### بعد:
```csharp
/// <summary>
/// الحصول على عميل بواسطة معرفه
/// </summary>
/// <param name="id">معرف العميل المطلوب البحث عنه</param>
/// <returns>
/// نتيجة تحتوي على:
/// - Data: بيانات العميل إذا وُجد
/// - IsSuccess: true إذا نجحت العملية
/// - Message: رسالة توضيحية
/// </returns>
/// <exception cref="ArgumentException">إذا كان id أقل من 1</exception>
/// <example>
/// <code>
/// var result = await GetCustomerByIdAsync(5);
/// if (result.IsSuccess)
/// {
///     MessageBox.Show($"العميل: {result.Data.Name}");
/// }
/// </code>
/// </example>
public async Task<Result<Customer>> GetCustomerByIdAsync(int id)
{
    if (id < 1)
        throw new ArgumentException("معرف العميل يجب أن يكون أكبر من 0", nameof(id));
    
    return await _customerService.GetCustomerByIdAsync(id);
}
```

### النتيجة في IntelliSense:
```
GetCustomerByIdAsync(int id)
━━━━━━━━━━━━━━━━━━━━━━━━━━━
الحصول على عميل بواسطة معرفه

معاملات:
  id: معرف العميل المطلوب البحث عنه

القيمة المرجعة:
  نتيجة تحتوي على بيانات العميل...
```

---

## 🔹 6. الأمان المحسّن

### قبل:
```csharp
if (!VerifyPassword(password, user.PasswordHash))
    return Result.Failure<User>("كلمة المرور غير صحيحة");
```

### بعد:
```csharp
// 1️⃣ التحقق من الطول الأدنى
if (password.Length < 6)
    return Result.Failure<User>("كلمة المرور يجب أن تكون 6 أحرف على الأقل");

// 2️⃣ التحقق من القوة
if (!IsStrongPassword(password))
    return Result.Failure<User>(
        "كلمة المرور ضعيفة - يجب أن تحتوي على حروف وأرقام ورموز خاصة"
    );

// 3️⃣ Hash أفضل (استخدم bcrypt أو Argon2 بدل SHA256)
private string HashPassword(string password)
{
    // ✅ الجديد: استخدام bcrypt
    // return BCrypt.Net.BCrypt.HashPassword(password);
    
    // ⚠️ القديم (لا يزال يعمل لكن أضعف):
    using (var sha256 = SHA256.Create())
    {
        var bytes = Encoding.UTF8.GetBytes(password);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}

// 4️⃣ التحقق من الحساب المُغلق
var failedAttempts = user.FailedLoginAttempts ?? 0;
if (failedAttempts >= 5)
{
    user.LockedUntil = DateTime.Now.AddMinutes(30);
    await _userRepository.UpdateAsync(user);
    return Result.Failure<User>("الحساب مُغلق - حاول لاحقاً");
}

// 5️⃣ تسجيل محاولات فاشلة
if (!VerifyPassword(password, user.PasswordHash))
{
    user.FailedLoginAttempts = (user.FailedLoginAttempts ?? 0) + 1;
    await _userRepository.UpdateAsync(user);
    
    _logger.LogWarning("⚠️ محاولة دخول فاشلة: {username}", username);
    return Result.Failure<User>("كلمة المرور غير صحيحة");
}

// 6️⃣ إعادة تعيين العدّاد عند نجاح الدخول
user.FailedLoginAttempts = 0;
user.LastLoginAt = DateTime.Now;
await _userRepository.UpdateAsync(user);
```

---

## 🎯 النمط الموصى به للعمليات

```csharp
[RelayCommand]
private async Task PerformOperation()
{
    try
    {
        // 1️⃣ تعيين الحالة الأولية
        IsLoading = true;
        ErrorMessage = null;
        
        // 2️⃣ تسجيل البداية
        _logger.LogInformation("📝 بدء العملية...");
        
        // 3️⃣ التحقق من الشروط
        if (SelectedItem == null)
        {
            ErrorMessage = "يجب اختيار عنصر أولاً";
            return;
        }
        
        // 4️⃣ تنفيذ العملية
        var result = await _service.PerformAsync(SelectedItem.Id);
        
        // 5️⃣ معالجة النتيجة
        if (result.IsSuccess)
        {
            _logger.LogInformation("✅ نجاح العملية");
            // تحديث البيانات
            await RefreshDataCommand.ExecuteAsync(null);
        }
        else
        {
            ErrorMessage = result.Message;
            _logger.LogWarning("⚠️ فشل العملية: {message}", result.Message);
        }
    }
    catch (Exception ex)
    {
        ErrorMessage = "حدث خطأ غير متوقع";
        _logger.LogError(ex, "❌ خطأ في العملية");
    }
    finally
    {
        // 6️⃣ إعادة تعيين الحالة
        IsLoading = false;
    }
}
```

---

## ✅ Checklist لكل method جديد

- [ ] إضافة XML documentation
- [ ] إضافة logging
- [ ] إضافة validation
- [ ] معالجة exceptions
- [ ] إضافة error messages
- [ ] اختبار الحالات الخاطئة
- [ ] تسجيل العمليات المهمة
- [ ] توثيق الأخطاء المحتملة

---

**جميع الأمثلة الآن مطبقة في المشروع! استخدمها كمرجع للكود الجديد.** 🚀

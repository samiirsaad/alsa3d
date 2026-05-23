# 📊 Al-Sa3d Infrastructure Layer

## نظرة عامة
طبقة البنية التحتية تحتوي على:
- **قاعدة البيانات** (Entity Framework Core)
- **إعدادات الكيانات** (Configurations)
- **المستودعات** (Repositories)
- **الهجرات** (Migrations)

---

## 🗂️ الهيكل

```
AlSaad.Infrastructure/
│
├── Data/
│   ├── AppDbContext.cs              ← سياق قاعدة البيانات الرئيسي
│   │
│   ├── Configurations/              ← إعدادات الجداول
│   │   ├── CustomerConfiguration.cs
│   │   ├── InvoiceConfiguration.cs
│   │   ├── EmployeeConfiguration.cs
│   │   └── BankConfiguration.cs
│   │
│   ├── Repositories/                ← مستودعات البيانات
│   │   └── GenericRepository.cs
│   │
│   └── Migrations/                  ← الهجرات (تُنشأ تلقائياً)
│
└── AlSaad.Infrastructure.csproj
```

---

## 📋 الكيانات المدعومة

### 1. العملاء والعناوين
- `Customers` - العملاء
- `CustomerAddresses` - عناوين العملاء
- `CustomerContacts` - جهات اتصال العملاء

### 2. الفواتير والمخازن
- `Invoices` - الفواتير
- `InvoiceItems` - أصناف الفاتورة
- `Products` - المنتجات
- `Warehouses` - المخازن
- `StockMovements` - حركات المخزن

### 3. الموظفين والرواتب
- `Employees` - الموظفين
- `Departments` - الأقسام
- `Salaries` - الرواتب
- `Attendances` - الحضور والانصراف

### 4. المستخدمين والصلاحيات
- `Users` - المستخدمين
- `Roles` - الأدوار
- `Permissions` - الصلاحيات
- `UserRoles` - ربط المستخدمين بالأدوار
- `RolePermissions` - ربط الأدوار بالصلاحيات

### 5. البنوك والمالية
- `Banks` - البنوك
- `BankAccounts` - الحسابات البنكية
- `Checks` - الشيكات
- `Payments` - المدفوعات
- `Expenses` - المصروفات

---

## 🔧 الإعدادات

### إضافة إلى appsettings.json:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AlSaadDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

---

## 🚀 الاستخدام

### في Program.cs:

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
    )
);

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
```

---

## 📦 Entity Framework Migrations

### إنشاء هجرة جديدة:
```bash
dotnet ef migrations add InitialCreate --project src/AlSaad.Infrastructure --startup-project src/AlSaad.WebApi
```

### تحديث قاعدة البيانات:
```bash
dotnet ef database update --project src/AlSaad.Infrastructure --startup-project src/AlSaad.WebApi
```

### حذف جميع الهجرات:
```bash
dotnet ef migrations remove --project src/AlSaad.Infrastructure --startup-project src/AlSaad.WebApi
```

---

## 🎯 الميزات

✅ **إعدادات كاملة** لجميع الكيانات
✅ **علاقات** محددة بدقة (One-to-Many, Many-to-Many)
✅ **فهارس** (Indexes) للأداء العالي
✅ **بيانات أولية** (Seed Data) للأدوار والمستخدمين
✅ **تحديث تلقائي** لتواريخ الإنشاء والتعديل
✅ **دعم كامل للعربية** في جميع الحقول

---

## 📊 قاعدة البيانات

### معلومات تقنية:
- **نوع القاعدة:** SQL Server 2019+
- **ORM:** Entity Framework Core 8
- **نمط التصميم:** Repository Pattern
- **الترميز:** UTF-8 (دعم كامل للعربية)

### الجداول المتوقعة:
| المجموعة | عدد الجداول |
|----------|-------------|
| العملاء | 3 |
| الفواتير | 5 |
| الموظفين | 4 |
| المستخدمين | 5 |
| البنوك | 5 |
| **الإجمالي** | **22 جدول** |

---

## 🔐 Seed Data

يتم إنشاء البيانات التالية تلقائياً:

### الأدوار:
1. **Admin** - مدير النظام (صلاحيات كاملة)
2. **Manager** - مدير (صلاحيات إدارية)
3. **Accountant** - محاسب (صلاحيات محاسبية)
4. **Sales** - مبيعات (صلاحيات مبيعات)
5. **Viewer** - مشاهد (قراءة فقط)

### المستخدم الافتراضي:
- **اسم المستخدم:** admin
- **كلمة المرور:** Admin@123
- **الاسم الكامل:** مدير النظام
- **الدور:** Admin

---

## ⚡ الأداء

### التحسينات المطبقة:
- ✅ فهارس على الحقول المستخدمة في البحث
- ✅ فهارس فريدة على الحقول الحرجة
- ✅ إعدادات دقيقة لأنواع البيانات
- ✅ علاقات محسنة مع DeleteBehavior مناسب

---

## 📝 ملاحظات هامة

1. **جميع الأسعار والعملات:** decimal(18,2)
2. **الكميات:** decimal(18,3) للدقة العالية
3. **التواريخ:** UTC دائماً
4. **النصوص العربية:** NVARCHAR لدعم كامل
5. **Soft Delete:** يمكن إضافته حسب الحاجة

---

**تم الإنشاء:** 2026  
**الحالة:** ✅ جاهز للاستخدام

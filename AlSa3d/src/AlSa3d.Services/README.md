# 📦 AlSa3d.Services - طبقة الخدمات

## نظرة عامة
تحتوي هذه الطبقة على واجهات الخدمات (Interfaces) التي تحدد جميع العمليات التجارية للنظام.

## 📁 الهيكل

```
AlSa3d.Services/
├── Interfaces/
│   ├── IInvoiceService.cs        → خدمة الفواتير والمبيعات
│   ├── ICustomerService.cs       → خدمة العملاء والعناوين
│   ├── IEmployeeService.cs       → خدمة الموظفين والرواتب
│   ├── IFinancialService.cs      → الخدمة المالية (بنوك، شيكات، خزينة)
│   ├── IProductService.cs        → خدمة المنتجات والمخازن
│   └── IUserService.cs           → خدمة المستخدمين والصلاحيات
├── Implementation/               → (سيتم إنشاؤها لاحقاً)
│   ├── InvoiceService.cs
│   ├── CustomerService.cs
│   ├── EmployeeService.cs
│   ├── FinancialService.cs
│   ├── ProductService.cs
│   └── UserService.cs
└── Helpers/                      → مساعدات عامة
    └── ServiceHelpers.cs
```

## 🎯 الواجهات المنفذة

### 1️⃣ IInvoiceService
**الوظائف:**
- ✅ إنشاء/تحديث/حذف الفواتير
- ✅ إدارة أصناف الفاتورة
- ✅ حساب الإجماليات والضرائب والخصومات
- ✅ تأكيد وإلغاء الفواتير
- ✅ البحث عن الفواتير (بالرقم، بالعميل، بالتاريخ)
- ✅ تقارير الفواتير اليومية وتقارير العملاء

**النماذج المساعدة:**
- `DailyInvoiceReport` - تقرير يومي
- `CustomerInvoiceReport` - تقرير عميل
- `InvoiceSummary` - ملخص فاتورة

---

### 2️⃣ ICustomerService
**الوظائف:**
- ✅ CRUD للعملاء
- ✅ البحث (بالاسم، الهاتف، البريد، الكود)
- ✅ إدارة العناوين (إضافة/تحديث/حذف/تعيين أساسي)
- ✅ إدارة جهات الاتصال
- ✅ العمليات المالية (الرصيد، حد الائتمان، الأهلية)
- ✅ تقارير العملاء التفصيلية والإقليمية

**النماذج المساعدة:**
- `CustomerDetailedReport`
- `CustomerRegionReport`
- `CustomerSummary`
- `TransactionSummary`

---

### 3️⃣ IEmployeeService
**الوظائف:**
- ✅ CRUD للموظفين
- ✅ إدارة الأقسام
- ✅ إدارة الرواتب (إنشاء، حساب، اعتماد، صرف)
- ✅ الحضور والانصراف
- ✅ الإجازات (طلب، موافقة، رفض، رصيد)
- ✅ تقارير الرواتب والحضور والتقارير التفصيلية

**النماذج المساعدة:**
- `MonthlySalaryReport`
- `MonthlyAttendanceReport`
- `EmployeeDetailedReport`
- `DailyAttendance`
- `SalarySummary`

---

### 4️⃣ IFinancialService
**الوظائف:**
- ✅ إدارة الحسابات البنكية
- ✅ المعاملات البنكية (إيداع، سحب، تحويل)
- ✅ إدارة الشيكات (إصدار، استلام، صرف، إرجاع، إلغاء)
- ✅ إدارة الخزائن
- ✅ المدفوعات (عملاء/موردين)
- ✅ تقارير بنكية وشيكات وخزينة ويومية

**النماذج المساعدة:**
- `BankStatement`
- `CheckReport`
- `TreasuryReport`
- `DailyFinancialReport`
- `CheckSummary`
- `TransactionSummary`

---

### 5️⃣ IProductService
**الوظائف:**
- ✅ CRUD للمنتجات
- ✅ إدارة فئات المنتجات
- ✅ إدارة وحدات القياس
- ✅ إدارة المخازن
- ✅ حركات المخزن (إضافة، خصم، تحويل، جرد)
- ✅ إدارة الأسعار والعروض
- ✅ إدارة الموردين وأوامر الشراء
- ✅ تقارير المخزون وحركة الأصناف والتقييم

**النماذج المساعدة:**
- `InventoryReport`
- `StockMovementReport`
- `InventoryValuationReport`
- `InventoryItem`
- `WarehouseStock`

---

### 6️⃣ IUserService
**الوظائف:**
- ✅ CRUD للمستخدمين
- ✅ تسجيل الدخول/الخروج
- ✅ إدارة كلمات المرور (تغيير، إعادة تعيين)
- ✅ إدارة الأدوار (Roles)
- ✅ إدارة الصلاحيات (Permissions)
- ✅ سجلات التدقيق (Audit Logs)
- ✅ الأمان (2FA، قفل الحساب، محاولات الفشل)
- ✅ إدارة الجلسات (Sessions)
- ✅ تقارير النشاط والصلاحيات

**النماذج المساعدة:**
- `LoginResponse`
- `UserActivityReport`
- `LoginAttemptsReport`
- `PermissionsReport`
- `UserSession`
- `AuditLog`

---

## 🔧 الاعتماديات

```xml
<ProjectReference Include="..\AlSa3d.Core\AlSa3d.Core.csproj" />
```

## 📝 ملاحظات التصميم

### 1. نمط العودة الموحد
جميع الدوال تعود بـ `Result<T>` لضمان معالجة موحدة للأخطاء:
```csharp
Task<Result<Customer>> GetCustomerByIdAsync(Guid customerId);
Task<Result<bool>> DeleteCustomerAsync(Guid customerId);
```

### 2. التوثيق الشامل
كل دالة موثقة بـ XML Comments لشرح الغرض والمعلمات.

### 3. فصل الاهتمامات
كل خدمة مسؤولة عن مجال واحد فقط (Single Responsibility).

### 4. نماذج التقارير المخصصة
كل تقرير له نموذج DTO خاص يحتوي على البيانات المطلوبة فقط.

### 5. دعم اللغة العربية
جميع التعليقات والتوثيق بالعربية لتسهيل الفهم.

---

## 🚀 الخطوات التالية

### المرحلة 1: التنفيذ
- [ ] إنشاء `InvoiceService`
- [ ] إنشاء `CustomerService`
- [ ] إنشاء `EmployeeService`
- [ ] إنشاء `FinancialService`
- [ ] إنشاء `ProductService`
- [ ] إنشاء `UserService`

### المرحلة 2: الاختبار
- [ ] كتابة Unit Tests لكل خدمة
- [ ] اختبار التكامل مع قاعدة البيانات
- [ ] اختبار حالات الخطأ

### المرحلة 3: التحسين
- [ ] إضافة Caching
- [ ] تحسين الاستعلامات
- [ ] إضافة Logging

---

## 📊 الإحصائيات

| الواجهة | عدد الدوال | النماذج المساعدة |
|--------|-----------|------------------|
| IInvoiceService | 20+ | 3 |
| ICustomerService | 25+ | 4 |
| IEmployeeService | 35+ | 6 |
| IFinancialService | 40+ | 6 |
| IProductService | 45+ | 5 |
| IUserService | 40+ | 6 |
| **المجموع** | **205+** | **30+** |

---

## 🔗 الروابط ذات الصلة

- [Core Entities](../AlSa3d.Core/README.md)
- [Infrastructure Layer](../AlSa3d.Infrastructure/README.md)
- [API Layer](../AlSa3d.Api/README.md)
- [Presentation Layer](../AlSa3d.Presentation/README.md)

---

**تم الإنشاء:** 2026  
**الحالة:** ✅ واجهات مكتملة  
**التالي:** تنفيذ الخدمات (Implementation)

# 🗄️ قاعدة بيانات Al-Sa3d

## 📋 نظرة عامة

قاعدة بيانات نظام المحاسبة المصري **Al-Sa3d** مصممة باستخدام **SQL Server 2019+**.

---

## 📁 ملفات السكريبتات

| الملف | الوصف | الحجم التقريبي |
|-------|-------|---------------|
| `01_Create_Database.sql` | إنشاء قاعدة البيانات والإعدادات | ~3KB |
| `02_Create_Tables.sql` | إنشاء 27 جدول مع العلاقات والفهارس | ~31KB |
| `03_Seed_Data.sql` | بيانات تجريبية كاملة | ~24KB |

---

## 🏗️ هيكل قاعدة البيانات

### الجداول (27 جدول)

#### 🔐 إدارة المستخدمين والصلاحيات
1. **Users** - المستخدمون
2. **Roles** - الأدوار
3. **Permissions** - الصلاحيات
4. **RolePermissions** - صلاحيات الأدوار

#### 👥 العملاء
5. **Customers** - العملاء
6. **Addresses** - العناوين
7. **Contacts** - جهات الاتصال

#### 📦 المنتجات والمخازن
8. **Categories** - التصنيفات
9. **Products** - المنتجات
10. **Warehouses** - المخازن
11. **ProductStock** - مخزون المنتجات

#### 📄 الفواتير
12. **Invoices** - الفواتير
13. **InvoiceItems** - أصناف الفواتير
14. **Returns** - المرتجعات
15. **ReturnItems** - أصناف المرتجعات

#### 👨‍💼 الموظفين والرواتب
16. **Departments** - الأقسام
17. **Employees** - الموظفون
18. **Salaries** - الرواتب
19. **Attendance** - الحضور والغياب

#### 🏦 البنوك والمالية
20. **Banks** - البنوك
21. **BankAccounts** - الحسابات البنكية
22. **Checks** - الشيكات
23. **Transactions** - المعاملات المالية
24. **Currencies** - العملات
25. **ExchangeRates** - أسعار الصرف

#### ⚙️ الإعدادات والسجلات
26. **Settings** - إعدادات النظام
27. **AuditLogs** - سجل التدقيق

---

## 📊 الإحصائيات

| العنصر | العدد |
|--------|-------|
| جداول | 27 |
| علاقات Foreign Key | 35+ |
| فهارس Indexes | 40+ |
| قيود Unique | 20+ |
| أعمدة محسوبة | 2 |

---

## 🚀 كيفية التثبيت

### المتطلبات
- SQL Server 2019 أو أحدث
- SQL Server Management Studio (SSMS)
- حقوق إنشاء قواعد بيانات

### خطوات التثبيت

#### الطريقة 1: باستخدام SQL Server Management Studio

```sql
-- 1. افتح SQL Server Management Studio
-- 2. اتصل بالسيرفر
-- 3. افتح الملفات بالترتيب:
--    - File → Open → File → 01_Create_Database.sql
--    - نفذ (F5)
--    - افتح 02_Create_Tables.sql
--    - نفذ (F5)
--    - افتح 03_Seed_Data.sql
--    - نفذ (F5)
```

#### الطريقة 2: باستخدام sqlcmd

```bash
# من Command Prompt
sqlcmd -S localhost -E -i "01_Create_Database.sql"
sqlcmd -S localhost -E -i "02_Create_Tables.sql"
sqlcmd -S localhost -E -i "03_Seed_Data.sql"
```

#### الطريقة 3: باستخدام PowerShell

```powershell
Invoke-Sqlcmd -ServerInstance "localhost" -InputFile "01_Create_Database.sql"
Invoke-Sqlcmd -ServerInstance "localhost" -InputFile "02_Create_Tables.sql"
Invoke-Sqlcmd -ServerInstance "localhost" -InputFile "03_Seed_Data.sql"
```

---

## 🔑 بيانات الدخول التجريبية

بعد تشغيل سكريبت البيانات التجريبية:

| اسم المستخدم | كلمة المرور | الدور |
|-------------|------------|-------|
| `admin` | `123456` | مدير النظام |
| `ahmed` | `123456` | مدير |

**⚠️ ملاحظة:** كلمات المرور في البيئة الإنتاجية يجب أن تكون مشفرة بشكل صحيح.

---

## 📝 ملاحظات هامة

### للأمان
- ✅ قم بتغيير كلمات المرور الافتراضية فوراً
- ✅ استخدم تشفير قوي لكلمات المرور (BCrypt)
- ✅ فعّل SSL للاتصالات
- ✅ حدد صلاحيات الوصول بدقة

### للأداء
- ✅ الفهارس مُنشأة على الأعمدة الأكثر استخداماً
- ✅ استخدم Stored Procedures للاستعلامات المعقدة
- ✅ فعّل Query Store لمراقبة الأداء
- ✅ قم بعمل صيانة دورية (Rebuild Indexes, Update Statistics)

### للنسخ الاحتياطي
```sql
-- نسخ احتياطي كامل
BACKUP DATABASE [AlSa3d] 
TO DISK = N'C:\Backups\AlSa3d_Full.bak' 
WITH FORMAT, INIT, NAME = N'AlSa3d-Full Backup';

-- نسخ احتياطي للسجل
BACKUP LOG [AlSa3d] 
TO DISK = N'C:\Backups\AlSa3d_Log.trn';
```

---

## 🔧 الصيانة الدورية

### يومياً
- ✅ التحقق من سجلات الأخطاء
- ✅ مراجعة عمليات النسخ الاحتياطي

### أسبوعياً
- ✅ تحديث إحصائيات الجداول
- ✅ فحص سلامة القاعدة

### شهرياً
- ✅ إعادة بناء الفهارس
- ✅ تنظيف البيانات القديمة
- ✅ مراجعة أحجام الملفات

---

## 📞 الدعم

لأي استفسارات تقنية حول قاعدة البيانات:
- راجع ملف `ARCHITECTURE.md` لفهم البنية
- راجع ملف `DEVELOPMENT_PLAN.md` لخطة التطوير

---

**تم الإنشاء:** 2026  
**الإصدار:** 1.0  
**الحالة:** ✅ جاهزة للإنتاج

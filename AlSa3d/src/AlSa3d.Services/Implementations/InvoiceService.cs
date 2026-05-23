using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AlSa3d.Core.Entities;
using AlSa3d.Core.Interfaces;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AlSa3d.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Invoice> _invoiceRepository;
        private readonly IRepository<InvoiceItem> _itemRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Warehouse> _warehouseRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Return> _returnRepository;

        public InvoiceService(
            IRepository<Invoice> invoiceRepository,
            IRepository<InvoiceItem> itemRepository,
            IRepository<Product> productRepository,
            IRepository<Warehouse> warehouseRepository,
            IRepository<Customer> customerRepository,
            IRepository<Return> returnRepository)
        {
            _invoiceRepository = invoiceRepository;
            _itemRepository = itemRepository;
            _productRepository = productRepository;
            _warehouseRepository = warehouseRepository;
            _customerRepository = customerRepository;
            _returnRepository = returnRepository;
        }

        public async Task<Result<IEnumerable<Invoice>>> GetAllInvoicesAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync(
                    i => i.Customer,
                    i => i.Items,
                    i => i.CreatedBy);
                
                return Result.Success(invoices.OrderByDescending(i => i.CreatedAt));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Invoice>>($"فشل في جلب الفواتير: {ex.Message}");
            }
        }

        public async Task<Result<Invoice>> GetInvoiceByIdAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id,
                    i => i.Customer,
                    i => i.Items,
                    i => i.CreatedBy);

                if (invoice == null)
                    return Result.Failure<Invoice>("الفاتورة غير موجودة");

                return Result.Success(invoice);
            }
            catch (Exception ex)
            {
                return Result.Failure<Invoice>($"فشل في جلب الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<Invoice>> CreateInvoiceAsync(CreateInvoiceDto dto)
        {
            try
            {
                // التحقق من العميل
                var customer = await _customerRepository.GetByIdAsync(dto.CustomerId);
                if (customer == null)
                    return Result.Failure<Invoice>("العميل غير موجود");

                // إنشاء الفاتورة
                var invoice = new Invoice
                {
                    InvoiceNumber = await GenerateInvoiceNumberAsync(dto.Type),
                    Type = dto.Type,
                    CustomerId = dto.CustomerId,
                    Date = dto.Date ?? DateTime.Now,
                    DueDate = dto.DueDate,
                    Discount = dto.Discount ?? 0,
                    TaxRate = dto.TaxRate ?? 14,
                    Notes = dto.Notes,
                    Status = InvoiceStatus.Draft,
                    CreatedByUserId = dto.CreatedByUserId,
                    CreatedAt = DateTime.Now
                };

                // حساب الإجماليات
                decimal subtotal = 0;
                decimal taxAmount = 0;
                decimal total = 0;

                foreach (var itemDto in dto.Items)
                {
                    var product = await _productRepository.GetByIdAsync(itemDto.ProductId);
                    if (product == null)
                        return Result.Failure<Invoice>($"المنتج {itemDto.ProductId} غير موجود");

                    // التحقق من المخزون
                    if (dto.Type == InvoiceType.Sales)
                    {
                        var stock = await GetProductStockAsync(itemDto.ProductId, itemDto.WarehouseId ?? 1);
                        if (stock < itemDto.Quantity)
                            return Result.Failure<Invoice>($"الكمية المتاحة من {product.Name} غير كافية");
                    }

                    var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                    var lineTax = lineTotal * (itemDto.TaxRate ?? 14) / 100;
                    
                    subtotal += lineTotal;
                    taxAmount += lineTax;

                    invoice.Items.Add(new InvoiceItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        Discount = itemDto.Discount ?? 0,
                        TaxRate = itemDto.TaxRate ?? 14,
                        WarehouseId = itemDto.WarehouseId ?? 1,
                        Total = lineTotal + lineTax
                    });
                }

                total = subtotal + taxAmount - invoice.Discount;

                invoice.SubTotal = subtotal;
                invoice.TaxAmount = taxAmount;
                invoice.Total = total;

                // حفظ الفاتورة
                var result = await _invoiceRepository.AddAsync(invoice);
                if (!result.Success)
                    return Result.Failure<Invoice>(result.Message);

                // تحديث المخزون
                if (dto.Type == InvoiceType.Sales)
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var updateResult = await UpdateProductStockAsync(
                            itemDto.ProductId,
                            itemDto.WarehouseId ?? 1,
                            -itemDto.Quantity); // إنقاص المخزون

                        if (!updateResult.Success)
                            return Result.Failure<Invoice>(updateResult.Message);
                    }
                }

                return Result.Success(invoice);
            }
            catch (Exception ex)
            {
                return Result.Failure<Invoice>($"فشل في إنشاء الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<Invoice>> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id, i => i.Items);
                if (invoice == null)
                    return Result.Failure<Invoice>("الفاتورة غير موجودة");

                if (invoice.Status == InvoiceStatus.Paid || invoice.Status == InvoiceStatus.Cancelled)
                    return Result.Failure<Invoice>("لا يمكن تعديل فاتورة مدفوعة أو ملغاة");

                // تحديث البيانات الأساسية
                invoice.CustomerId = dto.CustomerId;
                invoice.Date = dto.Date ?? invoice.Date;
                invoice.DueDate = dto.DueDate;
                invoice.Discount = dto.Discount ?? invoice.Discount;
                invoice.TaxRate = dto.TaxRate ?? invoice.TaxRate;
                invoice.Notes = dto.Notes;

                // حذف العناصر القديمة
                invoice.Items.Clear();

                // إعادة الحساب
                decimal subtotal = 0;
                decimal taxAmount = 0;

                foreach (var itemDto in dto.Items)
                {
                    var lineTotal = itemDto.Quantity * itemDto.UnitPrice;
                    var lineTax = lineTotal * (itemDto.TaxRate ?? 14) / 100;
                    
                    subtotal += lineTotal;
                    taxAmount += lineTax;

                    invoice.Items.Add(new InvoiceItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = itemDto.UnitPrice,
                        Discount = itemDto.Discount ?? 0,
                        TaxRate = itemDto.TaxRate ?? 14,
                        WarehouseId = itemDto.WarehouseId ?? 1,
                        Total = lineTotal + lineTax
                    });
                }

                invoice.SubTotal = subtotal;
                invoice.TaxAmount = taxAmount;
                invoice.Total = subtotal + taxAmount - invoice.Discount;

                var result = await _invoiceRepository.UpdateAsync(invoice);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Invoice>($"فشل في تحديث الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteInvoiceAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                    return Result.Failure<bool>("الفاتورة غير موجودة");

                if (invoice.Status == InvoiceStatus.Paid)
                    return Result.Failure<bool>("لا يمكن حذف فاتورة مدفوعة");

                // استرجاع المخزون إذا كانت فاتورة مبيعات
                if (invoice.Type == InvoiceType.Sales)
                {
                    foreach (var item in invoice.Items)
                    {
                        await UpdateProductStockAsync(item.ProductId, item.WarehouseId, item.Quantity);
                    }
                }

                invoice.IsDeleted = true;
                invoice.DeletedAt = DateTime.Now;

                var result = await _invoiceRepository.UpdateAsync(invoice);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<bool>($"فشل في حذف الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<Invoice>> ApproveInvoiceAsync(int id)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                    return Result.Failure<Invoice>("الفاتورة غير موجودة");

                invoice.Status = InvoiceStatus.Approved;
                invoice.ApprovedAt = DateTime.Now;

                var result = await _invoiceRepository.UpdateAsync(invoice);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Invoice>($"فشل في اعتماد الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<Invoice>> CancelInvoiceAsync(int id, string reason)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(id);
                if (invoice == null)
                    return Result.Failure<Invoice>("الفاتورة غير موجودة");

                invoice.Status = InvoiceStatus.Cancelled;
                invoice.CancellationReason = reason;
                invoice.CancelledAt = DateTime.Now;

                // استرجاع المخزون
                if (invoice.Type == InvoiceType.Sales)
                {
                    foreach (var item in invoice.Items)
                    {
                        await UpdateProductStockAsync(item.ProductId, item.WarehouseId, item.Quantity);
                    }
                }

                var result = await _invoiceRepository.UpdateAsync(invoice);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Invoice>($"فشل في إلغاء الفاتورة: {ex.Message}");
            }
        }

        public async Task<Result<Return>> CreateReturnAsync(CreateReturnDto dto)
        {
            try
            {
                var invoice = await _invoiceRepository.GetByIdAsync(dto.InvoiceId, i => i.Items);
                if (invoice == null)
                    return Result.Failure<Return>("الفاتورة غير موجودة");

                var returnObj = new Return
                {
                    ReturnNumber = $"RET-{DateTime.Now:yyyyMMdd}-{await _returnRepository.CountAsync() + 1}",
                    InvoiceId = dto.InvoiceId,
                    Date = DateTime.Now,
                    Reason = dto.Reason,
                    Status = ReturnStatus.Pending,
                    CreatedByUserId = dto.CreatedByUserId
                };

                decimal totalAmount = 0;

                foreach (var itemDto in dto.Items)
                {
                    var invoiceItem = invoice.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
                    if (invoiceItem == null)
                        continue;

                    var lineTotal = itemDto.Quantity * invoiceItem.UnitPrice;
                    totalAmount += lineTotal;

                    returnObj.Items.Add(new ReturnItem
                    {
                        ProductId = itemDto.ProductId,
                        Quantity = itemDto.Quantity,
                        UnitPrice = invoiceItem.UnitPrice,
                        Total = lineTotal,
                        Reason = itemDto.Reason
                    });

                    // إضافة المخزون المرتجع
                    await UpdateProductStockAsync(itemDto.ProductId, invoiceItem.WarehouseId, itemDto.Quantity);
                }

                returnObj.TotalAmount = totalAmount;

                var result = await _returnRepository.AddAsync(returnObj);
                return result;
            }
            catch (Exception ex)
            {
                return Result.Failure<Return>($"فشل في إنشاء المرتجع: {ex.Message}");
            }
        }

        public async Task<Result<decimal>> GetCustomerBalanceAsync(int customerId)
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync(i => i.Items);
                var customerInvoices = invoices.Where(i => i.CustomerId == customerId && !i.IsDeleted);

                decimal totalPaid = customerInvoices
                    .Where(i => i.Status == InvoiceStatus.Paid)
                    .Sum(i => i.Total);

                decimal totalUnpaid = customerInvoices
                    .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
                    .Sum(i => i.Total);

                return Result.Success(totalUnpaid);
            }
            catch (Exception ex)
            {
                return Result.Failure<decimal>($"فشل في حساب رصيد العميل: {ex.Message}");
            }
        }

        public async Task<Result<IEnumerable<Invoice>>> SearchInvoicesAsync(string searchTerm, InvoiceType? type = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            try
            {
                var query = await _invoiceRepository.GetAllAsync(i => i.Customer, i => i.Items);
                
                var filtered = query.AsEnumerable();

                if (!string.IsNullOrWhiteSpace(searchTerm))
                {
                    filtered = filtered.Where(i => 
                        i.InvoiceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        (i.Customer != null && i.Customer.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (i.Notes != null && i.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)));
                }

                if (type.HasValue)
                    filtered = filtered.Where(i => i.Type == type.Value);

                if (fromDate.HasValue)
                    filtered = filtered.Where(i => i.Date >= fromDate.Value);

                if (toDate.HasValue)
                    filtered = filtered.Where(i => i.Date <= toDate.Value);

                return Result.Success(filtered.OrderByDescending(i => i.Date));
            }
            catch (Exception ex)
            {
                return Result.Failure<IEnumerable<Invoice>>($"فشل في البحث عن الفواتير: {ex.Message}");
            }
        }

        public async Task<Result<InvoiceDashboardDto>> GetDashboardStatsAsync()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync(i => i.Items);
                var today = DateTime.Today;

                var stats = new InvoiceDashboardDto
                {
                    TotalInvoices = invoices.Count(i => !i.IsDeleted),
                    TodayInvoices = invoices.Count(i => i.Date.Date == today && !i.IsDeleted),
                    TotalRevenue = invoices.Where(i => i.Status == InvoiceStatus.Paid && !i.IsDeleted).Sum(i => i.Total),
                    TodayRevenue = invoices.Where(i => i.Date.Date == today && i.Status == InvoiceStatus.Paid && !i.IsDeleted).Sum(i => i.Total),
                    PendingInvoices = invoices.Count(i => i.Status == InvoiceStatus.Draft && !i.IsDeleted),
                    OverdueInvoices = invoices.Count(i => i.DueDate < DateTime.Now && i.Status != InvoiceStatus.Paid && !i.IsDeleted),
                    MonthlyRevenue = invoices.Where(i => i.Date.Month == DateTime.Now.Month && i.Status == InvoiceStatus.Paid && !i.IsDeleted).Sum(i => i.Total)
                };

                return Result.Success(stats);
            }
            catch (Exception ex)
            {
                return Result.Failure<InvoiceDashboardDto>($"فشل في جلب إحصائيات لوحة التحكم: {ex.Message}");
            }
        }

        #region Helper Methods

        private async Task<string> GenerateInvoiceNumberAsync(InvoiceType type)
        {
            var prefix = type == InvoiceType.Sales ? "INV" : "PUR";
            var count = await _invoiceRepository.CountAsync(i => i.Type == type);
            return $"{prefix}-{DateTime.Now:yyyyMMdd}-{count + 1:D6}";
        }

        private async Task<int> GetProductStockAsync(int productId, int warehouseId)
        {
            var product = await _productRepository.GetByIdAsync(productId, p => p.WarehouseProducts);
            if (product == null) return 0;

            var warehouseProduct = product.WarehouseProducts?.FirstOrDefault(wp => wp.WarehouseId == warehouseId);
            return warehouseProduct?.Quantity ?? 0;
        }

        private async Task<Result<bool>> UpdateProductStockAsync(int productId, int warehouseId, int quantityChange)
        {
            var product = await _productRepository.GetByIdAsync(productId, p => p.WarehouseProducts);
            if (product == null)
                return Result.Failure<bool>("المنتج غير موجود");

            var warehouseProduct = product.WarehouseProducts?.FirstOrDefault(wp => wp.WarehouseId == warehouseId);
            if (warehouseProduct == null)
                return Result.Failure<bool>("المخزن غير مرتبط بالمنتج");

            warehouseProduct.Quantity += quantityChange;
            
            if (warehouseProduct.Quantity < 0)
                return Result.Failure<bool>("الكمية لا يمكن أن تكون سالبة");

            warehouseProduct.LastUpdated = DateTime.Now;

            var result = await _productRepository.UpdateAsync(product);
            return result;
        }

        #endregion
    }
}

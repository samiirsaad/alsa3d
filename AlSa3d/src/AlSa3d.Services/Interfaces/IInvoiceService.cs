using AlSa3d.Core;
using AlSa3d.Core.DTOs;
using AlSa3d.Core.Entities;

namespace AlSa3d.Services.Interfaces;

public interface IInvoiceService
{
    Task<Result<IEnumerable<Invoice>>> GetAllInvoicesAsync();
    Task<Result<IEnumerable<InvoiceDto>>> GetRecentInvoicesAsync(int count);
    Task<Result<Invoice>> GetInvoiceByIdAsync(int id);
    Task<Result<Invoice>> CreateInvoiceAsync(CreateInvoiceDto dto);
    Task<Result<Invoice>> UpdateInvoiceAsync(int id, UpdateInvoiceDto dto);
    Task<Result<bool>> DeleteInvoiceAsync(int id);
    Task<Result<Invoice>> ApproveInvoiceAsync(int id);
    Task<Result<Invoice>> CancelInvoiceAsync(int id, string reason);
    Task<Result<Return>> CreateReturnAsync(CreateReturnDto dto);
    Task<Result<decimal>> GetCustomerBalanceAsync(int customerId);
    Task<Result<IEnumerable<Invoice>>> SearchInvoicesAsync(string searchTerm, InvoiceType? type = null, DateTime? fromDate = null, DateTime? toDate = null);
    Task<Result<InvoiceDashboardDto>> GetDashboardStatsAsync();
}

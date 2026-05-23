namespace AlSa3d.Core.DTOs;

public class CreateInvoiceDto
{
    public int CustomerId { get; set; }
    public string Type { get; set; } = "Sales";
    public DateTime? Date { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; } = 14;
    public string? Notes { get; set; }
    public int? CreatedByUserId { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; } = 14;
    public int? WarehouseId { get; set; }
}

public class UpdateInvoiceDto
{
    public int CustomerId { get; set; }
    public DateTime? Date { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal? Discount { get; set; }
    public decimal? TaxRate { get; set; }
    public string? Notes { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateReturnDto
{
    public int InvoiceId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public List<ReturnItemDto> Items { get; set; } = new();
}

public class ReturnItemDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string? Reason { get; set; }
}

public class InvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public string StatusName { get; set; } = string.Empty;
}

public class InvoiceDashboardDto
{
    public int TotalInvoices { get; set; }
    public int TodayInvoices { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TodayRevenue { get; set; }
    public int PendingInvoices { get; set; }
    public int OverdueInvoices { get; set; }
    public decimal MonthlyRevenue { get; set; }
}

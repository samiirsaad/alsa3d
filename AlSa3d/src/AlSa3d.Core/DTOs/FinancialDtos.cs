namespace AlSa3d.Core.DTOs;

public class CreateBankDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
}

public class UpdateBankDto
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateAccountDto
{
    public int BankId { get; set; }
    public string AccountNumber { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Checking";
    public int? CurrencyId { get; set; }
    public decimal? InitialBalance { get; set; }
}

public class CreateTransactionDto
{
    public int AccountId { get; set; }
    public string Type { get; set; } = "Deposit";
    public decimal Amount { get; set; }
    public int? CurrencyId { get; set; }
    public string? Description { get; set; }
    public string? ReferenceType { get; set; }
    public int? ReferenceId { get; set; }
    public DateTime? Date { get; set; }
}

public class CreateCheckDto
{
    public int AccountId { get; set; }
    public string CheckNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? CurrencyId { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? PayeeName { get; set; }
    public string? Notes { get; set; }
}

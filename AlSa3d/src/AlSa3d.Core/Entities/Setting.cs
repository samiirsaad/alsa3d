using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Setting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Type { get; set; } = "String";
}

public class AuditLog : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public int RecordId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? OldValues { get; set; }
    public string? NewValues { get; set; }
    public int? UserId { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public DateTime? Timestamp { get; set; }
    public string? IpAddress { get; set; }
}

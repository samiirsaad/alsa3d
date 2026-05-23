using AlSa3d.Core.Common;

namespace AlSa3d.Core.Entities;

public class Setting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public SettingType Type { get; set; } = SettingType.String;
}

public class AuditLog : BaseEntity
{
    public string TableName { get; set; } = string.Empty;
    public int RecordId { get; set; }
    public AuditAction Action { get; set; }
    public string? OldValues { get; set; } // JSON
    public string? NewValues { get; set; } // JSON
    public string? UserId { get; set; }
    public DateTime ActionDate { get; set; } = DateTime.Now;
    public string? IPAddress { get; set; }
}

public enum SettingType
{
    String = 0,
    Number = 1,
    Boolean = 2,
    DateTime = 3
}

public enum AuditAction
{
    Create = 0,
    Update = 1,
    Delete = 2
}

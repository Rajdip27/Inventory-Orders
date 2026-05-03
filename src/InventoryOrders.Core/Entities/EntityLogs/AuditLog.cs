using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using InventoryOrders.Core.Entities.BaseEntities;

namespace InventoryOrders.Core.Entities.EntityLogs;

public class AuditLog: AuditableEntity
{
    public long UserId { get; set; }
    public string Type { get; set; }
    public string TableName { get; set; }
    public DateTimeOffset DateTime { get; set; }
    public string OldValues { get; set; }
    public string NewValues { get; set; }
    public string AffectedColumns { get; set; }
    public string PrimaryKey { get; set; }
}
public class AuditEntry
{
    public AuditEntry(EntityEntry entry)
    {
        Entry = entry;
    }

    public EntityEntry Entry { get; }

    public long UserId { get; set; }
    public string TableName { get; set; }
    public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
    public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
    public AuditType AuditType { get; set; }
    public List<string> ChangedColumns { get; } = new List<string>();

    public AuditLog ToAuditLog()
    {
        var audit = new AuditLog
        {
            UserId = UserId,
            Type = AuditType.ToString(),
            TableName = TableName,
            DateTime = DateTime.Now,
            PrimaryKey = JsonConvert.SerializeObject(KeyValues),
            OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues),
            NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues),
            AffectedColumns = ChangedColumns.Count == 0 ? null : JsonConvert.SerializeObject(ChangedColumns)
        };
        switch (AuditType)
        {
            case (AuditType)1:
                audit.CreatedDate = DateTime.Now;
                audit.CreatedBy = UserId;
                break;
            case (AuditType)2:
                audit.ModifiedDate = DateTime.Now;
                audit.ModifiedBy = UserId;
                break;
            default:
                audit.ModifiedDate = DateTime.Now;
                audit.ModifiedBy = UserId;
                audit.IsDelete = true;
                break;
        }
        return audit;
    }
}

public enum AuditType
{
    None = 0,
    Create = 1,
    Update = 2,
    Delete = 3
}

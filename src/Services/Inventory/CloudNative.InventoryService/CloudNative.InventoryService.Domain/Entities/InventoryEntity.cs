using CloudNative.Core.Entities;

namespace CloudNative.InventoryService.Domain.Entities;

public class InventoryItem : AuditableEntity
{
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool   IsActive    { get; set; } = true;
}

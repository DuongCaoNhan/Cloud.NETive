using CloudNative.Core.Entities;

namespace CloudNative.HRService.Domain.Entities;

public class HRItem : AuditableEntity
{
    public string Name        { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool   IsActive    { get; set; } = true;
}

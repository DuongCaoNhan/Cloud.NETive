namespace CloudNative.Core.Entities;

/// <summary>Adds created / modified audit stamps to any entity.</summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAt  { get; set; } = DateTime.UtcNow;
    public string?  CreatedBy  { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string?  UpdatedBy  { get; set; }
}

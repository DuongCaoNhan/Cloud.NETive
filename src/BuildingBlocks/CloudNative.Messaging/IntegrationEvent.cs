namespace CloudNative.Messaging;

public abstract record IntegrationEvent(
    Guid   Id          = default,
    DateTime CreatedAt = default)
{
    public Guid     Id        { get; } = Id == default ? Guid.NewGuid() : Id;
    public DateTime CreatedAt { get; } = CreatedAt == default ? DateTime.UtcNow : CreatedAt;
}

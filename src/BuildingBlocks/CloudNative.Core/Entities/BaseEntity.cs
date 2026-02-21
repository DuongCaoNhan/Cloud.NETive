namespace CloudNative.Core.Entities;

/// <summary>Root base entity with a strongly-typed primary key.</summary>
public abstract class BaseEntity<TKey>
{
    public TKey Id { get; protected set; } = default!;
}

/// <summary>Convenience alias using int key.</summary>
public abstract class BaseEntity : BaseEntity<int> { }

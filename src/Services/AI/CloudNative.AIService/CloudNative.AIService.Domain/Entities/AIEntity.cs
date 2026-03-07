using CloudNative.Core.Entities;

namespace CloudNative.AIService.Domain.Entities;

/// <summary>
/// Pragmatic DDD: Public setters for simple properties are acceptable.
/// Key state transitions are still protected via business methods.
/// No Domain Events, no Value Objects — focus on practicality.
/// </summary>
public class AIItem : AuditableEntity
{
    // Pragmatic: public setters for simple scalar properties are fine
    public string   Name        { get; set; } = string.Empty;
    public string   Description { get; set; } = string.Empty;
    public string   Category    { get; set; } = "General";

    // Protected state — mutated only via business methods
    public bool     IsActive    { get; private set; } = true;

    /// <summary>
    /// 384-dimension vector for semantic search.
    /// Stored as SQL Server 2025 native vector(384) column.
    /// CLR type: float[] with JSON value converter fallback.
    /// </summary>
    public float[]? Embedding   { get; private set; }

    // EF Core materialisation constructor
    private AIItem() { }

    // Pragmatic factory — lighter than Pure DDD, no invariant exceptions
    public static AIItem Create(string name, string description, string category = "General") =>
        new() { Name = name, Description = description, Category = category };

    // ── Business methods (Rich Model preserved even in Pragmatic DDD) ─────
    public void UpdateEmbedding(float[] embedding)
    {
        Embedding = embedding;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive  = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive  = false;
        UpdatedAt = DateTime.UtcNow;
    }
}


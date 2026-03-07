using CloudNative.AIService.Domain.Entities;

namespace CloudNative.AIService.Application.Model;

/// <summary>
/// Pragmatic DDD: DTOs, Entities, and Service Interfaces co-exist in the
/// same Model namespace — no strict separation layers required.
/// </summary>
public record AIItemDto(
    int      Id,
    string   Name,
    string   Description,
    string   Category,
    bool     IsActive,
    bool     HasEmbedding,
    DateTime CreatedAt,
    DateTime? UpdatedAt);

public record AIItemSearchResultDto(
    int     Id,
    string  Name,
    string  Description,
    string  Category,
    float   SimilarityScore);

/// <summary>Input DTOs — pragmatic: simple records, no FluentValidation pipeline needed</summary>
public record CreateAIItemRequest(
    string Name,
    string Description,
    string Category      = "General",
    bool   AutoEmbed     = true);

public record UpdateAIItemRequest(
    string Name,
    string Description,
    string Category);

public record SemanticSearchRequest(
    string Query,
    int    TopK        = 5,
    string? Category   = null);

/// <summary>Mapping helpers — pragmatic: extension methods, no AutoMapper</summary>
public static class AIItemMappings
{
    public static AIItemDto ToDto(this AIItem item) => new(
        item.Id,
        item.Name,
        item.Description,
        item.Category,
        item.IsActive,
        item.Embedding is not null,
        item.CreatedAt,
        item.UpdatedAt);

    public static AIItemSearchResultDto ToSearchDto(this AIItem item, float score) => new(
        item.Id, item.Name, item.Description, item.Category, score);
}

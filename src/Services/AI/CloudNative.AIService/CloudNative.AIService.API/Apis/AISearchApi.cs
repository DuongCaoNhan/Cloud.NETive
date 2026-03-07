namespace CloudNative.AIService.API.Apis;

/// <summary>
/// Semantic search endpoint using vector similarity.
/// Pragmatic: loads candidates from DB, computes cosine similarity in-memory.
/// Production upgrade path: use SQL Server 2025 native VECTOR_DISTANCE() function.
/// </summary>
public static class AISearchApi
{
    public static RouteGroupBuilder MapAISearchApi(this RouteGroupBuilder group)
    {
        group.MapPost("/semantic", SemanticSearch)
             .WithSummary("Semantic similarity search using vector embeddings");
        return group;
    }

    static async Task<IResult> SemanticSearch(
        SemanticSearchRequest request,
        AIDbContext db,
        IAIEmbeddingService embedding,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return Results.BadRequest(ApiResponse<object>.Fail("Query is required."));

        // Generate query embedding
        var queryVector = await embedding.GenerateEmbeddingAsync(request.Query, ct);

        // Load active items that have an embedding
        // Production upgrade: replace with SQL Server 2025 native vector search:
        //   SELECT TOP(@topK) *, VECTOR_DISTANCE('cosine', embedding, @queryVector) AS score
        //   FROM ai.Items WHERE IsActive = 1 ORDER BY score ASC
        var candidates = await db.Items
            .Where(x => x.IsActive)
            .ToListAsync(ct);

        if (request.Category is not null)
            candidates = candidates.Where(x => x.Category == request.Category).ToList();

        var results = candidates
            .Where(x => x.Embedding is not null)
            .Select(x => (item: x, score: AIEmbeddingService.CosineSimilarity(queryVector, x.Embedding!)))
            .OrderByDescending(x => x.score)
            .Take(request.TopK)
            .Select(x => x.item.ToSearchDto(x.score))
            .ToList();

        return Results.Ok(ApiResponse<IReadOnlyList<AIItemSearchResultDto>>.Ok(results));
    }
}

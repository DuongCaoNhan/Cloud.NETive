namespace CloudNative.AIService.API.Apis;

/// <summary>
/// Pragmatic DDD: Minimal API endpoints — inject DbContext and services directly.
/// No MediatR, no Commands/Queries. CRUD logic lives here, close to the data.
/// </summary>
public static class AIItemsApi
{
    public static RouteGroupBuilder MapAIItemsApi(this RouteGroupBuilder group)
    {
        group.MapGet("/",         GetAll);
        group.MapGet("/{id:int}", GetById);
        group.MapPost("/",        Create);
        group.MapPut("/{id:int}", Update);
        group.MapDelete("/{id:int}", Delete);
        return group;
    }

    // ── Handlers — direct DbContext injection (Pragmatic DDD) ────────────────

    static async Task<IResult> GetAll(AIDbContext db, CancellationToken ct)
    {
        var items = await db.Items.ToListAsync(ct);
        return Results.Ok(ApiResponse<IReadOnlyList<AIItemDto>>.Ok(
            items.Select(x => x.ToDto()).ToList()));
    }

    static async Task<IResult> GetById(int id, AIDbContext db, CancellationToken ct)
    {
        var item = await db.Items.FindAsync([id], ct);
        return item is null
            ? Results.NotFound(ApiResponse<AIItemDto>.Fail($"Item {id} not found."))
            : Results.Ok(ApiResponse<AIItemDto>.Ok(item.ToDto()));
    }

    static async Task<IResult> Create(
        CreateAIItemRequest request,
        AIDbContext db,
        IAIEmbeddingService embedding,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest(ApiResponse<AIItemDto>.Fail("Name is required."));

        var item = AIItem.Create(request.Name, request.Description, request.Category);

        if (request.AutoEmbed)
        {
            var text   = $"{request.Name} {request.Description}";
            var vector = await embedding.GenerateEmbeddingAsync(text, ct);
            item.UpdateEmbedding(vector);
        }

        db.Items.Add(item);
        await db.SaveChangesAsync(ct);

        return Results.Created($"/api/v1/ai/items/{item.Id}",
            ApiResponse<AIItemDto>.Ok(item.ToDto()));
    }

    static async Task<IResult> Update(
        int id,
        UpdateAIItemRequest request,
        AIDbContext db,
        IAIEmbeddingService embedding,
        CancellationToken ct)
    {
        var item = await db.Items.FindAsync([id], ct);
        if (item is null)
            return Results.NotFound(ApiResponse<AIItemDto>.Fail($"Item {id} not found."));

        // Pragmatic: direct property assignment for simple fields
        item.Name        = request.Name;
        item.Description = request.Description;
        item.Category    = request.Category;

        // Re-generate embedding when content changes
        var text   = $"{request.Name} {request.Description}";
        var vector = await embedding.GenerateEmbeddingAsync(text, ct);
        item.UpdateEmbedding(vector);

        await db.SaveChangesAsync(ct);
        return Results.Ok(ApiResponse<AIItemDto>.Ok(item.ToDto()));
    }

    static async Task<IResult> Delete(int id, AIDbContext db, CancellationToken ct)
    {
        var item = await db.Items.FindAsync([id], ct);
        if (item is null)
            return Results.NotFound(ApiResponse<AIItemDto>.Fail($"Item {id} not found."));

        db.Items.Remove(item);
        await db.SaveChangesAsync(ct);
        return Results.NoContent();
    }
}

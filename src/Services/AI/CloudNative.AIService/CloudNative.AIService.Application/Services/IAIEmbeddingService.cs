namespace CloudNative.AIService.Application.Services;

/// <summary>
/// Embedding service interface — lives in Application/Model (Pragmatic DDD).
/// Implemented in Infrastructure where external AI provider SDKs are referenced.
/// </summary>
public interface IAIEmbeddingService
{
    /// <summary>
    /// Generate a 384-dimension embedding vector for the given text.
    /// Replace mock implementation with Azure OpenAI / Semantic Kernel in production.
    /// </summary>
    Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default);
}

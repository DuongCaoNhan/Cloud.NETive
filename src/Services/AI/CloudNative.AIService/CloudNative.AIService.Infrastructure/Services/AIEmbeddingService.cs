using CloudNative.AIService.Application.Services;

namespace CloudNative.AIService.Infrastructure.Services;

/// <summary>
/// Pragmatic embedding service — deterministic mock implementation.
/// Replace with real provider for production:
///   - Azure OpenAI: text-embedding-3-small (1536-dim) or text-embedding-ada-002 (1536-dim)
///   - Ollama local:  nomic-embed-text (768-dim)
///   - Semantic Kernel: kernel.GetRequiredService&lt;ITextEmbeddingGenerationService&gt;()
/// </summary>
public sealed class AIEmbeddingService : IAIEmbeddingService
{
    private const int Dimensions = 384;

    public Task<float[]> GenerateEmbeddingAsync(string text, CancellationToken ct = default)
    {
        // Deterministic mock: same text always produces same vector (useful for testing)
        // TODO: Replace with real AI embedding call below:
        // var client = new AzureOpenAIClient(...);
        // var result = await client.GetEmbeddingAsync("text-embedding-3-small", text, ct);
        // return Task.FromResult(result.Value.ToFloats().ToArray());

        var seed   = text.GetHashCode();
        var rng    = new Random(seed);
        var vector = new float[Dimensions];

        for (int i = 0; i < Dimensions; i++)
            vector[i] = (float)(rng.NextDouble() * 2 - 1);

        // Normalize to unit vector (required for cosine similarity)
        var norm = MathF.Sqrt(vector.Sum(v => v * v));
        for (int i = 0; i < Dimensions; i++)
            vector[i] /= norm;

        return Task.FromResult(vector);
    }

    /// <summary>Cosine similarity: 1.0 = identical, 0.0 = orthogonal, -1.0 = opposite.</summary>
    public static float CosineSimilarity(float[] a, float[] b)
    {
        float dot = 0, normA = 0, normB = 0;
        int len = Math.Min(a.Length, b.Length);
        for (int i = 0; i < len; i++)
        {
            dot   += a[i] * b[i];
            normA += a[i] * a[i];
            normB += b[i] * b[i];
        }
        return dot / (MathF.Sqrt(normA) * MathF.Sqrt(normB) + 1e-10f);
    }
}

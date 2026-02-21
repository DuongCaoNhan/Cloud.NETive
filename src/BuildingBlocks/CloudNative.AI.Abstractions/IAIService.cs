namespace CloudNative.AI.Abstractions;

public interface IAIService
{
    Task<string> CompleteAsync(string prompt, CancellationToken ct = default);
    Task<IAsyncEnumerable<string>> StreamAsync(string prompt, CancellationToken ct = default);
}

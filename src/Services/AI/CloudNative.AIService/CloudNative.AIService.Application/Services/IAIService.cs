using CloudNative.AIService.Domain.Entities;

namespace CloudNative.AIService.Application.Services;

public interface IAIService
{
    Task<IReadOnlyList<AIItem>> GetAllAsync(CancellationToken ct = default);
    Task<AIItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AIItem> CreateAsync(AIItem item, CancellationToken ct = default);
    Task UpdateAsync(AIItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

using CloudNative.HRService.Domain.Entities;

namespace CloudNative.HRService.Application.Services;

public interface IHRService
{
    Task<IReadOnlyList<HRItem>> GetAllAsync(CancellationToken ct = default);
    Task<HRItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<HRItem> CreateAsync(HRItem item, CancellationToken ct = default);
    Task UpdateAsync(HRItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

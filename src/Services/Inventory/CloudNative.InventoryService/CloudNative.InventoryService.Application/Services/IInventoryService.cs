using CloudNative.InventoryService.Domain.Entities;

namespace CloudNative.InventoryService.Application.Services;

public interface IInventoryService
{
    Task<IReadOnlyList<InventoryItem>> GetAllAsync(CancellationToken ct = default);
    Task<InventoryItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<InventoryItem> CreateAsync(InventoryItem item, CancellationToken ct = default);
    Task UpdateAsync(InventoryItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

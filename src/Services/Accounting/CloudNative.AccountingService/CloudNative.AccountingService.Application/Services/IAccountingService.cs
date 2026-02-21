using CloudNative.AccountingService.Domain.Entities;

namespace CloudNative.AccountingService.Application.Services;

public interface IAccountingService
{
    Task<IReadOnlyList<AccountingItem>> GetAllAsync(CancellationToken ct = default);
    Task<AccountingItem?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<AccountingItem> CreateAsync(AccountingItem item, CancellationToken ct = default);
    Task UpdateAsync(AccountingItem item, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

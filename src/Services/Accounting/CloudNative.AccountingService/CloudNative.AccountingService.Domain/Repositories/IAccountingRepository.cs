using CloudNative.AccountingService.Domain.Entities;
using CloudNative.Core.Interfaces;

namespace CloudNative.AccountingService.Domain.Repositories;

/// <summary>Domain-specific repository interface — defined in Domain, implemented in Infrastructure.</summary>
public interface IAccountingRepository : IRepository<AccountingItem>
{
    Task<IReadOnlyList<AccountingItem>> GetActiveItemsAsync(CancellationToken ct = default);
}

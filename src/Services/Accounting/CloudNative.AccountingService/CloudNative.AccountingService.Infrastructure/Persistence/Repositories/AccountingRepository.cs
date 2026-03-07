using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using CloudNative.AccountingService.Domain.Entities;
using CloudNative.AccountingService.Domain.Repositories;
using CloudNative.AccountingService.Infrastructure.Persistence;

namespace CloudNative.AccountingService.Infrastructure.Persistence.Repositories;

/// <summary>
/// Concrete repository — lives in Infrastructure.
/// Application layer only depends on IAccountingRepository (Domain interface).
/// </summary>
public sealed class AccountingRepository(AccountingDbContext context) : IAccountingRepository
{
    public async Task<AccountingItem?> GetByIdAsync(int id, CancellationToken ct = default)
        => await context.Items.FindAsync([id], ct);

    public async Task<IReadOnlyList<AccountingItem>> ListAllAsync(CancellationToken ct = default)
        => await context.Items.ToListAsync(ct);

    public async Task<IReadOnlyList<AccountingItem>> ListAsync(
        Expression<Func<AccountingItem, bool>> predicate, CancellationToken ct = default)
        => await context.Items.Where(predicate).ToListAsync(ct);

    public async Task<AccountingItem> AddAsync(AccountingItem entity, CancellationToken ct = default)
    {
        context.Items.Add(entity);
        await context.SaveChangesAsync(ct);
        return entity;
    }

    public async Task UpdateAsync(AccountingItem entity, CancellationToken ct = default)
    {
        context.Items.Update(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(AccountingItem entity, CancellationToken ct = default)
    {
        context.Items.Remove(entity);
        await context.SaveChangesAsync(ct);
    }

    public async Task<IReadOnlyList<AccountingItem>> GetActiveItemsAsync(CancellationToken ct = default)
        => await context.Items
            .Where(x => x.Status == AccountItemStatus.Active)
            .ToListAsync(ct);
}

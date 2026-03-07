using Microsoft.EntityFrameworkCore;
using CloudNative.AccountingService.Domain.Entities;

namespace CloudNative.AccountingService.Infrastructure.Persistence;

public class AccountingDbContext(DbContextOptions<AccountingDbContext> options) : DbContext(options)
{
    public DbSet<AccountingItem> Items => Set<AccountingItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("accounting");
        // Picks up all IEntityTypeConfiguration<T> classes in this assembly automatically
        builder.ApplyConfigurationsFromAssembly(typeof(AccountingDbContext).Assembly);
    }
}

using Microsoft.EntityFrameworkCore;
using CloudNative.AccountingService.Domain.Entities;

namespace CloudNative.AccountingService.Infrastructure.Persistence;

public class AccountingDbContext(DbContextOptions<AccountingDbContext> options) : DbContext(options)
{
    public DbSet<AccountingItem> Items => Set<AccountingItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("accounting");
        b.Entity<AccountingItem>().HasKey(e => e.Id);
        b.Entity<AccountingItem>().Property(e => e.Name).IsRequired().HasMaxLength(200);
    }
}

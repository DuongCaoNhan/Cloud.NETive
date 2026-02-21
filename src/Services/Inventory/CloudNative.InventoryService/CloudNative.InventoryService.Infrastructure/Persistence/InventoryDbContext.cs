using Microsoft.EntityFrameworkCore;
using CloudNative.InventoryService.Domain.Entities;

namespace CloudNative.InventoryService.Infrastructure.Persistence;

public class InventoryDbContext(DbContextOptions<InventoryDbContext> options) : DbContext(options)
{
    public DbSet<InventoryItem> Items => Set<InventoryItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("inventory");
        b.Entity<InventoryItem>().HasKey(e => e.Id);
        b.Entity<InventoryItem>().Property(e => e.Name).IsRequired().HasMaxLength(200);
    }
}

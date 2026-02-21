using Microsoft.EntityFrameworkCore;
using CloudNative.AIService.Domain.Entities;

namespace CloudNative.AIService.Infrastructure.Persistence;

public class AIDbContext(DbContextOptions<AIDbContext> options) : DbContext(options)
{
    public DbSet<AIItem> Items => Set<AIItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("ai");
        b.Entity<AIItem>().HasKey(e => e.Id);
        b.Entity<AIItem>().Property(e => e.Name).IsRequired().HasMaxLength(200);
    }
}

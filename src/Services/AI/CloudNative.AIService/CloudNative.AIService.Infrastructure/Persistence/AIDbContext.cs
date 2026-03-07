using Microsoft.EntityFrameworkCore;
using CloudNative.AIService.Domain.Entities;

namespace CloudNative.AIService.Infrastructure.Persistence;

public class AIDbContext(DbContextOptions<AIDbContext> options) : DbContext(options)
{
    public DbSet<AIItem> Items => Set<AIItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("ai");
        // Picks up AIItemConfiguration automatically
        builder.ApplyConfigurationsFromAssembly(typeof(AIDbContext).Assembly);
    }
}


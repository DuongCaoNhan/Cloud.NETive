using Microsoft.EntityFrameworkCore;
using CloudNative.HRService.Domain.Entities;

namespace CloudNative.HRService.Infrastructure.Persistence;

public class HRDbContext(DbContextOptions<HRDbContext> options) : DbContext(options)
{
    public DbSet<HRItem> Items => Set<HRItem>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.HasDefaultSchema("hr");
        b.Entity<HRItem>().HasKey(e => e.Id);
        b.Entity<HRItem>().Property(e => e.Name).IsRequired().HasMaxLength(200);
    }
}

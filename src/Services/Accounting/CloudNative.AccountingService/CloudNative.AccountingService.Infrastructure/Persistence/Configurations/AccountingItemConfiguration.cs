using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CloudNative.AccountingService.Domain.Entities;

namespace CloudNative.AccountingService.Infrastructure.Persistence.Configurations;

/// <summary>
/// Separate IEntityTypeConfiguration — keeps DbContext clean.
/// Configures the Money Value Object as an EF Core Owned Entity.
/// </summary>
public sealed class AccountingItemConfiguration : IEntityTypeConfiguration<AccountingItem>
{
    public void Configure(EntityTypeBuilder<AccountingItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(500);

        builder.Property(e => e.Status)
            .HasConversion<string>()
            .HasMaxLength(20);

        // Money — Value Object mapped as Owned Entity (no separate table)
        builder.OwnsOne(e => e.Amount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("Amount")
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        // Domain events are in-memory only — never persisted
        builder.Ignore(e => e.DomainEvents);
    }
}

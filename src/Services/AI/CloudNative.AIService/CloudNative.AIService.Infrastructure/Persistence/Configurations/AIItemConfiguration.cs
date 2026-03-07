using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using CloudNative.AIService.Domain.Entities;

namespace CloudNative.AIService.Infrastructure.Persistence.Configurations;

public sealed class AIItemConfiguration : IEntityTypeConfiguration<AIItem>
{
    public void Configure(EntityTypeBuilder<AIItem> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.Category)
            .HasMaxLength(100)
            .HasDefaultValue("General");

        builder.Property(e => e.IsActive)
            .HasDefaultValue(true);

        // ── Vector storage ────────────────────────────────────────────────────
        // SQL Server 2025 native: uncomment below when upgrading to SqlClient 6+
        //   builder.Property(e => e.Embedding).HasColumnType("vector(384)");
        //
        // Current: JSON value converter — pragmatic fallback for compatibility
        var vectorConverter = new ValueConverter<float[]?, string?>(
            v  => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
            s  => s == null ? null : JsonSerializer.Deserialize<float[]>(s, (JsonSerializerOptions?)null));

        builder.Property(e => e.Embedding)
            .HasColumnType("nvarchar(max)")
            .HasColumnName("Embedding")
            .HasConversion(vectorConverter)
            .IsRequired(false);
    }
}

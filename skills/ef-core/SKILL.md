---
name: ef-core
description: 'Get best practices for Entity Framework Core'
---

# Entity Framework Core Best Practices

## DbContext Design

```csharp
// One DbContext per bounded context/service
// Inject as scoped lifetime (default in ASP.NET Core)
public class CatalogDbContext : DbContext
{
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply all configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
```

## Entity Design

```csharp
// Use IEntityTypeConfiguration<T> for clean separation
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        builder.HasIndex(p => p.Sku).IsUnique();
        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
```

## Performance

### AsNoTracking for read-only queries

```csharp
// Always use AsNoTracking for read-only queries
var products = await db.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .ToListAsync();
```

### Pagination (never skip without take)

```csharp
var page = await db.Products
    .AsNoTracking()
    .OrderBy(p => p.Name)
    .Skip((pageNumber - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();
```

### Eager loading (avoid N+1)

```csharp
// Explicit eager loading - don't rely on lazy loading in loops
var orders = await db.Orders
    .AsNoTracking()
    .Include(o => o.OrderLines)
        .ThenInclude(l => l.Product)
    .Include(o => o.Customer)
    .Where(o => o.Status == OrderStatus.Pending)
    .ToListAsync();
```

### Select only what you need

```csharp
// Projection reduces data transfer
var summaries = await db.Products
    .AsNoTracking()
    .Where(p => p.IsActive)
    .Select(p => new ProductSummary(p.Id, p.Name, p.Price))
    .ToListAsync();
```

### Compiled queries for hot paths

```csharp
private static readonly Func<AppDbContext, int, Task<Product?>> GetProductById =
    EF.CompileAsyncQuery((AppDbContext db, int id) =>
        db.Products.FirstOrDefault(p => p.Id == id));

// Usage
var product = await GetProductById(db, productId);
```

## Migrations

```bash
# Add migration
dotnet ef migrations add AddProductIndex --project DataNative.Data --startup-project DataNative.ApiService

# Apply migrations at startup (development only)
await db.Database.MigrateAsync();

# Generate SQL script for production
dotnet ef migrations script --idempotent -o migrations.sql
```

```csharp
// Apply migrations in Program.cs (development gate)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}
```

## Querying

```csharp
// Use FirstOrDefaultAsync, not First() — avoid exceptions for missing records
var product = await db.Products.FirstOrDefaultAsync(p => p.Id == id);
if (product is null) return Results.NotFound();

// Avoid loading entity just to check existence
var exists = await db.Products.AnyAsync(p => p.Sku == sku);

// Bulk operations (EF Core 7+)
await db.Products
    .Where(p => p.CategoryId == categoryId)
    .ExecuteUpdateAsync(s => s.SetProperty(p => p.IsActive, false));

await db.Products
    .Where(p => p.IsArchived && p.UpdatedAt < cutoff)
    .ExecuteDeleteAsync();
```

## Change Tracking

```csharp
// Detach entities you don't need to track
db.Entry(product).State = EntityState.Detached;

// Use Update only when you have a disconnected entity
db.Products.Update(detachedProduct);
await db.SaveChangesAsync();

// For partial updates, attach and set modified properties
db.Products.Attach(product);
db.Entry(product).Property(p => p.Price).IsModified = true;
await db.SaveChangesAsync();
```

## Security

```csharp
// Always parameterize — EF Core does this automatically with LINQ
// NEVER use string interpolation in FromSqlRaw
// SAFE:
var products = await db.Products
    .Where(p => p.Name == name)  // Parameterized automatically
    .ToListAsync();

// SAFE with raw SQL (uses parameters):
var products = await db.Products
    .FromSqlInterpolated($"SELECT * FROM Products WHERE Name = {name}")
    .ToListAsync();

// DANGEROUS — SQL injection risk:
// var products = await db.Products.FromSqlRaw($"SELECT * FROM Products WHERE Name = '{name}'").ToListAsync();
```

## Testing

```csharp
// Use in-memory SQLite for unit tests
public class ProductRepositoryTests
{
    private AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;
        var db = new AppDbContext(options);
        db.Database.EnsureCreated();
        return db;
    }

    [Fact]
    public async Task GetProduct_ReturnsCorrectProduct()
    {
        using var db = CreateContext();
        db.Products.Add(new Product { Id = 1, Name = "Test", Price = 9.99m });
        await db.SaveChangesAsync();

        var result = await new ProductRepository(db).GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Test", result.Name);
    }
}
```

## Key Rules Summary

| Rule | Reason |
|------|--------|
| `AsNoTracking()` for reads | 2–3x performance improvement |
| Always paginate | Prevents OOM on large tables |
| `Include()` explicitly | Avoids N+1 query problem |
| `Select()` projections | Reduces network data transfer |
| Never `FromSqlRaw` with string interpolation | SQL injection risk |
| `IEntityTypeConfiguration<T>` | Keeps entity classes clean |
| `MigrateAsync()` on startup in dev only | Safe schema evolution |
| Test with SQLite in-memory | Fast, realistic EF behavior |

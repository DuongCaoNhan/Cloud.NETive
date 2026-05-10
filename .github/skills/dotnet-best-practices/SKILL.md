---
name: dotnet-best-practices
description: 'Ensure .NET/C# code meets best practices for the solution/project.'
---

# .NET/C# Best Practices

## Documentation & Structure

```csharp
/// <summary>
/// Brief description of what this class/method does.
/// </summary>
/// <param name="id">The identifier of the resource.</param>
/// <returns>The resource, or null if not found.</returns>
public async Task<Product?> GetByIdAsync(int id)
```

- Add XML docs to all public types and members
- One class/interface per file, named after the type
- Organize by feature, not by type (e.g., `Products/` not `Controllers/Services/Repositories/`)

## Design Patterns

```csharp
// Repository pattern for data access
public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken ct = default);
    Task<Product> AddAsync(Product product, CancellationToken ct = default);
    Task UpdateAsync(Product product, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
}

// Result pattern instead of throwing exceptions for business rules
public record Result<T>(T? Value, string? Error, bool IsSuccess)
{
    public static Result<T> Success(T value) => new(value, null, true);
    public static Result<T> Failure(string error) => new(default, error, false);
}
```

## Dependency Injection & Services

```csharp
// Register services in Program.cs or extension methods
public static IServiceCollection AddProductFeature(this IServiceCollection services)
{
    services.AddScoped<IProductRepository, ProductRepository>();
    services.AddScoped<IProductService, ProductService>();
    return services;
}

// Inject via constructor (not property injection)
public class ProductService(IProductRepository repository, ILogger<ProductService> logger)
{
    // Use primary constructor syntax (.NET 8+)
}

// Service lifetimes:
// AddSingleton — shared across all requests (thread-safe state)
// AddScoped    — one per HTTP request (DbContext, user state)
// AddTransient — new instance every time (lightweight, stateless)
```

## Resource Management

```csharp
// Always await async streams, never discard
await foreach (var item in source.WithCancellation(ct))
{
    await ProcessAsync(item);
}

// Dispose IDisposable via using
await using var transaction = await db.Database.BeginTransactionAsync(ct);
try
{
    await repository.UpdateAsync(entity, ct);
    await transaction.CommitAsync(ct);
}
catch
{
    await transaction.RollbackAsync(ct);
    throw;
}

// Use CancellationToken throughout the call chain
public async Task<Product> GetAsync(int id, CancellationToken ct = default)
    => await repository.GetByIdAsync(id, ct);
```

## Async/Await

```csharp
// GOOD: async all the way down
public async Task<IActionResult> GetProduct(int id, CancellationToken ct)
{
    var product = await service.GetByIdAsync(id, ct);
    return product is null ? NotFound() : Ok(product);
}

// AVOID: blocking on async (causes deadlocks)
// var product = service.GetByIdAsync(id).Result;  // BAD
// service.GetByIdAsync(id).Wait();                // BAD

// GOOD: ConfigureAwait(false) in library code (not needed in ASP.NET Core)
public async Task<T> LibraryMethodAsync<T>()
{
    var result = await SomeOperationAsync().ConfigureAwait(false);
    return result;
}

// Use ValueTask for frequently non-async paths (hot paths)
public ValueTask<Product?> GetCachedAsync(int id)
{
    if (_cache.TryGetValue(id, out var cached)) return ValueTask.FromResult<Product?>(cached);
    return new ValueTask<Product?>(FetchFromDbAsync(id));
}
```

## Testing (MSTest + FluentAssertions + Moq)

```csharp
[TestClass]
public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock = new();
    private readonly ProductService _sut;

    public ProductServiceTests()
    {
        _sut = new ProductService(_repositoryMock.Object, NullLogger<ProductService>.Instance);
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var expected = new Product { Id = 1, Name = "Widget" };
        _repositoryMock.Setup(r => r.GetByIdAsync(1, default)).ReturnsAsync(expected);

        // Act
        var result = await _sut.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Widget");
    }

    [TestMethod]
    public async Task GetByIdAsync_WhenProductNotFound_ReturnsNull()
    {
        _repositoryMock.Setup(r => r.GetByIdAsync(99, default)).ReturnsAsync((Product?)null);

        var result = await _sut.GetByIdAsync(99);

        result.Should().BeNull();
    }
}
```

**Test naming**: `MethodName_Scenario_ExpectedResult`
**Test structure**: Arrange / Act / Assert (AAA)
**Mocking**: Moq for interfaces, `NullLogger<T>` for logging

## Configuration (IOptions Pattern)

```csharp
// Define strongly-typed settings
public class AiServiceOptions
{
    public const string SectionName = "AiService";

    [Required] public string ModelId { get; init; } = "";
    [Required] public string Endpoint { get; init; } = "";
    public int MaxTokens { get; init; } = 1000;
    public float Temperature { get; init; } = 0.7f;
}

// Register with validation
builder.Services.AddOptions<AiServiceOptions>()
    .BindConfiguration(AiServiceOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();  // Fail fast at startup

// Inject
public class AiService(IOptions<AiServiceOptions> options)
{
    private readonly AiServiceOptions _options = options.Value;
}
```

```json
// appsettings.json
{
  "AiService": {
    "ModelId": "gpt-4o",
    "Endpoint": "https://...",
    "MaxTokens": 2000
  }
}
```

## Error Handling & Logging

```csharp
// Use ILogger<T> structured logging — never Console.WriteLine
private readonly ILogger<ProductService> _logger;

// Log with structure, not string interpolation
_logger.LogInformation("Retrieving product {ProductId}", id);
_logger.LogWarning("Product {ProductId} not found in cache, fetching from database", id);
_logger.LogError(ex, "Failed to retrieve product {ProductId}", id);

// Global exception middleware (Program.cs)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/problem+json";
        var error = context.Features.Get<IExceptionHandlerFeature>();
        // Log and return ProblemDetails
    });
});

// Use ProblemDetails for API error responses
return TypedResults.Problem(
    title: "Product not found",
    statusCode: StatusCodes.Status404NotFound,
    detail: $"No product with ID {id} exists.");
```

## Performance & Security

```csharp
// Use spans for string manipulation in hot paths
ReadOnlySpan<char> trimmed = input.AsSpan().Trim();

// Use frozen collections for read-only lookups initialized at startup
private static readonly FrozenSet<string> AllowedOrigins =
    new[] { "https://app.datanative.ai" }.ToFrozenSet();

// Validate all inputs at service boundaries
public async Task<Result<Product>> CreateAsync(CreateProductRequest request)
{
    ArgumentNullException.ThrowIfNull(request);
    if (string.IsNullOrWhiteSpace(request.Name))
        return Result<Product>.Failure("Name is required");
    if (request.Price < 0)
        return Result<Product>.Failure("Price must be non-negative");
    // ...
}

// Use SecureRandom for cryptographic operations
var token = RandomNumberGenerator.GetBytes(32);
var base64Token = Convert.ToBase64String(token);
```

## Code Quality

```csharp
// Prefer pattern matching over type checks
if (response is { StatusCode: HttpStatusCode.Ok, Content: not null } okResponse)
    return await okResponse.Content.ReadAsStringAsync();

// Prefer records for DTOs and value objects
public record ProductDto(int Id, string Name, decimal Price, bool IsActive);

// Use collection expressions (.NET 8+)
List<string> tags = ["electronics", "featured", "new"];

// Null coalescing for defaults
var displayName = product.DisplayName ?? product.Name ?? "Unknown";

// Use switch expressions for mapping
var statusCode = result.Status switch
{
    ProcessingStatus.Success => HttpStatusCode.OK,
    ProcessingStatus.NotFound => HttpStatusCode.NotFound,
    ProcessingStatus.Conflict => HttpStatusCode.Conflict,
    _ => HttpStatusCode.InternalServerError
};
```

## Key Checklist

| Practice | Check |
|----------|-------|
| Async all the way | No `.Result` or `.Wait()` |
| CancellationToken threading | Passed through all async calls |
| IOptions<T> for config | No `IConfiguration.GetValue` in business code |
| ILogger<T> for logging | No `Console.WriteLine` |
| Constructor injection | No `ServiceLocator.GetService()` |
| `AsNoTracking()` for reads | On all EF Core read queries |
| XML docs on public API | All public types and members documented |
| Input validation at boundary | Before any business logic runs |
| ProblemDetails for API errors | Consistent error response shape |
| Unit tests with AAA pattern | `MethodName_Scenario_ExpectedResult` |

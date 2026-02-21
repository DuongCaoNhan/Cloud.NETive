var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CloudNative Monitoring Service", Version = "v1" }));

var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();

// Prometheus metrics scrape endpoint
app.MapGet("/metrics", () => Results.Text("# CloudNative Prometheus metrics\n# TYPE cloudnative_requests_total counter\ncloudnative_requests_total 0\n", "text/plain"))
   .WithSummary("Prometheus metrics");

app.MapGet("/api/v1/monitoring/status", () => new
{
    status  = "healthy",
    utc     = DateTime.UtcNow,
    services = new[]
    {
        new { name = "apiservice",   status = "unknown" },
        new { name = "gateway",      status = "unknown" },
        new { name = "webfrontend",  status = "unknown" }
    }
})
.WithSummary("Aggregate service status");

app.Run();

using CloudNative.Core.Models;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.SwaggerDoc("v1", new() { Title = "CloudNative Accounting Service", Version = "v1" }));

var app = builder.Build();
app.MapDefaultEndpoints();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/v1/accounting/ping", () =>
    ApiResponse<object>.Ok(new { service = "Accounting", utc = DateTime.UtcNow }))
   .WithTags("Accounting")
   .WithSummary("Service health ping");

app.Run();

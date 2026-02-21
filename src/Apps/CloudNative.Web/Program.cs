using CloudNative.Web.Components;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Typed HTTP client pointing to the API service via Aspire service discovery
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});

var app = builder.Build();

app.MapDefaultEndpoints();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();

/// <summary>Typed HTTP client for the CloudNative API service.</summary>
public sealed class ApiClient(HttpClient http)
{
    public async Task<WeatherForecast[]?> GetWeatherAsync(CancellationToken ct = default)
        => await http.GetFromJsonAsync<WeatherForecast[]>("/api/v1/weather", ct);
}

/// <summary>Shared weather forecast model.</summary>
public record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

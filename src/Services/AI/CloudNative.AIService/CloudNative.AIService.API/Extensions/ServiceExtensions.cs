using CloudNative.AIService.API.Apis;
using CloudNative.AIService.Infrastructure;

namespace CloudNative.AIService.API.Extensions;

/// <summary>
/// Pragmatic DDD: Extension methods keep Program.cs clean.
/// All DI wiring lives here — no separate Application.DependencyInjection needed.
/// </summary>
public static class ServiceExtensions
{
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
            c.SwaggerDoc("v1", new() { Title = "CloudNative AI Service — Pragmatic DDD", Version = "v1" }));

        return builder;
    }
}

public static class WebAppExtensions
{
    public static WebApplication MapAIEndpoints(this WebApplication app)
    {
        var v1 = app.MapGroup("/api/v1");
        v1.MapGroup("/ai/items").MapAIItemsApi().WithTags("AI Items");
        v1.MapGroup("/ai/search").MapAISearchApi().WithTags("AI Search");
        return app;
    }
}

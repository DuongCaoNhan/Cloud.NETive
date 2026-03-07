using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudNative.AIService.Application.Services;
using CloudNative.AIService.Infrastructure.Persistence;
using CloudNative.AIService.Infrastructure.Services;

namespace CloudNative.AIService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AIDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("AIDb"),
                sql => sql.MigrationsAssembly(typeof(AIDbContext).Assembly.FullName)));

        services.AddScoped<IAIEmbeddingService, AIEmbeddingService>();
        return services;
    }
}

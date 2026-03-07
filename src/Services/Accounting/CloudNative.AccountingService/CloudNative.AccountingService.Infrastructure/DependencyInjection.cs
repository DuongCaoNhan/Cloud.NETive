using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using CloudNative.AccountingService.Domain.Repositories;
using CloudNative.AccountingService.Infrastructure.Persistence;
using CloudNative.AccountingService.Infrastructure.Persistence.Repositories;

namespace CloudNative.AccountingService.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AccountingDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("AccountingDb"),
                sql => sql.MigrationsAssembly(typeof(AccountingDbContext).Assembly.FullName)));

        services.AddScoped<IAccountingRepository, AccountingRepository>();
        return services;
    }
}

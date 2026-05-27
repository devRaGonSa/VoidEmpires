using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure;

public static class VoidEmpiresPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddVoidEmpiresPersistence(
        this IServiceCollection services,
        string? connectionString)
    {
        ArgumentNullException.ThrowIfNull(services);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return services;
        }

        services.AddDbContext<VoidEmpiresDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}

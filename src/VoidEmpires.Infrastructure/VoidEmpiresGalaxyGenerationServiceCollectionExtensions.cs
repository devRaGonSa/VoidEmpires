using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Galaxy;
using VoidEmpires.Infrastructure.GalaxyGeneration;

namespace VoidEmpires.Infrastructure;

public static class VoidEmpiresGalaxyGenerationServiceCollectionExtensions
{
    public static IServiceCollection AddVoidEmpiresGalaxyGeneration(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton<IGalaxyGenerator, GalaxyGenerator>();

        return services;
    }
}

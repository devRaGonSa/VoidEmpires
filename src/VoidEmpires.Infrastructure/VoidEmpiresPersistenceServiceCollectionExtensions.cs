using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Assets;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Application.Colonization;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Application.Research;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Infrastructure.Assets;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Colonization;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;
using VoidEmpires.Infrastructure.Research;
using VoidEmpires.Infrastructure.Visuals;

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
        services.AddScoped<IStartingCivilizationService, StartingCivilizationService>();
        services.AddScoped<IPlanetColonizationService, PlanetColonizationService>();
        services.AddScoped<IPlanetEconomyTickService, PlanetEconomyTickService>();
        services.AddScoped<IPlanetBuildingConstructionService, PlanetBuildingConstructionService>();
        services.AddScoped<IPlanetBuildingUpgradeService, PlanetBuildingUpgradeService>();
        services.AddScoped<IPlanetConstructionQueueService, PlanetConstructionQueueService>();
        services.AddScoped<IConstructionOrderCompletionService, ConstructionOrderCompletionService>();
        services.AddScoped<IResearchUpgradeService, ResearchUpgradeService>();
        services.AddScoped<IResearchQueueService, ResearchQueueService>();
        services.AddScoped<IResearchOrderCompletionService, ResearchOrderCompletionService>();
        services.AddScoped<IAssetProductionQueueService, AssetProductionQueueService>();
        services.AddScoped<IAssetOrderProcessor, AssetOrderProcessor>();
        services.AddScoped<IOrbitalGroupService, OrbitalStockGroupService>();
        services.AddScoped<IOrbitalGroupLookupService, OrbitalGroupLookupService>();
        services.AddScoped<IOrbitalGroupTransferPlanningService, OrbitalGroupPlannerService>();
        services.AddScoped<IOrbitalTransferPersistenceService, OrbitalTransferPersistenceService>();
        services.AddScoped<IOrbitalTransferCompletionService, OrbitalTransferCompletionService>();
        services.AddScoped<IOrbitalTransferLookupService, OrbitalTransferLookupService>();
        services.AddScoped<IPlanetVisualStateService, PlanetVisualStateService>();

        return services;
    }

    public static IServiceCollection AddVoidEmpiresConstructionQueueWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(ConstructionQueueWorkerOptions.SectionName);
        services.Configure<ConstructionQueueWorkerOptions>(section);

        if (section.GetValue<bool>(nameof(ConstructionQueueWorkerOptions.Enabled)))
        {
            services.AddHostedService<ConstructionQueueWorker>();
        }

        return services;
    }

    public static IServiceCollection AddVoidEmpiresResearchQueueWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(ResearchQueueWorkerOptions.SectionName);
        services.Configure<ResearchQueueWorkerOptions>(section);

        if (section.GetValue<bool>(nameof(ResearchQueueWorkerOptions.Enabled)))
        {
            services.AddHostedService<ResearchProgressWorker>();
        }

        return services;
    }

    public static IServiceCollection AddVoidEmpiresAssetProductionWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(AssetProductionWorkerOptions.SectionName);
        services.Configure<AssetProductionWorkerOptions>(section);

        if (section.GetValue<bool>(nameof(AssetProductionWorkerOptions.Enabled)))
        {
            services.AddHostedService<AssetProductionWorker>();
        }

        return services;
    }

    public static IServiceCollection AddVoidEmpiresOrbitalTransferWorker(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var section = configuration.GetSection(OrbitalTransferWorkerOptions.SectionName);
        services.Configure<OrbitalTransferWorkerOptions>(section);

        if (section.GetValue<bool>(nameof(OrbitalTransferWorkerOptions.Enabled)))
        {
            services.AddHostedService<OrbitalTransferWorker>();
        }

        return services;
    }

    public static IServiceCollection AddVoidEmpiresIdentity(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services
            .AddIdentityCore<VoidEmpiresUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<VoidEmpiresDbContext>()
            .AddTokenProvider<DataProtectorTokenProvider<VoidEmpiresUser>>(TokenOptions.DefaultProvider);

        services.AddScoped<IdentityAccountService>();
        services.AddScoped<IUserRegistrationService>(provider => provider.GetRequiredService<IdentityAccountService>());
        services.AddScoped<IEmailConfirmationService>(provider => provider.GetRequiredService<IdentityAccountService>());

        return services;
    }
}

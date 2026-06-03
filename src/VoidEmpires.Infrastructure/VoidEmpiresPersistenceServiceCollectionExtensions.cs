using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Assets;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Application.Colonization;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.Identity;
using VoidEmpires.Application.Players;
using VoidEmpires.Application.Planets;
using VoidEmpires.Application.Research;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Infrastructure.Assets;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Colonization;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Economy;
using VoidEmpires.Infrastructure.Fleets;
using VoidEmpires.Infrastructure.Identity;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Players;
using VoidEmpires.Infrastructure.Planets;
using VoidEmpires.Infrastructure.Research;
using VoidEmpires.Infrastructure.StrategicMap;
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
        services.AddScoped<IDevelopmentSeedService, DevelopmentSeedService>();
        services.AddScoped<IStartingCivilizationService, StartingCivilizationService>();
        services.AddScoped<IPlanetColonizationService, PlanetColonizationService>();
        services.AddScoped<IPlanetEconomyTickService, PlanetEconomyTickService>();
        services.AddScoped<IResourceSpendService, ResourceSpendService>();
        services.AddScoped<IPlanetBuildingConstructionService, PlanetBuildingConstructionService>();
        services.AddScoped<IPlanetBuildingUpgradeService, PlanetBuildingUpgradeService>();
        services.AddScoped<IPlanetConstructionQueueService, PlanetConstructionQueueService>();
        services.AddScoped<IConstructionOrderCompletionService, ConstructionOrderCompletionService>();
        services.AddScoped<IResearchUpgradeService, ResearchUpgradeService>();
        services.AddScoped<IResearchQueueService, ResearchQueueService>();
        services.AddScoped<IResearchOrderCompletionService, ResearchOrderCompletionService>();
        services.AddScoped<IAssetProductionQueueService, AssetProductionQueueService>();
        services.AddScoped<IAssetOrderProcessor, AssetOrderProcessor>();
        services.AddScoped<IDevShipyardUiStateService, DevShipyardUiStateService>();
        services.AddScoped<IOrbitalGroupService, OrbitalStockGroupService>();
        services.AddScoped<IOrbitalGroupSplitService, OrbitalGroupSplitService>();
        services.AddScoped<IOrbitalGroupMergeService, OrbitalGroupMergeService>();
        services.AddScoped<IOrbitalGroupLookupService, OrbitalGroupLookupService>();
        services.AddScoped<IOrbitalGroupTransferPlanningService, OrbitalGroupPlannerService>();
        services.AddScoped<IOrbitalRouteProfileService, OrbitalRouteProfileService>();
        services.AddScoped<IOrbitalFuelReadinessService, OrbitalFuelReadinessService>();
        services.AddScoped<IOrbitalTravelEstimateService, OrbitalTravelEstimateService>();
        services.AddScoped<IOrbitalTransferPersistenceService, OrbitalTransferPersistenceService>();
        services.AddScoped<IOrbitalTransferCancelService, OrbitalTransferCancelService>();
        services.AddScoped<IOrbitalTransferCompletionService, OrbitalTransferCompletionService>();
        services.AddScoped<IOrbitalTransferLookupService, OrbitalTransferLookupService>();
        services.AddScoped<IFleetOperationalOverviewService, FleetOperationalOverviewService>();
        services.AddScoped<IDevFleetActionManifestService, DevFleetActionManifestService>();
        services.AddScoped<IDevFleetUiStateService, DevFleetUiStateService>();
        services.AddScoped<IDevPlanetUiStateService, DevPlanetUiStateService>();
        services.AddScoped<IDevDefenseUiStateService, DevDefenseUiStateService>();
        services.AddScoped<IPlanetVisualStateService, PlanetVisualStateService>();
        services.AddScoped<ISystemVisualStateService, SystemVisualStateService>();
        services.AddScoped<IStrategicMapService, StrategicMapService>();
        services.AddScoped<IMapVisibilityService, MapVisibilityService>();
        services.AddScoped<ISensorProfileService, SensorProfileService>();
        services.AddScoped<IDetectionCoverageService, DetectionCoverageService>();
        services.AddScoped<IInterceptionOpportunityService, InterceptionOpportunityService>();
        services.AddScoped<IAllianceReadinessQueryService, AllianceReadinessQueryService>();
        services.AddScoped<IAlliancePactReadinessQueryService, AlliancePactReadinessQueryService>();
        services.AddScoped<IDiplomaticContactQueryService, DiplomaticContactQueryService>();
        services.AddScoped<IExplorationActionPreviewService, ExplorationActionPreviewService>();
        services.AddScoped<IExplorationMissionCreateService, ExplorationMissionCreateService>();
        services.AddScoped<IExplorationMissionCompletionService, ExplorationMissionCompletionService>();
        services.AddScoped<IExplorationMissionQueryService, ExplorationMissionQueryService>();
        services.AddScoped<IExplorationKnowledgeQueryService, ExplorationKnowledgeQueryService>();

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

        services.AddDataProtection();

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

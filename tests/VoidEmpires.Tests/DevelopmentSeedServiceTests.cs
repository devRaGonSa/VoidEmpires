using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Application.Markets;
using VoidEmpires.Application.Rankings;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Population;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Development;
using VoidEmpires.Infrastructure.Markets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Rankings;
using VoidEmpires.Infrastructure.StrategicMap;
using VoidEmpires.Infrastructure.Visuals;

namespace VoidEmpires.Tests;

public class DevelopmentSeedServiceTests
{
    private static readonly Guid PlayerProfileId = Guid.Parse("90000000-0000-0000-0000-000000000001");
    private static readonly Guid CivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid GalaxyId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid SystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid OwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid VisibleComparisonPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid KnownComparisonPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");
    private static readonly Guid SeedDiplomaticContactCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private const int SeededConstructionSequenceStart = 10_000;
    private const int SeededResearchSequenceStart = 20_000;
    private const int SeededAssetProductionSequenceStart = 30_000;

    [Fact]
    public async Task ApplyAsyncCreatesExpectedStrategicMapDataset()
    {
        await using var dbContext = CreateDbContext();

        var result = await new DevelopmentSeedService(dbContext)
            .ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.True(result.Succeeded);
        Assert.NotNull(result.ProfileMetadata);
        Assert.Equal("minimal-validation", result.ProfileMetadata.Name);
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Research");
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/research?", StringComparison.Ordinal));
        Assert.Contains(result.AppliedSteps, x => x.Contains(CivilizationId.ToString(), StringComparison.Ordinal));
        var playerProfile = await dbContext.Set<PlayerProfile>().SingleAsync(x => x.Id == PlayerProfileId);
        var civilization = await dbContext.Set<Civilization>().SingleAsync(x => x.Id == CivilizationId);
        Assert.Equal(PlayerProfileId, playerProfile.Id);
        Assert.Equal(CivilizationId, civilization.Id);
        Assert.Equal(PlayerProfileId, civilization.PlayerProfileId);
        Assert.Equal(OwnedPlanetId, civilization.HomePlanetId);
        var galaxy = await dbContext.Galaxies.SingleAsync(x => x.Id == GalaxyId);
        var solarSystem = await dbContext.Set<SolarSystem>().SingleAsync(x => x.Id == SystemId);
        Assert.Equal(GalaxyId, galaxy.Id);
        Assert.Equal(GalaxyId, solarSystem.GalaxyId);
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.True(await dbContext.Set<PlanetOwnership>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(125, stockpile.Credits);
        Assert.Equal(160, stockpile.Metal);
        Assert.Equal(100, stockpile.Crystal);
        Assert.Equal(50, stockpile.Gas);
        var productionProfile = await dbContext.PlanetProductionProfiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(18, productionProfile.CreditsPerHour);
        Assert.Equal(14, productionProfile.MetalPerHour);
        Assert.Equal(6, productionProfile.CrystalPerHour);
        Assert.Equal(3, productionProfile.GasPerHour);
        Assert.True(await dbContext.Set<PlanetBuilding>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.Shipyard && x.Level == 1));
        Assert.True(await dbContext.Set<PlanetPopulationProfile>().AnyAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.True(await dbContext.Set<OrbitalAssetStock>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft && x.Quantity == 4));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.True(await dbContext.Set<OrbitalGroup>().AnyAsync(
            x => x.CivilizationId == CivilizationId &&
                x.CurrentPlanetId == OwnedPlanetId &&
                x.AssetType == SpaceAssetType.CargoCraft &&
                x.Quantity == 2 &&
                x.Status == OrbitalGroupStatus.Reserved));
        Assert.True(await dbContext.Set<OrbitalTransfer>().AnyAsync(x => x.CivilizationId == CivilizationId && x.DestinationPlanetId != x.OriginPlanetId && x.Status == OrbitalTransferStatus.Planned));
    }

    [Fact]
    public async Task ApplyAsyncSupportsCockpitValidationProfileWithoutDuplicatingRicherHistoryRows()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        var result = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        Assert.True(result.Succeeded);
        Assert.Equal("cockpit-validation", result.Profile);
        Assert.NotNull(result.ProfileMetadata);
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Espionage");
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Market");
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Defenses");
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Ground Army");
        Assert.Contains(result.ProfileMetadata.IntendedCockpits, x => x == "Ranking");
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/market?", StringComparison.Ordinal));
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/ranking?", StringComparison.Ordinal));
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/espionage?", StringComparison.Ordinal));
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/defenses?", StringComparison.Ordinal));
        Assert.Contains(result.ProfileMetadata.RecommendedQaUrls, x => x.StartsWith("/ground-army?", StringComparison.Ordinal));
        Assert.Contains("Aurelia", result.ProfileMetadata.AvailabilityNote, StringComparison.Ordinal);
        Assert.Contains(result.AppliedSteps, x => x.Contains("Helios Gate", StringComparison.Ordinal));
        Assert.Contains(result.AppliedSteps, x => x.Contains("Aurelia", StringComparison.Ordinal));
        Assert.Contains(result.AppliedSteps, x => x.Contains("Cinder Reach", StringComparison.Ordinal));
        Assert.Contains(result.AppliedSteps, x => x.Contains("Aether Crown", StringComparison.Ordinal));
        Assert.Contains(result.AppliedSteps, x => x.Contains("completed queue history", StringComparison.OrdinalIgnoreCase));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(220, stockpile.Credits);
        Assert.Equal(320, stockpile.Metal);
        Assert.Equal(220, stockpile.Crystal);
        Assert.Equal(120, stockpile.Gas);
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.DefenseGrid && x.Level == 1));
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.Barracks && x.Level == 1));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.Status == ConstructionQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<ResearchProject>().CountAsync(x => x.CivilizationId == CivilizationId && x.ResearchType == ResearchType.EnergySystems));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == CivilizationId && x.ResearchType == ResearchType.EnergySystems && x.Status == ResearchQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.SpaceAssetType == SpaceAssetType.ScoutCraft && x.Status == AssetProductionOrderStatus.Completed));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.PlanetaryAssetType == PlanetaryAssetType.PatrolGroup && x.Status == AssetProductionOrderStatus.Completed));
        Assert.True(await dbContext.Set<OrbitalAssetStock>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.ScoutCraft && x.Quantity == 1));
        Assert.True(await dbContext.Set<PlanetaryAssetStock>().AnyAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == PlanetaryAssetType.PatrolGroup && x.Quantity == 2));
        Assert.Equal(1, await dbContext.Set<DiplomaticContact>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.ContactedCivilizationId == SeedDiplomaticContactCivilizationId &&
            x.Status == DiplomaticContactStatus.Contacted));
    }

    [Fact]
    public async Task CockpitValidationSeedSupportsAllianceUiStateAfterReapply()
    {
        await using var dbContext = CreateDbContext();
        var seedService = new DevelopmentSeedService(dbContext);

        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var alliance = await new DevAllianceUiStateService(dbContext)
            .GetAsync(new GetDevAllianceUiStateRequest(CivilizationId));

        Assert.Empty(alliance.Errors);
        Assert.NotNull(alliance.Identity);
        Assert.NotNull(alliance.Alliance);
        Assert.Equal(CivilizationId, alliance.Identity.CivilizationId);
        Assert.Equal("Void Seed Civilization", alliance.Identity.CivilizationName);
        Assert.Equal("None", alliance.Alliance.StateKey);
        Assert.False(alliance.Alliance.HasActiveAlliance);
        Assert.Single(alliance.KnownContacts);
        Assert.Equal(3, alliance.FuturePacts.Count);
        Assert.Equal(3, alliance.FutureActions.Count);
        Assert.All(alliance.FutureActions, action => Assert.False(action.IsAvailable));
        Assert.Equal(6, alliance.ActionSummary?.DisabledActionCount);
        Assert.Contains(alliance.Limitations, x =>
            x.Contains("cabina", StringComparison.OrdinalIgnoreCase) &&
            x.Contains("lectura", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(alliance.Limitations, x => x.Contains("does not mutate", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CockpitValidationSeedSupportsMeaningfulMarketReadModelAfterReapply()
    {
        await using var dbContext = CreateDbContext();
        var seedService = new DevelopmentSeedService(dbContext);

        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var market = await new DevMarketUiStateService(dbContext)
            .GetAsync(new GetDevMarketUiStateRequest(CivilizationId, OwnedPlanetId));

        Assert.Empty(market.Errors);
        Assert.NotNull(market.Market);
        Assert.Equal(OwnedPlanetId, market.SelectedPlanetId);
        Assert.NotEmpty(market.KnownPlanets);
        Assert.Equal("Aurelia", market.Market.SelectedPlanetName);
        Assert.Equal("Helios Gate", market.Market.SelectedSolarSystemName);
        Assert.NotEmpty(market.Market.CivilizationReserves);
        Assert.NotEmpty(market.Market.SelectedPlanetReserves);
        Assert.NotNull(market.Market.SelectedPlanetProduction);
        Assert.NotEmpty(market.Market.ReferenceRatios);
        Assert.Contains(market.Market.Signals, x => x.SignalKey == "FutureTradeRoute");
        Assert.All(market.Market.FutureActions, x => Assert.False(x.IsEnabled));
        Assert.Contains(market.Market.ReferenceRatios, x => x.IsAdvisory);
        Assert.Contains(market.Market.CivilizationReserves, x => x.ResourceType == ResourceType.Credits && x.Quantity >= 220);
        Assert.Contains(market.Market.SelectedPlanetReserves, x => x.ResourceType == ResourceType.Metal && x.Quantity >= 320);
        Assert.Contains(market.Market.Limitations, x =>
            x.Contains("mercado", StringComparison.OrdinalIgnoreCase) &&
            x.Contains("lectura", StringComparison.OrdinalIgnoreCase));
        Assert.Contains(market.Market.Limitations, x => x.Contains("advisory", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task CockpitValidationSeedSupportsRankingUiStateAfterReapply()
    {
        await using var dbContext = CreateDbContext();
        var seedService = new DevelopmentSeedService(dbContext);

        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        _ = await seedService.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var ranking = await new DevRankingUiStateService(dbContext, CreateStrategicMapService(dbContext))
            .GetAsync(new GetDevRankingUiStateRequest(CivilizationId));

        Assert.Empty(ranking.Errors);
        Assert.NotNull(ranking.Identity);
        Assert.NotNull(ranking.Summary);
        Assert.NotNull(ranking.Diagnostics);
        Assert.Equal(CivilizationId, ranking.Identity.CivilizationId);
        Assert.Equal("Void Seed Civilization", ranking.Identity.CivilizationName);
        Assert.True(ranking.Summary.TotalPowerIndex > 0);
        Assert.Contains(ranking.Summary.Categories, x => x.CategoryKey == "economicPower" && x.Score > 0);
        Assert.Contains(ranking.Summary.Categories, x => x.CategoryKey == "orbitalCapacity" && x.Score > 0);
        Assert.Equal(3, ranking.DemoComparisons.Count);
        Assert.Equal(3, ranking.FuturePlaceholders.Count);
        Assert.Equal(3, ranking.DisabledActions.Count);
        Assert.All(ranking.DemoComparisons, x => Assert.True(x.IsDemoOnly));
        Assert.All(ranking.DisabledActions, x => Assert.False(x.IsAvailable));
        Assert.Contains(ranking.Limitations, x => x.Contains("read-only", StringComparison.OrdinalIgnoreCase));
        Assert.Equal(1, await dbContext.Set<DiplomaticContact>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.ContactedCivilizationId == SeedDiplomaticContactCivilizationId &&
            x.Status == DiplomaticContactStatus.Contacted));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.ResearchType == ResearchType.EnergySystems &&
            x.Status == ResearchQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.SpaceAssetType == SpaceAssetType.ScoutCraft &&
            x.Status == AssetProductionOrderStatus.Completed));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.PlanetaryAssetType == PlanetaryAssetType.PatrolGroup &&
            x.Status == AssetProductionOrderStatus.Completed));
    }

    [Fact]
    public async Task ApplyAsyncSupportsCockpitValidationAfterExistingResearchQueueState()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        dbContext.Set<ResearchOrder>().Add(ResearchOrder.Create(
            CivilizationId,
            OwnedPlanetId,
            ResearchType.PlanetaryEngineering,
            1,
            1,
            new DateTime(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 1, 12, 10, 0, DateTimeKind.Utc),
            ResearchQueueItemStatus.Active));
        await dbContext.SaveChangesAsync();

        var firstApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        var secondApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        Assert.True(firstApply.Succeeded);
        Assert.True(secondApply.Succeeded);
        Assert.Equal(2, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.ResearchType == ResearchType.EnergySystems &&
            x.TargetLevel == 1 &&
            x.Status == ResearchQueueItemStatus.Completed));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.Sequence == 1));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x =>
            x.CivilizationId == CivilizationId &&
            x.Sequence >= SeededResearchSequenceStart));
    }

    [Fact]
    public async Task ApplyAsyncSupportsCockpitValidationAfterExistingConstructionAndProductionQueueState()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        dbContext.Set<PlanetConstructionOrder>().Add(PlanetConstructionOrder.Create(
            OwnedPlanetId,
            ConstructionQueueItemAction.Upgrade,
            BuildingType.CommandCenter,
            5,
            1,
            new DateTime(2026, 1, 2, 8, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 2, 8, 5, 0, DateTimeKind.Utc),
            ConstructionQueueItemStatus.Active));
        dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
            OwnedPlanetId,
            AssetProductionTarget.Orbital,
            null,
            SpaceAssetType.CargoCraft,
            1,
            1,
            new DateTime(2026, 1, 2, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 2, 9, 3, 0, DateTimeKind.Utc),
            AssetProductionOrderStatus.Active));
        await dbContext.SaveChangesAsync();

        var firstApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        var secondApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        Assert.True(firstApply.Succeeded);
        Assert.True(secondApply.Succeeded);
        Assert.Equal(2, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(3, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.Sequence == 1));
        Assert.Equal(1, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.Sequence >= SeededConstructionSequenceStart));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.Sequence == 1));
        Assert.Equal(2, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.Sequence >= SeededAssetProductionSequenceStart));
    }

    [Fact]
    public async Task ApplyAsyncSupportsCockpitValidationAfterExistingManualShipyardOrderWithoutDuplicatingSeededHistory()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
            OwnedPlanetId,
            AssetProductionTarget.Orbital,
            null,
            SpaceAssetType.CargoCraft,
            1,
            1,
            new DateTime(2026, 1, 3, 9, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 1, 3, 9, 3, 0, DateTimeKind.Utc),
            AssetProductionOrderStatus.Active));
        await dbContext.SaveChangesAsync();

        var firstApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));
        var secondApply = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        Assert.True(firstApply.Succeeded);
        Assert.True(secondApply.Succeeded);
        Assert.Equal(3, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.SpaceAssetType == SpaceAssetType.CargoCraft &&
            x.Status == AssetProductionOrderStatus.Active &&
            x.Sequence == 1));
        Assert.Equal(1, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.SpaceAssetType == SpaceAssetType.ScoutCraft &&
            x.Status == AssetProductionOrderStatus.Completed));
        Assert.Equal(2, await dbContext.Set<AssetProductionOrder>().CountAsync(x =>
            x.PlanetId == OwnedPlanetId &&
            x.Sequence >= SeededAssetProductionSequenceStart));
    }

    [Fact]
    public async Task ApplyAsyncSupportsResearchValidationProfileWithoutDuplicatingCompletedResearchHistory()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        var result = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("research-validation"));
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("research-validation"));

        Assert.True(result.Succeeded);
        Assert.Equal("research-validation", result.Profile);
        Assert.NotNull(result.ProfileMetadata);
        Assert.Contains(result.AppliedSteps, x => x.Contains("completed project", StringComparison.OrdinalIgnoreCase));

        var stockpile = await dbContext.PlanetResourceStockpiles.SingleAsync(x => x.PlanetId == OwnedPlanetId);
        Assert.Equal(125, stockpile.Credits);
        Assert.Equal(110, stockpile.Metal);
        Assert.Equal(70, stockpile.Crystal);
        Assert.Equal(30, stockpile.Gas);
        Assert.Equal(1, await dbContext.Set<ResearchProject>().CountAsync(x => x.CivilizationId == CivilizationId && x.ResearchType == ResearchType.EnergySystems));
        Assert.Equal(1, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == CivilizationId && x.ResearchType == ResearchType.EnergySystems && x.Status == ResearchQueueItemStatus.Completed));
    }

    [Fact]
    public async Task ApplyAsyncIsIdempotent()
    {
        await using var dbContext = CreateDbContext();
        var service = new DevelopmentSeedService(dbContext);

        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));
        _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        Assert.Equal(1, await dbContext.Set<PlayerProfile>().CountAsync(x => x.Id == PlayerProfileId));
        Assert.Equal(1, await dbContext.Set<Civilization>().CountAsync(x => x.Id == CivilizationId));
        Assert.Equal(1, await dbContext.Galaxies.CountAsync(x => x.Id == GalaxyId));
        Assert.Equal(1, await dbContext.Set<SolarSystem>().CountAsync(x => x.Id == SystemId));
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.Equal(1, await dbContext.Set<PlanetOwnership>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.PlanetResourceStockpiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.PlanetProductionProfiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.Shipyard));
        Assert.Equal(1, await dbContext.Set<PlanetPopulationProfile>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<OrbitalAssetStock>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft));
        Assert.Equal(4, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.Set<OrbitalTransfer>().CountAsync(x => x.CivilizationId == CivilizationId && x.Status == OrbitalTransferStatus.Planned));
    }

    [Fact]
    public async Task ApplyingEachImplementedProfileTwiceDoesNotDuplicateDeterministicSeedRows()
    {
        foreach (var profile in DevelopmentSeedProfiles.All.Where(x => x.IsImplemented).Select(x => x.Name))
        {
            await using var dbContext = CreateDbContext();
            var service = new DevelopmentSeedService(dbContext);

            _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest(profile));
            _ = await service.ApplyAsync(new ApplyDevelopmentSeedRequest(profile));

            await AssertCommonSeedCountsAsync(dbContext);

            switch (profile)
            {
                case "minimal-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 0, researchOrders: 0, researchProjects: 0, assetProductionOrders: 0, orbitalAssetStocks: 1, orbitalGroups: 4, orbitalTransfers: 1);
                    break;
                case "cockpit-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 1, researchOrders: 1, researchProjects: 1, assetProductionOrders: 2, orbitalAssetStocks: 2, orbitalGroups: 4, orbitalTransfers: 1);
                    Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.DefenseGrid));
                    Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.Barracks));
                    Assert.Equal(1, await dbContext.Set<PlanetaryAssetStock>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.AssetType == PlanetaryAssetType.PatrolGroup));
                    break;
                case "shipyard-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 0, researchOrders: 0, researchProjects: 0, assetProductionOrders: 1, orbitalAssetStocks: 2, orbitalGroups: 4, orbitalTransfers: 1);
                    break;
                case "fleet-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 0, researchOrders: 0, researchProjects: 0, assetProductionOrders: 0, orbitalAssetStocks: 1, orbitalGroups: 6, orbitalTransfers: 2);
                    break;
                case "research-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 0, researchOrders: 1, researchProjects: 1, assetProductionOrders: 0, orbitalAssetStocks: 1, orbitalGroups: 4, orbitalTransfers: 1);
                    break;
                case "planet-full-validation":
                    await AssertProfileCountsAsync(dbContext, constructionOrders: 1, researchOrders: 0, researchProjects: 0, assetProductionOrders: 0, orbitalAssetStocks: 1, orbitalGroups: 4, orbitalTransfers: 1);
                    break;
                default:
                    throw new InvalidOperationException($"Unhandled profile '{profile}'.");
            }
        }
    }

    [Fact]
    public async Task ApplyAsyncReturnsKnownProfilesForUnsupportedProfile()
    {
        await using var dbContext = CreateDbContext();

        var result = await new DevelopmentSeedService(dbContext)
            .ApplyAsync(new ApplyDevelopmentSeedRequest("unknown-profile"));

        Assert.False(result.Succeeded);
        Assert.Null(result.ProfileMetadata);
        Assert.Contains(result.Errors, x => x.Contains("Unsupported development seed profile 'unknown-profile'.", StringComparison.Ordinal));
        Assert.Contains(result.KnownProfiles, x => x.Name == "minimal-validation" && x.IsImplemented);
        Assert.Contains(result.KnownProfiles, x => x.Name == "research-validation" && x.IsImplemented);
    }

    [Fact]
    public async Task StrategicMapServiceReturnsNonEmptySystemsFromSeededDataset()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("minimal-validation"));

        var result = await CreateStrategicMapService(dbContext).GetAsync(new GetStrategicMapRequest(CivilizationId));

        var system = Assert.Single(result.Systems);
        Assert.Equal(SystemId, system.SystemId);
        Assert.Equal(MapVisibilityLevel.Visible, system.VisibilityLevel);
        Assert.True(system.IsVisible);
        Assert.Equal(3, system.Planets.Count);
        Assert.Contains(system.Planets, x => x.PlanetId == OwnedPlanetId && x.IsOwnedByRequestingCivilization);
    }

    [Fact]
    public async Task CockpitValidationSeedKeepsStrategicMapFleetContextVisible()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var result = await CreateStrategicMapService(dbContext).GetAsync(new GetStrategicMapRequest(CivilizationId));

        var system = Assert.Single(result.Systems);
        Assert.Equal(SystemId, system.SystemId);
        Assert.Equal("Helios Gate", system.SystemName);
        Assert.Equal(MapVisibilityLevel.Visible, system.VisibilityLevel);
        Assert.True(system.IsVisible);
        Assert.Equal(12, system.CoordinateX);
        Assert.Equal(-4, system.CoordinateY);
        Assert.Equal(3, system.CoordinateZ);
        Assert.Equal(3, system.Planets.Count);
        Assert.Contains(system.Planets, x =>
            x.PlanetId == OwnedPlanetId &&
            x.PlanetName == "Aurelia" &&
            x.IsOwnedByRequestingCivilization &&
            x.VisibilityLevel == MapVisibilityLevel.Owned);
        Assert.Contains(system.Planets, x =>
            x.PlanetId == VisibleComparisonPlanetId &&
            x.PlanetName == "Cinder Reach" &&
            x.IsVisible &&
            !x.IsOwnedByRequestingCivilization &&
            x.VisibilityLevel == MapVisibilityLevel.Visible);
        Assert.Contains(system.Planets, x =>
            x.PlanetId == KnownComparisonPlanetId &&
            x.PlanetName == "Aether Crown" &&
            x.IsVisible &&
            !x.IsOwnedByRequestingCivilization &&
            x.VisibilityLevel == MapVisibilityLevel.Visible);
        Assert.Equal(4, system.FleetPresence.Count);
        Assert.Single(system.TransferOverlays);
    }

    [Fact]
    public async Task GalaxyCockpitRegressionSmoke_CockpitValidationSeedKeepsProjectableFocusableReadModel()
    {
        await using var dbContext = CreateDbContext();
        await new DevelopmentSeedService(dbContext).ApplyAsync(new ApplyDevelopmentSeedRequest("cockpit-validation"));

        var result = await CreateStrategicMapService(dbContext).GetAsync(new GetStrategicMapRequest(CivilizationId));

        Assert.True(result.Systems.Count >= 1);
        Assert.Contains(result.Systems, system => system.IsVisible || system.IsOwnedByRequestingCivilization);

        var focusableSystem = result.Systems
            .FirstOrDefault(system => system.IsOwnedByRequestingCivilization)
            ?? result.Systems.FirstOrDefault(system => system.IsVisible);

        Assert.NotNull(focusableSystem);
        Assert.NotEqual(Guid.Empty, focusableSystem.SystemId);
        Assert.False(string.IsNullOrWhiteSpace(focusableSystem.SystemName));
        Assert.Contains(result.Systems, system =>
            system.CoordinateX == 12 &&
            system.CoordinateY == -4 &&
            system.CoordinateZ == 3);
        Assert.True(result.Systems.Sum(system => system.Planets.Count) >= 1);
        Assert.True(
            focusableSystem.FleetPresence.Count > 0 ||
            focusableSystem.TransferOverlays.Count > 0);
    }

    private static StrategicMapService CreateStrategicMapService(VoidEmpiresDbContext dbContext) =>
        new(
            dbContext,
            new SystemVisualStateService(dbContext, new PlanetVisualStateService(dbContext)),
            new MapVisibilityService(dbContext));

    private static async Task AssertCommonSeedCountsAsync(VoidEmpiresDbContext dbContext)
    {
        Assert.Equal(1, await dbContext.Set<PlayerProfile>().CountAsync(x => x.Id == PlayerProfileId));
        Assert.Equal(1, await dbContext.Set<Civilization>().CountAsync(x => x.Id == CivilizationId));
        Assert.Equal(1, await dbContext.Galaxies.CountAsync(x => x.Id == GalaxyId));
        Assert.Equal(1, await dbContext.Set<SolarSystem>().CountAsync(x => x.Id == SystemId));
        Assert.Equal(3, await dbContext.Set<Planet>().CountAsync(x => x.SolarSystemId == SystemId));
        Assert.Equal(1, await dbContext.Set<PlanetOwnership>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.CivilizationId == CivilizationId));
        Assert.Equal(1, await dbContext.PlanetResourceStockpiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.PlanetProductionProfiles.CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<PlanetPopulationProfile>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.CommandCenter));
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.HabitationDistrict));
        Assert.Equal(1, await dbContext.Set<PlanetBuilding>().CountAsync(x => x.PlanetId == OwnedPlanetId && x.BuildingType == BuildingType.Shipyard));
    }

    private static async Task AssertProfileCountsAsync(
        VoidEmpiresDbContext dbContext,
        int constructionOrders,
        int researchOrders,
        int researchProjects,
        int assetProductionOrders,
        int orbitalAssetStocks,
        int orbitalGroups,
        int orbitalTransfers)
    {
        Assert.Equal(constructionOrders, await dbContext.Set<PlanetConstructionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(researchOrders, await dbContext.Set<ResearchOrder>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(researchProjects, await dbContext.Set<ResearchProject>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(assetProductionOrders, await dbContext.Set<AssetProductionOrder>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(orbitalAssetStocks, await dbContext.Set<OrbitalAssetStock>().CountAsync(x => x.PlanetId == OwnedPlanetId));
        Assert.Equal(orbitalGroups, await dbContext.Set<OrbitalGroup>().CountAsync(x => x.CivilizationId == CivilizationId));
        Assert.Equal(orbitalTransfers, await dbContext.Set<OrbitalTransfer>().CountAsync(x => x.CivilizationId == CivilizationId && x.Status == OrbitalTransferStatus.Planned));
    }

    private static VoidEmpiresDbContext CreateDbContext() =>
        new(new DbContextOptionsBuilder<VoidEmpiresDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options);
}

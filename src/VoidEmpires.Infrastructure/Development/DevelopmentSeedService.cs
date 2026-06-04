using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Population;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace VoidEmpires.Infrastructure.Development;

public sealed class DevelopmentSeedService(VoidEmpiresDbContext dbContext) : IDevelopmentSeedService
{
    private static readonly Guid SeedPlayerProfileId = Guid.Parse("90000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedGalaxyId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedSystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedStarId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid SeedIcePlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");
    private const string SeedUserId = "seed-user-minimal-validation";
    private const string SeedPlayerDisplayName = "Validation Commander";
    private const string SeedCivilizationName = "Void Seed Civilization";
    private const string SeedGalaxyName = "Validation Galaxy";
    private const string SeedSystemName = "Helios Gate";
    private const string SeedStarName = "Helios Gate Star";
    private const int SeedCredits = 125;
    private const int SeedMetal = 160;
    private const int SeedCrystal = 100;
    private const int SeedGas = 50;
    private const int CockpitValidationCredits = 220;
    private const int CockpitValidationMetal = 320;
    private const int CockpitValidationCrystal = 220;
    private const int CockpitValidationGas = 120;
    private const int ShipyardValidationCredits = 180;
    private const int ShipyardValidationMetal = 180;
    private const int ShipyardValidationCrystal = 110;
    private const int ShipyardValidationGas = 70;
    private const int FleetValidationCredits = 260;
    private const int FleetValidationMetal = 260;
    private const int FleetValidationCrystal = 160;
    private const int FleetValidationGas = 100;
    private const int ResearchValidationCredits = 125;
    private const int ResearchValidationMetal = 110;
    private const int ResearchValidationCrystal = 70;
    private const int ResearchValidationGas = 30;
    private const int PlanetValidationCredits = 240;
    private const int PlanetValidationMetal = 220;
    private const int PlanetValidationCrystal = 140;
    private const int PlanetValidationGas = 90;
    private static readonly DateTime SeedTransferDepartureAtUtc = new(2026, 6, 2, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SeedTransferArrivalAtUtc = new(2026, 6, 2, 12, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FleetValidationTransferDepartureAtUtc = new(2026, 6, 1, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FleetValidationTransferArrivalAtUtc = new(2026, 6, 1, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationConstructionStartAtUtc = new(2026, 5, 31, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationConstructionEndAtUtc = new(2026, 5, 31, 8, 5, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationResearchStartAtUtc = new(2026, 5, 31, 9, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationResearchEndAtUtc = new(2026, 5, 31, 9, 12, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationProductionStartAtUtc = new(2026, 5, 31, 10, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationProductionEndAtUtc = new(2026, 5, 31, 10, 3, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationGroundArmyStartAtUtc = new(2026, 5, 31, 10, 15, 0, DateTimeKind.Utc);
    private static readonly DateTime CockpitValidationGroundArmyEndAtUtc = new(2026, 5, 31, 10, 18, 0, DateTimeKind.Utc);
    private const int SeededConstructionSequenceStart = 10_000;
    private const int SeededResearchSequenceStart = 20_000;
    private const int SeededAssetProductionSequenceStart = 30_000;
    private const int SeededGroundArmyProductionSequenceStart = 31_000;

    public async Task<ApplyDevelopmentSeedResult> ApplyAsync(
        ApplyDevelopmentSeedRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var profile = request.Profile.Trim();

        if (!DevelopmentSeedProfiles.TryGetImplementedProfile(profile, out var profileMetadata))
        {
            var implementedProfiles = string.Join(", ", DevelopmentSeedProfiles.All.Where(x => x.IsImplemented).Select(x => x.Name));
            return ApplyDevelopmentSeedResult.Failure(
                profile,
                [$"Unsupported development seed profile '{profile}'. Implemented profiles: {implementedProfiles}."],
                DevelopmentSeedProfiles.All);
        }

        var appliedSteps = new List<string>();

        switch (profileMetadata.Name)
        {
            case "minimal-validation":
                await SeedMinimalValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated strategic-map seed for civilization {SeedCivilizationId}.",
                    $"System {SeedSystemId} includes planets {SeedOwnedPlanetId}, {SeedOuterPlanetId}, and {SeedIcePlanetId}.",
                    "Fleet validation rows include stationed groups, one active own transfer, and owned-planet resource context.",
                    "Owned planet construction validation includes visible stockpile, existing buildings, a readable economy summary, and both affordable and blocked actions."
                ]);
                break;
            case "cockpit-validation":
                await SeedCockpitValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated richer cockpit seed for civilization {SeedCivilizationId}.",
                    $"System {SeedSystemId} keeps Aurelia plus visible comparison planets {SeedOuterPlanetId} and {SeedIcePlanetId}.",
                    "Planet and Construction gain completed queue history without blocking current actions.",
                    "Research and Shipyard gain richer completed history while preserving available and blocked read-state.",
                    "Defenses now include a visible Defense Grid readiness baseline without seeding combat or an active queue.",
                    "Ground Army now includes one Barracks baseline, one completed patrol training history row, one ready patrol stock row, and truthful blocked comparisons."
                ]);
                break;
            case "shipyard-validation":
                await SeedShipyardValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated shipyard-focused seed for civilization {SeedCivilizationId}.",
                    "Aurelia keeps one available basic hull, multiple blocked comparisons with distinct reasons, and richer local stock.",
                    "Shipyard queue history is seeded as completed-only so the primary enqueue path remains available."
                ]);
                break;
            case "fleet-validation":
                await SeedFleetValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated fleet-focused seed for civilization {SeedCivilizationId}.",
                    "Aurelia keeps multiple stationed groups plus a stationed logistics example.",
                    "Fleet state now includes one standard active transfer and one additional due transfer for complete-due QA."
                ]);
                break;
            case "research-validation":
                await SeedResearchValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated research-focused seed for civilization {SeedCivilizationId}.",
                    "Aurelia keeps one deterministic available technology plus multiple insufficient-resource comparisons.",
                    "Research history now includes one completed project and one completed order without seeding an active queue item."
                ]);
                break;
            case "planet-full-validation":
                await SeedPlanetFullValidationProfileAsync(cancellationToken);
                appliedSteps.AddRange([
                    $"Validated planet-focused seed for civilization {SeedCivilizationId}.",
                    "Aurelia now includes richer general infrastructure and completed construction history.",
                    "Planet and Construction keep available and blocked actions without seeding an open queue."
                ]);
                break;
            default:
                throw new InvalidOperationException($"Implemented seed profile '{profileMetadata.Name}' is not mapped.");
        }

        return ApplyDevelopmentSeedResult.Success(
            profileMetadata.Name,
            appliedSteps,
            profileMetadata,
            DevelopmentSeedProfiles.All);
    }

    private async Task SeedMinimalValidationProfileAsync(CancellationToken cancellationToken)
    {
        if (!await dbContext.Set<PlayerProfile>().AnyAsync(x => x.Id == SeedPlayerProfileId, cancellationToken))
        {
            var profile = PlayerProfile.Create(SeedUserId, SeedPlayerDisplayName);
            dbContext.Entry(profile).Property(x => x.Id).CurrentValue = SeedPlayerProfileId;
            dbContext.Set<PlayerProfile>().Add(profile);
        }

        if (!await dbContext.Galaxies.AnyAsync(x => x.Id == SeedGalaxyId, cancellationToken))
        {
            dbContext.Galaxies.Add(new Galaxy(SeedGalaxyId, SeedGalaxyName));
        }

        if (!await dbContext.Set<SolarSystem>().AnyAsync(x => x.Id == SeedSystemId, cancellationToken))
        {
            dbContext.Set<SolarSystem>().Add(new SolarSystem(
                SeedSystemId,
                SeedGalaxyId,
                SeedSystemName,
                new GalaxyCoordinates(12, -4, 3),
                new Star(SeedStarId, SeedSystemId, SeedStarName, StarType.YellowDwarf)));
        }

        if (!await dbContext.Set<Planet>().AnyAsync(x => x.Id == SeedOwnedPlanetId, cancellationToken))
        {
            dbContext.Set<Planet>().Add(new Planet(
                SeedOwnedPlanetId,
                SeedSystemId,
                "Aurelia",
                1,
                PlanetType.Terran,
                118,
                PlanetColonizationStatus.Colonized));
        }

        if (!await dbContext.Set<Planet>().AnyAsync(x => x.Id == SeedOuterPlanetId, cancellationToken))
        {
            dbContext.Set<Planet>().Add(new Planet(
                SeedOuterPlanetId,
                SeedSystemId,
                "Cinder Reach",
                2,
                PlanetType.Desert,
                94));
        }

        if (!await dbContext.Set<Planet>().AnyAsync(x => x.Id == SeedIcePlanetId, cancellationToken))
        {
            dbContext.Set<Planet>().Add(new Planet(
                SeedIcePlanetId,
                SeedSystemId,
                "Aether Crown",
                3,
                PlanetType.GasGiant,
                160));
        }

        if (!await dbContext.Set<PlanetOwnership>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.CivilizationId == SeedCivilizationId,
                cancellationToken))
        {
            dbContext.Set<PlanetOwnership>().Add(PlanetOwnership.Create(SeedOwnedPlanetId, SeedCivilizationId));
        }

        if (!await dbContext.Set<Civilization>().AnyAsync(x => x.Id == SeedCivilizationId, cancellationToken))
        {
            var civilization = Civilization.Create(
                SeedPlayerProfileId,
                SeedCivilizationName,
                CivilizationArchetype.Balanced,
                SeedOwnedPlanetId);
            dbContext.Entry(civilization).Property(x => x.Id).CurrentValue = SeedCivilizationId;
            dbContext.Set<Civilization>().Add(civilization);
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        if (stockpile is null)
        {
            stockpile = PlanetResourceStockpile.Create(SeedOwnedPlanetId);
            dbContext.PlanetResourceStockpiles.Add(stockpile);
        }

        EnsureMinimumStockpile(stockpile);

        if (!await dbContext.PlanetProductionProfiles.AnyAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken))
        {
            dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(
                SeedOwnedPlanetId,
                18,
                14,
                6,
                3));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.CommandCenter,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.CommandCenter, 4, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.HabitationDistrict,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.HabitationDistrict, 3, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.Shipyard,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.Shipyard, 1, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOuterPlanetId && x.BuildingType == BuildingType.MetalMine,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOuterPlanetId, BuildingType.MetalMine, 6, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOuterPlanetId && x.BuildingType == BuildingType.Shipyard,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOuterPlanetId, BuildingType.Shipyard, 2, 1));
        }

        if (!await dbContext.Set<PlanetBuildingCapacity>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId,
                cancellationToken))
        {
            dbContext.Set<PlanetBuildingCapacity>().Add(PlanetBuildingCapacity.Create(SeedOwnedPlanetId, 120));
        }

        if (!await dbContext.Set<PlanetBuildingCapacity>().AnyAsync(
                x => x.PlanetId == SeedOuterPlanetId,
                cancellationToken))
        {
            dbContext.Set<PlanetBuildingCapacity>().Add(PlanetBuildingCapacity.Create(SeedOuterPlanetId, 120));
        }

        if (!await dbContext.Set<PlanetPopulationProfile>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId,
                cancellationToken))
        {
            dbContext.Set<PlanetPopulationProfile>().Add(PlanetPopulationProfile.Create(SeedOwnedPlanetId, 2_000, 500, 100));
        }

        if (!await dbContext.Set<OrbitalAssetStock>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == SpaceAssetType.EscortCraft,
                cancellationToken))
        {
            dbContext.Set<OrbitalAssetStock>().Add(OrbitalAssetStock.Create(SeedOwnedPlanetId, SpaceAssetType.EscortCraft, 4));
        }

        if (!await dbContext.Set<OrbitalGroup>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedOwnedPlanetId &&
                    x.AssetType == SpaceAssetType.ScoutCraft &&
                    x.Quantity == 3 &&
                    x.Status == OrbitalGroupStatus.Stationed,
                cancellationToken))
        {
            dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedOwnedPlanetId,
                SpaceAssetType.ScoutCraft,
                3));
        }

        if (!await dbContext.Set<OrbitalGroup>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedOwnedPlanetId &&
                    x.AssetType == SpaceAssetType.ScoutCraft &&
                    x.Quantity == 2 &&
                    x.Status == OrbitalGroupStatus.Stationed,
                cancellationToken))
        {
            dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedOwnedPlanetId,
                SpaceAssetType.ScoutCraft,
                2));
        }

        if (!await dbContext.Set<OrbitalGroup>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedIcePlanetId &&
                    x.AssetType == SpaceAssetType.EscortCraft &&
                    x.Quantity == 4 &&
                    x.Status == OrbitalGroupStatus.Stationed,
                cancellationToken))
        {
            dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedIcePlanetId,
                SpaceAssetType.EscortCraft,
                4));
        }

        var transferGroup = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedOwnedPlanetId &&
                    x.AssetType == SpaceAssetType.CargoCraft &&
                    x.Quantity == 2,
                cancellationToken);

        if (transferGroup is null)
        {
            transferGroup = OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedOwnedPlanetId,
                SpaceAssetType.CargoCraft,
                2);
            transferGroup.Reserve();
            dbContext.Set<OrbitalGroup>().Add(transferGroup);
        }
        else if (transferGroup.Status == OrbitalGroupStatus.Stationed)
        {
            transferGroup.Reserve();
        }

        if (!await dbContext.Set<OrbitalTransfer>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OrbitalGroupId == transferGroup.Id &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.DestinationPlanetId == SeedOuterPlanetId &&
                    x.Status == OrbitalTransferStatus.Planned,
                cancellationToken))
        {
            dbContext.Set<OrbitalTransfer>().Add(OrbitalTransfer.CreatePlanned(
                SeedCivilizationId,
                transferGroup.Id,
                SeedOwnedPlanetId,
                SeedOuterPlanetId,
                2,
                SeedTransferDepartureAtUtc,
                SeedTransferArrivalAtUtc));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureMinimumStockpile(PlanetResourceStockpile stockpile)
    {
        if (stockpile.Credits < SeedCredits)
        {
            stockpile.Increase(ResourceType.Credits, SeedCredits - stockpile.Credits);
        }

        if (stockpile.Metal < SeedMetal)
        {
            stockpile.Increase(ResourceType.Metal, SeedMetal - stockpile.Metal);
        }

        if (stockpile.Crystal < SeedCrystal)
        {
            stockpile.Increase(ResourceType.Crystal, SeedCrystal - stockpile.Crystal);
        }

        if (stockpile.Gas < SeedGas)
        {
            stockpile.Increase(ResourceType.Gas, SeedGas - stockpile.Gas);
        }
    }

    private async Task SeedCockpitValidationProfileAsync(CancellationToken cancellationToken)
    {
        await SeedMinimalValidationProfileAsync(cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        EnsureCockpitValidationStockpile(stockpile);

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.DefenseGrid,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.DefenseGrid, 1, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.Barracks,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.Barracks, 1, 1));
        }

        await EnsureSeededConstructionOrderAsync(
            SeedOwnedPlanetId,
            ConstructionQueueItemAction.Construct,
            BuildingType.SolarPlant,
            1,
            CockpitValidationConstructionStartAtUtc,
            CockpitValidationConstructionEndAtUtc,
            ConstructionQueueItemStatus.Completed,
            SeededConstructionSequenceStart,
            cancellationToken);

        if (!await dbContext.Set<ResearchProject>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId && x.ResearchType == ResearchType.EnergySystems,
                cancellationToken))
        {
            dbContext.Set<ResearchProject>().Add(ResearchProject.Create(SeedCivilizationId, ResearchType.EnergySystems));
        }

        await EnsureSeededResearchOrderAsync(
            SeedCivilizationId,
            SeedOwnedPlanetId,
            ResearchType.EnergySystems,
            1,
            CockpitValidationResearchStartAtUtc,
            CockpitValidationResearchEndAtUtc,
            ResearchQueueItemStatus.Completed,
            SeededResearchSequenceStart,
            cancellationToken);

        await EnsureSeededAssetProductionOrderAsync(
            SeedOwnedPlanetId,
            AssetProductionTarget.Orbital,
            null,
            SpaceAssetType.ScoutCraft,
            1,
            CockpitValidationProductionStartAtUtc,
            CockpitValidationProductionEndAtUtc,
            AssetProductionOrderStatus.Completed,
            SeededAssetProductionSequenceStart,
            cancellationToken);

        await EnsureSeededAssetProductionOrderAsync(
            SeedOwnedPlanetId,
            AssetProductionTarget.Planetary,
            PlanetaryAssetType.PatrolGroup,
            null,
            2,
            CockpitValidationGroundArmyStartAtUtc,
            CockpitValidationGroundArmyEndAtUtc,
            AssetProductionOrderStatus.Completed,
            SeededGroundArmyProductionSequenceStart,
            cancellationToken);

        if (!await dbContext.Set<OrbitalAssetStock>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == SpaceAssetType.ScoutCraft,
                cancellationToken))
        {
            dbContext.Set<OrbitalAssetStock>().Add(OrbitalAssetStock.Create(SeedOwnedPlanetId, SpaceAssetType.ScoutCraft, 1));
        }

        if (!await dbContext.Set<PlanetaryAssetStock>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == PlanetaryAssetType.PatrolGroup,
                cancellationToken))
        {
            dbContext.Set<PlanetaryAssetStock>().Add(PlanetaryAssetStock.Create(SeedOwnedPlanetId, PlanetaryAssetType.PatrolGroup, 2));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureCockpitValidationStockpile(PlanetResourceStockpile stockpile)
    {
        if (stockpile.Credits < CockpitValidationCredits)
        {
            stockpile.Increase(ResourceType.Credits, CockpitValidationCredits - stockpile.Credits);
        }

        if (stockpile.Metal < CockpitValidationMetal)
        {
            stockpile.Increase(ResourceType.Metal, CockpitValidationMetal - stockpile.Metal);
        }

        if (stockpile.Crystal < CockpitValidationCrystal)
        {
            stockpile.Increase(ResourceType.Crystal, CockpitValidationCrystal - stockpile.Crystal);
        }

        if (stockpile.Gas < CockpitValidationGas)
        {
            stockpile.Increase(ResourceType.Gas, CockpitValidationGas - stockpile.Gas);
        }
    }

    private async Task SeedShipyardValidationProfileAsync(CancellationToken cancellationToken)
    {
        await SeedMinimalValidationProfileAsync(cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        EnsureShipyardValidationStockpile(stockpile);

        await EnsureSeededAssetProductionOrderAsync(
            SeedOwnedPlanetId,
            AssetProductionTarget.Orbital,
            null,
            SpaceAssetType.ScoutCraft,
            1,
            CockpitValidationProductionStartAtUtc,
            CockpitValidationProductionEndAtUtc,
            AssetProductionOrderStatus.Completed,
            SeededAssetProductionSequenceStart,
            cancellationToken);

        if (!await dbContext.Set<OrbitalAssetStock>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.AssetType == SpaceAssetType.ScoutCraft,
                cancellationToken))
        {
            dbContext.Set<OrbitalAssetStock>().Add(OrbitalAssetStock.Create(SeedOwnedPlanetId, SpaceAssetType.ScoutCraft, 1));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureShipyardValidationStockpile(PlanetResourceStockpile stockpile)
    {
        if (stockpile.Credits < ShipyardValidationCredits)
        {
            stockpile.Increase(ResourceType.Credits, ShipyardValidationCredits - stockpile.Credits);
        }

        if (stockpile.Metal < ShipyardValidationMetal)
        {
            stockpile.Increase(ResourceType.Metal, ShipyardValidationMetal - stockpile.Metal);
        }

        if (stockpile.Crystal < ShipyardValidationCrystal)
        {
            stockpile.Increase(ResourceType.Crystal, ShipyardValidationCrystal - stockpile.Crystal);
        }

        if (stockpile.Gas < ShipyardValidationGas)
        {
            stockpile.Increase(ResourceType.Gas, ShipyardValidationGas - stockpile.Gas);
        }
    }

    private async Task SeedFleetValidationProfileAsync(CancellationToken cancellationToken)
    {
        await SeedMinimalValidationProfileAsync(cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        EnsureFleetValidationStockpile(stockpile);

        if (!await dbContext.Set<OrbitalGroup>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedOwnedPlanetId &&
                    x.AssetType == SpaceAssetType.CargoCraft &&
                    x.Quantity == 1 &&
                    x.Status == OrbitalGroupStatus.Stationed,
                cancellationToken))
        {
            dbContext.Set<OrbitalGroup>().Add(OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedOwnedPlanetId,
                SpaceAssetType.CargoCraft,
                1));
        }

        var dueTransferGroup = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.CurrentPlanetId == SeedOwnedPlanetId &&
                    x.AssetType == SpaceAssetType.CargoCraft &&
                    x.Quantity == 1 &&
                    x.Status == OrbitalGroupStatus.Reserved,
                cancellationToken);

        if (dueTransferGroup is null)
        {
            dueTransferGroup = OrbitalGroup.CreateStationed(
                SeedCivilizationId,
                SeedOwnedPlanetId,
                SeedOwnedPlanetId,
                SpaceAssetType.CargoCraft,
                1);
            dueTransferGroup.Reserve();
            dbContext.Set<OrbitalGroup>().Add(dueTransferGroup);
        }
        else if (dueTransferGroup.Status == OrbitalGroupStatus.Stationed)
        {
            dueTransferGroup.Reserve();
        }

        if (!await dbContext.Set<OrbitalTransfer>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId &&
                    x.OrbitalGroupId == dueTransferGroup.Id &&
                    x.OriginPlanetId == SeedOwnedPlanetId &&
                    x.DestinationPlanetId == SeedIcePlanetId &&
                    x.Status == OrbitalTransferStatus.Planned,
                cancellationToken))
        {
            dbContext.Set<OrbitalTransfer>().Add(OrbitalTransfer.CreatePlanned(
                SeedCivilizationId,
                dueTransferGroup.Id,
                SeedOwnedPlanetId,
                SeedIcePlanetId,
                2,
                FleetValidationTransferDepartureAtUtc,
                FleetValidationTransferArrivalAtUtc));
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsureFleetValidationStockpile(PlanetResourceStockpile stockpile)
    {
        if (stockpile.Credits < FleetValidationCredits)
        {
            stockpile.Increase(ResourceType.Credits, FleetValidationCredits - stockpile.Credits);
        }

        if (stockpile.Metal < FleetValidationMetal)
        {
            stockpile.Increase(ResourceType.Metal, FleetValidationMetal - stockpile.Metal);
        }

        if (stockpile.Crystal < FleetValidationCrystal)
        {
            stockpile.Increase(ResourceType.Crystal, FleetValidationCrystal - stockpile.Crystal);
        }

        if (stockpile.Gas < FleetValidationGas)
        {
            stockpile.Increase(ResourceType.Gas, FleetValidationGas - stockpile.Gas);
        }
    }

    private async Task SeedResearchValidationProfileAsync(CancellationToken cancellationToken)
    {
        await SeedMinimalValidationProfileAsync(cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        SetResearchValidationStockpile(stockpile);

        if (!await dbContext.Set<ResearchProject>().AnyAsync(
                x => x.CivilizationId == SeedCivilizationId && x.ResearchType == ResearchType.EnergySystems,
                cancellationToken))
        {
            dbContext.Set<ResearchProject>().Add(ResearchProject.Create(SeedCivilizationId, ResearchType.EnergySystems));
        }

        await EnsureSeededResearchOrderAsync(
            SeedCivilizationId,
            SeedOwnedPlanetId,
            ResearchType.EnergySystems,
            1,
            CockpitValidationResearchStartAtUtc,
            CockpitValidationResearchEndAtUtc,
            ResearchQueueItemStatus.Completed,
            SeededResearchSequenceStart,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private void SetResearchValidationStockpile(PlanetResourceStockpile stockpile)
    {
        var entry = dbContext.Entry(stockpile);
        entry.Property(x => x.Credits).CurrentValue = ResearchValidationCredits;
        entry.Property(x => x.Metal).CurrentValue = ResearchValidationMetal;
        entry.Property(x => x.Crystal).CurrentValue = ResearchValidationCrystal;
        entry.Property(x => x.Gas).CurrentValue = ResearchValidationGas;
    }

    private async Task SeedPlanetFullValidationProfileAsync(CancellationToken cancellationToken)
    {
        await SeedMinimalValidationProfileAsync(cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken);

        EnsurePlanetValidationStockpile(stockpile);

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.SolarPlant,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.SolarPlant, 2, 1));
        }

        if (!await dbContext.Set<PlanetBuilding>().AnyAsync(
                x => x.PlanetId == SeedOwnedPlanetId && x.BuildingType == BuildingType.MetalMine,
                cancellationToken))
        {
            dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(SeedOwnedPlanetId, BuildingType.MetalMine, 2, 1));
        }

        await EnsureSeededConstructionOrderAsync(
            SeedOwnedPlanetId,
            ConstructionQueueItemAction.Upgrade,
            BuildingType.SolarPlant,
            2,
            CockpitValidationConstructionStartAtUtc,
            CockpitValidationConstructionEndAtUtc,
            ConstructionQueueItemStatus.Completed,
            SeededConstructionSequenceStart + 100,
            cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static void EnsurePlanetValidationStockpile(PlanetResourceStockpile stockpile)
    {
        if (stockpile.Credits < PlanetValidationCredits)
        {
            stockpile.Increase(ResourceType.Credits, PlanetValidationCredits - stockpile.Credits);
        }

        if (stockpile.Metal < PlanetValidationMetal)
        {
            stockpile.Increase(ResourceType.Metal, PlanetValidationMetal - stockpile.Metal);
        }

        if (stockpile.Crystal < PlanetValidationCrystal)
        {
            stockpile.Increase(ResourceType.Crystal, PlanetValidationCrystal - stockpile.Crystal);
        }

        if (stockpile.Gas < PlanetValidationGas)
        {
            stockpile.Increase(ResourceType.Gas, PlanetValidationGas - stockpile.Gas);
        }
    }

    private async Task EnsureSeededResearchOrderAsync(
        Guid civilizationId,
        Guid sourcePlanetId,
        ResearchType researchType,
        int targetLevel,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ResearchQueueItemStatus status,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var existingLogicalOrder = await dbContext.Set<ResearchOrder>()
            .SingleOrDefaultAsync(
                x => x.CivilizationId == civilizationId &&
                    x.SourcePlanetId == sourcePlanetId &&
                    x.ResearchType == researchType &&
                    x.TargetLevel == targetLevel &&
                    x.StartsAtUtc == startsAtUtc &&
                    x.EndsAtUtc == endsAtUtc &&
                    x.Status == status,
                cancellationToken);

        if (existingLogicalOrder is not null)
        {
            return;
        }

        var sequence = await GetNextAvailableResearchSequenceAsync(civilizationId, preferredSequence, cancellationToken);
        dbContext.Set<ResearchOrder>().Add(ResearchOrder.Create(
            civilizationId,
            sourcePlanetId,
            researchType,
            targetLevel,
            sequence,
            startsAtUtc,
            endsAtUtc,
            status));
    }

    private async Task EnsureSeededConstructionOrderAsync(
        Guid planetId,
        ConstructionQueueItemAction action,
        BuildingType buildingType,
        int targetLevel,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        ConstructionQueueItemStatus status,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var existingLogicalOrder = await dbContext.Set<PlanetConstructionOrder>()
            .SingleOrDefaultAsync(
                x => x.PlanetId == planetId &&
                    x.Action == action &&
                    x.BuildingType == buildingType &&
                    x.TargetLevel == targetLevel &&
                    x.StartsAtUtc == startsAtUtc &&
                    x.EndsAtUtc == endsAtUtc &&
                    x.Status == status,
                cancellationToken);

        if (existingLogicalOrder is not null)
        {
            return;
        }

        var sequence = await GetNextAvailableConstructionSequenceAsync(planetId, preferredSequence, cancellationToken);
        dbContext.Set<PlanetConstructionOrder>().Add(PlanetConstructionOrder.Create(
            planetId,
            action,
            buildingType,
            targetLevel,
            sequence,
            startsAtUtc,
            endsAtUtc,
            status));
    }

    private async Task EnsureSeededAssetProductionOrderAsync(
        Guid planetId,
        AssetProductionTarget target,
        PlanetaryAssetType? planetaryAssetType,
        SpaceAssetType? spaceAssetType,
        int quantity,
        DateTime startsAtUtc,
        DateTime endsAtUtc,
        AssetProductionOrderStatus status,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var existingLogicalOrder = await dbContext.Set<AssetProductionOrder>()
            .SingleOrDefaultAsync(
                x => x.PlanetId == planetId &&
                    x.Target == target &&
                    x.PlanetaryAssetType == planetaryAssetType &&
                    x.SpaceAssetType == spaceAssetType &&
                    x.Quantity == quantity &&
                    x.StartsAtUtc == startsAtUtc &&
                    x.EndsAtUtc == endsAtUtc &&
                    x.Status == status,
                cancellationToken);

        if (existingLogicalOrder is not null)
        {
            return;
        }

        var sequence = await GetNextAvailableAssetProductionSequenceAsync(planetId, preferredSequence, cancellationToken);
        dbContext.Set<AssetProductionOrder>().Add(AssetProductionOrder.Create(
            planetId,
            target,
            planetaryAssetType,
            spaceAssetType,
            quantity,
            sequence,
            startsAtUtc,
            endsAtUtc,
            status));
    }

    private async Task<int> GetNextAvailableResearchSequenceAsync(
        Guid civilizationId,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var usedSequences = await dbContext.Set<ResearchOrder>()
            .Where(x => x.CivilizationId == civilizationId && x.Sequence >= preferredSequence)
            .Select(x => x.Sequence)
            .ToListAsync(cancellationToken);

        return GetNextAvailableSequence(usedSequences, preferredSequence);
    }

    private async Task<int> GetNextAvailableConstructionSequenceAsync(
        Guid planetId,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var usedSequences = await dbContext.Set<PlanetConstructionOrder>()
            .Where(x => x.PlanetId == planetId && x.Sequence >= preferredSequence)
            .Select(x => x.Sequence)
            .ToListAsync(cancellationToken);

        return GetNextAvailableSequence(usedSequences, preferredSequence);
    }

    private async Task<int> GetNextAvailableAssetProductionSequenceAsync(
        Guid planetId,
        int preferredSequence,
        CancellationToken cancellationToken)
    {
        var usedSequences = await dbContext.Set<AssetProductionOrder>()
            .Where(x => x.PlanetId == planetId && x.Sequence >= preferredSequence)
            .Select(x => x.Sequence)
            .ToListAsync(cancellationToken);

        return GetNextAvailableSequence(usedSequences, preferredSequence);
    }

    private static int GetNextAvailableSequence(IEnumerable<int> usedSequences, int preferredSequence)
    {
        var used = usedSequences.ToHashSet();
        var candidate = preferredSequence;

        while (used.Contains(candidate))
        {
            candidate++;
        }

        return candidate;
    }
}

using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace VoidEmpires.Infrastructure.Development;

public sealed class DevelopmentSeedService(VoidEmpiresDbContext dbContext) : IDevelopmentSeedService
{
    private static readonly Guid SeedCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedGalaxyId = Guid.Parse("10000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedSystemId = Guid.Parse("20000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedStarId = Guid.Parse("30000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOwnedPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private static readonly Guid SeedOuterPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000002");
    private static readonly Guid SeedIcePlanetId = Guid.Parse("40000000-0000-0000-0000-000000000003");
    private static readonly DateTime SeedTransferDepartureAtUtc = new(2026, 6, 2, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime SeedTransferArrivalAtUtc = new(2026, 6, 2, 12, 0, 0, DateTimeKind.Utc);

    public async Task<ApplyDevelopmentSeedResult> ApplyAsync(
        ApplyDevelopmentSeedRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var profile = request.Profile.Trim();

        if (!string.Equals(profile, "minimal-validation", StringComparison.OrdinalIgnoreCase))
        {
            return ApplyDevelopmentSeedResult.Failure(profile, ["Unsupported development seed profile."]);
        }

        await SeedMinimalValidationProfileAsync(cancellationToken);

        return ApplyDevelopmentSeedResult.Success(profile, [
            $"Validated strategic-map seed for civilization {SeedCivilizationId}.",
            $"System {SeedSystemId} includes planets {SeedOwnedPlanetId}, {SeedOuterPlanetId}, and {SeedIcePlanetId}.",
            "Fleet validation rows include stationed groups, one active own transfer, and owned-planet resource context."
        ]);
    }

    private async Task SeedMinimalValidationProfileAsync(CancellationToken cancellationToken)
    {
        if (!await dbContext.Set<SolarSystem>().AnyAsync(x => x.Id == SeedSystemId, cancellationToken))
        {
            dbContext.Set<SolarSystem>().Add(new SolarSystem(
                SeedSystemId,
                SeedGalaxyId,
                "Helios Gate",
                new GalaxyCoordinates(12, -4, 3),
                new Star(SeedStarId, SeedSystemId, "Helios Gate Star", StarType.YellowDwarf)));
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

        if (!await dbContext.PlanetResourceStockpiles.AnyAsync(x => x.PlanetId == SeedOwnedPlanetId, cancellationToken))
        {
            var stockpile = PlanetResourceStockpile.Create(SeedOwnedPlanetId);
            stockpile.Increase(ResourceType.Credits, 125);
            stockpile.Increase(ResourceType.Metal, 80);
            stockpile.Increase(ResourceType.Crystal, 35);
            stockpile.Increase(ResourceType.Gas, 20);
            dbContext.PlanetResourceStockpiles.Add(stockpile);
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
}

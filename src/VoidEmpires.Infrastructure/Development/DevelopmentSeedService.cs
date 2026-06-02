using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
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
            $"System {SeedSystemId} includes planets {SeedOwnedPlanetId}, {SeedOuterPlanetId}, and {SeedIcePlanetId}."
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
                "Frost Hollow",
                3,
                PlanetType.Ice,
                86));
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

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

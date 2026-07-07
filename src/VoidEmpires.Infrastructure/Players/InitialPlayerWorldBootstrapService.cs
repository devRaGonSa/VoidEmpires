using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Players;

public sealed class InitialPlayerWorldBootstrapService(VoidEmpiresDbContext dbContext) : IInitialPlayerWorldBootstrapService
{
    public async Task<InitialPlayerWorldBootstrapResult> CreateAsync(
        InitialPlayerWorldBootstrapRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return InitialPlayerWorldBootstrapResult.Failure(errors.ToArray());

        var userId = request.UserId.Trim();
        var displayName = request.DisplayName.Trim();
        var civilizationName = request.CivilizationName.Trim();
        var homePlanetName = string.IsNullOrWhiteSpace(request.HomePlanetName)
            ? $"{civilizationName} Prime"
            : request.HomePlanetName.Trim();
        var normalizedDisplayName = PlayerProfile.NormalizeLookupKey(displayName);
        var normalizedCivilizationName = PlayerProfile.NormalizeLookupKey(civilizationName);

        if (await dbContext.PlayerProfiles.AnyAsync(x => x.UserId == userId, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Player profile already exists for this user.");
        if (await dbContext.PlayerProfiles.AnyAsync(x => x.NormalizedDisplayName == normalizedDisplayName, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Display name is already in use.");
        if (await dbContext.Civilizations.AnyAsync(x => x.NormalizedName == normalizedCivilizationName, cancellationToken)) return InitialPlayerWorldBootstrapResult.Failure("Civilization name is already in use.");

        var galaxy = Galaxy.Create($"{civilizationName} Realm");
        var solarSystemId = Guid.NewGuid();
        var solarSystem = new SolarSystem(
            solarSystemId,
            galaxy.Id,
            $"{homePlanetName} System",
            new GalaxyCoordinates(0, 0, 0),
            Star.Create(solarSystemId, $"{homePlanetName} Star", StarType.YellowDwarf));
        var homePlanet = Planet.Create(solarSystemId, homePlanetName, 1, PlanetType.Terran, 118, PlanetColonizationStatus.Colonized);
        var profile = PlayerProfile.Create(userId, displayName);
        var civilization = Civilization.Create(profile.Id, civilizationName, CivilizationArchetype.Balanced, homePlanet.Id);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 220);
        stockpile.Increase(ResourceType.Metal, 320);
        stockpile.Increase(ResourceType.Crystal, 220);
        stockpile.Increase(ResourceType.Gas, 120);

        profile.AddCivilization(civilization);
        dbContext.Galaxies.Add(galaxy);
        dbContext.SolarSystems.Add(solarSystem);
        dbContext.Planets.Add(homePlanet);
        dbContext.PlayerProfiles.Add(profile);
        dbContext.PlanetOwnerships.Add(PlanetOwnership.Create(homePlanet.Id, civilization.Id));
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(homePlanet.Id, 18, 14, 6, 3));
        await dbContext.SaveChangesAsync(cancellationToken);

        return InitialPlayerWorldBootstrapResult.Success(
            userId,
            profile.Id,
            civilization.Id,
            homePlanet.Id,
            homePlanet.Name,
            new CreateStartingCivilizationResourceSnapshot(stockpile.Credits, stockpile.Metal, stockpile.Crystal, stockpile.Gas));
    }

    private static List<string> Validate(InitialPlayerWorldBootstrapRequest request)
    {
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(request.UserId)) errors.Add("User id is required.");
        if (string.IsNullOrWhiteSpace(request.DisplayName)) errors.Add("Display name is required.");
        if (string.IsNullOrWhiteSpace(request.CivilizationName)) errors.Add("Civilization name is required.");
        return errors;
    }
}

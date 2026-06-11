using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Players;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Players;

public sealed class StartingCivilizationService(VoidEmpiresDbContext dbContext) : IStartingCivilizationService
{
    private static readonly IReadOnlyList<string> Limitations =
    [
        "Development-only playable start.",
        "No authenticated session is created.",
        "Cockpit routes still require explicit civilizationId and planetId."
    ];

    public async Task<CreateStartingCivilizationResult> CreateAsync(
        CreateStartingCivilizationRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return CreateStartingCivilizationResult.Failure(validationErrors.ToArray());
        }

        var displayName = request.DisplayName.Trim();
        var civilizationName = request.CivilizationName.Trim();
        var homePlanetName = string.IsNullOrWhiteSpace(request.HomePlanetName)
            ? $"{civilizationName} Prime"
            : request.HomePlanetName.Trim();
        var userId = string.IsNullOrWhiteSpace(request.UserId)
            ? $"dev-start-{Guid.NewGuid():N}"
            : request.UserId.Trim();

        if (await dbContext.PlayerProfiles.AnyAsync(
                profile => profile.UserId == userId,
                cancellationToken))
        {
            return CreateStartingCivilizationResult.Failure("Player profile already exists for this user.");
        }

        if (await dbContext.PlayerProfiles.AnyAsync(
                profile => profile.DisplayName.ToLower() == displayName.ToLower(),
                cancellationToken))
        {
            return CreateStartingCivilizationResult.Failure("Display name is already in use.");
        }

        if (await dbContext.Civilizations.AnyAsync(
                civilization => civilization.Name.ToLower() == civilizationName.ToLower(),
                cancellationToken))
        {
            return CreateStartingCivilizationResult.Failure("Civilization name is already in use.");
        }

        var galaxy = Galaxy.Create($"{civilizationName} Sandbox");
        var solarSystemId = Guid.NewGuid();
        var systemName = $"{homePlanetName} System";
        var star = Star.Create(solarSystemId, $"{systemName} Star", StarType.YellowDwarf);
        var solarSystem = new SolarSystem(solarSystemId, galaxy.Id, systemName, new GalaxyCoordinates(0, 0, 0), star);
        var homePlanet = Planet.Create(solarSystemId, homePlanetName, 1, PlanetType.Terran, 118, PlanetColonizationStatus.Colonized);

        var profile = PlayerProfile.Create(userId, displayName);
        var civilization = Civilization.Create(profile.Id, civilizationName, request.Archetype, homePlanet.Id);
        profile.AddCivilization(civilization);

        var ownership = PlanetOwnership.Create(homePlanet.Id, civilization.Id);
        var stockpile = PlanetResourceStockpile.Create(homePlanet.Id);
        stockpile.Increase(ResourceType.Credits, 220);
        stockpile.Increase(ResourceType.Metal, 320);
        stockpile.Increase(ResourceType.Crystal, 220);
        stockpile.Increase(ResourceType.Gas, 120);

        dbContext.Galaxies.Add(galaxy);
        dbContext.Set<SolarSystem>().Add(solarSystem);
        dbContext.Planets.Add(homePlanet);
        dbContext.PlayerProfiles.Add(profile);
        dbContext.PlanetOwnerships.Add(ownership);
        dbContext.PlanetResourceStockpiles.Add(stockpile);
        dbContext.PlanetProductionProfiles.Add(PlanetProductionProfile.Create(homePlanet.Id, 18, 14, 6, 3));
        dbContext.Set<PlanetPopulationProfile>().Add(PlanetPopulationProfile.Create(homePlanet.Id, 2_000, 500, 100));
        dbContext.Set<PlanetBuildingCapacity>().Add(PlanetBuildingCapacity.Create(homePlanet.Id, 120));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(homePlanet.Id, BuildingType.CommandCenter, 4, 1));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(homePlanet.Id, BuildingType.HabitationDistrict, 3, 1));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(homePlanet.Id, BuildingType.Shipyard, 1, 1));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(homePlanet.Id, BuildingType.DefenseGrid, 1, 1));
        dbContext.Set<PlanetBuilding>().Add(PlanetBuilding.Create(homePlanet.Id, BuildingType.Barracks, 1, 1));

        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateStartingCivilizationResult.Success(
            userId,
            profile.Id,
            civilization.Id,
            homePlanet.Id,
            homePlanet.Name,
            solarSystem.Id,
            solarSystem.Name,
            new CreateStartingCivilizationResourceSnapshot(stockpile.Credits, stockpile.Metal, stockpile.Crystal, stockpile.Gas),
            Limitations);
    }

    private static List<string> Validate(CreateStartingCivilizationRequest request)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(request.DisplayName))
        {
            errors.Add("Display name is required.");
        }

        if (string.IsNullOrWhiteSpace(request.CivilizationName))
        {
            errors.Add("Civilization name is required.");
        }

        if (!string.IsNullOrWhiteSpace(request.HomePlanetName) && request.HomePlanetName.Trim().Length > 128)
        {
            errors.Add("Home planet name is too long.");
        }

        return errors;
    }
}

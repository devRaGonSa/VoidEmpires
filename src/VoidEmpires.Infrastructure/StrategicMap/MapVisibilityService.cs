using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class MapVisibilityService(VoidEmpiresDbContext dbContext) : IMapVisibilityService
{
    public async Task<GetMapVisibilityResult> GetAsync(
        GetMapVisibilityRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetMapVisibilityResult(request.CivilizationId, []);
        }

        var systems = await dbContext.Set<SolarSystem>()
            .AsNoTracking()
            .Include(x => x.Star)
            .Include(x => x.Planets)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var planetIds = systems.SelectMany(x => x.Planets).Select(x => x.Id).ToArray();
        var ownerships = await dbContext.Set<PlanetOwnership>()
            .AsNoTracking()
            .Where(x => planetIds.Contains(x.PlanetId) && x.Status == PlanetControlStatus.Active)
            .ToDictionaryAsync(x => x.PlanetId, cancellationToken);

        var ownedPlanetIds = ownerships.Values
            .Where(x => x.CivilizationId == request.CivilizationId)
            .Select(x => x.PlanetId)
            .ToHashSet();
        var knowledge = await dbContext.Set<ExplorationKnowledge>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .ToListAsync(cancellationToken);
        var knownSystemIds = knowledge.Select(x => x.SystemId).ToHashSet();
        var knownPlanetIds = knowledge.Where(x => x.PlanetId.HasValue).Select(x => x.PlanetId!.Value).ToHashSet();

        var resultSystems = systems
            .Select(system => CreateSystem(system, request.CivilizationId, ownedPlanetIds, ownerships, knownSystemIds, knownPlanetIds))
            .ToArray();

        return new GetMapVisibilityResult(request.CivilizationId, resultSystems);
    }

    private static MapSystemVisibilityDto CreateSystem(
        SolarSystem system,
        Guid civilizationId,
        IReadOnlySet<Guid> ownedPlanetIds,
        IReadOnlyDictionary<Guid, PlanetOwnership> ownerships,
        IReadOnlySet<Guid> knownSystemIds,
        IReadOnlySet<Guid> knownPlanetIds)
    {
        var hasOwnedPlanet = system.Planets.Any(x => ownedPlanetIds.Contains(x.Id));
        var isVisible = hasOwnedPlanet || knownSystemIds.Contains(system.Id);
        var visibilityLevel = isVisible ? MapVisibilityLevel.Visible : MapVisibilityLevel.Unknown;
        var reason = hasOwnedPlanet ? MapVisibilityReason.SystemContainsOwnedPlanet :
            isVisible ? MapVisibilityReason.ExploredSystem : MapVisibilityReason.NoKnownVisibilitySource;

        return new MapSystemVisibilityDto(
            system.Id,
            system.GalaxyId,
            isVisible ? system.Name : null,
            isVisible ? system.CoordinateX : null,
            isVisible ? system.CoordinateY : null,
            isVisible ? system.CoordinateZ : null,
            isVisible ? system.Star.StarType : null,
            visibilityLevel,
            reason,
            isVisible,
            ownedPlanetIds.Overlaps(system.Planets.Select(x => x.Id)),
            system.Planets
                .OrderBy(x => x.OrbitalSlot)
                .ThenBy(x => x.Id)
                .Select(planet => CreatePlanet(planet, civilizationId, hasOwnedPlanet, ownedPlanetIds, ownerships, knownPlanetIds))
                .ToArray());
    }

    private static MapPlanetVisibilityDto CreatePlanet(
        Planet planet,
        Guid civilizationId,
        bool systemContainsOwnedPlanet,
        IReadOnlySet<Guid> ownedPlanetIds,
        IReadOnlyDictionary<Guid, PlanetOwnership> ownerships,
        IReadOnlySet<Guid> knownPlanetIds)
    {
        var isOwned = ownedPlanetIds.Contains(planet.Id);
        var isKnown = knownPlanetIds.Contains(planet.Id);
        var isVisible = isOwned || systemContainsOwnedPlanet || isKnown;
        var visibilityLevel = isOwned
            ? MapVisibilityLevel.Owned
            : isVisible
                ? MapVisibilityLevel.Visible
                : MapVisibilityLevel.Unknown;
        var reason = isOwned ? MapVisibilityReason.OwnedPlanet :
            systemContainsOwnedPlanet ? MapVisibilityReason.SystemContainsOwnedPlanet :
            isKnown ? MapVisibilityReason.ExploredPlanet : MapVisibilityReason.NoKnownVisibilitySource;

        return new MapPlanetVisibilityDto(
            planet.Id,
            isVisible ? planet.Name : null,
            isVisible ? planet.PlanetType : null,
            isVisible ? planet.Size : null,
            isVisible ? planet.ColonizationStatus : null,
            isVisible ? planet.OrbitalSlot : null,
            visibilityLevel,
            reason,
            isVisible,
            isOwned,
            isOwned && ownerships.TryGetValue(planet.Id, out var ownership) ? civilizationId : null);
    }
}

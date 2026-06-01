using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class SensorProfileService(VoidEmpiresDbContext dbContext) : ISensorProfileService
{
    public async Task<GetSensorProfilesResult> GetAsync(
        GetSensorProfilesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetSensorProfilesResult(request.CivilizationId, []);
        }

        var planetProfiles = await dbContext.Set<PlanetOwnership>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == PlanetControlStatus.Active)
            .Join(
                dbContext.Set<Planet>().AsNoTracking(),
                ownership => ownership.PlanetId,
                planet => planet.Id,
                (_, planet) => planet)
            .OrderBy(x => x.Name)
            .ThenBy(x => x.Id)
            .Select(x => CreatePlanetProfile(x))
            .ToListAsync(cancellationToken);

        var orbitalGroupProfiles = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == OrbitalGroupStatus.Stationed)
            .OrderBy(x => x.CurrentPlanetId)
            .ThenBy(x => x.AssetType)
            .ThenBy(x => x.Id)
            .Select(x => CreateOrbitalGroupProfile(x))
            .ToListAsync(cancellationToken);

        return new GetSensorProfilesResult(request.CivilizationId, planetProfiles.Concat(orbitalGroupProfiles).ToArray());
    }

    private static SensorProfileDto CreatePlanetProfile(Planet planet)
    {
        var isColonized = planet.ColonizationStatus == PlanetColonizationStatus.Colonized;
        return new SensorProfileDto(planet.Id, SensorProfileSourceKind.Planet,
            isColonized ? SensorProfileClass.Orbital : SensorProfileClass.Basic, planet.Id, planet.SolarSystemId,
            null, null, isColonized ? 2 : 1, isColonized ? 20 : 10,
            "Derived read-only placeholder from active planet ownership and colonization status.");
    }

    private static SensorProfileDto CreateOrbitalGroupProfile(OrbitalGroup group)
    {
        var isScout = group.AssetType == SpaceAssetType.ScoutCraft;
        return new SensorProfileDto(group.Id, SensorProfileSourceKind.OrbitalGroup,
            isScout ? SensorProfileClass.Orbital : SensorProfileClass.Basic, group.CurrentPlanetId, null,
            group.Id, group.AssetType, isScout ? 2 : 1, isScout ? 15 : 5,
            "Derived read-only placeholder from stationed orbital group composition.");
    }
}

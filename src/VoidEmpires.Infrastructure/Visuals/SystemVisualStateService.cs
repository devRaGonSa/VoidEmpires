using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Visuals;

public sealed class SystemVisualStateService(
    VoidEmpiresDbContext dbContext,
    IPlanetVisualStateService planetVisualStateService) : ISystemVisualStateService
{
    public async Task<GetSystemVisualStateResult> GetAsync(
        GetSystemVisualStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.SystemId == Guid.Empty)
        {
            return GetSystemVisualStateResult.Failure("System id is required.");
        }

        var system = await dbContext.Set<SolarSystem>()
            .AsNoTracking()
            .Include(x => x.Star)
            .SingleOrDefaultAsync(x => x.Id == request.SystemId, cancellationToken);

        if (system is null)
        {
            return GetSystemVisualStateResult.Failure("System was not found.");
        }

        var planets = await dbContext.Set<Planet>()
            .AsNoTracking()
            .Where(x => x.SolarSystemId == request.SystemId)
            .OrderBy(x => x.OrbitalSlot)
            .Select(x => new PlanetLayoutSource(x.Id, x.OrbitalSlot, x.Size))
            .ToListAsync(cancellationToken);

        var visualPlanets = new List<PlanetVisualStateDto>();

        foreach (var planet in planets)
        {
            var result = await planetVisualStateService.GetAsync(new GetPlanetVisualStateRequest(planet.PlanetId), cancellationToken);

            if (!result.Succeeded || result.VisualState is null)
            {
                return GetSystemVisualStateResult.Failure(result.Errors.ToArray());
            }

            visualPlanets.Add(result.VisualState);
        }

        return GetSystemVisualStateResult.Success(new SystemVisualStateDto(
            system.Id,
            system.GalaxyId,
            system.Name,
            system.CoordinateX,
            system.CoordinateY,
            system.CoordinateZ,
            CreateStarVisualState(system.Star),
            planets.Select(CreateLayoutHint).ToList(),
            visualPlanets));
    }

    private static StarVisualStateDto CreateStarVisualState(Star star) =>
        new(star.Id, star.Name, star.StarType, GetStarVisualClass(star.StarType), GetStarLightIntensity(star.StarType));

    private static PlanetVisualLayoutHintDto CreateLayoutHint(PlanetLayoutSource planet)
    {
        var orbitRadius = 4f + planet.OrbitalSlot * 1.75f;
        var orbitAngle = NormalizeDegrees(planet.OrbitalSlot * 47f);
        var visualScale = Math.Clamp(planet.Size / 100f, 0.45f, 1.75f);

        return new PlanetVisualLayoutHintDto(planet.PlanetId, planet.OrbitalSlot, orbitRadius, orbitAngle, visualScale);
    }

    private static string GetStarVisualClass(StarType starType) => starType switch
    {
        StarType.RedDwarf => "red_dwarf",
        StarType.YellowDwarf => "yellow_dwarf",
        StarType.BlueGiant => "blue_giant",
        StarType.WhiteDwarf => "white_dwarf",
        StarType.NeutronStar => "neutron_star",
        _ => "unknown_star"
    };

    private static float GetStarLightIntensity(StarType starType) => starType switch
    {
        StarType.RedDwarf => 0.55f,
        StarType.YellowDwarf => 0.75f,
        StarType.BlueGiant => 1.00f,
        StarType.WhiteDwarf => 0.85f,
        StarType.NeutronStar => 0.95f,
        _ => 0.65f
    };

    private static float NormalizeDegrees(float value)
    {
        var normalized = value % 360f;
        return normalized < 0 ? normalized + 360f : normalized;
    }

    private sealed record PlanetLayoutSource(Guid PlanetId, int OrbitalSlot, int Size);
}

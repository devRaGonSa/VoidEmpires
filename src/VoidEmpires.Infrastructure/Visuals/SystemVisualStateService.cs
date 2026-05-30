using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Fleets;
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

        var planetIds = planets.Select(x => x.PlanetId).ToHashSet();
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

        var orbitalGroupMarkers = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => planetIds.Contains(x.CurrentPlanetId))
            .OrderBy(x => x.CurrentPlanetId)
            .ThenBy(x => x.AssetType)
            .ThenBy(x => x.Id)
            .Select(x => new OrbitalGroupVisualMarkerDto(
                x.Id,
                x.CivilizationId,
                x.OriginPlanetId,
                x.CurrentPlanetId,
                x.AssetType,
                x.Quantity,
                x.Status,
                CreateOrbitalGroupMarkerScale(x.Quantity),
                CreateOrbitalGroupMarkerKind(x.Status)))
            .ToListAsync(cancellationToken);

        var transferOverlays = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled &&
                (planetIds.Contains(x.OriginPlanetId) || planetIds.Contains(x.DestinationPlanetId)))
            .OrderBy(x => x.ArrivalAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new OrbitalTransferVisualOverlayDto(
                x.Id,
                x.CivilizationId,
                x.OrbitalGroupId,
                x.OriginPlanetId,
                x.DestinationPlanetId,
                x.Status,
                x.DepartureAtUtc,
                x.ArrivalAtUtc,
                CreateTransferProgress(x.DepartureAtUtc, x.ArrivalAtUtc),
                CreateTransferOverlayKind(x.Status)))
            .ToListAsync(cancellationToken);

        return GetSystemVisualStateResult.Success(new SystemVisualStateDto(
            system.Id,
            system.GalaxyId,
            system.Name,
            system.CoordinateX,
            system.CoordinateY,
            system.CoordinateZ,
            CreateStarVisualState(system.Star),
            planets.Select(CreateLayoutHint).ToList(),
            orbitalGroupMarkers,
            transferOverlays,
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

    private static float CreateOrbitalGroupMarkerScale(int quantity) =>
        Math.Clamp(quantity / 25f, 0.5f, 4f);

    private static string CreateOrbitalGroupMarkerKind(OrbitalGroupStatus status) => status switch
    {
        OrbitalGroupStatus.Stationed => "stationed_orbital_group",
        OrbitalGroupStatus.Reserved => "reserved_orbital_group",
        OrbitalGroupStatus.Decommissioned => "decommissioned_orbital_group",
        _ => "unknown_orbital_group"
    };

    private static float CreateTransferProgress(DateTime departureAtUtc, DateTime arrivalAtUtc)
    {
        var now = DateTime.UtcNow;

        if (now <= departureAtUtc)
        {
            return 0f;
        }

        if (now >= arrivalAtUtc)
        {
            return 1f;
        }

        var total = arrivalAtUtc - departureAtUtc;
        var elapsed = now - departureAtUtc;
        return Math.Clamp((float)(elapsed.TotalSeconds / total.TotalSeconds), 0f, 1f);
    }

    private static string CreateTransferOverlayKind(OrbitalTransferStatus status) => status switch
    {
        OrbitalTransferStatus.Planned => "planned_transfer_route",
        OrbitalTransferStatus.InTransit => "active_transfer_route",
        _ => "unknown_transfer_route"
    };

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

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class StrategicMapService(
    VoidEmpiresDbContext dbContext,
    ISystemVisualStateService systemVisualStateService) : IStrategicMapService
{
    public async Task<GetStrategicMapResult> GetAsync(
        GetStrategicMapRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetStrategicMapResult(request.CivilizationId, [], CreateRouteFuelNotes());
        }

        var ownedPlanetIds = await dbContext.Set<PlanetOwnership>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .Select(x => x.PlanetId)
            .ToListAsync(cancellationToken);

        var activeTransfers = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId &&
                x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled)
            .OrderBy(x => x.ArrivalAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var relevantPlanetIds = ownedPlanetIds
            .Concat(activeTransfers.SelectMany(x => new[] { x.OriginPlanetId, x.DestinationPlanetId }))
            .Distinct()
            .ToArray();

        if (relevantPlanetIds.Length == 0)
        {
            return new GetStrategicMapResult(request.CivilizationId, [], CreateRouteFuelNotes());
        }

        var systemIds = await dbContext.Set<Planet>()
            .AsNoTracking()
            .Where(x => relevantPlanetIds.Contains(x.Id))
            .Select(x => x.SolarSystemId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var systems = new List<StrategicMapSystemDto>();
        foreach (var systemId in systemIds)
        {
            var visualResult = await systemVisualStateService.GetAsync(new GetSystemVisualStateRequest(systemId), cancellationToken);
            if (!visualResult.Succeeded || visualResult.VisualState is null)
            {
                continue;
            }

            systems.Add(CreateSystem(visualResult.VisualState, request.CivilizationId, activeTransfers));
        }

        return new GetStrategicMapResult(
            request.CivilizationId,
            systems.OrderBy(x => x.SystemName).ThenBy(x => x.SystemId).ToArray(),
            CreateRouteFuelNotes());
    }

    private static StrategicMapSystemDto CreateSystem(
        SystemVisualStateDto visualState,
        Guid civilizationId,
        IReadOnlyCollection<OrbitalTransfer> activeTransfers)
    {
        var layoutByPlanetId = visualState.LayoutHints.ToDictionary(x => x.PlanetId);
        var transfersById = activeTransfers.ToDictionary(x => x.Id);

        return new StrategicMapSystemDto(
            visualState.SystemId,
            visualState.GalaxyId,
            visualState.SystemName,
            visualState.CoordinateX,
            visualState.CoordinateY,
            visualState.CoordinateZ,
            visualState.Star.StarType,
            visualState.Planets.Select(x => CreatePlanet(x, civilizationId, layoutByPlanetId)).ToArray(),
            visualState.OrbitalGroupMarkers
                .Where(x => x.CivilizationId == civilizationId)
                .Select(x => new StrategicMapFleetPresenceDto(
                    x.OrbitalGroupId,
                    x.CurrentPlanetId,
                    x.AssetType,
                    x.Quantity,
                    x.Status,
                    x.MarkerKind))
                .ToArray(),
            visualState.TransferOverlays
                .Where(x => x.CivilizationId == civilizationId)
                .Select(x =>
                {
                    transfersById.TryGetValue(x.TransferId, out var transfer);
                    return new StrategicMapTransferOverlayDto(
                        x.TransferId,
                        x.OrbitalGroupId,
                        x.OriginPlanetId,
                        x.DestinationPlanetId,
                        transfer?.AbstractDistanceUnits ?? 0,
                        x.Status,
                        x.DepartureAtUtc,
                        x.ArrivalAtUtc,
                        x.Progress,
                        x.OverlayKind);
                })
                .ToArray());
    }

    private static StrategicMapPlanetDto CreatePlanet(
        PlanetVisualStateDto visualState,
        Guid civilizationId,
        IReadOnlyDictionary<Guid, PlanetVisualLayoutHintDto> layoutByPlanetId)
    {
        layoutByPlanetId.TryGetValue(visualState.PlanetId, out var layout);
        var isOwnedByRequester = visualState.CivilizationId == civilizationId;
        var exposeVisualDetail = !visualState.IsOwned || isOwnedByRequester;

        return new StrategicMapPlanetDto(
            visualState.PlanetId,
            visualState.PlanetName,
            visualState.PlanetType,
            visualState.Size,
            visualState.ColonizationStatus,
            isOwnedByRequester,
            isOwnedByRequester ? civilizationId : null,
            layout?.OrbitalSlot ?? 0,
            layout?.OrbitRadius ?? 0f,
            layout?.OrbitAngleDegrees ?? 0f,
            layout?.VisualScale ?? 0f,
            exposeVisualDetail ? visualState.ColonizationIntensity : 0f,
            exposeVisualDetail ? visualState.UrbanIntensity : 0f,
            exposeVisualDetail ? visualState.IndustrialIntensity : 0f,
            exposeVisualDetail ? visualState.MilitaryIntensity : 0f,
            exposeVisualDetail ? visualState.OrbitalPresenceIntensity : 0f);
    }

    private static IReadOnlyList<StrategicMapRouteFuelNoteDto> CreateRouteFuelNotes() =>
        [
            new(
                "fleet.travel.estimate",
                true,
                OrbitalFuelReadinessPolicy.PlaceholderDerived,
                "Strategic map route/fuel data is capability metadata only; concrete route profiles and fuel readiness require a destinationPlanetId.")
        ];
}

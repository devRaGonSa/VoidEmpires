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
    ISystemVisualStateService systemVisualStateService,
    IMapVisibilityService mapVisibilityService) : IStrategicMapService
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
        var visibility = await mapVisibilityService
            .GetAsync(new GetMapVisibilityRequest(request.CivilizationId), cancellationToken);
        var visibilityBySystemId = visibility.Systems.ToDictionary(x => x.SystemId);

        var systems = new List<StrategicMapSystemDto>();
        foreach (var systemId in systemIds)
        {
            var visualResult = await systemVisualStateService.GetAsync(new GetSystemVisualStateRequest(systemId), cancellationToken);
            if (!visualResult.Succeeded || visualResult.VisualState is null)
            {
                continue;
            }

            visibilityBySystemId.TryGetValue(systemId, out var systemVisibility);
            systems.Add(CreateSystem(visualResult.VisualState, request.CivilizationId, activeTransfers, systemVisibility));
        }

        return new GetStrategicMapResult(
            request.CivilizationId,
            systems.OrderBy(x => x.SystemName).ThenBy(x => x.SystemId).ToArray(),
            CreateRouteFuelNotes());
    }

    private static StrategicMapSystemDto CreateSystem(
        SystemVisualStateDto visualState,
        Guid civilizationId,
        IReadOnlyCollection<OrbitalTransfer> activeTransfers,
        MapSystemVisibilityDto? visibility)
    {
        var layoutByPlanetId = visualState.LayoutHints.ToDictionary(x => x.PlanetId);
        var transfersById = activeTransfers.ToDictionary(x => x.Id);
        var visibilityByPlanetId = visibility?.Planets.ToDictionary(x => x.PlanetId) ?? [];
        var fleetPresence = visualState.OrbitalGroupMarkers
            .Where(x => x.CivilizationId == civilizationId)
            .Select(x => new StrategicMapFleetPresenceDto(
                x.OrbitalGroupId,
                x.CurrentPlanetId,
                x.AssetType,
                x.Quantity,
                x.Status,
                x.MarkerKind))
            .ToArray();
        var hasFleetContext = fleetPresence.Length > 0;

        return new StrategicMapSystemDto(
            visualState.SystemId,
            visualState.GalaxyId,
            visualState.SystemName,
            visualState.CoordinateX,
            visualState.CoordinateY,
            visualState.CoordinateZ,
            visualState.Star.StarType,
            visibility?.VisibilityLevel ?? MapVisibilityLevel.Unknown,
            visibility?.VisibilityReason ?? MapVisibilityReason.NoKnownVisibilitySource,
            visibility?.IsVisible ?? false,
            visibility?.IsOwnedByRequestingCivilization ?? false,
            CreateSystemCommands(visibility),
            visualState.Planets.Select(x =>
            {
                visibilityByPlanetId.TryGetValue(x.PlanetId, out var planetVisibility);
                return CreatePlanet(x, civilizationId, layoutByPlanetId, planetVisibility, hasFleetContext);
            }).ToArray(),
            fleetPresence,
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
        IReadOnlyDictionary<Guid, PlanetVisualLayoutHintDto> layoutByPlanetId,
        MapPlanetVisibilityDto? visibility,
        bool hasFleetContext)
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
            visibility?.VisibilityLevel ?? MapVisibilityLevel.Unknown,
            visibility?.VisibilityReason ?? MapVisibilityReason.NoKnownVisibilitySource,
            visibility?.IsVisible ?? false,
            CreatePlanetCommands(visibility, hasFleetContext),
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

    private static IReadOnlyList<StrategicMapCommandAvailabilityDto> CreateSystemCommands(
        MapSystemVisibilityDto? visibility)
    {
        var isVisible = visibility?.IsVisible == true;
        return
        [
            Command(
                "strategicMap.system.view",
                isVisible,
                isVisible ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.NotVisible,
                isVisible ? "System can be viewed from the current visibility projection." : "System is not visible to the requesting civilization.")
        ];
    }

    private static IReadOnlyList<StrategicMapCommandAvailabilityDto> CreatePlanetCommands(
        MapPlanetVisibilityDto? visibility,
        bool hasFleetContext)
    {
        var isVisible = visibility?.IsVisible == true;
        var visibilityBlockReason = visibility?.VisibilityLevel == MapVisibilityLevel.Unknown
            ? StrategicMapCommandBlockReason.Unknown
            : StrategicMapCommandBlockReason.NotVisible;
        var travelAvailable = isVisible && hasFleetContext;

        return
        [
            Command(
                "strategicMap.planet.viewDetail",
                isVisible,
                isVisible ? StrategicMapCommandBlockReason.None : visibilityBlockReason,
                isVisible ? "Planet detail can be viewed from the current visibility projection." : "Planet is unknown or not visible."),
            Command(
                "fleet.travel.estimate",
                travelAvailable,
                GetFleetBlockReason(isVisible, hasFleetContext, visibilityBlockReason),
                travelAvailable ? "Capability hint only; destination-specific estimate validation still applies." : "Requires a visible destination and a requesting-civilization fleet context."),
            Command(
                "fleet.transfer.create",
                travelAvailable,
                travelAvailable ? StrategicMapCommandBlockReason.None : GetFleetBlockReason(isVisible, hasFleetContext, visibilityBlockReason),
                travelAvailable ? "Capability hint only; transfer creation must use the existing fleet command path." : "Requires a visible destination and a requesting-civilization fleet context.")
        ];
    }

    private static StrategicMapCommandBlockReason GetFleetBlockReason(
        bool isVisible,
        bool hasFleetContext,
        StrategicMapCommandBlockReason visibilityBlockReason)
    {
        if (!isVisible)
        {
            return visibilityBlockReason;
        }

        return hasFleetContext
            ? StrategicMapCommandBlockReason.None
            : StrategicMapCommandBlockReason.NoFleetContext;
    }

    private static StrategicMapCommandAvailabilityDto Command(
        string actionKey,
        bool isAvailable,
        StrategicMapCommandBlockReason blockReason,
        string note) => new(actionKey, isAvailable, blockReason, note);

    private static IReadOnlyList<StrategicMapRouteFuelNoteDto> CreateRouteFuelNotes() =>
        [
            new(
                "fleet.travel.estimate",
                true,
                OrbitalFuelReadinessPolicy.PlaceholderDerived,
                "Strategic map route/fuel data is capability metadata only; concrete route profiles and fuel readiness require a destinationPlanetId.")
        ];
}

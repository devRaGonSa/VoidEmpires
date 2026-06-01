using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class StrategicMapService(
    VoidEmpiresDbContext dbContext,
    ISystemVisualStateService systemVisualStateService,
    IMapVisibilityService mapVisibilityService,
    ISensorProfileService? sensorProfileService = null,
    IDetectionCoverageService? detectionCoverageService = null) : IStrategicMapService
{
    public async Task<GetStrategicMapResult> GetAsync(
        GetStrategicMapRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetStrategicMapResult(request.CivilizationId, [], CreateRouteFuelNotes(), CreateSensorNotes(), CreateDetectionNotes());
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
        var knownSystemIds = await dbContext.Set<ExplorationKnowledge>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .Select(x => x.SystemId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var relevantPlanetIds = ownedPlanetIds
            .Concat(activeTransfers.SelectMany(x => new[] { x.OriginPlanetId, x.DestinationPlanetId }))
            .Distinct()
            .ToArray();

        if (relevantPlanetIds.Length == 0 && knownSystemIds.Count == 0)
        {
            return new GetStrategicMapResult(request.CivilizationId, [], CreateRouteFuelNotes(), CreateSensorNotes(), CreateDetectionNotes());
        }

        var systemIds = await dbContext.Set<Planet>()
            .AsNoTracking()
            .Where(x => relevantPlanetIds.Contains(x.Id))
            .Select(x => x.SolarSystemId)
            .Distinct()
            .ToListAsync(cancellationToken);
        systemIds = systemIds.Concat(knownSystemIds).Distinct().ToList();
        var visibility = await mapVisibilityService
            .GetAsync(new GetMapVisibilityRequest(request.CivilizationId), cancellationToken);
        var visibilityBySystemId = visibility.Systems.ToDictionary(x => x.SystemId);
        var resolvedSensorProfileService = sensorProfileService ?? new SensorProfileService(dbContext);
        var sensorProfiles = (await resolvedSensorProfileService
            .GetAsync(new GetSensorProfilesRequest(request.CivilizationId), cancellationToken))
            .Profiles;
        var detectionCoverage = (await (detectionCoverageService ?? new DetectionCoverageService(dbContext, resolvedSensorProfileService))
            .GetAsync(new GetDetectionCoverageRequest(request.CivilizationId), cancellationToken))
            .Coverages;

        var visualStates = new List<SystemVisualStateDto>();
        foreach (var systemId in systemIds)
        {
            var visualResult = await systemVisualStateService.GetAsync(new GetSystemVisualStateRequest(systemId), cancellationToken);
            if (!visualResult.Succeeded || visualResult.VisualState is null)
            {
                continue;
            }

            visualStates.Add(visualResult.VisualState);
        }
        var hasMapFleetContext = visualStates
            .SelectMany(x => x.OrbitalGroupMarkers)
            .Any(x => x.CivilizationId == request.CivilizationId);
        var systems = visualStates.Select(visualState =>
        {
            visibilityBySystemId.TryGetValue(visualState.SystemId, out var systemVisibility);
            return CreateSystem(visualState, request.CivilizationId, activeTransfers, systemVisibility, hasMapFleetContext, sensorProfiles, detectionCoverage);
        }).ToArray();

        return new GetStrategicMapResult(
            request.CivilizationId,
            systems.OrderBy(x => x.SystemName).ThenBy(x => x.SystemId).ToArray(),
            CreateRouteFuelNotes(),
            CreateSensorNotes(),
            CreateDetectionNotes());
    }

    private static StrategicMapSystemDto CreateSystem(
        SystemVisualStateDto visualState,
        Guid civilizationId,
        IReadOnlyCollection<OrbitalTransfer> activeTransfers,
        MapSystemVisibilityDto? visibility,
        bool hasMapFleetContext,
        IReadOnlyList<SensorProfileDto> sensorProfiles,
        IReadOnlyList<DetectionCoverageDto> detectionCoverage)
    {
        var layoutByPlanetId = visualState.LayoutHints.ToDictionary(x => x.PlanetId);
        var transfersById = activeTransfers.ToDictionary(x => x.Id);
        var visibilityByPlanetId = visibility?.Planets.ToDictionary(x => x.PlanetId) ?? [];
        var sensorsByPlanetId = sensorProfiles
            .Where(x => x.PlanetId.HasValue)
            .GroupBy(x => x.PlanetId!.Value)
            .ToDictionary(x => x.Key, x => x.Select(CreateSensorSummary).ToArray());
        var sensorsByGroupId = sensorProfiles
            .Where(x => x.OrbitalGroupId.HasValue)
            .ToDictionary(x => x.OrbitalGroupId!.Value, CreateSensorSummary);
        var fleetPresence = visualState.OrbitalGroupMarkers
            .Where(x => x.CivilizationId == civilizationId)
            .Select(x => new StrategicMapFleetPresenceDto(
                x.OrbitalGroupId,
                x.CurrentPlanetId,
                x.AssetType,
                x.Quantity,
                x.Status,
                x.MarkerKind,
                sensorsByGroupId.GetValueOrDefault(x.OrbitalGroupId)))
            .ToArray();
        var visibilityLevel = visibility?.VisibilityLevel ?? MapVisibilityLevel.Unknown;
        var explorationPreview = ExplorationActionPreviewService.CreatePreview(visibilityLevel);
        var systemSensorProfiles = visibility?.IsVisible == true
            ? sensorProfiles.Where(x => x.SolarSystemId == visualState.SystemId).Select(CreateSensorSummary).ToArray()
            : [];
        var systemDetectionCoverage = visibility?.IsVisible == true
            ? detectionCoverage.Where(x => x.SourceSystemId == visualState.SystemId).Select(CreateDetectionSummary).ToArray()
            : [];

        return new StrategicMapSystemDto(
            visualState.SystemId,
            visualState.GalaxyId,
            visualState.SystemName,
            visualState.CoordinateX,
            visualState.CoordinateY,
            visualState.CoordinateZ,
            visualState.Star.StarType,
            visibilityLevel,
            visibility?.VisibilityReason ?? MapVisibilityReason.NoKnownVisibilitySource,
            visibility?.IsVisible ?? false,
            visibility?.IsOwnedByRequestingCivilization ?? false,
            explorationPreview,
            CreateSystemCommands(visibility, explorationPreview),
            visualState.Planets.Select(x =>
            {
                visibilityByPlanetId.TryGetValue(x.PlanetId, out var planetVisibility);
                sensorsByPlanetId.TryGetValue(x.PlanetId, out var planetSensorProfiles);
                return CreatePlanet(
                    x,
                    civilizationId,
                    layoutByPlanetId,
                    planetVisibility,
                    hasMapFleetContext,
                    planetSensorProfiles ?? [],
                    visibility?.IsVisible == true
                        ? detectionCoverage.Where(coverage => coverage.SourcePlanetId == x.PlanetId).Select(CreateDetectionSummary).ToArray()
                        : []);
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
                .ToArray(),
            systemSensorProfiles,
            systemDetectionCoverage);
    }

    private static StrategicMapPlanetDto CreatePlanet(
        PlanetVisualStateDto visualState,
        Guid civilizationId,
        IReadOnlyDictionary<Guid, PlanetVisualLayoutHintDto> layoutByPlanetId,
        MapPlanetVisibilityDto? visibility,
        bool hasFleetContext,
        IReadOnlyList<StrategicMapSensorProfileSummaryDto> sensorProfiles,
        IReadOnlyList<StrategicMapDetectionCoverageSummaryDto> detectionCoverage)
    {
        layoutByPlanetId.TryGetValue(visualState.PlanetId, out var layout);
        var isOwnedByRequester = visualState.CivilizationId == civilizationId;
        var visibilityLevel = visibility?.VisibilityLevel ?? MapVisibilityLevel.Unknown;
        var exposePlanetDetail = visibility?.IsVisible == true;
        var exposeVisualDetail = exposePlanetDetail && (!visualState.IsOwned || isOwnedByRequester);
        var explorationPreview = ExplorationActionPreviewService.CreatePreview(visibilityLevel);

        return new StrategicMapPlanetDto(
            visualState.PlanetId,
            exposePlanetDetail ? visualState.PlanetName : null,
            exposePlanetDetail ? visualState.PlanetType : null,
            exposePlanetDetail ? visualState.Size : null,
            exposePlanetDetail ? visualState.ColonizationStatus : null,
            isOwnedByRequester,
            visibilityLevel,
            visibility?.VisibilityReason ?? MapVisibilityReason.NoKnownVisibilitySource,
            visibility?.IsVisible ?? false,
            explorationPreview,
            CreatePlanetCommands(visibility, hasFleetContext, explorationPreview),
            isOwnedByRequester ? civilizationId : null,
            exposePlanetDetail ? layout?.OrbitalSlot : null,
            exposePlanetDetail ? layout?.OrbitRadius : null,
            exposePlanetDetail ? layout?.OrbitAngleDegrees : null,
            exposePlanetDetail ? layout?.VisualScale : null,
            CreateIntensity(exposePlanetDetail, exposeVisualDetail, visualState.ColonizationIntensity),
            CreateIntensity(exposePlanetDetail, exposeVisualDetail, visualState.UrbanIntensity),
            CreateIntensity(exposePlanetDetail, exposeVisualDetail, visualState.IndustrialIntensity),
            CreateIntensity(exposePlanetDetail, exposeVisualDetail, visualState.MilitaryIntensity),
            CreateIntensity(exposePlanetDetail, exposeVisualDetail, visualState.OrbitalPresenceIntensity),
            visibility?.IsVisible == true ? sensorProfiles : [],
            visibility?.IsVisible == true ? detectionCoverage : []);
    }

    private static float? CreateIntensity(bool exposePlanetDetail, bool exposeVisualDetail, float value) =>
        exposePlanetDetail ? exposeVisualDetail ? value : 0f : null;

    private static IReadOnlyList<StrategicMapCommandAvailabilityDto> CreateSystemCommands(
        MapSystemVisibilityDto? visibility,
        StrategicMapExplorationPreviewDto explorationPreview)
    {
        var isVisible = visibility?.IsVisible == true;
        return
        [
            Command(
                "strategicMap.system.view",
                isVisible,
                isVisible ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.NotVisible,
                isVisible ? "System can be viewed from the current visibility projection." : "System is not visible to the requesting civilization."),
            Command(
                "exploration.preview",
                explorationPreview.CanPreviewExploration,
                explorationPreview.CanPreviewExploration ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.ExplorationPreviewUnavailable,
                explorationPreview.Note),
            Command(
                "exploration.mission.create",
                explorationPreview.CanPreviewExploration,
                explorationPreview.CanPreviewExploration ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.ExplorationPreviewUnavailable,
                explorationPreview.CanPreviewExploration ? "Capability hint only; mission creation must use the exploration mission create endpoint and validation." : explorationPreview.Note)
        ];
    }

    private static IReadOnlyList<StrategicMapCommandAvailabilityDto> CreatePlanetCommands(
        MapPlanetVisibilityDto? visibility,
        bool hasFleetContext,
        StrategicMapExplorationPreviewDto explorationPreview)
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
                "exploration.preview",
                explorationPreview.CanPreviewExploration,
                explorationPreview.CanPreviewExploration ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.ExplorationPreviewUnavailable,
                explorationPreview.Note),
            Command(
                "exploration.mission.create",
                explorationPreview.CanPreviewExploration,
                explorationPreview.CanPreviewExploration ? StrategicMapCommandBlockReason.None : StrategicMapCommandBlockReason.ExplorationPreviewUnavailable,
                explorationPreview.CanPreviewExploration ? "Capability hint only; mission creation must use the exploration mission create endpoint and validation." : explorationPreview.Note),
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

    private static StrategicMapSensorProfileSummaryDto CreateSensorSummary(SensorProfileDto profile) =>
        new(
            profile.SourceId,
            profile.SourceKind,
            profile.SensorClass,
            profile.DetectionRangeTier,
            profile.ScanStrength,
            profile.Note);

    private static StrategicMapDetectionCoverageSummaryDto CreateDetectionSummary(DetectionCoverageDto coverage) =>
        new(
            coverage.SourceId,
            coverage.SourceKind,
            coverage.CoverageClass,
            coverage.DetectionRangeTier,
            coverage.CoverageConfidencePercent,
            coverage.Note);

    private static IReadOnlyList<StrategicMapRouteFuelNoteDto> CreateRouteFuelNotes() =>
        [
            new(
                "fleet.travel.estimate",
                true,
                OrbitalFuelReadinessPolicy.PlaceholderDerived,
                "Strategic map route/fuel data is capability metadata only; concrete route profiles and fuel readiness require a destinationPlanetId.")
        ];

    private static IReadOnlyList<StrategicMapSensorNoteDto> CreateSensorNotes() =>
        [
            new("Sensor profiles are derived metadata only; they do not reveal visibility, scan targets, or change command validation.")
        ];

    private static IReadOnlyList<StrategicMapDetectionNoteDto> CreateDetectionNotes() =>
        [
            new("Detection coverage is derived metadata only; it does not reveal unknown systems or planets, change visibility, or alter command validation.")
        ];
}

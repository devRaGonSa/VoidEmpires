using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetStrategicMapResult(
    Guid CivilizationId,
    IReadOnlyList<StrategicMapSystemDto> Systems,
    IReadOnlyList<StrategicMapRouteFuelNoteDto> RouteFuelNotes,
    IReadOnlyList<StrategicMapSensorNoteDto> SensorNotes = null!,
    IReadOnlyList<StrategicMapDetectionNoteDto> DetectionNotes = null!,
    IReadOnlyList<StrategicMapInterceptionNoteDto> InterceptionNotes = null!);

public sealed record StrategicMapSystemDto(
    Guid SystemId,
    Guid GalaxyId,
    string SystemName,
    int CoordinateX,
    int CoordinateY,
    int CoordinateZ,
    StarType StarType,
    MapVisibilityLevel VisibilityLevel,
    MapVisibilityReason VisibilityReason,
    bool IsVisible,
    bool IsOwnedByRequestingCivilization,
    StrategicMapExplorationPreviewDto ExplorationPreview,
    IReadOnlyList<StrategicMapCommandAvailabilityDto> Commands,
    IReadOnlyList<StrategicMapPlanetDto> Planets,
    IReadOnlyList<StrategicMapFleetPresenceDto> FleetPresence,
    IReadOnlyList<StrategicMapTransferOverlayDto> TransferOverlays,
    IReadOnlyList<StrategicMapSensorProfileSummaryDto> SensorProfiles = null!,
    IReadOnlyList<StrategicMapDetectionCoverageSummaryDto> DetectionCoverage = null!);

public sealed record StrategicMapPlanetDto(
    Guid PlanetId,
    string? PlanetName,
    PlanetType? PlanetType,
    int? Size,
    PlanetColonizationStatus? ColonizationStatus,
    bool IsOwnedByRequestingCivilization,
    MapVisibilityLevel VisibilityLevel,
    MapVisibilityReason VisibilityReason,
    bool IsVisible,
    StrategicMapExplorationPreviewDto ExplorationPreview,
    IReadOnlyList<StrategicMapCommandAvailabilityDto> Commands,
    Guid? CivilizationId,
    int? OrbitalSlot,
    float? OrbitRadius,
    float? OrbitAngleDegrees,
    float? VisualScale,
    float? ColonizationIntensity,
    float? UrbanIntensity,
    float? IndustrialIntensity,
    float? MilitaryIntensity,
    float? OrbitalPresenceIntensity,
    IReadOnlyList<StrategicMapSensorProfileSummaryDto> SensorProfiles = null!,
    IReadOnlyList<StrategicMapDetectionCoverageSummaryDto> DetectionCoverage = null!);

public sealed record StrategicMapFleetPresenceDto(
    Guid OrbitalGroupId,
    Guid PlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    string MarkerKind,
    StrategicMapSensorProfileSummaryDto? SensorProfile = null);

public sealed record StrategicMapTransferOverlayDto(
    Guid TransferId,
    Guid OrbitalGroupId,
    Guid OriginPlanetId,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    OrbitalTransferStatus Status,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    float Progress,
    string OverlayKind,
    InterceptionReadinessSummaryDto? InterceptionReadiness = null);

public sealed record StrategicMapRouteFuelNoteDto(
    string ActionKey,
    bool RequiresDestination,
    OrbitalFuelReadinessPolicy FuelReadinessPolicy,
    string Note);

public sealed record StrategicMapSensorNoteDto(string Note);

public sealed record StrategicMapDetectionNoteDto(string Note);

public sealed record StrategicMapInterceptionNoteDto(string Note);

public sealed record StrategicMapSensorProfileSummaryDto(
    Guid SourceId,
    SensorProfileSourceKind SourceKind,
    SensorProfileClass SensorClass,
    int DetectionRangeTier,
    int ScanStrength,
    string Note);

public sealed record StrategicMapDetectionCoverageSummaryDto(
    Guid SourceId,
    DetectionCoverageSourceKind SourceKind,
    DetectionCoverageClass CoverageClass,
    int DetectionRangeTier,
    int CoverageConfidencePercent,
    string Note);

public sealed record StrategicMapExplorationPreviewDto(
    bool CanPreviewExploration,
    ExplorationActionBlockReason BlockReason,
    string Note);

public sealed record StrategicMapCommandAvailabilityDto(
    string ActionKey,
    bool IsAvailable,
    StrategicMapCommandBlockReason BlockReason,
    string Note);

public enum StrategicMapCommandBlockReason
{
    None = 0,
    Unknown = 1,
    NotVisible = 2,
    NoFleetContext = 3,
    ExplorationPreviewUnavailable = 4
}

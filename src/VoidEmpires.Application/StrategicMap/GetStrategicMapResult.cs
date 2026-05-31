using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetStrategicMapResult(
    Guid CivilizationId,
    IReadOnlyList<StrategicMapSystemDto> Systems,
    IReadOnlyList<StrategicMapRouteFuelNoteDto> RouteFuelNotes);

public sealed record StrategicMapSystemDto(
    Guid SystemId,
    Guid GalaxyId,
    string SystemName,
    int CoordinateX,
    int CoordinateY,
    int CoordinateZ,
    StarType StarType,
    IReadOnlyList<StrategicMapPlanetDto> Planets,
    IReadOnlyList<StrategicMapFleetPresenceDto> FleetPresence,
    IReadOnlyList<StrategicMapTransferOverlayDto> TransferOverlays);

public sealed record StrategicMapPlanetDto(
    Guid PlanetId,
    string PlanetName,
    PlanetType PlanetType,
    int Size,
    PlanetColonizationStatus ColonizationStatus,
    bool IsOwnedByRequestingCivilization,
    Guid? CivilizationId,
    int OrbitalSlot,
    float OrbitRadius,
    float OrbitAngleDegrees,
    float VisualScale,
    float ColonizationIntensity,
    float UrbanIntensity,
    float IndustrialIntensity,
    float MilitaryIntensity,
    float OrbitalPresenceIntensity);

public sealed record StrategicMapFleetPresenceDto(
    Guid OrbitalGroupId,
    Guid PlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    string MarkerKind);

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
    string OverlayKind);

public sealed record StrategicMapRouteFuelNoteDto(
    string ActionKey,
    bool RequiresDestination,
    OrbitalFuelReadinessPolicy FuelReadinessPolicy,
    string Note);

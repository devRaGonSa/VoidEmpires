using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Visuals;

public sealed record SystemVisualStateDto(
    Guid SystemId,
    Guid GalaxyId,
    string SystemName,
    int CoordinateX,
    int CoordinateY,
    int CoordinateZ,
    StarVisualStateDto Star,
    IReadOnlyList<PlanetVisualLayoutHintDto> LayoutHints,
    IReadOnlyList<PlanetVisualStateDto> Planets,
    IReadOnlyList<OrbitalGroupVisualMarkerDto> OrbitalGroupMarkers,
    IReadOnlyList<OrbitalTransferVisualOverlayDto> TransferOverlays);

public sealed record StarVisualStateDto(
    Guid StarId,
    string StarName,
    StarType StarType,
    string VisualClass,
    float LightIntensity);

public sealed record PlanetVisualLayoutHintDto(
    Guid PlanetId,
    int OrbitalSlot,
    float OrbitRadius,
    float OrbitAngleDegrees,
    float VisualScale);

public sealed record OrbitalGroupVisualMarkerDto(
    Guid OrbitalGroupId,
    Guid CivilizationId,
    Guid PlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    float MarkerIntensity);

public sealed record OrbitalTransferVisualOverlayDto(
    Guid TransferId,
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid OriginPlanetId,
    Guid DestinationPlanetId,
    OrbitalTransferStatus Status,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    float Progress);

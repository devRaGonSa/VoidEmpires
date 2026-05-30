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
    IReadOnlyList<PlanetVisualStateDto> Planets);

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

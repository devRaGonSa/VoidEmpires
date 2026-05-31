using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.StrategicMap;

public sealed record GetMapVisibilityResult(
    Guid CivilizationId,
    IReadOnlyList<MapSystemVisibilityDto> Systems);

public sealed record MapSystemVisibilityDto(
    Guid SystemId,
    Guid GalaxyId,
    string? SystemName,
    int? CoordinateX,
    int? CoordinateY,
    int? CoordinateZ,
    StarType? StarType,
    MapVisibilityLevel VisibilityLevel,
    MapVisibilityReason VisibilityReason,
    bool IsVisible,
    bool IsOwnedByRequestingCivilization,
    IReadOnlyList<MapPlanetVisibilityDto> Planets);

public sealed record MapPlanetVisibilityDto(
    Guid PlanetId,
    string? PlanetName,
    PlanetType? PlanetType,
    int? Size,
    PlanetColonizationStatus? ColonizationStatus,
    int? OrbitalSlot,
    MapVisibilityLevel VisibilityLevel,
    MapVisibilityReason VisibilityReason,
    bool IsVisible,
    bool IsOwnedByRequestingCivilization,
    Guid? CivilizationId);

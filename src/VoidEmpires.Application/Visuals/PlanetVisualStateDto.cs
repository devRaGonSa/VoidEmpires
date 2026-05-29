using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Visuals;

public sealed record PlanetVisualStateDto(
    Guid PlanetId,
    string PlanetName,
    PlanetType PlanetType,
    int Size,
    PlanetColonizationStatus ColonizationStatus,
    bool IsOwned,
    Guid? CivilizationId,
    string? CivilizationColor,
    int VisualSeed,
    float ColonizationIntensity,
    float UrbanIntensity,
    float IndustrialIntensity,
    float TerraformingIntensity,
    float MilitaryIntensity,
    float OrbitalPresenceIntensity,
    PlanetVisualProfileDto Profile);

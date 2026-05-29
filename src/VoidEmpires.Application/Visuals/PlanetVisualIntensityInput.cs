using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Application.Visuals;

public sealed record PlanetVisualIntensityInput(
    Guid PlanetId,
    PlanetType PlanetType,
    int Size,
    PlanetColonizationStatus ColonizationStatus,
    bool IsOwned,
    int BuildingCount,
    int TotalBuildingLevels,
    int UrbanBuildingLevels,
    int IndustrialBuildingLevels,
    int TerraformingBuildingLevels,
    int MilitaryBuildingLevels,
    int OrbitalGroupStrength);

using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public sealed record AssetRequirement(
    int PopulationCapacity,
    int OperatorCapacity,
    BuildingType RequiredBuildingType,
    int RequiredBuildingLevel);

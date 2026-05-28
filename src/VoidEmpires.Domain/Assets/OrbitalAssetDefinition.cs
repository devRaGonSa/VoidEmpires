using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public sealed record OrbitalAssetDefinition(
    SpaceAssetType AssetType,
    AssetRequirement Requirement,
    ConstructionCost Cost,
    int StorageCapacity,
    int OperatingRange);

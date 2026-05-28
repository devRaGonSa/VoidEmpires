using VoidEmpires.Domain.Buildings;

namespace VoidEmpires.Domain.Assets;

public sealed record PlanetaryAssetDefinition(
    PlanetaryAssetType AssetType,
    AssetRequirement Requirement,
    ConstructionCost Cost);

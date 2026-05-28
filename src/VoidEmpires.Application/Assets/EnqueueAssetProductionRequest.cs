using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.Assets;

public sealed record EnqueueAssetProductionRequest(
    Guid PlanetId,
    AssetProductionTarget Target,
    PlanetaryAssetType? PlanetaryAssetType,
    SpaceAssetType? SpaceAssetType,
    int Quantity,
    DateTime RequestedAtUtc);

using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.Fleets;

public sealed record CreateOrbitalGroupRequest(
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity);

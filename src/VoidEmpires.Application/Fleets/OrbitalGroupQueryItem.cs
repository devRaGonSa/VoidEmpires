using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalGroupQueryItem(
    Guid Id,
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    bool IsStationedAwayFromOrigin);

using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalGroupQueryRequest(
    Guid CivilizationId,
    Guid? CurrentPlanetId = null,
    Guid? OriginPlanetId = null,
    SpaceAssetType? AssetType = null,
    OrbitalGroupStatus? Status = null);

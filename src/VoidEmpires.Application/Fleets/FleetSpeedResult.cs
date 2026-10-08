using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.Fleets;

/// <summary>Immutable calculation input for either unpersisted previews or projected mission composition.</summary>
public sealed record FleetSpeedShipInput(SpaceAssetType AssetType, int Quantity);

/// <summary>Abstract fleet speed and its limiting ship; excludes distance, duration, fuel and cargo calculations.</summary>
public sealed record FleetSpeedResult(
    decimal BaseSpeed,
    decimal PropulsionModifier,
    decimal EffectiveSpeed,
    SpaceAssetType LimitingAssetType);

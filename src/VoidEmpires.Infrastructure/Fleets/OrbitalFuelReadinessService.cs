using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalFuelReadinessService : IOrbitalFuelReadinessService
{
    public OrbitalFuelReadinessDto GetReadiness(SpaceAssetType assetType, int quantity, int abstractDistanceUnits, OrbitalRouteProfileDto routeProfile)
    {
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        if (abstractDistanceUnits <= 0) throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));

        var fuelRequired = abstractDistanceUnits * quantity * GetFuelUnitsPerDistance(assetType) * routeProfile.FuelMultiplier;
        var rangeAvailable = GetRangeUnitsAvailable(assetType);
        var isReady = abstractDistanceUnits <= rangeAvailable;

        return new OrbitalFuelReadinessDto(
            fuelRequired,
            rangeAvailable,
            isReady,
            isReady ? null : "Route distance exceeds placeholder range for asset type.",
            OrbitalFuelReadinessPolicy.PlaceholderDerived);
    }

    private static decimal GetFuelUnitsPerDistance(SpaceAssetType assetType) =>
        assetType switch
        {
            SpaceAssetType.ScoutCraft => 1m,
            SpaceAssetType.CargoCraft => 2m,
            SpaceAssetType.EscortCraft => 2.5m,
            SpaceAssetType.ColonyCraft => 4m,
            _ => throw new ArgumentOutOfRangeException(nameof(assetType))
        };

    private static int GetRangeUnitsAvailable(SpaceAssetType assetType) =>
        assetType switch
        {
            SpaceAssetType.ScoutCraft => 6,
            SpaceAssetType.CargoCraft => 4,
            SpaceAssetType.EscortCraft => 5,
            SpaceAssetType.ColonyCraft => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(assetType))
        };
}

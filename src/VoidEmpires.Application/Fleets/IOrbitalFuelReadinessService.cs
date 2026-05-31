using VoidEmpires.Domain.Assets;

namespace VoidEmpires.Application.Fleets;

public interface IOrbitalFuelReadinessService
{
    OrbitalFuelReadinessDto GetReadiness(SpaceAssetType assetType, int quantity, int abstractDistanceUnits, OrbitalRouteProfileDto routeProfile);
}

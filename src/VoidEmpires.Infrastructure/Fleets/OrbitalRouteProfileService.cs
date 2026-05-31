using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalRouteProfileService : IOrbitalRouteProfileService
{
    public OrbitalRouteProfileDto GetProfile(int abstractDistanceUnits)
    {
        var profile = OrbitalTravelEstimator.EstimateRouteProfile(abstractDistanceUnits);

        return new OrbitalRouteProfileDto(profile.RouteClass, profile.DistanceBand, profile.RiskBand, profile.FuelMultiplier, profile.ComplexityNotes, profile.IsSupported);
    }
}

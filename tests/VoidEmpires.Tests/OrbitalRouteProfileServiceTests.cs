using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;

namespace VoidEmpires.Tests;

public class OrbitalRouteProfileServiceTests
{
    [Theory]
    [InlineData(1, OrbitalRouteClass.LocalOrbit, 1, OrbitalRouteRiskBand.Low)]
    [InlineData(2, OrbitalRouteClass.InnerSystem, 2, OrbitalRouteRiskBand.Moderate)]
    [InlineData(3, OrbitalRouteClass.InnerSystem, 2, OrbitalRouteRiskBand.Moderate)]
    [InlineData(4, OrbitalRouteClass.OuterSystem, 3, OrbitalRouteRiskBand.Elevated)]
    [InlineData(6, OrbitalRouteClass.OuterSystem, 3, OrbitalRouteRiskBand.Elevated)]
    [InlineData(7, OrbitalRouteClass.LongRange, 4, OrbitalRouteRiskBand.Severe)]
    public void GetProfileClassifiesDistanceBandsDeterministically(
        int abstractDistanceUnits,
        OrbitalRouteClass routeClass,
        int distanceBand,
        OrbitalRouteRiskBand riskBand)
    {
        var service = new OrbitalRouteProfileService();

        var profile = service.GetProfile(abstractDistanceUnits);

        Assert.Equal(routeClass, profile.RouteClass);
        Assert.Equal(distanceBand, profile.DistanceBand);
        Assert.Equal(riskBand, profile.RiskBand);
        Assert.Equal(1m, profile.FuelMultiplier);
        Assert.True(profile.IsSupported);
        Assert.NotEmpty(profile.ComplexityNotes);
    }

    [Fact]
    public void GetProfileRejectsInvalidDistance()
    {
        var service = new OrbitalRouteProfileService();

        Assert.Throws<ArgumentOutOfRangeException>(() => service.GetProfile(0));
    }
}

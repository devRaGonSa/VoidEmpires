using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Fleets;

namespace VoidEmpires.Tests;

public class OrbitalFuelReadinessServiceTests
{
    [Fact]
    public void GetReadinessReturnsReadyForShortScoutRoute()
    {
        var readiness = new OrbitalFuelReadinessService().GetReadiness(
            SpaceAssetType.ScoutCraft,
            1,
            1,
            RouteProfile(OrbitalRouteClass.LocalOrbit));

        Assert.Equal(1m, readiness.EstimatedFuelUnitsRequired);
        Assert.Equal(6, readiness.EstimatedRangeUnitsAvailable);
        Assert.True(readiness.IsFuelReady);
        Assert.Null(readiness.NotReadyReason);
        Assert.Equal(OrbitalFuelReadinessPolicy.PlaceholderDerived, readiness.Policy);
    }

    [Fact]
    public void GetReadinessReturnsNotReadyForVeryLongColonyRoute()
    {
        var readiness = new OrbitalFuelReadinessService().GetReadiness(
            SpaceAssetType.ColonyCraft,
            2,
            7,
            RouteProfile(OrbitalRouteClass.LongRange));

        Assert.Equal(56m, readiness.EstimatedFuelUnitsRequired);
        Assert.Equal(3, readiness.EstimatedRangeUnitsAvailable);
        Assert.False(readiness.IsFuelReady);
        Assert.Equal("Route distance exceeds placeholder range for asset type.", readiness.NotReadyReason);
    }

    [Fact]
    public void GetReadinessVariesByAssetType()
    {
        var profile = RouteProfile(OrbitalRouteClass.InnerSystem);
        var scout = new OrbitalFuelReadinessService().GetReadiness(SpaceAssetType.ScoutCraft, 1, 2, profile);
        var cargo = new OrbitalFuelReadinessService().GetReadiness(SpaceAssetType.CargoCraft, 1, 2, profile);

        Assert.True(scout.EstimatedFuelUnitsRequired < cargo.EstimatedFuelUnitsRequired);
        Assert.True(scout.EstimatedRangeUnitsAvailable > cargo.EstimatedRangeUnitsAvailable);
    }

    private static OrbitalRouteProfileDto RouteProfile(OrbitalRouteClass routeClass) =>
        new(routeClass, 1, OrbitalRouteRiskBand.Low, 1m, ["Test route profile."], true);
}

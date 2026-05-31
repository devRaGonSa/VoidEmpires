using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Tests;

public class OrbitalTravelEstimatorTests
{
    [Fact]
    public void EstimateAbstractDistanceUnitsRequiresCurrentPlanetId()
    {
        var destinationPlanetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.EstimateAbstractDistanceUnits(Guid.Empty, destinationPlanetId));
    }

    [Fact]
    public void EstimateAbstractDistanceUnitsRequiresDestinationPlanetId()
    {
        var currentPlanetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.EstimateAbstractDistanceUnits(currentPlanetId, Guid.Empty));
    }

    [Fact]
    public void EstimateAbstractDistanceUnitsRequiresDifferentPlanets()
    {
        var planetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.EstimateAbstractDistanceUnits(planetId, planetId));
    }

    [Fact]
    public void EstimateAbstractDistanceUnitsReturnsCurrentFoundationValue()
    {
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();

        var result = OrbitalTravelEstimator.EstimateAbstractDistanceUnits(currentPlanetId, destinationPlanetId);

        Assert.Equal(OrbitalTravelEstimator.AbstractDistanceUnitsPerPlanetTransfer, result);
    }

    [Fact]
    public void EstimateTravelDurationReturnsOneHourPerAbstractDistanceUnit()
    {
        var result = OrbitalTravelEstimator.EstimateTravelDuration(2);

        Assert.Equal(TimeSpan.FromHours(2), result);
    }

    [Fact]
    public void EstimateTravelDurationRequiresPositiveDistance()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            OrbitalTravelEstimator.EstimateTravelDuration(0));
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, 1)]
    [InlineData(SpaceAssetType.CargoCraft, 2)]
    [InlineData(SpaceAssetType.EscortCraft, 3)]
    [InlineData(SpaceAssetType.ColonyCraft, 4)]
    public void EstimateTravelCostUsesAssetMultiplier(SpaceAssetType assetType, int expectedMultiplier)
    {
        var result = OrbitalTravelEstimator.EstimateTravelCost(assetType, quantity: 3, abstractDistanceUnits: 2);

        var component = Assert.Single(result.Components);
        Assert.Equal(ResourceType.Gas, component.ResourceType);
        Assert.Equal(OrbitalTravelEstimator.BaseGasCostPerDistanceUnit * expectedMultiplier * 3 * 2, component.Amount);
    }

    [Fact]
    public void EstimateTravelCostRequiresPositiveQuantity()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            OrbitalTravelEstimator.EstimateTravelCost(SpaceAssetType.ScoutCraft, 0, 1));
    }

    [Fact]
    public void EstimateTravelCostRequiresPositiveDistance()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            OrbitalTravelEstimator.EstimateTravelCost(SpaceAssetType.ScoutCraft, 1, 0));
    }
}

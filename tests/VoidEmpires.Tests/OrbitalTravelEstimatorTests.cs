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
    public void EstimateRequiresCurrentPlanetId()
    {
        var destinationPlanetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.Estimate(Guid.Empty, destinationPlanetId, SpaceAssetType.ScoutCraft));
    }

    [Fact]
    public void EstimateAbstractDistanceUnitsRequiresDestinationPlanetId()
    {
        var currentPlanetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.EstimateAbstractDistanceUnits(currentPlanetId, Guid.Empty));
    }

    [Fact]
    public void EstimateRequiresDestinationPlanetId()
    {
        var currentPlanetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.Estimate(currentPlanetId, Guid.Empty, SpaceAssetType.ScoutCraft));
    }

    [Fact]
    public void EstimateAbstractDistanceUnitsRequiresDifferentPlanets()
    {
        var planetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.EstimateAbstractDistanceUnits(planetId, planetId));
    }

    [Fact]
    public void EstimateRequiresDifferentPlanets()
    {
        var planetId = Guid.NewGuid();

        Assert.Throws<ArgumentException>(() =>
            OrbitalTravelEstimator.Estimate(planetId, planetId, SpaceAssetType.ScoutCraft));
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

    [Fact]
    public void EstimateReturnsDistanceAndDuration()
    {
        var currentPlanetId = Guid.NewGuid();
        var destinationPlanetId = Guid.NewGuid();

        var result = OrbitalTravelEstimator.Estimate(
            currentPlanetId,
            destinationPlanetId,
            SpaceAssetType.ScoutCraft);

        Assert.Equal(OrbitalTravelEstimator.AbstractDistanceUnitsPerPlanetTransfer, result.AbstractDistanceUnits);
        Assert.Equal(OrbitalTravelEstimator.DurationPerAbstractDistanceUnit, result.EstimatedDuration);
    }

    [Fact]
    public void EstimateReturnsEstimatedResourceCosts()
    {
        var result = OrbitalTravelEstimator.Estimate(1, SpaceAssetType.ScoutCraft);

        Assert.Contains(result.ResourceCosts, cost =>
            cost.ResourceType == ResourceType.Credits &&
            cost.Quantity == OrbitalTravelEstimator.CreditsPerAbstractDistanceUnit);
        Assert.Contains(result.ResourceCosts, cost =>
            cost.ResourceType == ResourceType.Gas &&
            cost.Quantity == OrbitalTravelEstimator.GasPerAbstractDistanceUnit);
    }

    [Fact]
    public void EstimateResourceCostsScaleByAssetTypeMultiplier()
    {
        var scout = OrbitalTravelEstimator.Estimate(1, SpaceAssetType.ScoutCraft);
        var colony = OrbitalTravelEstimator.Estimate(1, SpaceAssetType.ColonyCraft);

        Assert.Equal(
            GasCost(scout) * OrbitalTravelEstimator.GetCostMultiplier(SpaceAssetType.ColonyCraft),
            GasCost(colony));
    }

    [Fact]
    public void EstimateResourceCostsScaleByDistance()
    {
        var oneUnit = OrbitalTravelEstimator.Estimate(1, SpaceAssetType.CargoCraft);
        var threeUnits = OrbitalTravelEstimator.Estimate(3, SpaceAssetType.CargoCraft);

        Assert.Equal(GasCost(oneUnit) * 3, GasCost(threeUnits));
    }

    [Fact]
    public void EstimateResourceCostsAreNotExternallyMutable()
    {
        var estimate = OrbitalTravelEstimator.Estimate(1, SpaceAssetType.ScoutCraft);

        Assert.IsAssignableFrom<IReadOnlyList<OrbitalTravelResourceCost>>(estimate.ResourceCosts);
        Assert.False(estimate.ResourceCosts is IList<OrbitalTravelResourceCost> { IsReadOnly: false });
    }

    private static decimal GasCost(OrbitalTravelEstimate estimate)
    {
        return estimate.ResourceCosts.Single(cost => cost.ResourceType == ResourceType.Gas).Quantity;
    }
}

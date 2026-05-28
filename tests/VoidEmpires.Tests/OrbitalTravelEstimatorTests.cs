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
}

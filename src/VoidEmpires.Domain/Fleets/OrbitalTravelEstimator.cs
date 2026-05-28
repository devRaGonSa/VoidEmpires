namespace VoidEmpires.Domain.Fleets;

public static class OrbitalTravelEstimator
{
    public const int AbstractDistanceUnitsPerPlanetTransfer = 1;
    public static readonly TimeSpan DurationPerAbstractDistanceUnit = TimeSpan.FromHours(1);

    public static int EstimateAbstractDistanceUnits(Guid currentPlanetId, Guid destinationPlanetId)
    {
        if (currentPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Current planet id is required.", nameof(currentPlanetId));
        }

        if (destinationPlanetId == Guid.Empty)
        {
            throw new ArgumentException("Destination planet id is required.", nameof(destinationPlanetId));
        }

        if (currentPlanetId == destinationPlanetId)
        {
            throw new ArgumentException("Destination planet must be different from the current planet.", nameof(destinationPlanetId));
        }

        return AbstractDistanceUnitsPerPlanetTransfer;
    }

    public static TimeSpan EstimateTravelDuration(int abstractDistanceUnits)
    {
        if (abstractDistanceUnits <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(abstractDistanceUnits));
        }

        return TimeSpan.FromTicks(DurationPerAbstractDistanceUnit.Ticks * abstractDistanceUnits);
    }
}

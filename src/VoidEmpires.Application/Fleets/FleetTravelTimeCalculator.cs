using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public static class FleetTravelTimeCalculator
{
    public const decimal ReferenceSpeed = 100m;
    public const decimal BaseSecondsPerDistanceUnitAtReferenceSpeed = 3600m;
    public const long MinimumTravelDurationSeconds = 1L;

    /// <summary>
    /// Composes authoritative distance and effective speed into a whole-second launch snapshot.
    /// The caller supplies server UTC; research, eligibility and persistence remain outside this calculation.
    /// </summary>
    public static FleetTravelTimeResult Calculate(
        GalacticDistanceResult distance, FleetSpeedResult fleetSpeed, DateTime departureAtUtc)
    {
        ArgumentNullException.ThrowIfNull(fleetSpeed);
        if (distance.DistanceUnits <= 0)
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance units must be positive.");
        if (!Enum.IsDefined(distance.Scope))
            throw new ArgumentOutOfRangeException(nameof(distance), "Distance scope must be defined.");
        if (fleetSpeed.EffectiveSpeed <= 0)
            throw new ArgumentOutOfRangeException(nameof(fleetSpeed), "Effective fleet speed must be positive.");
        if (departureAtUtc.Kind != DateTimeKind.Utc)
            throw new ArgumentException("Departure timestamp must be UTC.", nameof(departureAtUtc));

        var rawSeconds = checked(distance.DistanceUnits * BaseSecondsPerDistanceUnitAtReferenceSpeed
            * ReferenceSpeed / fleetSpeed.EffectiveSpeed);
        var roundedSeconds = Math.Max(MinimumTravelDurationSeconds, decimal.Ceiling(rawSeconds));
        // Bound whole seconds before conversion; never route authoritative duration through double.
        if (roundedSeconds > long.MaxValue / TimeSpan.TicksPerSecond)
            throw new OverflowException("Travel duration exceeds the TimeSpan range.");
        var duration = TimeSpan.FromTicks(checked((long)roundedSeconds * TimeSpan.TicksPerSecond));
        if (duration.Ticks > DateTime.MaxValue.Ticks - departureAtUtc.Ticks)
            throw new OverflowException("Travel arrival exceeds the DateTime range.");
        var arrivalAtUtc = departureAtUtc.AddTicks(duration.Ticks);
        return new FleetTravelTimeResult(distance.DistanceUnits, distance.Scope, fleetSpeed.EffectiveSpeed,
            departureAtUtc, duration, arrivalAtUtc);
    }
}

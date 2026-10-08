using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class FleetTravelTimeCalculatorTests
{
    private static readonly DateTime Departure = new(2026, 10, 8, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void ReferencePolicyIsOneHourPerUnitAtSpeedOneHundredWithOneSecondMinimum()
    {
        Assert.Equal(100m, FleetTravelTimeCalculator.ReferenceSpeed);
        Assert.Equal(3600m, FleetTravelTimeCalculator.BaseSecondsPerDistanceUnitAtReferenceSpeed);
        Assert.Equal(1L, FleetTravelTimeCalculator.MinimumTravelDurationSeconds);

        var result = Calculate(1, 100m);
        Assert.Equal(new FleetTravelTimeResult(1, GalacticDistanceScope.SameSystem, 100m,
            Departure, Seconds(3600), new DateTime(2026, 10, 8, 13, 0, 0, DateTimeKind.Utc)), result);
    }

    [Theory]
    [InlineData(SpaceAssetType.ScoutCraft, 1, 3000)]
    [InlineData(SpaceAssetType.EscortCraft, 1, 3600)]
    [InlineData(SpaceAssetType.CargoCraft, 1, 4500)]
    [InlineData(SpaceAssetType.ColonyCraft, 1, 6000)]
    [InlineData(SpaceAssetType.ScoutCraft, 2, 6000)]
    [InlineData(SpaceAssetType.EscortCraft, 2, 7200)]
    [InlineData(SpaceAssetType.CargoCraft, 2, 9000)]
    [InlineData(SpaceAssetType.ColonyCraft, 2, 12000)]
    [InlineData(SpaceAssetType.ScoutCraft, 6, 18000)]
    [InlineData(SpaceAssetType.EscortCraft, 6, 21600)]
    [InlineData(SpaceAssetType.CargoCraft, 6, 27000)]
    [InlineData(SpaceAssetType.ColonyCraft, 6, 36000)]
    public void CatalogFleetSpeedProducesExactOneTwoAndSixUnitExamples(SpaceAssetType type, int distanceUnits,
        int expectedSeconds)
    {
        var speed = FleetSpeedCalculator.Calculate([new(type, 1)]);
        var distance = new GalacticDistanceResult(distanceUnits, GalacticDistanceScope.IntraGalaxy);

        var result = FleetTravelTimeCalculator.Calculate(distance, speed, Departure);

        Assert.Equal(new FleetTravelTimeResult(distanceUnits, GalacticDistanceScope.IntraGalaxy, speed.EffectiveSpeed,
            Departure, Seconds(expectedSeconds), Departure.AddTicks(Seconds(expectedSeconds).Ticks)), result);
        AssertUtcPositiveTimeline(result);
    }

    public static IEnumerable<object[]> FractionalSecondCases =>
    [
        [3L, 88m, 12273L], // 12272.7272... seconds.
        [1L, 700m, 515L], // 514.2857...: rounding to nearest would incorrectly produce 514.
        [1L, 99.999999m, 3601L], // Slightly above a whole-second boundary.
        [1L, 100.000001m, 3600L], // Slightly below a whole-second boundary.
        [1L, 359999.999999m, 2L], // Slightly above one second must not use the minimum as a clamp.
        [1L, 360000m, 1L]
    ];

    [Theory]
    [MemberData(nameof(FractionalSecondCases))]
    public void FractionalSecondsAlwaysUseCeilingAndExactIntegralDurations(long distanceUnits, decimal speed,
        long expectedSeconds)
    {
        var result = Calculate(distanceUnits, speed);

        Assert.Equal(Seconds(expectedSeconds), result.Duration);
        Assert.Equal(0L, result.Duration.Ticks % TimeSpan.TicksPerSecond);
        Assert.Equal(Departure.AddTicks(Seconds(expectedSeconds).Ticks), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void CargoArrivalUsesTheExplicitUtcDeparture()
    {
        var result = Calculate(1, 80m);

        Assert.Equal(Seconds(4500), result.Duration);
        Assert.Equal(new DateTime(2026, 10, 8, 13, 15, 0, DateTimeKind.Utc), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void LongRouteCrossesMultipleDaysWithoutLosingUtcKind()
    {
        var result = Calculate(100, 80m);

        Assert.Equal(new TimeSpan(5, 5, 0, 0), result.Duration);
        Assert.Equal(new DateTime(2026, 10, 13, 17, 0, 0, DateTimeKind.Utc), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void ArrivalCanCrossAYearBoundary()
    {
        var departure = new DateTime(2027, 12, 31, 23, 30, 0, DateTimeKind.Utc);
        var result = FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.IntraGalaxy), Speed(100m), departure);

        Assert.Equal(new DateTime(2028, 1, 1, 0, 30, 0, DateTimeKind.Utc), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void WholeSecondDurationPreservesSuppliedDepartureSubsecondTicks()
    {
        var departure = Departure.AddTicks(1234567);
        var result = FleetTravelTimeCalculator.Calculate(new(3, GalacticDistanceScope.SameSystem), Speed(88m), departure);

        Assert.Equal(departure, result.DepartureAtUtc);
        Assert.Equal(Seconds(12273), result.Duration);
        Assert.Equal(departure.Ticks + 12273L * TimeSpan.TicksPerSecond, result.ArrivalAtUtc.Ticks);
        Assert.Equal(departure.Ticks % TimeSpan.TicksPerSecond, result.ArrivalAtUtc.Ticks % TimeSpan.TicksPerSecond);
        AssertUtcPositiveTimeline(result);
    }

    [Theory]
    [InlineData(GalacticDistanceScope.SameSystem)]
    [InlineData(GalacticDistanceScope.IntraGalaxy)]
    public void DistanceScopeIsPreservedWithoutChangingTheNumericFormula(GalacticDistanceScope scope)
    {
        var result = FleetTravelTimeCalculator.Calculate(new(3, scope), Speed(88m), Departure);

        Assert.Equal(scope, result.DistanceScope);
        Assert.Equal(3L, result.DistanceUnits);
        Assert.Equal(88m, result.EffectiveSpeed);
        Assert.Equal(Seconds(12273), result.Duration);
    }

    [Theory]
    [InlineData(0L)]
    [InlineData(-1L)]
    [InlineData(long.MinValue)]
    public void ManuallyConstructedNonpositiveDistanceIsRejected(long distanceUnits) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Calculate(distanceUnits, 100m));

    [Fact]
    public void DefaultDistanceResultIsRejected() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => FleetTravelTimeCalculator.Calculate(default, Speed(100m), Departure));

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(3)]
    [InlineData(int.MaxValue)]
    public void UndefinedDistanceScopesAreRejectedEvenWithPositiveDistance(int scope) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            FleetTravelTimeCalculator.Calculate(new(1, (GalacticDistanceScope)scope), Speed(100m), Departure));

    [Fact]
    public void NullFleetSpeedResultIsRejected() =>
        Assert.Throws<ArgumentNullException>(() => FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem),
            null!, Departure));

    public static IEnumerable<object[]> NonpositiveSpeeds => [[0m], [-1m], [decimal.MinValue]];

    [Theory]
    [MemberData(nameof(NonpositiveSpeeds))]
    public void ManuallyConstructedNonpositiveEffectiveSpeedIsRejected(decimal speed) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => Calculate(1, speed));

    public static IEnumerable<object[]> SubsecondSpeeds => [[360001m], [720000m], [decimal.MaxValue]];

    [Theory]
    [MemberData(nameof(SubsecondSpeeds))]
    public void PositiveSubsecondTravelHasExactlyOneSecondMinimum(decimal speed)
    {
        var result = Calculate(1, speed);

        Assert.Equal(Seconds(1), result.Duration);
        Assert.NotEqual(TimeSpan.Zero, result.Duration);
        Assert.Equal(Departure.AddTicks(TimeSpan.TicksPerSecond), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void FullLongDistanceDomainCanBeUsedWithSufficientlyHighEffectiveSpeed()
    {
        // long.MaxValue * 3600m * 100m fits decimal; a long intermediate multiplication would overflow.
        var result = Calculate(long.MaxValue, decimal.MaxValue);

        Assert.Equal(long.MaxValue, result.DistanceUnits);
        Assert.Equal(decimal.MaxValue, result.EffectiveSpeed);
        Assert.Equal(Seconds(1), result.Duration);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void PositiveTinySpeedWhoseDurationExceedsTimeSpanCapacityIsRejected() =>
        Assert.Throws<OverflowException>(() => Calculate(1, 0.00000001m));

    [Fact]
    public void HugeDistanceWhoseDecimalDurationFitsButTimeSpanDoesNotIsRejected() =>
        Assert.Throws<OverflowException>(() => Calculate(long.MaxValue, 100m));

    [Fact]
    public void SmallestPositiveDecimalSpeedCausesDeliberateDivisionOverflow() =>
        Assert.Throws<OverflowException>(() => Calculate(1, 0.0000000000000000000000000001m));

    [Theory]
    [InlineData(DateTimeKind.Local)]
    [InlineData(DateTimeKind.Unspecified)]
    public void NonUtcDepartureIsRejectedRatherThanNormalized(DateTimeKind kind)
    {
        var departure = DateTime.SpecifyKind(Departure, kind);
        Assert.Throws<ArgumentException>(() => FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem),
            Speed(100m), departure));
    }

    [Fact]
    public void ExplicitUtcDateTimeMinimumIsValidWithoutConsultingCurrentTime()
    {
        var departure = new DateTime(DateTime.MinValue.Ticks, DateTimeKind.Utc);
        var result = FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem), Speed(decimal.MaxValue), departure);

        Assert.Equal(departure, result.DepartureAtUtc);
        Assert.Equal(new DateTime(TimeSpan.TicksPerSecond, DateTimeKind.Utc), result.ArrivalAtUtc);
        AssertUtcPositiveTimeline(result);
    }

    [Fact]
    public void ArrivalCanReachExactlyDateTimeMaximumButOneTickBeyondIsRejected()
    {
        var duration = Seconds(4500);
        var departure = new DateTime(DateTime.MaxValue.Ticks - duration.Ticks, DateTimeKind.Utc);
        var distance = new GalacticDistanceResult(1, GalacticDistanceScope.SameSystem);
        var speed = Speed(80m);

        var result = FleetTravelTimeCalculator.Calculate(distance, speed, departure);

        Assert.Equal(duration, result.Duration);
        Assert.Equal(DateTime.MaxValue.Ticks, result.ArrivalAtUtc.Ticks);
        AssertUtcPositiveTimeline(result);
        Assert.Throws<OverflowException>(() => FleetTravelTimeCalculator.Calculate(distance, speed, departure.AddTicks(1)));
    }

    [Fact]
    public void MinimumDurationStillRejectsArrivalBeyondDateTimeMaximum()
    {
        var departure = new DateTime(DateTime.MaxValue.Ticks, DateTimeKind.Utc);
        Assert.Throws<OverflowException>(() => FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem),
            Speed(decimal.MaxValue), departure));
    }

    [Fact]
    public void RealDistanceAndMixedFleetSpeedComposeWithoutRepeatingPropulsionModifier()
    {
        var galaxy = Guid.Parse("00000000-0000-0000-0000-000000000001");
        var system = Guid.Parse("00000000-0000-0000-0000-000000000010");
        var origin = new GalacticPlanetLocation(Guid.Parse("00000000-0000-0000-0000-000000000100"), galaxy, system,
            new GalaxyCoordinates(-100, 200, -300), 1);
        var destination = origin with { PlanetId = Guid.Parse("00000000-0000-0000-0000-000000000200"), OrbitalSlot = 8 };
        var distance = GalacticDistanceCalculator.Calculate(origin, destination);
        FleetSpeedShipInput[] composition = [new(SpaceAssetType.ScoutCraft, 10), new(SpaceAssetType.CargoCraft, 2)];
        var speed = FleetSpeedCalculator.Calculate(composition);

        var result = FleetTravelTimeCalculator.Calculate(distance, speed, Departure);

        Assert.Equal(new GalacticDistanceResult(2, GalacticDistanceScope.SameSystem), distance);
        Assert.Equal(80m, speed.EffectiveSpeed);
        Assert.Equal(SpaceAssetType.CargoCraft, speed.LimitingAssetType);
        Assert.Equal(Seconds(9000), result.Duration);
        Assert.Equal(new DateTime(2026, 10, 8, 14, 30, 0, DateTimeKind.Utc), result.ArrivalAtUtc);

        var boostedSpeed = FleetSpeedCalculator.Calculate(composition, 1.25m);
        var boostedResult = FleetTravelTimeCalculator.Calculate(distance, boostedSpeed, Departure);
        Assert.Equal(100m, boostedSpeed.EffectiveSpeed);
        Assert.Equal(Seconds(7200), boostedResult.Duration);
        Assert.Equal(new DateTime(2026, 10, 8, 14, 0, 0, DateTimeKind.Utc), boostedResult.ArrivalAtUtc);
        // A later calculation at a changed speed cannot alter the earlier immutable launch/preview result.
        Assert.Equal(80m, result.EffectiveSpeed);
        Assert.Equal(Seconds(9000), result.Duration);
        Assert.Equal(new DateTime(2026, 10, 8, 14, 30, 0, DateTimeKind.Utc), result.ArrivalAtUtc);
    }

    [Fact]
    public void ExplicitEffectiveSpeedIsTheAuthorityInsteadOfReconstructingFleetMetadata()
    {
        var suppliedSpeed = new FleetSpeedResult(60m, 2m, 80m, SpaceAssetType.ColonyCraft);
        var result = FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem), suppliedSpeed, Departure);

        Assert.Equal(80m, result.EffectiveSpeed);
        Assert.Equal(Seconds(4500), result.Duration);
    }

    [Fact]
    public void RepeatedCallsAndInterleavedFailuresDoNotMutateTypedInputsOrPreviousResults()
    {
        var distance = new GalacticDistanceResult(3, GalacticDistanceScope.IntraGalaxy);
        var speed = FleetSpeedCalculator.Calculate([new(SpaceAssetType.CargoCraft, 3)], 1.10m);
        var beforeDistance = distance;
        var beforeSpeed = speed with { };
        var expected = new FleetTravelTimeResult(3, GalacticDistanceScope.IntraGalaxy, 88m,
            Departure, Seconds(12273), new DateTime(2026, 10, 8, 15, 24, 33, DateTimeKind.Utc));
        var result = FleetTravelTimeCalculator.Calculate(distance, speed, Departure);

        for (var i = 0; i < 5; i++)
        {
            Assert.Equal(expected, FleetTravelTimeCalculator.Calculate(distance, speed, Departure));
            Assert.Throws<ArgumentOutOfRangeException>(() => FleetTravelTimeCalculator.Calculate(distance with { DistanceUnits = 0 },
                speed, Departure));
            Assert.Throws<OverflowException>(() => FleetTravelTimeCalculator.Calculate(distance,
                speed with { EffectiveSpeed = 0.0000000000000000000000000001m }, Departure));
            Assert.Equal(beforeDistance, distance);
            Assert.Equal(beforeSpeed, speed);
            Assert.Equal(expected, result);
        }
    }

    [Fact]
    public void NewFleetTimingDoesNotChangeLegacyFixedHourEstimator()
    {
        var speed = FleetSpeedCalculator.Calculate([new(SpaceAssetType.ScoutCraft, 1)]);
        var result = FleetTravelTimeCalculator.Calculate(new(1, GalacticDistanceScope.SameSystem), speed, Departure);

        Assert.Equal(Seconds(3000), result.Duration);
        Assert.Equal(Seconds(3600), OrbitalTravelEstimator.EstimateTravelDuration(1));
        Assert.Equal(Seconds(7200), OrbitalTravelEstimator.EstimateTravelDuration(2));
    }

    private static FleetTravelTimeResult Calculate(long distanceUnits, decimal speed) =>
        FleetTravelTimeCalculator.Calculate(new(distanceUnits, GalacticDistanceScope.SameSystem), Speed(speed), Departure);

    private static FleetSpeedResult Speed(decimal effectiveSpeed) =>
        new(effectiveSpeed, 1m, effectiveSpeed, SpaceAssetType.EscortCraft);

    private static TimeSpan Seconds(long seconds) => TimeSpan.FromTicks(checked(seconds * TimeSpan.TicksPerSecond));

    private static void AssertUtcPositiveTimeline(FleetTravelTimeResult result)
    {
        Assert.True(result.Duration > TimeSpan.Zero);
        Assert.True(result.ArrivalAtUtc > result.DepartureAtUtc);
        Assert.Equal(DateTimeKind.Utc, result.DepartureAtUtc.Kind);
        Assert.Equal(DateTimeKind.Utc, result.ArrivalAtUtc.Kind);
        Assert.Equal(result.Duration.Ticks, result.ArrivalAtUtc.Ticks - result.DepartureAtUtc.Ticks);
    }
}

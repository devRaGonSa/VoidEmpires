using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Tests;

public class GalacticDistanceCalculatorTests
{
    private static readonly Guid GalaxyId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherGalaxyId = Guid.Parse("00000000-0000-0000-0000-000000000002");
    private static readonly Guid OriginSystemId = Guid.Parse("00000000-0000-0000-0000-000000000010");
    private static readonly Guid DestinationSystemId = Guid.Parse("00000000-0000-0000-0000-000000000020");
    private static readonly Guid OriginPlanetId = Guid.Parse("00000000-0000-0000-0000-000000000100");
    private static readonly Guid DestinationPlanetId = Guid.Parse("00000000-0000-0000-0000-000000000200");

    [Theory]
    [InlineData(1, 2, 1)]
    [InlineData(2, 1, 1)]
    [InlineData(1, 6, 1)]
    [InlineData(1, 7, 1)]
    [InlineData(1, 8, 2)]
    [InlineData(1, 12, 2)]
    [InlineData(1, 13, 2)]
    [InlineData(1, 14, 3)]
    [InlineData(int.MaxValue - 6, int.MaxValue, 1)]
    [InlineData(int.MaxValue - 7, int.MaxValue, 2)]
    [InlineData(1, int.MaxValue, 357913941)]
    public void SameSystemUsesCeilingOfSlotSeparationSymmetrically(int fromSlot, int toSlot, int expected)
    {
        var origin = Origin(fromSlot);
        var destination = origin with { PlanetId = DestinationPlanetId, OrbitalSlot = toSlot };
        AssertSymmetric(origin, destination, new(expected, GalacticDistanceScope.SameSystem));
    }

    [Fact]
    public void SameSystemDistanceIsIndependentOfItsSharedCoordinates()
    {
        var origin = Origin();
        var destination = origin with { PlanetId = DestinationPlanetId, OrbitalSlot = 8 };
        var expected = GalacticDistanceCalculator.Calculate(origin, destination);
        foreach (var coordinates in new[] { new GalaxyCoordinates(-100, 200, -300), new GalaxyCoordinates(int.MinValue, 0, int.MaxValue) })
            AssertSymmetric(origin with { SystemCoordinates = coordinates }, destination with { SystemCoordinates = coordinates }, expected);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(9999, 2)]
    [InlineData(10000, 2)]
    [InlineData(10001, 3)]
    [InlineData(20000, 3)]
    [InlineData(20001, 4)]
    [InlineData(30000, 4)]
    [InlineData(30001, 5)]
    [InlineData(40000, 5)]
    [InlineData(40001, 6)]
    public void IntraGalaxyUsesBaseCrossingPlusIntegerCeilingAtEveryThreshold(int span, int expected) =>
        AssertSymmetric(Origin(), Destination(new(span, 0, 0)), new(expected, GalacticDistanceScope.IntraGalaxy));

    public static IEnumerable<object[]> SignedCoordinates =>
    [
        [new GalaxyCoordinates(100, -200, 300), new GalaxyCoordinates(-400, 500, -600), 2], // span 2100
        [new GalaxyCoordinates(-50000, -8000, -1000), new GalaxyCoordinates(-10000, -7000, -999), 6], // span 41001
        [new GalaxyCoordinates(-5000, -5000, -1), new GalaxyCoordinates(0, 0, 0), 3], // span 10001
        [new GalaxyCoordinates(8000, 6000, 2000), new GalaxyCoordinates(-3000, -1000, -1000), 4], // span 21000
        [new GalaxyCoordinates(0, 0, 0), new GalaxyCoordinates(4000, 4000, 4000), 3], // L1, not Euclidean/max-axis
        [new GalaxyCoordinates(0, 0, 0), new GalaxyCoordinates(0, 10001, 0), 3],
        [new GalaxyCoordinates(0, 0, 0), new GalaxyCoordinates(0, 0, 10001), 3]
    ];

    [Theory]
    [MemberData(nameof(SignedCoordinates))]
    public void AllSignedAxesContributeAbsoluteManhattanSeparation(GalaxyCoordinates from, GalaxyCoordinates to, int expected) =>
        AssertSymmetric(Origin(coordinates: from), Destination(to), new(expected, GalacticDistanceScope.IntraGalaxy));

    [Fact]
    public void CoordinateTranslationAxisOrderAndSignReflectionPreserveDistance()
    {
        var origin = Origin(coordinates: new(100, -200, 300));
        var destination = Destination(new(-6000, 4200, -501));
        var expected = new GalacticDistanceResult(3, GalacticDistanceScope.IntraGalaxy); // span 11301
        Func<GalaxyCoordinates, GalaxyCoordinates>[] transforms =
        [
            p => p,
            p => new(p.X + 1000, p.Y - 2000, p.Z + 3000),
            p => new(p.X, p.Z, p.Y), p => new(p.Y, p.X, p.Z),
            p => new(p.Y, p.Z, p.X), p => new(p.Z, p.X, p.Y), p => new(p.Z, p.Y, p.X),
            p => new(-p.X, -p.Y, -p.Z)
        ];
        foreach (var transform in transforms)
            AssertSymmetric(origin with { SystemCoordinates = transform(origin.SystemCoordinates) },
                destination with { SystemCoordinates = transform(destination.SystemCoordinates) }, expected);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 12)]
    [InlineData(12, 1)]
    [InlineData(int.MaxValue, 1)]
    [InlineData(1, int.MaxValue)]
    [InlineData(int.MaxValue, int.MaxValue)]
    public void InterSystemDistanceDoesNotAddOrbitalSlotOffsets(int fromSlot, int toSlot) =>
        AssertSymmetric(Origin(fromSlot), Destination(new(10000, 0, 0), toSlot), new(2, GalacticDistanceScope.IntraGalaxy));

    [Theory]
    [InlineData(1, 429498)]
    [InlineData(2, 858995)]
    [InlineData(3, 1288492)]
    public void FullIntCoordinateDomainUsesLongBeforeSubtractionAndSummation(int extremeAxes, int expected)
    {
        // Each extreme axis spans 4,294,967,295; all three span 12,884,901,885 before division.
        var from = new GalaxyCoordinates(int.MinValue, extremeAxes >= 2 ? int.MinValue : 0, extremeAxes >= 3 ? int.MinValue : 0);
        var to = new GalaxyCoordinates(int.MaxValue, extremeAxes >= 2 ? int.MaxValue : 0, extremeAxes >= 3 ? int.MaxValue : 0);
        AssertSymmetric(Origin(coordinates: from), Destination(to), new(expected, GalacticDistanceScope.IntraGalaxy));
    }

    [Fact]
    public void GeneratedExtentHasSixUnitsWithoutApplyingShipRangeRestrictions()
    {
        // Current generation bounds give span 20,000 + 20,000 + 2,000 = 42,000.
        AssertSymmetric(Origin(coordinates: new(-10000, -10000, -1000)), Destination(new(10000, 10000, 1000)),
            new(6, GalacticDistanceScope.IntraGalaxy));
    }

    [Theory]
    [InlineData("planet")]
    [InlineData("galaxy")]
    [InlineData("system")]
    public void EachIdentityIsRequiredAtBothEndpoints(string field)
    {
        GalacticPlanetLocation Invalid(GalacticPlanetLocation location) => field switch
        {
            "planet" => location with { PlanetId = Guid.Empty },
            "galaxy" => location with { GalaxyId = Guid.Empty },
            _ => location with { SolarSystemId = Guid.Empty }
        };
        var origin = Origin();
        var destination = Destination(new(1, 0, 0));
        var originError = Assert.Throws<ArgumentException>(() => GalacticDistanceCalculator.Calculate(Invalid(origin), destination));
        var destinationError = Assert.Throws<ArgumentException>(() => GalacticDistanceCalculator.Calculate(origin, Invalid(destination)));
        Assert.Equal("origin", originError.ParamName);
        Assert.Equal("destination", destinationError.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(int.MinValue)]
    public void EveryEndpointRequiresAPositiveSlotEvenForInterSystemRoutes(int slot)
    {
        var origin = Origin();
        var destination = Destination(new(1, 0, 0));
        Assert.Throws<ArgumentOutOfRangeException>(() => GalacticDistanceCalculator.Calculate(origin with { OrbitalSlot = slot }, destination));
        Assert.Throws<ArgumentOutOfRangeException>(() => GalacticDistanceCalculator.Calculate(origin, destination with { OrbitalSlot = slot }));
    }

    [Fact]
    public void DefaultLocationSnapshotsAreRejected()
    {
        Assert.Throws<ArgumentException>(() => GalacticDistanceCalculator.Calculate(default, Destination(new(1, 0, 0))));
        Assert.Throws<ArgumentException>(() => GalacticDistanceCalculator.Calculate(Origin(), default));
    }

    [Theory]
    [InlineData("same")]
    [InlineData("slot")]
    [InlineData("coordinates")]
    [InlineData("system")]
    [InlineData("galaxy")]
    public void SamePlanetIsRejectedRegardlessOfOtherSpatialFacts(string changed)
    {
        var origin = Origin();
        var destination = changed switch
        {
            "slot" => origin with { OrbitalSlot = 2 },
            "coordinates" => origin with { SystemCoordinates = new(1, 0, 0) },
            "system" => origin with { SolarSystemId = DestinationSystemId, SystemCoordinates = new(1, 0, 0) },
            "galaxy" => origin with { GalaxyId = OtherGalaxyId, SolarSystemId = DestinationSystemId },
            _ => origin
        };
        AssertRejectedBothWays<ArgumentException>(origin, destination);
    }

    [Theory]
    [InlineData("galaxy")]
    [InlineData("x")]
    [InlineData("y")]
    [InlineData("z")]
    [InlineData("slot")]
    public void SameSystemSnapshotsMustAgreeOnGalaxyCoordinatesAndUniqueSlots(string inconsistent)
    {
        var origin = Origin();
        var destination = origin with { PlanetId = DestinationPlanetId, OrbitalSlot = 2 };
        destination = inconsistent switch
        {
            "galaxy" => destination with { GalaxyId = OtherGalaxyId },
            "x" => destination with { SystemCoordinates = new(1, 0, 0) },
            "y" => destination with { SystemCoordinates = new(0, 1, 0) },
            "z" => destination with { SystemCoordinates = new(0, 0, 1) },
            _ => destination with { OrbitalSlot = origin.OrbitalSlot }
        };
        AssertRejectedBothWays<ArgumentException>(origin, destination);
    }

    [Fact]
    public void DifferentSystemsWithinOneGalaxyCannotHaveIdenticalCoordinates() =>
        AssertRejectedBothWays<ArgumentException>(Origin(coordinates: new(-1, 0, 1)), Destination(new(-1, 0, 1)));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DifferentGalaxiesAreExplicitlyUnsupportedRegardlessOfCoordinateOverlap(bool sameCoordinates)
    {
        var origin = Origin();
        var destination = Destination(sameCoordinates ? default : new(1, 2, 3)) with { GalaxyId = OtherGalaxyId };
        var exception = Assert.Throws<NotSupportedException>(() => GalacticDistanceCalculator.Calculate(origin, destination));
        Assert.Contains("Cross-galaxy", exception.Message);
        AssertRejectedBothWays<NotSupportedException>(origin, destination);
    }

    [Fact]
    public void RepeatedCallsArePureAndUnrelatedCalculationsCannotChangeResults()
    {
        var origin = Origin(coordinates: new(-100, 200, -300));
        var destination = Destination(new(10000, 0, 0), 12);
        var beforeOrigin = origin;
        var beforeDestination = destination;
        var result = GalacticDistanceCalculator.Calculate(origin, destination);
        Assert.Equal(new GalacticDistanceResult(3, GalacticDistanceScope.IntraGalaxy), result);
        for (var i = 0; i < 5; i++)
        {
            Assert.Equal(result, GalacticDistanceCalculator.Calculate(origin, destination));
            Assert.Throws<ArgumentException>(() => GalacticDistanceCalculator.Calculate(origin, origin));
            Assert.Throws<NotSupportedException>(() => GalacticDistanceCalculator.Calculate(origin, destination with { GalaxyId = OtherGalaxyId }));
            Assert.Equal(result, GalacticDistanceCalculator.Calculate(destination, origin));
        }
        Assert.Equal(beforeOrigin, origin);
        Assert.Equal(beforeDestination, destination);
    }

    [Fact]
    public void NewDistanceDoesNotActivateOrChangeLegacyFixedDistance()
    {
        var origin = Origin();
        var destination = Destination(new(40001, 0, 0));
        Assert.Equal(6L, GalacticDistanceCalculator.Calculate(origin, destination).DistanceUnits);
        Assert.Equal(1, OrbitalTravelEstimator.EstimateAbstractDistanceUnits(origin.PlanetId, destination.PlanetId));
    }

    private static GalacticPlanetLocation Origin(int slot = 1, GalaxyCoordinates coordinates = default) =>
        new(OriginPlanetId, GalaxyId, OriginSystemId, coordinates, slot);
    private static GalacticPlanetLocation Destination(GalaxyCoordinates coordinates, int slot = 1) =>
        new(DestinationPlanetId, GalaxyId, DestinationSystemId, coordinates, slot);
    private static void AssertSymmetric(GalacticPlanetLocation origin, GalacticPlanetLocation destination, GalacticDistanceResult expected)
    {
        Assert.Equal(expected, GalacticDistanceCalculator.Calculate(origin, destination));
        Assert.Equal(expected, GalacticDistanceCalculator.Calculate(destination, origin));
        Assert.True(expected.DistanceUnits > 0);
    }
    private static void AssertRejectedBothWays<T>(GalacticPlanetLocation origin, GalacticPlanetLocation destination) where T : Exception
    {
        Assert.Throws<T>(() => GalacticDistanceCalculator.Calculate(origin, destination));
        Assert.Throws<T>(() => GalacticDistanceCalculator.Calculate(destination, origin));
    }
}

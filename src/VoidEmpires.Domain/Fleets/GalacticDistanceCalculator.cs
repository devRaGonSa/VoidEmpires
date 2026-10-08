using VoidEmpires.Domain.Galaxy;

namespace VoidEmpires.Domain.Fleets;

/// <summary>Immutable persisted spatial facts projected by a backend caller, not visual map coordinates.</summary>
public readonly record struct GalacticPlanetLocation(
    Guid PlanetId,
    Guid GalaxyId,
    Guid SolarSystemId,
    GalaxyCoordinates SystemCoordinates,
    int OrbitalSlot);

public enum GalacticDistanceScope
{
    SameSystem = 1,
    IntraGalaxy = 2
}

/// <summary>Abstract integer gameplay distance; range eligibility, duration and fuel are separate calculations.</summary>
public readonly record struct GalacticDistanceResult(long DistanceUnits, GalacticDistanceScope Scope);

public static class GalacticDistanceCalculator
{
    public const long OrbitalSlotsPerDistanceUnit = 6;
    public const long SystemCoordinateUnitsPerDistanceUnit = 10_000;
    public const long InterSystemBaseDistanceUnits = 1;

    public static GalacticDistanceResult Calculate(GalacticPlanetLocation origin, GalacticPlanetLocation destination)
    {
        ValidateLocation(origin, nameof(origin));
        ValidateLocation(destination, nameof(destination));
        if (origin.PlanetId == destination.PlanetId)
            throw new ArgumentException("Origin and destination must be different planets.", nameof(destination));

        if (origin.SolarSystemId == destination.SolarSystemId)
        {
            if (origin.GalaxyId != destination.GalaxyId || origin.SystemCoordinates != destination.SystemCoordinates)
                throw new ArgumentException("Snapshots of the same system must agree on galaxy and coordinates.", nameof(destination));
            var slotDelta = AbsoluteDifference(origin.OrbitalSlot, destination.OrbitalSlot);
            if (slotDelta == 0)
                throw new ArgumentException("Different planets in one system must occupy different orbital slots.", nameof(destination));
            return new GalacticDistanceResult(CeilingDivide(slotDelta, OrbitalSlotsPerDistanceUnit), GalacticDistanceScope.SameSystem);
        }

        if (origin.GalaxyId != destination.GalaxyId)
            throw new NotSupportedException("Cross-galaxy fleet travel is not supported by the v1 distance model.");

        var from = origin.SystemCoordinates;
        var to = destination.SystemCoordinates;
        var coordinateSpan = checked(AbsoluteDifference(from.X, to.X) + AbsoluteDifference(from.Y, to.Y) + AbsoluteDifference(from.Z, to.Z));
        if (coordinateSpan == 0)
            throw new ArgumentException("Different systems in one galaxy must have distinct coordinates.", nameof(destination));
        // Inter-system orbital ingress/egress is included in the base unit; do not add raw orbital slots.
        var distance = checked(InterSystemBaseDistanceUnits + CeilingDivide(coordinateSpan, SystemCoordinateUnitsPerDistanceUnit));
        return new GalacticDistanceResult(distance, GalacticDistanceScope.IntraGalaxy);
    }

    private static void ValidateLocation(GalacticPlanetLocation location, string parameterName)
    {
        if (location.PlanetId == Guid.Empty) throw new ArgumentException("Planet id is required.", parameterName);
        if (location.GalaxyId == Guid.Empty) throw new ArgumentException("Galaxy id is required.", parameterName);
        if (location.SolarSystemId == Guid.Empty) throw new ArgumentException("Solar system id is required.", parameterName);
        if (location.OrbitalSlot <= 0) throw new ArgumentOutOfRangeException(parameterName, "Orbital slot must be positive.");
    }

    // Widen before subtraction: even opposite int endpoints have an absolute delta safely inside Int64.
    private static long AbsoluteDifference(int first, int second) => Math.Abs(checked((long)first - second));

    // Callers supply a positive numerator and a positive policy constant; avoid numerator + divisor - 1.
    private static long CeilingDivide(long numerator, long divisor) =>
        checked(numerator / divisor + (numerator % divisor == 0 ? 0 : 1));
}

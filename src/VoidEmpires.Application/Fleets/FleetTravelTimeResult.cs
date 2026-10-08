using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

/// <summary>Immutable preview/launch timing evidence; later research never recalculates a persisted deadline.</summary>
public sealed record FleetTravelTimeResult(
    long DistanceUnits,
    GalacticDistanceScope DistanceScope,
    decimal EffectiveSpeed,
    DateTime DepartureAtUtc,
    TimeSpan Duration,
    DateTime ArrivalAtUtc);

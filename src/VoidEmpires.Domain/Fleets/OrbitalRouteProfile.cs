namespace VoidEmpires.Domain.Fleets;

public sealed record OrbitalRouteProfile(
    OrbitalRouteClass RouteClass,
    int DistanceBand,
    OrbitalRouteRiskBand RiskBand,
    decimal FuelMultiplier,
    IReadOnlyList<string> ComplexityNotes,
    bool IsSupported);

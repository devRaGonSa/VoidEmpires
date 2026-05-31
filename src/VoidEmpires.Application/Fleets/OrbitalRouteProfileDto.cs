using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalRouteProfileDto(OrbitalRouteClass RouteClass, int DistanceBand, OrbitalRouteRiskBand RiskBand, decimal FuelMultiplier, IReadOnlyList<string> ComplexityNotes, bool IsSupported);

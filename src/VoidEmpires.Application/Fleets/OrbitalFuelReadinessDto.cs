namespace VoidEmpires.Application.Fleets;

public sealed record OrbitalFuelReadinessDto(decimal EstimatedFuelUnitsRequired, int EstimatedRangeUnitsAvailable, bool IsFuelReady, string? NotReadyReason, OrbitalFuelReadinessPolicy Policy);

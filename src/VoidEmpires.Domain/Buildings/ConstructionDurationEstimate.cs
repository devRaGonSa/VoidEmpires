namespace VoidEmpires.Domain.Buildings;

public sealed record ConstructionDurationEstimate(
    TimeSpan BaseDuration,
    TimeSpan EffectiveDuration,
    decimal SpeedMultiplier);

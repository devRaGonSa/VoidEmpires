namespace VoidEmpires.Domain.Research;

public sealed record ResearchCost(
    decimal Credits,
    decimal Metal,
    decimal Crystal,
    decimal Gas);

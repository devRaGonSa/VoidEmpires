namespace VoidEmpires.Domain.Buildings;

public sealed record ConstructionCost(
    decimal Credits,
    decimal Metal,
    decimal Crystal,
    decimal Gas)
{
    public static ConstructionCost Zero => new(0, 0, 0, 0);
}

namespace VoidEmpires.Domain.Galaxy;

public readonly record struct GalaxyCoordinates(int X, int Y, int Z)
{
    public override string ToString() => $"{X}:{Y}:{Z}";
}

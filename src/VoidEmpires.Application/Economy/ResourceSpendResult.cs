namespace VoidEmpires.Application.Economy;

public sealed record ResourceSpendResult(
    bool Succeeded,
    Guid? PlanetId,
    IReadOnlyList<string> Errors)
{
    public static ResourceSpendResult Success(Guid planetId) => new(true, planetId, []);

    public static ResourceSpendResult Failure(params string[] errors) => new(false, null, errors);
}

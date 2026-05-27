namespace VoidEmpires.Application.Economy;

public sealed record ApplyPlanetProductionResult(
    bool Succeeded,
    Guid? PlanetId,
    IReadOnlyList<string> Errors)
{
    public static ApplyPlanetProductionResult Success(Guid planetId) => new(true, planetId, []);

    public static ApplyPlanetProductionResult Failure(params string[] errors) => new(false, null, errors);
}

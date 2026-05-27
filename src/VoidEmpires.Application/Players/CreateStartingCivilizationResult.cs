namespace VoidEmpires.Application.Players;

public sealed record CreateStartingCivilizationResult(
    bool Succeeded,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    IReadOnlyList<string> Errors)
{
    public static CreateStartingCivilizationResult Success(Guid playerProfileId, Guid civilizationId, Guid? homePlanetId) =>
        new(true, playerProfileId, civilizationId, homePlanetId, []);

    public static CreateStartingCivilizationResult Failure(params string[] errors) =>
        new(false, null, null, null, errors);
}

namespace VoidEmpires.Application.Players;

public sealed record InitialPlayerWorldBootstrapResult(
    bool Succeeded,
    string? UserId,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    string? HomePlanetName,
    CreateStartingCivilizationResourceSnapshot? StartingResources,
    IReadOnlyList<string> Errors)
{
    public static InitialPlayerWorldBootstrapResult Success(
        string userId,
        Guid playerProfileId,
        Guid civilizationId,
        Guid homePlanetId,
        string homePlanetName,
        CreateStartingCivilizationResourceSnapshot startingResources) =>
        new(true, userId, playerProfileId, civilizationId, homePlanetId, homePlanetName, startingResources, []);

    public static InitialPlayerWorldBootstrapResult Failure(params string[] errors) =>
        new(false, null, null, null, null, null, null, errors);
}

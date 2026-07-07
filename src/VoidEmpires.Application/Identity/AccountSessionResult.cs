namespace VoidEmpires.Application.Identity;

public sealed record AccountSessionResult(
    bool Succeeded,
    string? UserId,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    string? HomePlanetName,
    string? NextRoute,
    IReadOnlyList<AccountSessionError> Errors)
{
    public static AccountSessionResult Success(
        string userId,
        Guid? playerProfileId,
        Guid? civilizationId,
        Guid? homePlanetId,
        string? homePlanetName,
        string? nextRoute) =>
        new(true, userId, playerProfileId, civilizationId, homePlanetId, homePlanetName, nextRoute, []);

    public static AccountSessionResult Failure(params AccountSessionError[] errors) =>
        new(false, null, null, null, null, null, null, errors);
}

public sealed record AccountSessionError(
    string Code,
    string Message,
    string? Field = null);

namespace VoidEmpires.Application.Identity;

public sealed record AccountRegistrationResult(
    bool Succeeded,
    string? UserId,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    string? HomePlanetName,
    string? NextRoute,
    IReadOnlyList<AccountRegistrationError> Errors)
{
    public static AccountRegistrationResult Success(
        string userId,
        Guid playerProfileId,
        Guid civilizationId,
        Guid homePlanetId,
        string homePlanetName,
        string nextRoute) =>
        new(true, userId, playerProfileId, civilizationId, homePlanetId, homePlanetName, nextRoute, []);

    public static AccountRegistrationResult Failure(params AccountRegistrationError[] errors) =>
        new(false, null, null, null, null, null, null, errors);
}

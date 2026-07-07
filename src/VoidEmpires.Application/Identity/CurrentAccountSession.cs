namespace VoidEmpires.Application.Identity;

public sealed record CurrentAccountSession(
    bool Succeeded,
    string? UserId,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    string? HomePlanetName,
    string? NextRoute,
    IReadOnlyList<AccountSessionError> Errors)
{
    public static CurrentAccountSession From(AccountSessionResult result) =>
        new(
            result.Succeeded,
            result.UserId,
            result.PlayerProfileId,
            result.CivilizationId,
            result.HomePlanetId,
            result.HomePlanetName,
            result.NextRoute,
            result.Errors);

    public static CurrentAccountSession Unauthenticated() =>
        new(false, null, null, null, null, null, null, [new AccountSessionError("Unauthenticated", "Authentication is required.")]);
}

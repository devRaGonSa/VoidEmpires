namespace VoidEmpires.Application.Players;

public sealed record RenamePlayerWorldRequest(
    string UserId,
    string? CivilizationName,
    string? HomePlanetName);

public sealed record RenamePlayerWorldResult(
    bool Succeeded,
    string? CivilizationName,
    string? HomePlanetName,
    IReadOnlyList<string> Errors)
{
    public static RenamePlayerWorldResult Success(string civilizationName, string homePlanetName) =>
        new(true, civilizationName, homePlanetName, []);

    public static RenamePlayerWorldResult Failure(params string[] errors) =>
        new(false, null, null, errors);
}

public interface IPlayerWorldRenameService
{
    public const int MaxNameLength = 128;

    Task<RenamePlayerWorldResult> RenameAsync(
        RenamePlayerWorldRequest request,
        CancellationToken cancellationToken = default);
}

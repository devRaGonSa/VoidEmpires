namespace VoidEmpires.Application.Players;

public sealed record CreateStartingCivilizationResult(
    bool Succeeded,
    string? UserId,
    Guid? PlayerProfileId,
    Guid? CivilizationId,
    Guid? HomePlanetId,
    string? HomePlanetName,
    Guid? HomeSystemId,
    string? HomeSystemName,
    CreateStartingCivilizationResourceSnapshot? StartingResources,
    IReadOnlyList<string> Limitations,
    IReadOnlyList<string> Errors)
{
    public static CreateStartingCivilizationResult Success(
        string userId,
        Guid playerProfileId,
        Guid civilizationId,
        Guid homePlanetId,
        string homePlanetName,
        Guid homeSystemId,
        string homeSystemName,
        CreateStartingCivilizationResourceSnapshot startingResources,
        IReadOnlyList<string> limitations) =>
        new(true, userId, playerProfileId, civilizationId, homePlanetId, homePlanetName, homeSystemId, homeSystemName, startingResources, limitations, []);

    public static CreateStartingCivilizationResult Failure(params string[] errors) =>
        new(false, null, null, null, null, null, null, null, null, [], errors);
}

public sealed record CreateStartingCivilizationResourceSnapshot(
    decimal Credits,
    decimal Metal,
    decimal Crystal,
    decimal Gas);

namespace VoidEmpires.Application.Galaxy;

public sealed record GenerateAndPersistGalaxyResult(
    bool Succeeded,
    Guid? GalaxyId,
    string? GalaxyName,
    int SolarSystemCount,
    int PlanetCount,
    IReadOnlyList<string> Errors)
{
    public static GenerateAndPersistGalaxyResult Success(
        Guid galaxyId,
        string galaxyName,
        int solarSystemCount,
        int planetCount) =>
        new(true, galaxyId, galaxyName, solarSystemCount, planetCount, []);

    public static GenerateAndPersistGalaxyResult Failure(params string[] errors) =>
        new(false, null, null, 0, 0, errors);
}

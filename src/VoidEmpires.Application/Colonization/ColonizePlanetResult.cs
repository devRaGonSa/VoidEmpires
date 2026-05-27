namespace VoidEmpires.Application.Colonization;

public sealed record ColonizePlanetResult(
    bool Succeeded,
    Guid? OwnershipId,
    IReadOnlyList<string> Errors)
{
    public static ColonizePlanetResult Success(Guid ownershipId) =>
        new(true, ownershipId, []);

    public static ColonizePlanetResult Failure(params string[] errors) =>
        new(false, null, errors);
}

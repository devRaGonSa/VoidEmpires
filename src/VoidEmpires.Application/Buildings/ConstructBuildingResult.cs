namespace VoidEmpires.Application.Buildings;

public sealed record ConstructBuildingResult(
    bool Succeeded,
    Guid? BuildingId,
    IReadOnlyList<string> Errors)
{
    public static ConstructBuildingResult Success(Guid buildingId) => new(true, buildingId, []);

    public static ConstructBuildingResult Failure(params string[] errors) => new(false, null, errors);
}

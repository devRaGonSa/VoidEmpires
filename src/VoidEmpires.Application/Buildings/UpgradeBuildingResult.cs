namespace VoidEmpires.Application.Buildings;

public sealed record UpgradeBuildingResult(
    bool Succeeded,
    int? NewLevel,
    IReadOnlyList<string> Errors)
{
    public static UpgradeBuildingResult Success(int level) => new(true, level, []);

    public static UpgradeBuildingResult Failure(params string[] errors) => new(false, null, errors);
}

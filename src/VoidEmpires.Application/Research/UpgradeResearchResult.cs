namespace VoidEmpires.Application.Research;

public sealed record UpgradeResearchResult(
    bool Succeeded,
    int? NewLevel,
    IReadOnlyList<string> Errors)
{
    public static UpgradeResearchResult Success(int level) => new(true, level, []);

    public static UpgradeResearchResult Failure(params string[] errors) => new(false, null, errors);
}

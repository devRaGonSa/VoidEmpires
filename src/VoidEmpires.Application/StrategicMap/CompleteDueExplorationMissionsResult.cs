namespace VoidEmpires.Application.StrategicMap;

public sealed record CompleteDueExplorationMissionsResult(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedMissionIds,
    IReadOnlyList<string> Errors)
{
    public static CompleteDueExplorationMissionsResult Success(IReadOnlyList<Guid> completedMissionIds) =>
        new(true, completedMissionIds.Count, completedMissionIds, []);

    public static CompleteDueExplorationMissionsResult Invalid(params string[] errors) =>
        new(false, 0, [], errors);
}

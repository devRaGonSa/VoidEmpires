namespace VoidEmpires.Application.StrategicMap;

public interface IExplorationMissionCompletionService
{
    Task<CompleteDueExplorationMissionsResult> CompleteDueAsync(
        CompleteDueExplorationMissionsRequest request,
        CancellationToken cancellationToken = default);
}

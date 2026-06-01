namespace VoidEmpires.Application.StrategicMap;

public interface IExplorationMissionQueryService
{
    Task<GetExplorationMissionsResult> GetAsync(
        GetExplorationMissionsRequest request,
        CancellationToken cancellationToken = default);
}

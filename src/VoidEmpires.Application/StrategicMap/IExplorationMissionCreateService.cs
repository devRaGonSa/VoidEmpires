namespace VoidEmpires.Application.StrategicMap;

public interface IExplorationMissionCreateService
{
    Task<CreateExplorationMissionResult> CreateAsync(
        CreateExplorationMissionRequest request,
        CancellationToken cancellationToken = default);
}

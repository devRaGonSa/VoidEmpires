namespace VoidEmpires.Application.StrategicMap;

public interface IExplorationActionPreviewService
{
    Task<GetExplorationActionPreviewResult> GetAsync(
        GetExplorationActionPreviewRequest request,
        CancellationToken cancellationToken = default);
}

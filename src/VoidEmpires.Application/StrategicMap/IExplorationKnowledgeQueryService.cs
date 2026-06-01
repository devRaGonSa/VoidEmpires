namespace VoidEmpires.Application.StrategicMap;

public interface IExplorationKnowledgeQueryService
{
    Task<GetExplorationKnowledgeResult> GetAsync(
        GetExplorationKnowledgeRequest request,
        CancellationToken cancellationToken = default);
}

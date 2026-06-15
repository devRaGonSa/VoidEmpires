namespace VoidEmpires.Application.Gameplay;

public interface IGameplayQueueMaterializationService
{
    Task<MaterializeGameplayQueuesResult> MaterializeDueAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record MaterializeGameplayQueuesRequest(
    Guid CivilizationId,
    Guid? PlanetId,
    DateTime NowUtc,
    bool IncludeConstruction,
    bool IncludeResearch,
    bool IncludeShipyard);

public sealed record MaterializeGameplayQueuesResult(
    bool Succeeded,
    QueueMaterializationSummary Construction,
    QueueMaterializationSummary Research,
    QueueMaterializationSummary Shipyard,
    IReadOnlyList<string> Notes);

public sealed record QueueMaterializationSummary(
    int ProcessedCount,
    int DueCount,
    int SkippedNotDueCount);

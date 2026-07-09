using VoidEmpires.Application.Economy;

namespace VoidEmpires.Application.Gameplay;

public interface IGameplayRefreshService
{
    Task<GameplayRefreshResult> RefreshAsync(
        GameplayRefreshRequest request,
        CancellationToken cancellationToken = default);
}

public sealed record GameplayRefreshRequest(
    Guid CivilizationId,
    Guid? PlanetId,
    DateTime NowUtc,
    bool IncludeResources = true,
    bool IncludeConstruction = true,
    bool IncludeResearch = true,
    bool IncludeProduction = true);

public sealed record GameplayRefreshResult(
    bool Succeeded,
    ResourceRefreshSummary Resources,
    QueueMaterializationSummary Construction,
    QueueMaterializationSummary Research,
    QueueMaterializationSummary Production,
    IReadOnlyList<string> Notes,
    IReadOnlyList<string> Errors);

public sealed record ResourceRefreshSummary(
    bool Attempted,
    TimeSpan AppliedElapsed,
    int ProcessedPlanetCount,
    ApplyPlanetProductionResult? Result);

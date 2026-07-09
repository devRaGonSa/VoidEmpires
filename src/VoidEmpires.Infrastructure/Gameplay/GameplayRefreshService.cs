using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Gameplay;

namespace VoidEmpires.Infrastructure.Gameplay;

public sealed class GameplayRefreshService(
    IPlanetEconomyTickService economyTickService,
    IGameplayQueueMaterializationService queueMaterializationService)
    : IGameplayRefreshService
{
    private static readonly QueueMaterializationSummary EmptyQueueSummary = new(0, 0, 0);
    private static readonly ResourceRefreshSummary EmptyResourceSummary = new(false, TimeSpan.Zero, null);

    public async Task<GameplayRefreshResult> RefreshAsync(
        GameplayRefreshRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return Failure("Civilization id is required.");
        }

        if (request.PlanetId == Guid.Empty)
        {
            return Failure("Planet id must be omitted or non-empty.");
        }

        if (request.NowUtc.Kind != DateTimeKind.Utc)
        {
            return Failure("Refresh date must be UTC.");
        }

        var notes = new List<string>();
        var resources = await RefreshResourcesAsync(request, notes, cancellationToken);
        if (resources.Result is { Succeeded: false })
        {
            return new(
                false,
                resources,
                EmptyQueueSummary,
                EmptyQueueSummary,
                EmptyQueueSummary,
                notes,
                resources.Result.Errors);
        }

        var queues = await queueMaterializationService.MaterializeDueAsync(
            new MaterializeGameplayQueuesRequest(
                request.CivilizationId,
                request.PlanetId,
                request.NowUtc,
                request.IncludeConstruction,
                request.IncludeResearch,
                request.IncludeProduction),
            cancellationToken);

        if (!queues.Succeeded)
        {
            return new(
                false,
                resources,
                queues.Construction,
                queues.Research,
                queues.Shipyard,
                [.. notes, .. queues.Notes],
                queues.Notes);
        }

        return new(
            true,
            resources,
            queues.Construction,
            queues.Research,
            queues.Shipyard,
            [.. notes, .. queues.Notes],
            []);
    }

    private async Task<ResourceRefreshSummary> RefreshResourcesAsync(
        GameplayRefreshRequest request,
        List<string> notes,
        CancellationToken cancellationToken)
    {
        if (!request.IncludeResources || request.PlanetId is null)
        {
            return EmptyResourceSummary;
        }

        var result = await economyTickService.ApplyProductionAsync(
            new ApplyPlanetProductionRequest(request.PlanetId.Value, request.CivilizationId, TimeSpan.Zero),
            cancellationToken);

        notes.Add("Resource refresh executed with zero elapsed time until persisted accrual state is available.");
        return new(true, TimeSpan.Zero, result);
    }

    private static GameplayRefreshResult Failure(string error) =>
        new(false, EmptyResourceSummary, EmptyQueueSummary, EmptyQueueSummary, EmptyQueueSummary, [], [error]);
}

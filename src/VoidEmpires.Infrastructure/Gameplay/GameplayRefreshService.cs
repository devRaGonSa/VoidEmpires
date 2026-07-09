using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Gameplay;

public sealed class GameplayRefreshService(
    VoidEmpiresDbContext dbContext,
    IPlanetEconomyTickService economyTickService,
    IGameplayQueueMaterializationService queueMaterializationService)
    : IGameplayRefreshService
{
    private static readonly QueueMaterializationSummary EmptyQueueSummary = new(0, 0, 0);
    private static readonly ResourceRefreshSummary EmptyResourceSummary = new(false, TimeSpan.Zero, 0, null);
    private static readonly TimeSpan MinimumResourceAccrualElapsed = TimeSpan.FromSeconds(1);

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

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId.Value, cancellationToken);

        if (stockpile is null)
        {
            return new(true, TimeSpan.Zero, 0, ApplyPlanetProductionResult.Failure("Planet resource stockpile was not found."));
        }

        var elapsed = request.NowUtc - stockpile.LastAccruedAtUtc;
        if (elapsed < MinimumResourceAccrualElapsed)
        {
            return new(true, TimeSpan.Zero, 0, ApplyPlanetProductionResult.Success(request.PlanetId.Value));
        }

        var result = await economyTickService.ApplyProductionAsync(
            new ApplyPlanetProductionRequest(request.PlanetId.Value, request.CivilizationId, elapsed),
            cancellationToken);

        if (!result.Succeeded)
        {
            return new(true, elapsed, 0, result);
        }

        stockpile.MarkAccrued(request.NowUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
        notes.Add("Resource refresh applied persisted elapsed production.");
        return new(true, elapsed, 1, result);
    }

    private static GameplayRefreshResult Failure(string error) =>
        new(false, EmptyResourceSummary, EmptyQueueSummary, EmptyQueueSummary, EmptyQueueSummary, [], [error]);
}

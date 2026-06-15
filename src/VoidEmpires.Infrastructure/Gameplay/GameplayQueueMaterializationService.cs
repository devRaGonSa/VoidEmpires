using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Gameplay;

public sealed class GameplayQueueMaterializationService(VoidEmpiresDbContext dbContext)
    : IGameplayQueueMaterializationService
{
    private static readonly QueueMaterializationSummary EmptySummary = new(0, 0, 0);

    public async Task<MaterializeGameplayQueuesResult> MaterializeDueAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty) return Failure("Civilization id is required.");
        if (request.PlanetId == Guid.Empty) return Failure("Planet id must be omitted or non-empty.");
        if (request.NowUtc.Kind != DateTimeKind.Utc) return Failure("Materialization date must be UTC.");

        var construction = request.IncludeConstruction
            ? await CountConstructionAsync(request, cancellationToken)
            : EmptySummary;
        var research = request.IncludeResearch
            ? await CountResearchAsync(request, cancellationToken)
            : EmptySummary;
        var shipyard = request.IncludeShipyard
            ? await CountShipyardAsync(request, cancellationToken)
            : EmptySummary;

        return new(true, construction, research, shipyard, ["Scoped materialization boundary prepared."]);
    }

    private async Task<QueueMaterializationSummary> CountConstructionAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken)
    {
        var query =
            from order in dbContext.PlanetConstructionOrders
            join ownership in dbContext.PlanetOwnerships on order.PlanetId equals ownership.PlanetId
            where ownership.CivilizationId == request.CivilizationId
                && ownership.Status == PlanetControlStatus.Active
                && (order.Status == ConstructionQueueItemStatus.Pending || order.Status == ConstructionQueueItemStatus.Active)
            select order;

        if (request.PlanetId is not null) query = query.Where(x => x.PlanetId == request.PlanetId.Value);

        return await CountOpenQueueAsync(query.Select(x => x.EndsAtUtc), request.NowUtc, cancellationToken);
    }

    private async Task<QueueMaterializationSummary> CountResearchAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.ResearchOrders
            .Where(x => x.CivilizationId == request.CivilizationId
                && (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active));

        if (request.PlanetId is not null) query = query.Where(x => x.SourcePlanetId == request.PlanetId.Value);

        return await CountOpenQueueAsync(query.Select(x => x.EndsAtUtc), request.NowUtc, cancellationToken);
    }

    private async Task<QueueMaterializationSummary> CountShipyardAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken)
    {
        var query =
            from order in dbContext.Set<AssetProductionOrder>()
            join ownership in dbContext.PlanetOwnerships on order.PlanetId equals ownership.PlanetId
            where ownership.CivilizationId == request.CivilizationId
                && ownership.Status == PlanetControlStatus.Active
                && order.Target == AssetProductionTarget.Orbital
                && (order.Status == AssetProductionOrderStatus.Pending || order.Status == AssetProductionOrderStatus.Active)
            select order;

        if (request.PlanetId is not null) query = query.Where(x => x.PlanetId == request.PlanetId.Value);

        return await CountOpenQueueAsync(query.Select(x => x.EndsAtUtc), request.NowUtc, cancellationToken);
    }

    private static async Task<QueueMaterializationSummary> CountOpenQueueAsync(
        IQueryable<DateTime> endsAtUtcQuery,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        var dueCount = await endsAtUtcQuery.CountAsync(x => x <= nowUtc, cancellationToken);
        var notDueCount = await endsAtUtcQuery.CountAsync(x => x > nowUtc, cancellationToken);

        return new(0, dueCount, notDueCount);
    }

    private static MaterializeGameplayQueuesResult Failure(string error) =>
        new(false, EmptySummary, EmptySummary, EmptySummary, [error]);
}

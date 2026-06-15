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

        var notes = new List<string>();
        var construction = request.IncludeConstruction
            ? await MaterializeConstructionAsync(request, notes, cancellationToken)
            : EmptySummary;
        var research = request.IncludeResearch
            ? await MaterializeResearchAsync(request, cancellationToken)
            : EmptySummary;
        var shipyard = request.IncludeShipyard
            ? await MaterializeShipyardAsync(request, notes, cancellationToken)
            : EmptySummary;

        if (notes.Count == 0) notes.Add("Scoped materialization completed.");
        return new(true, construction, research, shipyard, notes);
    }

    private async Task<QueueMaterializationSummary> MaterializeConstructionAsync(
        MaterializeGameplayQueuesRequest request,
        List<string> notes,
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

        var dueOrders = await query
            .Where(x => x.EndsAtUtc <= request.NowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
        var notDueCount = await query.CountAsync(x => x.EndsAtUtc > request.NowUtc, cancellationToken);
        var processedCount = 0;

        foreach (var order in dueOrders)
        {
            if (order.Action == ConstructionQueueItemAction.Construct)
            {
                var existingBuilding = await dbContext.PlanetBuildings
                    .SingleOrDefaultAsync(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType, cancellationToken);

                if (existingBuilding is null)
                {
                    var definition = BuildingCatalog.Get(order.BuildingType);
                    dbContext.PlanetBuildings.Add(PlanetBuilding.Create(order.PlanetId, order.BuildingType, order.TargetLevel, definition.Footprint));
                }
            }
            else if (order.Action == ConstructionQueueItemAction.Upgrade)
            {
                var building = await dbContext.PlanetBuildings
                    .SingleOrDefaultAsync(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType, cancellationToken);

                if (building is null)
                {
                    notes.Add($"Construction order {order.Id} was due but its target building was not found.");
                    continue;
                }

                while (building.Level < order.TargetLevel) building.Upgrade();
            }

            order.MarkCompleted();
            processedCount++;
        }

        if (processedCount > 0) await dbContext.SaveChangesAsync(cancellationToken);

        return new(processedCount, dueOrders.Count, notDueCount);
    }

    private async Task<QueueMaterializationSummary> MaterializeResearchAsync(
        MaterializeGameplayQueuesRequest request,
        CancellationToken cancellationToken)
    {
        var query = dbContext.ResearchOrders
            .Where(x => x.CivilizationId == request.CivilizationId
                && (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active));

        if (request.PlanetId is not null) query = query.Where(x => x.SourcePlanetId == request.PlanetId.Value);

        var dueOrders = await query
            .Where(x => x.EndsAtUtc <= request.NowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
        var notDueCount = await query.CountAsync(x => x.EndsAtUtc > request.NowUtc, cancellationToken);

        foreach (var order in dueOrders)
        {
            var project = await dbContext.ResearchProjects
                .SingleOrDefaultAsync(x => x.CivilizationId == order.CivilizationId && x.ResearchType == order.ResearchType, cancellationToken);

            if (project is null)
            {
                project = ResearchProject.Create(order.CivilizationId, order.ResearchType);
                dbContext.ResearchProjects.Add(project);
            }

            while (project.Level < order.TargetLevel) project.Upgrade();
            order.MarkCompleted();
        }

        if (dueOrders.Count > 0) await dbContext.SaveChangesAsync(cancellationToken);

        return new(dueOrders.Count, dueOrders.Count, notDueCount);
    }

    private async Task<QueueMaterializationSummary> MaterializeShipyardAsync(
        MaterializeGameplayQueuesRequest request,
        List<string> notes,
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

        var dueOrders = await query
            .Where(x => x.EndsAtUtc <= request.NowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);
        var notDueCount = await query.CountAsync(x => x.EndsAtUtc > request.NowUtc, cancellationToken);
        var processedCount = 0;

        foreach (var order in dueOrders)
        {
            if (order.SpaceAssetType is not { } assetType)
            {
                notes.Add($"Shipyard order {order.Id} was due but its orbital asset type was missing.");
                continue;
            }

            var stock = await dbContext.Set<OrbitalAssetStock>()
                .SingleOrDefaultAsync(x => x.PlanetId == order.PlanetId && x.AssetType == assetType, cancellationToken);

            if (stock is null)
            {
                stock = OrbitalAssetStock.Create(order.PlanetId, assetType);
                dbContext.Set<OrbitalAssetStock>().Add(stock);
            }

            stock.Increase(order.Quantity);
            order.MarkCompleted();
            processedCount++;
        }

        if (processedCount > 0) await dbContext.SaveChangesAsync(cancellationToken);

        return new(processedCount, dueOrders.Count, notDueCount);
    }

    private static MaterializeGameplayQueuesResult Failure(string error) =>
        new(false, EmptySummary, EmptySummary, EmptySummary, [error]);
}

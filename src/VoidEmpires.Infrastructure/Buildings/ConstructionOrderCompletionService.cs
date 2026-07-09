using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Buildings;

public sealed class ConstructionOrderCompletionService(VoidEmpiresDbContext dbContext) : IConstructionOrderCompletionService
{
    public async Task<CompleteConstructionOrdersResult> CompleteDueOrdersAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (nowUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Completion date must be UTC.", nameof(nowUtc));
        }

        await using var transaction = await BeginCompletionTransactionAsync(cancellationToken);
        var dueOrders = await dbContext.PlanetConstructionOrders
            .Where(x =>
                (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active) &&
                x.EndsAtUtc <= nowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);

        var completedOrderIds = new List<Guid>();

        foreach (var order in dueOrders)
        {
            if (order.Action == ConstructionQueueItemAction.Upgrade &&
                !await dbContext.PlanetBuildings.AnyAsync(
                    x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType,
                    cancellationToken))
            {
                continue;
            }

            if (!await TryClaimOrderAsync(order, nowUtc, cancellationToken))
            {
                continue;
            }

            if (order.Action == ConstructionQueueItemAction.Construct)
            {
                var existingBuilding = await dbContext.PlanetBuildings
                    .Where(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType)
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Footprint)
                    .FirstOrDefaultAsync(cancellationToken);

                if (existingBuilding is null)
                {
                    var definition = BuildingCatalog.Get(order.BuildingType);
                    var building = PlanetBuilding.Create(
                        order.PlanetId,
                        order.BuildingType,
                        order.TargetLevel,
                        definition.Footprint);

                    dbContext.PlanetBuildings.Add(building);
                }
                else
                {
                    while (existingBuilding.Level < order.TargetLevel)
                    {
                        existingBuilding.Upgrade();
                    }
                }
            }
            else if (order.Action == ConstructionQueueItemAction.Upgrade)
            {
                var building = await dbContext.PlanetBuildings
                    .Where(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType)
                    .OrderByDescending(x => x.Level)
                    .ThenByDescending(x => x.Footprint)
                    .FirstOrDefaultAsync(cancellationToken);

                if (building is null)
                {
                    continue;
                }

                while (building.Level < order.TargetLevel)
                {
                    building.Upgrade();
                }
            }

            completedOrderIds.Add(order.Id);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        if (transaction is not null) await transaction.CommitAsync(cancellationToken);

        return new CompleteConstructionOrdersResult(completedOrderIds.Count, completedOrderIds);
    }

    private async Task<IDbContextTransaction?> BeginCompletionTransactionAsync(CancellationToken cancellationToken) =>
        dbContext.Database.IsRelational()
            ? await dbContext.Database.BeginTransactionAsync(IsolationLevel.Serializable, cancellationToken)
            : null;

    private async Task<bool> TryClaimOrderAsync(
        PlanetConstructionOrder order,
        DateTime nowUtc,
        CancellationToken cancellationToken)
    {
        if (!dbContext.Database.IsRelational())
        {
            if (!order.IsOpen || order.EndsAtUtc > nowUtc) return false;
            order.MarkCompleted();
            return true;
        }

        var updated = await dbContext.PlanetConstructionOrders
            .Where(x =>
                x.Id == order.Id &&
                (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active) &&
                x.EndsAtUtc <= nowUtc)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(x => x.Status, ConstructionQueueItemStatus.Completed),
                cancellationToken);

        return updated == 1;
    }
}

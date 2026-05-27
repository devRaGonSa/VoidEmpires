using Microsoft.EntityFrameworkCore;
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
            if (order.Action == ConstructionQueueItemAction.Construct)
            {
                var existingBuilding = await dbContext.PlanetBuildings
                    .SingleOrDefaultAsync(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType, cancellationToken);

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
            }
            else if (order.Action == ConstructionQueueItemAction.Upgrade)
            {
                var building = await dbContext.PlanetBuildings
                    .SingleOrDefaultAsync(x => x.PlanetId == order.PlanetId && x.BuildingType == order.BuildingType, cancellationToken);

                if (building is null)
                {
                    continue;
                }

                while (building.Level < order.TargetLevel)
                {
                    building.Upgrade();
                }
            }

            order.MarkCompleted();
            completedOrderIds.Add(order.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteConstructionOrdersResult(completedOrderIds.Count, completedOrderIds);
    }
}

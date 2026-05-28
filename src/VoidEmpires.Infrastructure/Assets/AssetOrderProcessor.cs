using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Assets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Assets;

public sealed class AssetOrderProcessor(VoidEmpiresDbContext dbContext) : IAssetOrderProcessor
{
    public async Task<CompleteAssetProductionOrdersResult> ProcessAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (nowUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Processing date must be UTC.", nameof(nowUtc));
        }

        var dueOrders = await dbContext.Set<AssetProductionOrder>()
            .Where(x =>
                (x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active) &&
                x.EndsAtUtc <= nowUtc)
            .OrderBy(x => x.EndsAtUtc)
            .ThenBy(x => x.Sequence)
            .ToListAsync(cancellationToken);

        var processedOrderIds = new List<Guid>();

        foreach (var order in dueOrders)
        {
            if (order.Target == AssetProductionTarget.Planetary)
            {
                await IncreasePlanetaryStockAsync(order, cancellationToken);
            }
            else
            {
                await IncreaseOrbitalStockAsync(order, cancellationToken);
            }

            order.MarkCompleted();
            processedOrderIds.Add(order.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteAssetProductionOrdersResult(processedOrderIds.Count, processedOrderIds);
    }

    private async Task IncreasePlanetaryStockAsync(
        AssetProductionOrder order,
        CancellationToken cancellationToken)
    {
        var assetType = order.PlanetaryAssetType ?? throw new InvalidOperationException("Planetary asset type is required.");
        var stock = await dbContext.Set<PlanetaryAssetStock>()
            .SingleOrDefaultAsync(
                x => x.PlanetId == order.PlanetId && x.AssetType == assetType,
                cancellationToken);

        if (stock is null)
        {
            stock = PlanetaryAssetStock.Create(order.PlanetId, assetType);
            dbContext.Set<PlanetaryAssetStock>().Add(stock);
        }

        stock.Increase(order.Quantity);
    }

    private async Task IncreaseOrbitalStockAsync(
        AssetProductionOrder order,
        CancellationToken cancellationToken)
    {
        var assetType = order.SpaceAssetType ?? throw new InvalidOperationException("Space asset type is required.");
        var stock = await dbContext.Set<OrbitalAssetStock>()
            .SingleOrDefaultAsync(
                x => x.PlanetId == order.PlanetId && x.AssetType == assetType,
                cancellationToken);

        if (stock is null)
        {
            stock = OrbitalAssetStock.Create(order.PlanetId, assetType);
            dbContext.Set<OrbitalAssetStock>().Add(stock);
        }

        stock.Increase(order.Quantity);
    }
}

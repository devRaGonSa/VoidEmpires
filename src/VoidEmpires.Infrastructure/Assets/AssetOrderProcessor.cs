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
            order.MarkCompleted();
            processedOrderIds.Add(order.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteAssetProductionOrdersResult(processedOrderIds.Count, processedOrderIds);
    }
}

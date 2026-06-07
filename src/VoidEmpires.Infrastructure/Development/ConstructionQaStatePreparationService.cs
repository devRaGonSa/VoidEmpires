using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Development;

public sealed class ConstructionQaStatePreparationService(VoidEmpiresDbContext dbContext) : IConstructionQaStatePreparationService
{
    public static readonly Guid DefaultCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid DefaultPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private const decimal MinimumCredits = 1000m;
    private const decimal MinimumMetal = 1000m;
    private const decimal MinimumCrystal = 1000m;
    private const decimal MinimumGas = 1000m;

    public async Task<ConstructionQaStatePreparationResult> PrepareAsync(
        ConstructionQaStatePreparationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var civilizationId = request.CivilizationId ?? DefaultCivilizationId;
        var planetId = request.PlanetId ?? DefaultPlanetId;

        var ownedTarget = await dbContext.PlanetOwnerships.AnyAsync(
            x => x.PlanetId == planetId &&
                x.CivilizationId == civilizationId &&
                x.Status == PlanetControlStatus.Active,
            cancellationToken);

        if (!ownedTarget)
        {
            return ConstructionQaStatePreparationResult.Failure(
                civilizationId,
                planetId,
                ["Target planet is not owned by the requested civilization or does not exist."]);
        }

        var openOrders = await dbContext.PlanetConstructionOrders
            .Where(x =>
                x.PlanetId == planetId &&
                (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active))
            .ToListAsync(cancellationToken);

        var blockingOrdersBefore = openOrders.Count;

        foreach (var openOrder in openOrders)
        {
            openOrder.MarkCancelled();
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == planetId, cancellationToken);

        var resourcesBefore = stockpile is null
            ? null
            : new ConstructionQaStatePreparationResourceState(stockpile.Credits, stockpile.Metal, stockpile.Crystal, stockpile.Gas);
        var resourcesAfter = resourcesBefore;
        var notes = new List<string>();

        if (stockpile is null)
        {
            notes.Add("Target planet resource stockpile was not found. Resource top-up was skipped.");
        }
        else
        {
            EnsureResourceMinimum(stockpile, ResourceType.Credits, MinimumCredits);
            EnsureResourceMinimum(stockpile, ResourceType.Metal, MinimumMetal);
            EnsureResourceMinimum(stockpile, ResourceType.Crystal, MinimumCrystal);
            EnsureResourceMinimum(stockpile, ResourceType.Gas, MinimumGas);
            resourcesAfter = new ConstructionQaStatePreparationResourceState(
                stockpile.Credits,
                stockpile.Metal,
                stockpile.Crystal,
                stockpile.Gas);
            notes.Add("Open construction orders were cancelled.");
            notes.Add("Resources were topped up to QA minimums.");
        }

        if (blockingOrdersBefore == 0)
        {
            notes.Add("No open blocking construction orders were found.");
        }

        if (openOrders.Count > 0)
        {
            notes.Add("Construction queue is now unblocked.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ConstructionQaStatePreparationResult.Success(
            civilizationId,
            planetId,
            blockingOrdersBefore,
            0,
            resourcesBefore,
            resourcesAfter,
            notes);
    }

    private static void EnsureResourceMinimum(
        PlanetResourceStockpile stockpile,
        ResourceType resourceType,
        decimal minimum)
    {
        if (stockpile.HasAtLeast(resourceType, minimum))
        {
            return;
        }

        var current = resourceType switch
        {
            ResourceType.Credits => stockpile.Credits,
            ResourceType.Metal => stockpile.Metal,
            ResourceType.Crystal => stockpile.Crystal,
            ResourceType.Gas => stockpile.Gas,
            _ => 0m
        };

        stockpile.Increase(resourceType, minimum - current);
    }
}

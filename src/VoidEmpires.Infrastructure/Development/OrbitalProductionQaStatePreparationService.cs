using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Development;

public sealed class OrbitalProductionQaStatePreparationService(VoidEmpiresDbContext dbContext) : IOrbitalProductionQaStatePreparationService
{
    public static readonly Guid DefaultCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid DefaultPlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private const decimal MinimumCredits = 125m;
    private const decimal MinimumMetal = 160m;
    private const decimal MinimumCrystal = 100m;
    private const decimal MinimumGas = 50m;

    public async Task<OrbitalProductionQaStatePreparationResult> PrepareAsync(
        OrbitalProductionQaStatePreparationRequest request,
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
            return OrbitalProductionQaStatePreparationResult.Failure(
                civilizationId,
                planetId,
                ["Target planet is not owned by the requested civilization or does not exist."]);
        }

        var openOrders = await dbContext.Set<AssetProductionOrder>()
            .Where(x =>
                x.PlanetId == planetId &&
                (x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active))
            .ToListAsync(cancellationToken);

        foreach (var openOrder in openOrders)
        {
            openOrder.MarkCancelled();
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == planetId, cancellationToken);

        var resourcesBefore = stockpile is null
            ? null
            : new OrbitalProductionQaStatePreparationResourceState(stockpile.Credits, stockpile.Metal, stockpile.Crystal, stockpile.Gas);
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
            resourcesAfter = new OrbitalProductionQaStatePreparationResourceState(
                stockpile.Credits,
                stockpile.Metal,
                stockpile.Crystal,
                stockpile.Gas);
            notes.Add("Open asset production orders were cancelled for the targeted planet.");
            notes.Add("Targeted planet resources were topped up to the shared orbital QA minimums.");
        }

        if (openOrders.Count == 0)
        {
            notes.Add("No open blocking asset production orders were found.");
        }
        else
        {
            notes.Add("Orbital production queue is now unblocked for the targeted planet.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return OrbitalProductionQaStatePreparationResult.Success(
            civilizationId,
            planetId,
            openOrders.Count,
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

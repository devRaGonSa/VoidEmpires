using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Development;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Development;

public sealed class ResearchQaStatePreparationService(VoidEmpiresDbContext dbContext) : IResearchQaStatePreparationService
{
    public static readonly Guid DefaultCivilizationId = Guid.Parse("00000000-0000-0000-0000-000000000001");
    public static readonly Guid DefaultSourcePlanetId = Guid.Parse("40000000-0000-0000-0000-000000000001");
    private const decimal MinimumCredits = 125m;
    private const decimal MinimumMetal = 110m;
    private const decimal MinimumCrystal = 70m;
    private const decimal MinimumGas = 30m;

    public async Task<ResearchQaStatePreparationResult> PrepareAsync(
        ResearchQaStatePreparationRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var civilizationId = request.CivilizationId ?? DefaultCivilizationId;
        var sourcePlanetId = request.SourcePlanetId ?? DefaultSourcePlanetId;

        var ownedTarget = await dbContext.PlanetOwnerships.AnyAsync(
            x => x.PlanetId == sourcePlanetId &&
                x.CivilizationId == civilizationId &&
                x.Status == PlanetControlStatus.Active,
            cancellationToken);

        if (!ownedTarget)
        {
            return ResearchQaStatePreparationResult.Failure(
                civilizationId,
                sourcePlanetId,
                ["Target source planet is not owned by the requested civilization or does not exist."]);
        }

        var openOrders = await dbContext.ResearchOrders
            .Where(x =>
                x.CivilizationId == civilizationId &&
                (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active))
            .ToListAsync(cancellationToken);

        foreach (var openOrder in openOrders)
        {
            openOrder.MarkCancelled();
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == sourcePlanetId, cancellationToken);

        var resourcesBefore = stockpile is null
            ? null
            : new ResearchQaStatePreparationResourceState(stockpile.Credits, stockpile.Metal, stockpile.Crystal, stockpile.Gas);
        var resourcesAfter = resourcesBefore;
        var notes = new List<string>();

        if (stockpile is null)
        {
            notes.Add("Target source-planet resource stockpile was not found. Resource top-up was skipped.");
        }
        else
        {
            EnsureResourceMinimum(stockpile, ResourceType.Credits, MinimumCredits);
            EnsureResourceMinimum(stockpile, ResourceType.Metal, MinimumMetal);
            EnsureResourceMinimum(stockpile, ResourceType.Crystal, MinimumCrystal);
            EnsureResourceMinimum(stockpile, ResourceType.Gas, MinimumGas);
            resourcesAfter = new ResearchQaStatePreparationResourceState(
                stockpile.Credits,
                stockpile.Metal,
                stockpile.Crystal,
                stockpile.Gas);
            notes.Add("Open research orders were cancelled.");
            notes.Add("Source-planet resources were topped up to deterministic QA minimums.");
        }

        if (openOrders.Count == 0)
        {
            notes.Add("No open blocking research orders were found.");
        }
        else
        {
            notes.Add("Research queue is now unblocked for the targeted civilization.");
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return ResearchQaStatePreparationResult.Success(
            civilizationId,
            sourcePlanetId,
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

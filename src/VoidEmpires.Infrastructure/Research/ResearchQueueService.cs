using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Research;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Research;

public sealed class ResearchQueueService(VoidEmpiresDbContext dbContext) : IResearchQueueService
{
    private static readonly TimeSpan BaseResearchDuration = TimeSpan.FromMinutes(10);

    public async Task<EnqueueResearchOrderResult> EnqueueAsync(
        EnqueueResearchOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return EnqueueResearchOrderResult.Failure("Civilization id is required.");
        }

        if (request.SourcePlanetId == Guid.Empty)
        {
            return EnqueueResearchOrderResult.Failure("Source planet id is required.");
        }

        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc)
        {
            return EnqueueResearchOrderResult.Failure("Requested date must be UTC.");
        }

        var hasOpenOrder = await dbContext.ResearchOrders
            .AnyAsync(x => x.CivilizationId == request.CivilizationId &&
                (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active),
                cancellationToken);

        if (hasOpenOrder)
        {
            return EnqueueResearchOrderResult.Failure("Civilization already has an open research order.");
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.SourcePlanetId, cancellationToken);

        if (stockpile is null)
        {
            return EnqueueResearchOrderResult.Failure("Planet resource stockpile was not found.");
        }

        var project = await dbContext.ResearchProjects
            .SingleOrDefaultAsync(
                x => x.CivilizationId == request.CivilizationId && x.ResearchType == request.ResearchType,
                cancellationToken);

        var targetLevel = (project?.Level ?? 0) + 1;
        var definition = ResearchCatalog.Get(request.ResearchType);
        var credits = definition.BaseCost.Credits * targetLevel;
        var metal = definition.BaseCost.Metal * targetLevel;
        var crystal = definition.BaseCost.Crystal * targetLevel;
        var gas = definition.BaseCost.Gas * targetLevel;

        if (!stockpile.CanSpend(credits, metal, crystal, gas))
        {
            return EnqueueResearchOrderResult.Failure("Insufficient resources.");
        }

        var energySystemsLevel = await dbContext.ResearchProjects
            .Where(x => x.CivilizationId == request.CivilizationId && x.ResearchType == ResearchType.EnergySystems)
            .Select(x => x.Level)
            .SingleOrDefaultAsync(cancellationToken);

        var effectiveDuration = ResearchDurationCalculator.CalculateDuration(
            BaseResearchDuration * targetLevel,
            energySystemsLevel);

        var startsAtUtc = request.RequestedAtUtc;
        var endsAtUtc = startsAtUtc.Add(effectiveDuration);

        var nextSequence = await dbContext.ResearchOrders
            .Where(x => x.CivilizationId == request.CivilizationId)
            .Select(x => (int?)x.Sequence)
            .MaxAsync(cancellationToken) ?? 0;

        var order = ResearchOrder.Create(
            request.CivilizationId,
            request.SourcePlanetId,
            request.ResearchType,
            targetLevel,
            nextSequence + 1,
            startsAtUtc,
            endsAtUtc,
            ResearchQueueItemStatus.Active);

        stockpile.Spend(credits, metal, crystal, gas);
        dbContext.ResearchOrders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EnqueueResearchOrderResult.Success(order.Id, startsAtUtc, endsAtUtc);
    }
}

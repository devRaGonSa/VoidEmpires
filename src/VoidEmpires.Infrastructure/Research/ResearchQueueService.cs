using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Colonization;
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

        var hasOwnedSourcePlanet = await dbContext.PlanetOwnerships
            .AnyAsync(
                x => x.PlanetId == request.SourcePlanetId &&
                    x.CivilizationId == request.CivilizationId &&
                    x.Status == PlanetControlStatus.Active,
                cancellationToken);

        var hasOpenOrder = await dbContext.ResearchOrders
            .AnyAsync(x => x.CivilizationId == request.CivilizationId &&
                (x.Status == ResearchQueueItemStatus.Pending || x.Status == ResearchQueueItemStatus.Active),
                cancellationToken);

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.SourcePlanetId, cancellationToken);

        var project = await dbContext.ResearchProjects
            .SingleOrDefaultAsync(
                x => x.CivilizationId == request.CivilizationId && x.ResearchType == request.ResearchType,
                cancellationToken);

        var readiness = ResearchEnqueueReadinessEvaluator.Evaluate(
            hasOwnedSourcePlanet,
            hasOpenOrder,
            stockpile,
            request.ResearchType,
            project?.Level ?? 0);

        if (!readiness.CanEnqueue)
        {
            return EnqueueResearchOrderResult.Failure(readiness.Error!);
        }

        var energySystemsLevel = await dbContext.ResearchProjects
            .Where(x => x.CivilizationId == request.CivilizationId && x.ResearchType == ResearchType.EnergySystems)
            .Select(x => x.Level)
            .SingleOrDefaultAsync(cancellationToken);

        var effectiveDuration = ResearchDurationCalculator.CalculateDuration(
            BaseResearchDuration * readiness.TargetLevel,
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
            readiness.TargetLevel,
            nextSequence + 1,
            startsAtUtc,
            endsAtUtc,
            ResearchQueueItemStatus.Active);

        stockpile!.Spend(
            readiness.Cost.Credits,
            readiness.Cost.Metal,
            readiness.Cost.Crystal,
            readiness.Cost.Gas);
        dbContext.ResearchOrders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EnqueueResearchOrderResult.Success(order.Id, startsAtUtc, endsAtUtc);
    }
}

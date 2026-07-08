using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Buildings;

public sealed class PlanetConstructionQueueService(VoidEmpiresDbContext dbContext) : IPlanetConstructionQueueService
{
    private static readonly TimeSpan BaseConstructionDuration = TimeSpan.FromMinutes(5);

    public async Task<EnqueueConstructionOrderResult> EnqueueAsync(
        EnqueueConstructionOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return EnqueueConstructionOrderResult.Failure("Planet id is required.");
        }

        if (request.CivilizationId == Guid.Empty)
        {
            return EnqueueConstructionOrderResult.Failure("Civilization id is required.");
        }

        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc)
        {
            return EnqueueConstructionOrderResult.Failure("Requested date must be UTC.");
        }

        var hasOwnedPlanet = await dbContext.PlanetOwnerships
            .AnyAsync(
                x => x.PlanetId == request.PlanetId &&
                    x.CivilizationId == request.CivilizationId &&
                    x.Status == PlanetControlStatus.Active,
                cancellationToken);

        if (!hasOwnedPlanet)
        {
            return EnqueueConstructionOrderResult.Failure("Planet is not owned by the requesting civilization.");
        }

        var hasOpenOrder = await dbContext.PlanetConstructionOrders
            .AnyAsync(x => x.PlanetId == request.PlanetId &&
                (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active),
                cancellationToken);

        if (hasOpenOrder)
        {
            return EnqueueConstructionOrderResult.Failure("Planet already has an open construction order.");
        }

        var definition = BuildingCatalog.Get(request.BuildingType);
        var targetLevel = definition.InitialLevel;
        var costMultiplier = 1;

        if (request.Action == ConstructionQueueItemAction.Upgrade)
        {
            var existingBuilding = await dbContext.PlanetBuildings
                .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId && x.BuildingType == request.BuildingType, cancellationToken);

            if (existingBuilding is null)
            {
                return EnqueueConstructionOrderResult.Failure("Building was not found.");
            }

            targetLevel = existingBuilding.Level + 1;
            costMultiplier = targetLevel;
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId, cancellationToken);

        if (stockpile is null)
        {
            return EnqueueConstructionOrderResult.Failure("Planet resource stockpile was not found.");
        }

        var creditsCost = definition.Cost.Credits * costMultiplier;
        var metalCost = definition.Cost.Metal * costMultiplier;
        var crystalCost = definition.Cost.Crystal * costMultiplier;
        var gasCost = definition.Cost.Gas * costMultiplier;

        if (!stockpile.CanSpend(creditsCost, metalCost, crystalCost, gasCost))
        {
            return EnqueueConstructionOrderResult.Failure("Insufficient resources.");
        }

        if (request.Action == ConstructionQueueItemAction.Construct)
        {
            var capacity = await EnsureBuildingCapacityAsync(request.PlanetId, cancellationToken);

            var usedCapacity = await dbContext.PlanetBuildings
                .Where(x => x.PlanetId == request.PlanetId)
                .SumAsync(x => x.Footprint, cancellationToken);

            var planetaryEngineeringLevel = await dbContext.ResearchProjects
                .Where(x => x.CivilizationId == request.CivilizationId && x.ResearchType == ResearchType.PlanetaryEngineering)
                .Select(x => x.Level)
                .SingleOrDefaultAsync(cancellationToken);

            var capacityBonus = ResearchBonusCalculator.GetPlanetaryEngineeringCapacityBonus(planetaryEngineeringLevel);

            if (!capacity.CanFit(usedCapacity, definition.Footprint, capacityBonus))
            {
                return EnqueueConstructionOrderResult.Failure("Planet building capacity would be exceeded.");
            }
        }

        var constructionAutomationLevel = await dbContext.ResearchProjects
            .Where(x => x.CivilizationId == request.CivilizationId && x.ResearchType == ResearchType.ConstructionAutomation)
            .Select(x => x.Level)
            .SingleOrDefaultAsync(cancellationToken);

        var effectiveDuration = ConstructionDurationCalculator.CalculateDuration(
            BaseConstructionDuration * targetLevel,
            constructionAutomationLevel);

        var startsAtUtc = request.RequestedAtUtc;
        var endsAtUtc = startsAtUtc.Add(effectiveDuration);

        var nextSequence = await dbContext.PlanetConstructionOrders
            .Where(x => x.PlanetId == request.PlanetId)
            .Select(x => (int?)x.Sequence)
            .MaxAsync(cancellationToken) ?? 0;

        var order = PlanetConstructionOrder.Create(
            request.PlanetId,
            request.Action,
            request.BuildingType,
            targetLevel,
            nextSequence + 1,
            startsAtUtc,
            endsAtUtc,
            ConstructionQueueItemStatus.Active);

        stockpile.Spend(creditsCost, metalCost, crystalCost, gasCost);
        dbContext.PlanetConstructionOrders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EnqueueConstructionOrderResult.Success(order.Id, startsAtUtc, endsAtUtc);
    }

    private async Task<PlanetBuildingCapacity> EnsureBuildingCapacityAsync(
        Guid planetId,
        CancellationToken cancellationToken)
    {
        var capacity = await dbContext.PlanetBuildingCapacities
            .SingleOrDefaultAsync(x => x.PlanetId == planetId, cancellationToken);

        if (capacity is not null)
        {
            return capacity;
        }

        var planetSize = await dbContext.Set<Planet>()
            .Where(x => x.Id == planetId)
            .Select(x => x.Size)
            .SingleAsync(cancellationToken);

        capacity = PlanetBuildingCapacityDefaults.Create(planetId, planetSize);
        dbContext.PlanetBuildingCapacities.Add(capacity);
        return capacity;
    }
}

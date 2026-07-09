using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Assets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Assets;

public sealed class AssetProductionQueueService(VoidEmpiresDbContext dbContext) : IAssetProductionQueueService
{
    private static readonly TimeSpan BaseDuration = TimeSpan.FromMinutes(3);

    public async Task<EnqueueAssetProductionResult> EnqueueAsync(
        EnqueueAssetProductionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return EnqueueAssetProductionResult.Failure("Planet id is required.");
        }

        if (request.Quantity <= 0)
        {
            return EnqueueAssetProductionResult.Failure("Quantity must be positive.");
        }

        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc)
        {
            return EnqueueAssetProductionResult.Failure("Requested date must be UTC.");
        }

        var requirement = GetRequirement(request);
        if (requirement is null)
        {
            return EnqueueAssetProductionResult.Failure("Asset type is required.");
        }

        var blockingPlanetaryTypes = GetBlockingPlanetaryTypes(request);
        var hasOpenOrder = await dbContext.Set<AssetProductionOrder>()
            .AnyAsync(x =>
                x.PlanetId == request.PlanetId &&
                (x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active) &&
                (request.Target == AssetProductionTarget.Orbital
                    ? x.Target == AssetProductionTarget.Orbital
                    : x.Target == AssetProductionTarget.Planetary &&
                        x.PlanetaryAssetType != null &&
                        blockingPlanetaryTypes.Contains(x.PlanetaryAssetType.Value)),
                cancellationToken);

        if (hasOpenOrder)
        {
            return EnqueueAssetProductionResult.Failure("Planet already has an open asset production order.");
        }

        var requiredBuildingInProgress = await dbContext.Set<PlanetConstructionOrder>()
            .AnyAsync(x =>
                x.PlanetId == request.PlanetId &&
                x.BuildingType == requirement.RequiredBuildingType &&
                (x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active),
                cancellationToken);

        if (requiredBuildingInProgress)
        {
            return EnqueueAssetProductionResult.Failure("Required building is currently being built or upgraded.");
        }

        var cost = GetCost(request);
        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId, cancellationToken);

        if (stockpile is null)
        {
            return EnqueueAssetProductionResult.Failure("Planet resource stockpile was not found.");
        }

        var credits = cost.Credits * request.Quantity;
        var metal = cost.Metal * request.Quantity;
        var crystal = cost.Crystal * request.Quantity;
        var gas = cost.Gas * request.Quantity;

        if (!stockpile.CanSpend(credits, metal, crystal, gas))
        {
            return EnqueueAssetProductionResult.Failure("Insufficient resources.");
        }

        var buildings = await dbContext.PlanetBuildings
            .Where(x => x.PlanetId == request.PlanetId)
            .ToListAsync(cancellationToken);

        var requiredBuilding = buildings.SingleOrDefault(x => x.BuildingType == requirement.RequiredBuildingType);
        if (requiredBuilding is null || requiredBuilding.Level < requirement.RequiredBuildingLevel)
        {
            return EnqueueAssetProductionResult.Failure("Required building is missing or below required level.");
        }

        var population = await dbContext.PlanetPopulationProfiles
            .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId, cancellationToken);

        if (population is null)
        {
            return EnqueueAssetProductionResult.Failure("Planet population profile was not found.");
        }

        if (request.Target == AssetProductionTarget.Planetary)
        {
            var groundCapacity = PlanetMilitaryCapacityCalculator.CalculateGroundForceCapacity(population, buildings);
            if (requirement.PopulationCapacity * request.Quantity > groundCapacity)
            {
                return EnqueueAssetProductionResult.Failure("Insufficient local population capacity.");
            }
        }
        else
        {
            var crewCapacity = PlanetMilitaryCapacityCalculator.CalculateShipCrewCapacity(population, buildings);
            if (requirement.OperatorCapacity * request.Quantity > crewCapacity)
            {
                return EnqueueAssetProductionResult.Failure("Insufficient local operator capacity.");
            }
        }

        var nextSequence = await dbContext.Set<AssetProductionOrder>()
            .Where(x => x.PlanetId == request.PlanetId)
            .Select(x => (int?)x.Sequence)
            .MaxAsync(cancellationToken) ?? 0;

        var startsAtUtc = request.RequestedAtUtc;
        var endsAtUtc = startsAtUtc.Add(BaseDuration * request.Quantity);
        var order = AssetProductionOrder.Create(
            request.PlanetId,
            request.Target,
            request.PlanetaryAssetType,
            request.SpaceAssetType,
            request.Quantity,
            nextSequence + 1,
            startsAtUtc,
            endsAtUtc,
            AssetProductionOrderStatus.Active);

        stockpile.Spend(credits, metal, crystal, gas);
        dbContext.Set<AssetProductionOrder>().Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return EnqueueAssetProductionResult.Success(order.Id, startsAtUtc, endsAtUtc);
    }

    private static AssetRequirement? GetRequirement(EnqueueAssetProductionRequest request) => request.Target switch
    {
        AssetProductionTarget.Planetary when request.PlanetaryAssetType is not null =>
            PlanetaryAssetCatalog.Get(request.PlanetaryAssetType.Value).Requirement,
        AssetProductionTarget.Orbital when request.SpaceAssetType is not null =>
            OrbitalAssetCatalog.Get(request.SpaceAssetType.Value).Requirement,
        _ => null
    };

    private static ConstructionCost GetCost(EnqueueAssetProductionRequest request) => request.Target switch
    {
        AssetProductionTarget.Planetary => PlanetaryAssetCatalog.Get(request.PlanetaryAssetType!.Value).Cost,
        AssetProductionTarget.Orbital => OrbitalAssetCatalog.Get(request.SpaceAssetType!.Value).Cost,
        _ => ConstructionCost.Zero
    };

    private static PlanetaryAssetType[] GetBlockingPlanetaryTypes(EnqueueAssetProductionRequest request)
    {
        if (request.Target != AssetProductionTarget.Planetary || request.PlanetaryAssetType is null)
        {
            return [];
        }

        return IsDefenseAsset(request.PlanetaryAssetType.Value)
            ? [PlanetaryAssetType.MissileBattery, PlanetaryAssetType.LaserTurret, PlanetaryAssetType.IonCannon, PlanetaryAssetType.PlasmaCannon]
            : [PlanetaryAssetType.PatrolGroup, PlanetaryAssetType.ExpeditionGroup, PlanetaryAssetType.VehicleGroup, PlanetaryAssetType.SupportGroup];
    }

    private static bool IsDefenseAsset(PlanetaryAssetType assetType) =>
        assetType is PlanetaryAssetType.MissileBattery or
            PlanetaryAssetType.LaserTurret or
            PlanetaryAssetType.IonCannon or
            PlanetaryAssetType.PlasmaCannon;
}

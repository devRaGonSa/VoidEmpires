using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTravelEstimateService(
    VoidEmpiresDbContext dbContext,
    IResourceSpendService resourceSpendService) : IOrbitalTravelEstimateService
{
    public async Task<EstimateOrbitalTravelResult> EstimateAsync(
        EstimateOrbitalTravelRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return EstimateOrbitalTravelResult.Failure([.. validationErrors]);
        }

        var group = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x =>
                x.Id == request.OrbitalGroupId &&
                x.CivilizationId == request.CivilizationId,
                cancellationToken);

        if (group is null)
        {
            return EstimateOrbitalTravelResult.Failure("Orbital group was not found for the civilization.");
        }

        if (group.CurrentPlanetId == Guid.Empty)
        {
            return EstimateOrbitalTravelResult.Failure("Orbital group current planet is required.");
        }

        var destinationExists = await dbContext.Set<Planet>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.DestinationPlanetId, cancellationToken);

        if (!destinationExists)
        {
            return EstimateOrbitalTravelResult.Failure("Destination planet was not found.");
        }

        if (group.CurrentPlanetId == request.DestinationPlanetId)
        {
            return EstimateOrbitalTravelResult.Failure("Destination planet must be different from the current planet.");
        }

        var estimate = OrbitalTravelEstimator.Estimate(
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            group.AssetType);

        var costs = estimate.ResourceCosts
            .Select(x => new OrbitalTravelCostComponentDto(x.ResourceType, x.Quantity))
            .ToArray();
        var affordability = await resourceSpendService.CheckAffordabilityAsync(
            new ResourceSpendRequest(
                group.CurrentPlanetId,
                costs.Select(x => new ResourceCostDto(x.ResourceType, x.Quantity)).ToArray()),
            cancellationToken);
        var insufficientResources = affordability.Succeeded
            ? []
            : await GetInsufficientResourcesAsync(group.CurrentPlanetId, costs, cancellationToken);

        return EstimateOrbitalTravelResult.Success(
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            estimate.AbstractDistanceUnits,
            estimate.EstimatedDuration,
            costs,
            affordability.Succeeded,
            insufficientResources);
    }

    private async Task<IReadOnlyList<OrbitalTravelInsufficientResourceDto>> GetInsufficientResourcesAsync(
        Guid planetId,
        IReadOnlyList<OrbitalTravelCostComponentDto> costs,
        CancellationToken cancellationToken)
    {
        var stockpile = await dbContext.PlanetResourceStockpiles
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PlanetId == planetId, cancellationToken);

        return costs
            .Where(cost => GetAvailable(stockpile, cost.ResourceType) < cost.Quantity)
            .Select(cost => new OrbitalTravelInsufficientResourceDto(
                cost.ResourceType,
                cost.Quantity,
                GetAvailable(stockpile, cost.ResourceType)))
            .ToArray();
    }

    private static decimal GetAvailable(PlanetResourceStockpile? stockpile, ResourceType resourceType) =>
        resourceType switch
        {
            ResourceType.Credits => stockpile?.Credits ?? 0,
            ResourceType.Metal => stockpile?.Metal ?? 0,
            ResourceType.Crystal => stockpile?.Crystal ?? 0,
            ResourceType.Gas => stockpile?.Gas ?? 0,
            _ => 0
        };

    private static List<string> Validate(EstimateOrbitalTravelRequest request)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.OrbitalGroupId == Guid.Empty) errors.Add("Orbital group id is required.");
        if (request.DestinationPlanetId == Guid.Empty) errors.Add("Destination planet id is required.");
        return errors;
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTravelEstimateService(VoidEmpiresDbContext dbContext) : IOrbitalTravelEstimateService
{
    public async Task<EstimateOrbitalTravelResult> EstimateAsync(
        EstimateOrbitalTravelRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.OrbitalGroupId == Guid.Empty) errors.Add("Orbital group id is required.");
        if (request.DestinationPlanetId == Guid.Empty) errors.Add("Destination planet id is required.");
        if (errors.Count > 0) return EstimateOrbitalTravelResult.Failure([.. errors]);

        var group = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.OrbitalGroupId && x.CivilizationId == request.CivilizationId, cancellationToken);

        if (group is null)
        {
            return EstimateOrbitalTravelResult.Failure("Orbital group was not found for the civilization.");
        }

        if (group.Status != OrbitalGroupStatus.Stationed)
        {
            return EstimateOrbitalTravelResult.Failure("Only stationed orbital groups can be estimated.");
        }

        if (group.CurrentPlanetId == request.DestinationPlanetId)
        {
            return EstimateOrbitalTravelResult.Failure("Destination planet must be different from the current planet.");
        }

        var distance = OrbitalTravelEstimator.EstimateAbstractDistanceUnits(group.CurrentPlanetId, request.DestinationPlanetId);
        var duration = OrbitalTravelEstimator.EstimateTravelDuration(distance);
        var costs = OrbitalTravelEstimator.EstimateTravelCost(group.AssetType, group.Quantity, distance)
            .Components
            .Select(x => new OrbitalTravelCostComponentDto(x.ResourceType, x.Amount))
            .ToList();

        return EstimateOrbitalTravelResult.Success(
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            distance,
            duration,
            costs);
    }
}

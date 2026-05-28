using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalGroupPlannerService(VoidEmpiresDbContext dbContext) : IOrbitalGroupTransferPlanningService
{
    public async Task<PlanOrbitalGroupTransferResult> PlanAsync(
        PlanOrbitalGroupTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.OrbitalGroupId == Guid.Empty) errors.Add("Orbital group id is required.");
        if (request.DestinationPlanetId == Guid.Empty) errors.Add("Destination planet id is required.");
        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc) errors.Add("Requested date must be UTC.");
        if (errors.Count > 0) return PlanOrbitalGroupTransferResult.Failure([.. errors]);

        var group = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.OrbitalGroupId && x.CivilizationId == request.CivilizationId, cancellationToken);

        if (group is null) return PlanOrbitalGroupTransferResult.Failure("Orbital group was not found for the civilization.");
        if (group.Status != OrbitalGroupStatus.Stationed) return PlanOrbitalGroupTransferResult.Failure("Only stationed orbital groups can be planned.");
        if (group.CurrentPlanetId == request.DestinationPlanetId) return PlanOrbitalGroupTransferResult.Failure("Destination planet must be different from the current planet.");

        var distance = OrbitalTravelEstimator.EstimateAbstractDistanceUnits(group.CurrentPlanetId, request.DestinationPlanetId);
        var arrivalAtUtc = request.RequestedAtUtc.Add(OrbitalTravelEstimator.EstimateTravelDuration(distance));

        return PlanOrbitalGroupTransferResult.Success(
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            distance,
            request.RequestedAtUtc,
            arrivalAtUtc);
    }
}

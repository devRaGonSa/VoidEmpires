using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferPersistenceService(VoidEmpiresDbContext dbContext) : IOrbitalTransferPersistenceService
{
    public async Task<PersistOrbitalTransferResult> PersistAsync(
        PersistOrbitalTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return PersistOrbitalTransferResult.Failure([.. validationErrors]);
        }

        var group = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(x =>
                x.Id == request.OrbitalGroupId &&
                x.CivilizationId == request.CivilizationId,
                cancellationToken);

        if (group is null)
        {
            return PersistOrbitalTransferResult.Failure("Orbital group was not found for the civilization.");
        }

        if (group.Status != OrbitalGroupStatus.Stationed)
        {
            return PersistOrbitalTransferResult.Failure("Only stationed orbital groups can be persisted for transfer.");
        }

        if (group.CurrentPlanetId == request.DestinationPlanetId)
        {
            return PersistOrbitalTransferResult.Failure("Destination planet must be different from the current planet.");
        }

        var distance = OrbitalTravelEstimator.EstimateAbstractDistanceUnits(group.CurrentPlanetId, request.DestinationPlanetId);
        var arrivalAtUtc = request.RequestedAtUtc.Add(OrbitalTravelEstimator.EstimateTravelDuration(distance));
        var transfer = OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            distance,
            request.RequestedAtUtc,
            arrivalAtUtc);

        group.Reserve();
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync(cancellationToken);

        return PersistOrbitalTransferResult.Success(
            transfer.Id,
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            distance,
            request.RequestedAtUtc,
            arrivalAtUtc);
    }

    private static List<string> Validate(PersistOrbitalTransferRequest request)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.OrbitalGroupId == Guid.Empty) errors.Add("Orbital group id is required.");
        if (request.DestinationPlanetId == Guid.Empty) errors.Add("Destination planet id is required.");
        if (request.RequestedAtUtc.Kind != DateTimeKind.Utc) errors.Add("Requested date must be UTC.");
        return errors;
    }
}

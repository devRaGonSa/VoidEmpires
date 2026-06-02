using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferPersistenceService(
    VoidEmpiresDbContext dbContext,
    IResourceSpendService resourceSpendService) : IOrbitalTransferPersistenceService
{
    public async Task<PersistOrbitalTransferResult> PersistAsync(
        PersistOrbitalTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return PersistOrbitalTransferResult.ValidationFailure([.. validationErrors]);
        }

        var group = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(x =>
                x.Id == request.OrbitalGroupId &&
                x.CivilizationId == request.CivilizationId,
                cancellationToken);

        if (group is null)
        {
            return PersistOrbitalTransferResult.NotFound("Orbital group was not found for the civilization.");
        }

        if (group.Status != OrbitalGroupStatus.Stationed)
        {
            if (await OrbitalTransferActivityQueries.HasActiveTransferAsync(
                dbContext.Set<OrbitalTransfer>(),
                group.Id,
                cancellationToken))
            {
                return PersistOrbitalTransferResult.Conflict("Orbital group already has an active transfer.");
            }

            return PersistOrbitalTransferResult.Conflict("Only stationed orbital groups can be persisted for transfer.");
        }

        if (await OrbitalTransferActivityQueries.HasActiveTransferAsync(
            dbContext.Set<OrbitalTransfer>(),
            group.Id,
            cancellationToken))
        {
            return PersistOrbitalTransferResult.Conflict("Orbital group already has an active transfer.");
        }

        if (group.CurrentPlanetId == request.DestinationPlanetId)
        {
            return PersistOrbitalTransferResult.Conflict("Destination planet must be different from the current planet.");
        }

        var estimate = OrbitalTravelEstimator.Estimate(
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            group.AssetType);
        await using var transaction = dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory"
            ? null
            : await dbContext.Database.BeginTransactionAsync(cancellationToken);
        var spendResult = await resourceSpendService.SpendAsync(
            new ResourceSpendRequest(
                group.CurrentPlanetId,
                estimate.ResourceCosts
                    .Select(x => new ResourceCostDto(x.ResourceType, x.Quantity))
                    .ToArray()),
            cancellationToken);

        if (!spendResult.Succeeded)
        {
            return PersistOrbitalTransferResult.Conflict([.. spendResult.Errors]);
        }

        var arrivalAtUtc = request.RequestedAtUtc.Add(estimate.EstimatedDuration);
        var transfer = OrbitalTransfer.CreatePlanned(
            group.CivilizationId,
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            estimate.AbstractDistanceUnits,
            request.RequestedAtUtc,
            arrivalAtUtc);

        group.Reserve();
        dbContext.Set<OrbitalTransfer>().Add(transfer);
        await dbContext.SaveChangesAsync(cancellationToken);
        if (transaction is not null)
        {
            await transaction.CommitAsync(cancellationToken);
        }

        return PersistOrbitalTransferResult.Success(
            transfer.Id,
            group.Id,
            group.CurrentPlanetId,
            request.DestinationPlanetId,
            estimate.AbstractDistanceUnits,
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

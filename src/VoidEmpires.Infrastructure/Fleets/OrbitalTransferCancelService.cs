using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferCancelService(VoidEmpiresDbContext dbContext) : IOrbitalTransferCancelService
{
    public async Task<CancelOrbitalTransferResult> CancelAsync(
        CancelOrbitalTransferRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return CancelOrbitalTransferResult.ValidationFailure([.. validationErrors]);
        }

        var transfer = await dbContext.Set<OrbitalTransfer>()
            .SingleOrDefaultAsync(x => x.Id == request.OrbitalTransferId, cancellationToken);

        if (transfer is null)
        {
            return CancelOrbitalTransferResult.NotFound("Orbital transfer was not found.");
        }

        if (transfer.CivilizationId != request.CivilizationId)
        {
            return CancelOrbitalTransferResult.Conflict("Orbital transfer does not belong to the civilization.");
        }

        if (transfer.Status == OrbitalTransferStatus.Completed)
        {
            return CancelOrbitalTransferResult.Conflict("Completed orbital transfers cannot be cancelled.");
        }

        if (transfer.Status == OrbitalTransferStatus.Cancelled)
        {
            return CancelOrbitalTransferResult.Conflict("Orbital transfer is already cancelled.");
        }

        var group = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(x => x.Id == transfer.OrbitalGroupId, cancellationToken);

        if (group is null)
        {
            return CancelOrbitalTransferResult.Conflict("Orbital group was not found.");
        }

        if (group.CivilizationId != request.CivilizationId)
        {
            return CancelOrbitalTransferResult.Conflict("Orbital group does not belong to the civilization.");
        }

        if (group.Status != OrbitalGroupStatus.Reserved)
        {
            return CancelOrbitalTransferResult.Conflict("Only reserved orbital groups can be released.");
        }

        transfer.Cancel();
        group.ReleaseReservation();
        await dbContext.SaveChangesAsync(cancellationToken);

        return CancelOrbitalTransferResult.Success(transfer.Id, group.Id);
    }

    private static List<string> Validate(CancelOrbitalTransferRequest request)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.OrbitalTransferId == Guid.Empty) errors.Add("Orbital transfer id is required.");
        return errors;
    }
}

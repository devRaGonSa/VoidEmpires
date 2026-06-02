using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferCompletionService(VoidEmpiresDbContext dbContext) : IOrbitalTransferCompletionService
{
    public async Task<CompleteOrbitalTransfersResult> CompleteDueAsync(
        DateTime nowUtc,
        CancellationToken cancellationToken = default)
    {
        if (nowUtc.Kind != DateTimeKind.Utc)
        {
            throw new ArgumentException("Current date must be UTC.", nameof(nowUtc));
        }

        var dueTransfers = await dbContext.Set<OrbitalTransfer>()
            .Where(x =>
                x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled &&
                x.ArrivalAtUtc <= nowUtc)
            .OrderBy(x => x.ArrivalAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (dueTransfers.Count == 0)
        {
            return new CompleteOrbitalTransfersResult(0, [], []);
        }

        var groupIds = dueTransfers.Select(x => x.OrbitalGroupId).Distinct().ToArray();
        var groups = await dbContext.Set<OrbitalGroup>()
            .Where(x => groupIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        var completedTransferIds = new List<Guid>();
        var completedGroupIds = new List<Guid>();

        foreach (var transfer in dueTransfers)
        {
            if (!groups.TryGetValue(transfer.OrbitalGroupId, out var group))
            {
                continue;
            }

            if (group.CivilizationId != transfer.CivilizationId ||
                group.Status != OrbitalGroupStatus.Reserved)
            {
                continue;
            }

            group.ArriveAt(transfer.DestinationPlanetId);
            transfer.Complete(nowUtc);
            completedTransferIds.Add(transfer.Id);
            completedGroupIds.Add(group.Id);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return new CompleteOrbitalTransfersResult(
            completedTransferIds.Count,
            completedTransferIds,
            completedGroupIds);
    }
}

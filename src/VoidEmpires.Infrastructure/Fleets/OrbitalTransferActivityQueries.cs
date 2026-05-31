using Microsoft.EntityFrameworkCore;
using VoidEmpires.Domain.Fleets;

namespace VoidEmpires.Infrastructure.Fleets;

internal static class OrbitalTransferActivityQueries
{
    public static Task<bool> HasActiveTransferAsync(
        IQueryable<OrbitalTransfer> transfers,
        Guid orbitalGroupId,
        CancellationToken cancellationToken) =>
        transfers
            .AsNoTracking()
            .AnyAsync(x =>
                x.OrbitalGroupId == orbitalGroupId &&
                x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled,
                cancellationToken);

    public static async Task<HashSet<Guid>> GetActiveTransferGroupIdsAsync(
        IQueryable<OrbitalTransfer> transfers,
        IReadOnlyCollection<Guid> orbitalGroupIds,
        CancellationToken cancellationToken)
    {
        var activeGroupIds = await transfers
            .AsNoTracking()
            .Where(x =>
                orbitalGroupIds.Contains(x.OrbitalGroupId) &&
                x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled)
            .Select(x => x.OrbitalGroupId)
            .ToListAsync(cancellationToken);

        return activeGroupIds.ToHashSet();
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class FleetOperationalOverviewService(VoidEmpiresDbContext dbContext) : IFleetOperationalOverviewService
{
    public async Task<GetFleetOperationalOverviewResult> GetAsync(
        GetFleetOperationalOverviewRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetFleetOperationalOverviewResult(request.CivilizationId, []);
        }

        var groups = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .OrderBy(x => x.CurrentPlanetId)
            .ThenBy(x => x.AssetType)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        if (groups.Count == 0)
        {
            return new GetFleetOperationalOverviewResult(request.CivilizationId, []);
        }

        var groupIds = groups.Select(x => x.Id).ToArray();
        var activeTransfers = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == request.CivilizationId &&
                groupIds.Contains(x.OrbitalGroupId) &&
                x.Status != OrbitalTransferStatus.Completed &&
                x.Status != OrbitalTransferStatus.Cancelled)
            .OrderBy(x => x.DepartureAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        var activeTransferByGroupId = activeTransfers
            .GroupBy(x => x.OrbitalGroupId)
            .ToDictionary(x => x.Key, x => x.First());

        var mergeCandidateKeys = groups
            .Where(x => x.Status == OrbitalGroupStatus.Stationed && !activeTransferByGroupId.ContainsKey(x.Id))
            .GroupBy(x => new { x.CurrentPlanetId, x.AssetType })
            .Where(x => x.Count() > 1)
            .Select(x => x.Key)
            .ToHashSet();

        var overviewGroups = groups
            .Select(group =>
            {
                activeTransferByGroupId.TryGetValue(group.Id, out var activeTransfer);
                var hasActiveTransfer = activeTransfer is not null;
                var canCreateTransfer = group.Status == OrbitalGroupStatus.Stationed && !hasActiveTransfer;
                var canMerge = canCreateTransfer && mergeCandidateKeys.Contains(new { group.CurrentPlanetId, group.AssetType });

                return new FleetOperationalGroupDto(
                    group.Id,
                    group.CivilizationId,
                    group.OriginPlanetId,
                    group.CurrentPlanetId,
                    group.AssetType,
                    group.Quantity,
                    group.Status,
                    group.IsStationedAwayFromOrigin,
                    hasActiveTransfer,
                    activeTransfer is null
                        ? null
                        : new FleetOperationalTransferDto(
                            activeTransfer.Id,
                            activeTransfer.DestinationPlanetId,
                            activeTransfer.AbstractDistanceUnits,
                            activeTransfer.DepartureAtUtc,
                            activeTransfer.ArrivalAtUtc,
                            activeTransfer.Status),
                    new FleetOperationalCommandAvailabilityDto(
                        canCreateTransfer,
                        canCreateTransfer && group.Quantity > 1,
                        canMerge,
                        hasActiveTransfer && group.Status == OrbitalGroupStatus.Reserved));
            })
            .ToArray();

        return new GetFleetOperationalOverviewResult(request.CivilizationId, overviewGroups);
    }
}

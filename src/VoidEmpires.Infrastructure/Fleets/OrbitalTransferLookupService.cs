using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalTransferLookupService(VoidEmpiresDbContext dbContext) : IOrbitalTransferLookupService
{
    public async Task<IReadOnlyList<OrbitalTransferQueryItem>> ListAsync(
        OrbitalTransferQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return [];
        }

        var query = dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId);

        if (request.OrbitalGroupId is not null)
        {
            query = query.Where(x => x.OrbitalGroupId == request.OrbitalGroupId.Value);
        }

        if (request.OriginPlanetId is not null)
        {
            query = query.Where(x => x.OriginPlanetId == request.OriginPlanetId.Value);
        }

        if (request.DestinationPlanetId is not null)
        {
            query = query.Where(x => x.DestinationPlanetId == request.DestinationPlanetId.Value);
        }

        if (request.Status is not null)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        return await query
            .OrderBy(x => x.ArrivalAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new OrbitalTransferQueryItem(
                x.Id,
                x.CivilizationId,
                x.OrbitalGroupId,
                x.OriginPlanetId,
                x.DestinationPlanetId,
                x.AbstractDistanceUnits,
                x.DepartureAtUtc,
                x.ArrivalAtUtc,
                x.Status))
            .ToListAsync(cancellationToken);
    }
}

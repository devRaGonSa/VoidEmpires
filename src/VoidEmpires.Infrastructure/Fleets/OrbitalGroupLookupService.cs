using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalGroupLookupService(VoidEmpiresDbContext dbContext) : IOrbitalGroupLookupService
{
    public async Task<IReadOnlyList<OrbitalGroupQueryItem>> ListAsync(
        OrbitalGroupQueryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return [];
        }

        var query = dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId);

        if (request.CurrentPlanetId is not null)
        {
            query = query.Where(x => x.CurrentPlanetId == request.CurrentPlanetId.Value);
        }

        if (request.OriginPlanetId is not null)
        {
            query = query.Where(x => x.OriginPlanetId == request.OriginPlanetId.Value);
        }

        if (request.AssetType is not null)
        {
            query = query.Where(x => x.AssetType == request.AssetType.Value);
        }

        if (request.Status is not null)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        return await query
            .OrderBy(x => x.CurrentPlanetId)
            .ThenBy(x => x.AssetType)
            .ThenBy(x => x.Id)
            .Select(x => new OrbitalGroupQueryItem(
                x.Id,
                x.CivilizationId,
                x.OriginPlanetId,
                x.CurrentPlanetId,
                x.AssetType,
                x.Quantity,
                x.Status,
                x.IsStationedAwayFromOrigin))
            .ToListAsync(cancellationToken);
    }
}

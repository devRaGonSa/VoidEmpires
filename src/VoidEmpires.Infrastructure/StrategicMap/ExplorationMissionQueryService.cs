using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class ExplorationMissionQueryService(VoidEmpiresDbContext dbContext) : IExplorationMissionQueryService
{
    public async Task<GetExplorationMissionsResult> GetAsync(
        GetExplorationMissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return GetExplorationMissionsResult.Invalid(request.CivilizationId, request.Status, "Civilization id is required.");
        }

        var query = dbContext.ExplorationMissions
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId);

        if (request.Status is not null)
        {
            query = query.Where(x => x.Status == request.Status);
        }

        var missions = await query
            .OrderBy(x => x.RequestedAtUtc)
            .ThenBy(x => x.DueAtUtc)
            .ThenBy(x => x.Id)
            .Select(x => new ExplorationMissionDto(
                x.Id,
                x.CivilizationId,
                x.TargetSystemId,
                x.TargetPlanetId,
                x.Status,
                x.RequestedAtUtc,
                x.DueAtUtc,
                x.CompletedAtUtc))
            .ToArrayAsync(cancellationToken);

        return GetExplorationMissionsResult.Success(request.CivilizationId, request.Status, missions);
    }
}

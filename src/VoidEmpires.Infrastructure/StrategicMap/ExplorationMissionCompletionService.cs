using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Exploration;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class ExplorationMissionCompletionService(VoidEmpiresDbContext dbContext) : IExplorationMissionCompletionService
{
    public async Task<CompleteDueExplorationMissionsResult> CompleteDueAsync(
        CompleteDueExplorationMissionsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.NowUtc.Kind != DateTimeKind.Utc)
        {
            return CompleteDueExplorationMissionsResult.Invalid("Now must be UTC.");
        }

        var dueMissions = await dbContext.ExplorationMissions
            .Where(x => x.Status == ExplorationMissionStatus.Planned && x.DueAtUtc <= request.NowUtc)
            .OrderBy(x => x.DueAtUtc)
            .ThenBy(x => x.Id)
            .ToListAsync(cancellationToken);

        foreach (var mission in dueMissions)
        {
            mission.Complete(request.NowUtc);
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CompleteDueExplorationMissionsResult.Success(dueMissions.Select(x => x.Id).ToArray());
    }
}

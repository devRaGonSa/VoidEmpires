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
        var civilizationIds = dueMissions.Select(x => x.CivilizationId).Distinct().ToArray();
        var systemIds = dueMissions.Select(x => x.TargetSystemId).Distinct().ToArray();
        var knowledgeKeys = (await dbContext.ExplorationKnowledge
                .Where(x => civilizationIds.Contains(x.CivilizationId) && systemIds.Contains(x.SystemId))
                .Select(x => new KnowledgeKey(x.CivilizationId, x.SystemId, x.PlanetId))
                .ToListAsync(cancellationToken))
            .ToHashSet();

        foreach (var mission in dueMissions)
        {
            mission.Complete(request.NowUtc);
            AddKnowledge(mission, null, request.NowUtc, knowledgeKeys);

            if (mission.TargetPlanetId.HasValue)
            {
                AddKnowledge(mission, mission.TargetPlanetId, request.NowUtc, knowledgeKeys);
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);

        return CompleteDueExplorationMissionsResult.Success(dueMissions.Select(x => x.Id).ToArray());
    }

    private void AddKnowledge(
        ExplorationMission mission,
        Guid? planetId,
        DateTime discoveredAtUtc,
        HashSet<KnowledgeKey> knowledgeKeys)
    {
        var key = new KnowledgeKey(mission.CivilizationId, mission.TargetSystemId, planetId);

        if (!knowledgeKeys.Add(key))
        {
            return;
        }

        dbContext.ExplorationKnowledge.Add(ExplorationKnowledge.Create(
            mission.CivilizationId,
            mission.TargetSystemId,
            planetId,
            ExplorationKnowledgeSource.MissionCompletion,
            mission.Id,
            discoveredAtUtc));
    }

    private sealed record KnowledgeKey(Guid CivilizationId, Guid SystemId, Guid? PlanetId);
}

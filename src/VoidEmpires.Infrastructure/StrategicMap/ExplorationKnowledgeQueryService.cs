using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class ExplorationKnowledgeQueryService(VoidEmpiresDbContext dbContext) : IExplorationKnowledgeQueryService
{
    public async Task<GetExplorationKnowledgeResult> GetAsync(
        GetExplorationKnowledgeRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return GetExplorationKnowledgeResult.Invalid(request.CivilizationId, "Civilization id is required.");
        }

        var knowledge = await dbContext.ExplorationKnowledge
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .OrderBy(x => x.DiscoveredAtUtc)
            .ThenBy(x => x.SystemId)
            .ThenBy(x => x.PlanetId)
            .Select(x => new ExplorationKnowledgeDto(
                x.Id,
                x.CivilizationId,
                x.SystemId,
                x.PlanetId,
                x.Source,
                x.SourceMissionId,
                x.DiscoveredAtUtc))
            .ToArrayAsync(cancellationToken);

        return GetExplorationKnowledgeResult.Success(request.CivilizationId, knowledge);
    }
}

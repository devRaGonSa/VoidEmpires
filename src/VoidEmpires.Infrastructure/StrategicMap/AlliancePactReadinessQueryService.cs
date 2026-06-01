using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Diplomacy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class AlliancePactReadinessQueryService(VoidEmpiresDbContext dbContext) : IAlliancePactReadinessQueryService
{
    public async Task<GetAlliancePactReadinessResult> GetAsync(
        GetAlliancePactReadinessRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return GetAlliancePactReadinessResult.Invalid(request.CivilizationId, "Civilization id is required.");
        }

        var participantAllianceIds = dbContext.AllianceMemberships
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId && x.Status == AllianceMembershipStatus.Active)
            .Select(x => x.AllianceId);

        var pacts = await (
            from pact in dbContext.Set<AlliancePact>().AsNoTracking()
            join sourceAlliance in dbContext.Alliances.AsNoTracking() on pact.SourceAllianceId equals sourceAlliance.Id
            join targetAlliance in dbContext.Alliances.AsNoTracking() on pact.TargetAllianceId equals targetAlliance.Id
            where participantAllianceIds.Contains(pact.SourceAllianceId)
                || participantAllianceIds.Contains(pact.TargetAllianceId)
            orderby pact.CreatedAtUtc, pact.PactType, pact.SourceAllianceId, pact.TargetAllianceId, pact.Id
            select new AlliancePactReadinessDto(
                pact.Id,
                new AlliancePactAllianceDto(
                    sourceAlliance.Id,
                    sourceAlliance.Name,
                    sourceAlliance.Tag,
                    sourceAlliance.Status),
                new AlliancePactAllianceDto(
                    targetAlliance.Id,
                    targetAlliance.Name,
                    targetAlliance.Tag,
                    targetAlliance.Status),
                pact.PactType,
                pact.Status,
                pact.CreatedAtUtc))
            .ToArrayAsync(cancellationToken);

        return GetAlliancePactReadinessResult.Success(request.CivilizationId, pacts);
    }
}

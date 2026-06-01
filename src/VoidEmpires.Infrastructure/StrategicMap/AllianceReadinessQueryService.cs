using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class AllianceReadinessQueryService(VoidEmpiresDbContext dbContext) : IAllianceReadinessQueryService
{
    public async Task<GetAllianceReadinessResult> GetAsync(
        GetAllianceReadinessRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return GetAllianceReadinessResult.Invalid(request.CivilizationId, "Civilization id is required.");
        }

        var alliances = await (
            from membership in dbContext.AllianceMemberships.AsNoTracking()
            join alliance in dbContext.Alliances.AsNoTracking() on membership.AllianceId equals alliance.Id
            where membership.CivilizationId == request.CivilizationId
            orderby membership.JoinedAtUtc, alliance.Tag, alliance.Id, membership.Id
            select new AllianceReadinessDto(
                alliance.Id,
                alliance.Name,
                alliance.Tag,
                alliance.Status,
                alliance.CreatedAtUtc,
                new AllianceMembershipDto(
                    membership.Id,
                    membership.AllianceId,
                    membership.CivilizationId,
                    membership.Status,
                    membership.Role,
                    membership.JoinedAtUtc)))
            .ToArrayAsync(cancellationToken);

        return GetAllianceReadinessResult.Success(request.CivilizationId, alliances);
    }
}

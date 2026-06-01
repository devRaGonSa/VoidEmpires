using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DiplomaticContactQueryService(VoidEmpiresDbContext dbContext) : IDiplomaticContactQueryService
{
    public async Task<GetDiplomaticContactsResult> GetAsync(
        GetDiplomaticContactsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return GetDiplomaticContactsResult.Invalid(request.CivilizationId, "Civilization id is required.");
        }

        var contacts = await dbContext.DiplomaticContacts
            .AsNoTracking()
            .Where(x => x.CivilizationId == request.CivilizationId)
            .OrderBy(x => x.DiscoveredAtUtc)
            .ThenBy(x => x.ContactedCivilizationId)
            .Select(x => new DiplomaticContactDto(
                x.Id,
                x.CivilizationId,
                x.ContactedCivilizationId,
                x.Status,
                x.DiscoveredAtUtc,
                x.Source))
            .ToArrayAsync(cancellationToken);

        return GetDiplomaticContactsResult.Success(request.CivilizationId, contacts);
    }
}

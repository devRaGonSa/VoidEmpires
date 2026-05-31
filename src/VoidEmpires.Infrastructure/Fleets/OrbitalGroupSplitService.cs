using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalGroupSplitService(VoidEmpiresDbContext dbContext) : IOrbitalGroupSplitService
{
    public async Task<SplitOrbitalGroupResult> SplitAsync(
        SplitOrbitalGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return SplitOrbitalGroupResult.Failure([.. validationErrors]);
        }

        var source = await dbContext.Set<OrbitalGroup>()
            .SingleOrDefaultAsync(x => x.Id == request.SourceOrbitalGroupId, cancellationToken);

        if (source is null)
        {
            return SplitOrbitalGroupResult.Failure("Source orbital group was not found.");
        }

        if (source.CivilizationId != request.CivilizationId)
        {
            return SplitOrbitalGroupResult.Failure("Source orbital group does not belong to the civilization.");
        }

        if (await OrbitalTransferActivityQueries.HasActiveTransferAsync(
            dbContext.Set<OrbitalTransfer>(),
            source.Id,
            cancellationToken))
        {
            return SplitOrbitalGroupResult.Failure("Source orbital group already has an active transfer.");
        }

        if (source.Status != OrbitalGroupStatus.Stationed)
        {
            return SplitOrbitalGroupResult.Failure("Only stationed orbital groups can be split.");
        }

        if (request.Quantity >= source.Quantity)
        {
            return SplitOrbitalGroupResult.Failure("Split quantity must be lower than source quantity.");
        }

        var newGroup = source.SplitOff(request.Quantity);
        dbContext.Set<OrbitalGroup>().Add(newGroup);
        await dbContext.SaveChangesAsync(cancellationToken);

        return SplitOrbitalGroupResult.Success(
            source.Id,
            newGroup.Id,
            source.Quantity,
            newGroup.Quantity);
    }

    private static List<string> Validate(SplitOrbitalGroupRequest request)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.SourceOrbitalGroupId == Guid.Empty) errors.Add("Source orbital group id is required.");
        if (request.Quantity <= 0) errors.Add("Quantity must be positive.");
        return errors;
    }
}

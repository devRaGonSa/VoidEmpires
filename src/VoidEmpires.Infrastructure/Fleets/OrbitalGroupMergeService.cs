using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalGroupMergeService(VoidEmpiresDbContext dbContext) : IOrbitalGroupMergeService
{
    public async Task<MergeOrbitalGroupsResult> MergeAsync(
        MergeOrbitalGroupsRequest request,
        CancellationToken cancellationToken = default)
    {
        var validationErrors = Validate(request);
        if (validationErrors.Count > 0)
        {
            return MergeOrbitalGroupsResult.Failure([.. validationErrors]);
        }

        var groups = await dbContext.Set<OrbitalGroup>()
            .Where(x => x.Id == request.TargetOrbitalGroupId || x.Id == request.SourceOrbitalGroupId)
            .ToListAsync(cancellationToken);

        var target = groups.SingleOrDefault(x => x.Id == request.TargetOrbitalGroupId);
        if (target is null)
        {
            return MergeOrbitalGroupsResult.Failure("Target orbital group was not found.");
        }

        var source = groups.SingleOrDefault(x => x.Id == request.SourceOrbitalGroupId);
        if (source is null)
        {
            return MergeOrbitalGroupsResult.Failure("Source orbital group was not found.");
        }

        var compatibilityError = ValidateCompatibility(request.CivilizationId, target, source);
        if (compatibilityError is not null)
        {
            return MergeOrbitalGroupsResult.Failure(compatibilityError);
        }

        target.MergeFrom(source);
        dbContext.Set<OrbitalGroup>().Remove(source);
        await dbContext.SaveChangesAsync(cancellationToken);

        return MergeOrbitalGroupsResult.Success(target.Id, source.Id, target.Quantity);
    }

    private static List<string> Validate(MergeOrbitalGroupsRequest request)
    {
        var errors = new List<string>();
        if (request.CivilizationId == Guid.Empty) errors.Add("Civilization id is required.");
        if (request.TargetOrbitalGroupId == Guid.Empty) errors.Add("Target orbital group id is required.");
        if (request.SourceOrbitalGroupId == Guid.Empty) errors.Add("Source orbital group id is required.");
        if (request.TargetOrbitalGroupId == request.SourceOrbitalGroupId) errors.Add("Target and source orbital groups must be different.");
        return errors;
    }

    private static string? ValidateCompatibility(Guid civilizationId, OrbitalGroup target, OrbitalGroup source)
    {
        if (target.CivilizationId != civilizationId || source.CivilizationId != civilizationId)
        {
            return "Both orbital groups must belong to the civilization.";
        }

        if (target.CurrentPlanetId != source.CurrentPlanetId)
        {
            return "Orbital groups must be at the same current planet.";
        }

        if (target.AssetType != source.AssetType)
        {
            return "Orbital groups must have the same asset type.";
        }

        if (target.Status != OrbitalGroupStatus.Stationed || source.Status != OrbitalGroupStatus.Stationed)
        {
            return "Only stationed orbital groups can be merged.";
        }

        return null;
    }
}

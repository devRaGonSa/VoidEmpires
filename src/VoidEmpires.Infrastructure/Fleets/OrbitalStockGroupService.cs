using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class OrbitalStockGroupService(VoidEmpiresDbContext dbContext) : IOrbitalGroupService
{
    public async Task<CreateOrbitalGroupResult> CreateFromLocalStockAsync(
        CreateOrbitalGroupRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty ||
            request.OriginPlanetId == Guid.Empty ||
            request.CurrentPlanetId == Guid.Empty ||
            request.Quantity <= 0)
        {
            return CreateOrbitalGroupResult.Failure("Invalid request.");
        }

        var stock = await dbContext.Set<OrbitalAssetStock>()
            .SingleOrDefaultAsync(x =>
                x.PlanetId == request.OriginPlanetId &&
                x.AssetType == request.AssetType,
                cancellationToken);

        if (stock is null || stock.Quantity < request.Quantity)
        {
            return CreateOrbitalGroupResult.Failure("Insufficient stock.");
        }

        stock.Decrease(request.Quantity);

        var orbitalGroup = OrbitalGroup.CreateStationed(
            request.CivilizationId,
            request.OriginPlanetId,
            request.CurrentPlanetId,
            request.AssetType,
            request.Quantity);

        dbContext.Set<OrbitalGroup>().Add(orbitalGroup);
        await dbContext.SaveChangesAsync(cancellationToken);

        return CreateOrbitalGroupResult.Success(orbitalGroup.Id);
    }
}

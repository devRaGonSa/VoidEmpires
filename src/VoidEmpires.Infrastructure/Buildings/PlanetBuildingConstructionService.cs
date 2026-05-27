using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Buildings;

public sealed class PlanetBuildingConstructionService(VoidEmpiresDbContext dbContext) : IPlanetBuildingConstructionService
{
    public async Task<ConstructBuildingResult> ConstructAsync(
        ConstructBuildingRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return ConstructBuildingResult.Failure("Planet id is required.");
        }

        if (request.Level <= 0)
        {
            return ConstructBuildingResult.Failure("Building level must be positive.");
        }

        if (request.Footprint <= 0)
        {
            return ConstructBuildingResult.Failure("Building footprint must be positive.");
        }

        var capacity = await dbContext.PlanetBuildingCapacities
            .SingleOrDefaultAsync(item => item.PlanetId == request.PlanetId, cancellationToken);

        if (capacity is null)
        {
            return ConstructBuildingResult.Failure("Planet building capacity was not found.");
        }

        var usedCapacity = await dbContext.PlanetBuildings
            .Where(item => item.PlanetId == request.PlanetId)
            .SumAsync(item => item.Footprint, cancellationToken);

        if (!capacity.CanFit(usedCapacity, request.Footprint))
        {
            return ConstructBuildingResult.Failure("Planet building capacity would be exceeded.");
        }

        var building = PlanetBuilding.Create(request.PlanetId, request.BuildingType, request.Level, request.Footprint);
        dbContext.PlanetBuildings.Add(building);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ConstructBuildingResult.Success(building.Id);
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Buildings;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Buildings;

public sealed class PlanetBuildingUpgradeService(VoidEmpiresDbContext dbContext) : IPlanetBuildingUpgradeService
{
    public async Task<UpgradeBuildingResult> UpgradeAsync(
        UpgradeBuildingRequest request,
        CancellationToken cancellationToken = default)
    {
        var building = await dbContext.PlanetBuildings
            .SingleOrDefaultAsync(x => x.Id == request.BuildingId, cancellationToken);

        if (building is null)
        {
            return UpgradeBuildingResult.Failure("Building was not found.");
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(x => x.PlanetId == building.PlanetId, cancellationToken);

        if (stockpile is null)
        {
            return UpgradeBuildingResult.Failure("Planet resource stockpile was not found.");
        }

        var definition = BuildingCatalog.Get(building.BuildingType);

        var multiplier = building.Level + 1;

        var creditsCost = definition.Cost.Credits * multiplier;
        var metalCost = definition.Cost.Metal * multiplier;
        var crystalCost = definition.Cost.Crystal * multiplier;
        var gasCost = definition.Cost.Gas * multiplier;

        if (!stockpile.CanSpend(creditsCost, metalCost, crystalCost, gasCost))
        {
            return UpgradeBuildingResult.Failure("Insufficient resources.");
        }

        stockpile.Spend(creditsCost, metalCost, crystalCost, gasCost);
        building.Upgrade();

        await dbContext.SaveChangesAsync(cancellationToken);

        return UpgradeBuildingResult.Success(building.Level);
    }
}

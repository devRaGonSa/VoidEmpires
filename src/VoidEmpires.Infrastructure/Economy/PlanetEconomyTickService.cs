using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Economy;

public sealed class PlanetEconomyTickService(VoidEmpiresDbContext dbContext) : IPlanetEconomyTickService
{
    private readonly ResourceProductionService _productionService = new();

    public async Task<ApplyPlanetProductionResult> ApplyProductionAsync(
        ApplyPlanetProductionRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return ApplyPlanetProductionResult.Failure("Planet id is required.");
        }

        if (request.Elapsed < TimeSpan.Zero)
        {
            return ApplyPlanetProductionResult.Failure("Elapsed time cannot be negative.");
        }

        var profile = await dbContext.PlanetProductionProfiles
            .SingleOrDefaultAsync(item => item.PlanetId == request.PlanetId, cancellationToken);

        if (profile is null)
        {
            return ApplyPlanetProductionResult.Failure("Planet production profile was not found.");
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(item => item.PlanetId == request.PlanetId, cancellationToken);

        if (stockpile is null)
        {
            return ApplyPlanetProductionResult.Failure("Planet resource stockpile was not found.");
        }

        _productionService.ApplyProduction(profile, stockpile, request.Elapsed);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ApplyPlanetProductionResult.Success(request.PlanetId);
    }
}

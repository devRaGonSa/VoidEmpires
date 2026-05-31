using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Economy;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Economy;

public sealed class ResourceSpendService(VoidEmpiresDbContext dbContext) : IResourceSpendService
{
    public async Task<ResourceSpendResult> CheckAffordabilityAsync(
        ResourceSpendRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, _, _, _, _, _) = await ValidateAsync(request, cancellationToken);
        return result;
    }

    public async Task<ResourceSpendResult> SpendAsync(
        ResourceSpendRequest request,
        CancellationToken cancellationToken = default)
    {
        var (result, stockpile, credits, metal, crystal, gas) = await ValidateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return result;
        }

        stockpile!.Spend(credits, metal, crystal, gas);
        await dbContext.SaveChangesAsync(cancellationToken);
        return ResourceSpendResult.Success(request.PlanetId);
    }

    private async Task<(ResourceSpendResult Result, PlanetResourceStockpile? Stockpile, decimal Credits, decimal Metal, decimal Crystal, decimal Gas)> ValidateAsync(
        ResourceSpendRequest request,
        CancellationToken cancellationToken)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return Failure("Planet id is required.");
        }

        if (request.Costs.Count == 0)
        {
            return Failure("At least one resource cost is required.");
        }

        var credits = 0m;
        var metal = 0m;
        var crystal = 0m;
        var gas = 0m;

        foreach (var cost in request.Costs)
        {
            if (cost.Quantity < 0)
            {
                return Failure("Resource cost cannot be negative.");
            }

            switch (cost.ResourceType)
            {
                case ResourceType.Credits:
                    credits += cost.Quantity;
                    break;
                case ResourceType.Metal:
                    metal += cost.Quantity;
                    break;
                case ResourceType.Crystal:
                    crystal += cost.Quantity;
                    break;
                case ResourceType.Gas:
                    gas += cost.Quantity;
                    break;
                default:
                    return Failure("Resource type is unknown.");
            }
        }

        var stockpile = await dbContext.PlanetResourceStockpiles
            .SingleOrDefaultAsync(item => item.PlanetId == request.PlanetId, cancellationToken);

        if (stockpile is null)
        {
            return Failure("Planet resource stockpile was not found.");
        }

        if (!stockpile.HasAtLeast(ResourceType.Credits, credits)) return Failure("Insufficient Credits.");
        if (!stockpile.HasAtLeast(ResourceType.Metal, metal)) return Failure("Insufficient Metal.");
        if (!stockpile.HasAtLeast(ResourceType.Crystal, crystal)) return Failure("Insufficient Crystal.");
        if (!stockpile.HasAtLeast(ResourceType.Gas, gas)) return Failure("Insufficient Gas.");

        return (ResourceSpendResult.Success(request.PlanetId), stockpile, credits, metal, crystal, gas);
    }

    private static (ResourceSpendResult Result, PlanetResourceStockpile? Stockpile, decimal Credits, decimal Metal, decimal Crystal, decimal Gas)
        Failure(string error) => (ResourceSpendResult.Failure(error), null, 0, 0, 0, 0);
}

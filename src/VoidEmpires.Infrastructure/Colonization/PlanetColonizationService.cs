using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Colonization;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Colonization;

public sealed class PlanetColonizationService(VoidEmpiresDbContext dbContext) : IPlanetColonizationService
{
    public async Task<ColonizePlanetResult> ColonizeAsync(
        ColonizePlanetRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return ColonizePlanetResult.Failure("Planet id is required.");
        }

        if (request.CivilizationId == Guid.Empty)
        {
            return ColonizePlanetResult.Failure("Civilization id is required.");
        }

        var planetExists = await dbContext.Planets.AnyAsync(planet => planet.Id == request.PlanetId, cancellationToken);
        if (!planetExists)
        {
            return ColonizePlanetResult.Failure("Planet was not found.");
        }

        var civilizationExists = await dbContext.Civilizations.AnyAsync(civilization => civilization.Id == request.CivilizationId, cancellationToken);
        if (!civilizationExists)
        {
            return ColonizePlanetResult.Failure("Civilization was not found.");
        }

        var alreadyOwned = await dbContext.PlanetOwnerships.AnyAsync(
            ownership => ownership.PlanetId == request.PlanetId,
            cancellationToken);

        if (alreadyOwned)
        {
            return ColonizePlanetResult.Failure("Planet is already controlled.");
        }

        var ownership = PlanetOwnership.Create(request.PlanetId, request.CivilizationId);
        dbContext.PlanetOwnerships.Add(ownership);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ColonizePlanetResult.Success(ownership.Id);
    }
}

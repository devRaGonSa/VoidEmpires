using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Visuals;

public sealed class SystemVisualStateService(
    VoidEmpiresDbContext dbContext,
    IPlanetVisualStateService planetVisualStateService) : ISystemVisualStateService
{
    public async Task<GetSystemVisualStateResult> GetAsync(
        GetSystemVisualStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.SystemId == Guid.Empty)
        {
            return GetSystemVisualStateResult.Failure("System id is required.");
        }

        var exists = await dbContext.Set<SolarSystem>()
            .AsNoTracking()
            .AnyAsync(x => x.Id == request.SystemId, cancellationToken);

        if (!exists)
        {
            return GetSystemVisualStateResult.Failure("System was not found.");
        }

        var planets = await dbContext.Set<Planet>()
            .AsNoTracking()
            .Where(x => x.SolarSystemId == request.SystemId)
            .OrderBy(x => x.OrbitalSlot)
            .Select(x => x.Id)
            .ToListAsync(cancellationToken);

        var visualPlanets = new List<PlanetVisualStateDto>();

        foreach (var planetId in planets)
        {
            var result = await planetVisualStateService.GetAsync(new GetPlanetVisualStateRequest(planetId), cancellationToken);

            if (!result.Succeeded || result.VisualState is null)
            {
                return GetSystemVisualStateResult.Failure(result.Errors.ToArray());
            }

            visualPlanets.Add(result.VisualState);
        }

        return GetSystemVisualStateResult.Success(new SystemVisualStateDto(request.SystemId, visualPlanets));
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Visuals;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Visuals;

public sealed class PlanetVisualStateService(VoidEmpiresDbContext dbContext) : IPlanetVisualStateService
{
    public async Task<GetPlanetVisualStateResult> GetAsync(
        GetPlanetVisualStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.PlanetId == Guid.Empty)
        {
            return GetPlanetVisualStateResult.Failure("Planet id is required.");
        }

        var planet = await dbContext.Set<Planet>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.PlanetId, cancellationToken);

        if (planet is null)
        {
            return GetPlanetVisualStateResult.Failure("Planet was not found.");
        }

        var ownership = await dbContext.Set<PlanetOwnership>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PlanetId == request.PlanetId, cancellationToken);

        var buildings = await dbContext.Set<PlanetBuilding>()
            .AsNoTracking()
            .Where(x => x.PlanetId == request.PlanetId)
            .ToListAsync(cancellationToken);

        var orbitalGroupStrength = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .Where(x => x.CurrentPlanetId == request.PlanetId)
            .SumAsync(x => (int?)x.Quantity, cancellationToken) ?? 0;

        var intensity = PlanetVisualIntensityCalculator.Calculate(new PlanetVisualIntensityInput(
            planet.Id,
            planet.PlanetType,
            planet.Size,
            planet.ColonizationStatus,
            ownership is not null,
            buildings.Count,
            buildings.Sum(x => x.Level),
            SumLevels(buildings, BuildingType.CommandCenter, BuildingType.HabitationDistrict, BuildingType.MedicalCenter),
            SumLevels(buildings, BuildingType.MetalMine, BuildingType.CrystalMine, BuildingType.GasExtractor, BuildingType.SolarPlant, BuildingType.Shipyard),
            0,
            SumLevels(buildings, BuildingType.DefenseGrid, BuildingType.MilitaryAcademy, BuildingType.Barracks, BuildingType.CrewAcademy, BuildingType.FleetCommandCenter, BuildingType.LogisticsHub),
            orbitalGroupStrength));

        var state = new PlanetVisualStateDto(
            planet.Id,
            planet.Name,
            planet.PlanetType,
            planet.Size,
            planet.ColonizationStatus,
            ownership is not null,
            ownership?.CivilizationId,
            ownership is null ? null : GetCivilizationColor(ownership.CivilizationId),
            intensity.VisualSeed,
            intensity.ColonizationIntensity,
            intensity.UrbanIntensity,
            intensity.IndustrialIntensity,
            intensity.TerraformingIntensity,
            intensity.MilitaryIntensity,
            intensity.OrbitalPresenceIntensity,
            PlanetVisualProfileCatalog.GetProfile(planet.PlanetType));

        return GetPlanetVisualStateResult.Success(state);
    }

    private static int SumLevels(IEnumerable<PlanetBuilding> buildings, params BuildingType[] types)
    {
        var allowedTypes = types.ToHashSet();
        return buildings.Where(x => allowedTypes.Contains(x.BuildingType)).Sum(x => x.Level);
    }

    private static string GetCivilizationColor(Guid civilizationId)
    {
        var hash = Math.Abs(civilizationId.GetHashCode());
        var hue = hash % 360;
        return $"hsl({hue}, 70%, 55%)";
    }
}

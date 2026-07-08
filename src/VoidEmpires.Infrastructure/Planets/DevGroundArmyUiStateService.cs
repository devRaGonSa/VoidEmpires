using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Planets;

public sealed class DevGroundArmyUiStateService(
    IDevPlanetUiStateService planetUiStateService,
    VoidEmpiresDbContext dbContext) : IDevGroundArmyUiStateService
{
    private static readonly TimeSpan BaseDuration = TimeSpan.FromMinutes(3);
    private static readonly PlanetaryAssetType[] GroundAssetTypes =
    [
        PlanetaryAssetType.PatrolGroup,
        PlanetaryAssetType.ExpeditionGroup,
        PlanetaryAssetType.VehicleGroup,
        PlanetaryAssetType.SupportGroup
    ];

    public async Task<GetDevGroundArmyUiStateResult> GetAsync(GetDevGroundArmyUiStateRequest request, CancellationToken cancellationToken = default)
    {
        var planetUiState = await planetUiStateService.GetAsync(new GetDevPlanetUiStateRequest(request.CivilizationId, request.PlanetId), cancellationToken);
        if (planetUiState.Planet is null)
        {
            return new GetDevGroundArmyUiStateResult(planetUiState.CivilizationId, planetUiState.SelectedPlanetId, planetUiState.KnownPlanets, null, planetUiState.Errors);
        }

        var groundStructures = planetUiState.Planet.Buildings
            .Where(x => x.Category == BuildingCategory.MilitaryGround || x.BuildingType == BuildingType.LogisticsHub)
            .ToArray();

        if (!planetUiState.Planet.IsOwnedByRequestingCivilization)
        {
            return new GetDevGroundArmyUiStateResult(
                planetUiState.CivilizationId,
                planetUiState.SelectedPlanetId,
                planetUiState.KnownPlanets,
                new DevGroundArmyCockpitDto(
                    planetUiState.Planet.PlanetId,
                    planetUiState.Planet.PlanetName,
                    planetUiState.Planet.SolarSystemId,
                    planetUiState.Planet.SolarSystemName,
                    false,
                    planetUiState.Planet.OwnerCivilizationId,
                    planetUiState.Planet.OwnerCivilizationName,
                    planetUiState.Planet.ControlStatus,
                    [],
                    null,
                    [],
                    [],
                    [],
                    [],
                    new DevGroundArmyReadinessSummaryDto(0, 0, 0, 0, 0, 0, 0),
                    new DevGroundArmyActionSummaryDto(false, "Blocked", "Planet is not controlled by the requesting civilization.", false, "Unsupported", "No disponible en esta build."),
                    new DevGroundArmyDiagnosticsDto(request.PlanetId, planetUiState.Planet.SolarSystemId, planetUiState.Planet.OwnerCivilizationId, false, false, ["Selected planet is non-owned; ground management data stays hidden."], ["Ground Army remains readiness-only in this build.", "No combat, invasion, occupation, or troop movement is exposed here."])),
                []);
        }

        var realBuildings = await dbContext.Set<PlanetBuilding>().AsNoTracking().Where(x => x.PlanetId == planetUiState.Planet.PlanetId).ToListAsync(cancellationToken);
        var population = await dbContext.Set<PlanetPopulationProfile>().AsNoTracking().SingleOrDefaultAsync(x => x.PlanetId == planetUiState.Planet.PlanetId, cancellationToken);
        var stockRows = await dbContext.Set<PlanetaryAssetStock>().AsNoTracking().Where(x => x.PlanetId == planetUiState.Planet.PlanetId).OrderBy(x => x.AssetType).ToListAsync(cancellationToken);
        var queueRows = await dbContext.Set<AssetProductionOrder>().AsNoTracking()
            .Where(x => x.PlanetId == planetUiState.Planet.PlanetId && x.Target == AssetProductionTarget.Planetary)
            .OrderBy(x => x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active ? 0 : 1)
            .ThenBy(x => x.Sequence)
            .Take(8)
            .ToListAsync(cancellationToken);

        var nowUtc = DateTime.UtcNow;
        var openQueueCount = queueRows.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active);
        var dueQueueCount = queueRows.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && x.EndsAtUtc <= nowUtc);
        var buildingBonus = population is null ? 0 : realBuildings.Sum(PlanetMilitaryCapacityCalculator.GetGroundForceCapacityBonus);
        var totalCapacity = population is null ? 0 : PlanetMilitaryCapacityCalculator.CalculateGroundForceCapacity(population, realBuildings);

        var catalog = GroundAssetTypes.Select(assetType =>
        {
            var definition = PlanetaryAssetCatalog.Get(assetType);
            var requiredBuilding = realBuildings.SingleOrDefault(x => x.BuildingType == definition.Requirement.RequiredBuildingType);
            var currentStock = stockRows.SingleOrDefault(x => x.AssetType == assetType)?.Quantity ?? 0;
            var (status, reason) = ResolveAvailability(definition, planetUiState.Planet.Stockpile, population, requiredBuilding, totalCapacity, openQueueCount);
            return new DevGroundArmyCatalogItemDto(
                assetType.ToString(),
                definition.Requirement.RequiredBuildingType,
                definition.Requirement.RequiredBuildingLevel,
                definition.Requirement.PopulationCapacity,
                BaseDuration,
                [
                    new(ResourceType.Credits, definition.Cost.Credits),
                    new(ResourceType.Metal, definition.Cost.Metal),
                    new(ResourceType.Crystal, definition.Cost.Crystal),
                    new(ResourceType.Gas, definition.Cost.Gas)
                ],
                currentStock,
                status,
                reason);
        }).ToArray();

        var cockpit = new DevGroundArmyCockpitDto(
            planetUiState.Planet.PlanetId,
            planetUiState.Planet.PlanetName,
            planetUiState.Planet.SolarSystemId,
            planetUiState.Planet.SolarSystemName,
            true,
            planetUiState.Planet.OwnerCivilizationId,
            planetUiState.Planet.OwnerCivilizationName,
            planetUiState.Planet.ControlStatus,
            planetUiState.Planet.Stockpile,
            population is null ? null : new DevGroundArmyPopulationSummaryDto(population.TotalPopulation, population.BaseRecruitablePopulation, buildingBonus, totalCapacity),
            groundStructures,
            stockRows.Select(x => new DevGroundArmyUnitStockDto(x.AssetType.ToString(), x.Quantity)).ToArray(),
            catalog,
            queueRows.Select(x => new DevGroundArmyQueueItemDto(x.Id, x.PlanetaryAssetType?.ToString() ?? "Unknown", x.Quantity, x.Sequence, x.Status.ToString(), x.StartsAtUtc, x.EndsAtUtc, x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && x.EndsAtUtc <= nowUtc)).ToArray(),
            new DevGroundArmyReadinessSummaryDto(groundStructures.Length, stockRows.Count, stockRows.Sum(x => x.Quantity), catalog.Count(x => x.AvailabilityStatus == "Available"), catalog.Count(x => x.AvailabilityStatus != "Available"), queueRows.Count, dueQueueCount),
            new DevGroundArmyActionSummaryDto(catalog.Any(x => x.AvailabilityStatus == "Available") && openQueueCount == 0, openQueueCount > 0 ? "Blocked" : "Available", openQueueCount > 0 ? "Planet already has an open asset production order." : "Ground training can use the current development enqueue endpoint with explicit confirmation.", false, "Unsupported", "No disponible en esta build."),
            new DevGroundArmyDiagnosticsDto(request.PlanetId, planetUiState.Planet.SolarSystemId, planetUiState.Planet.OwnerCivilizationId, planetUiState.Planet.Stockpile.Count > 0, population is not null, ["Development-only ground army read model.", "This cockpit reflects terrestrial readiness and training preparation only."], ["No combat, invasion, occupation, or troop movement is exposed here.", "Complete-due remains unavailable here because the backend processing route is still global."]));

        return new GetDevGroundArmyUiStateResult(planetUiState.CivilizationId, planetUiState.SelectedPlanetId, planetUiState.KnownPlanets, cockpit, []);
    }

    private static (string Status, string Reason) ResolveAvailability(PlanetaryAssetDefinition definition, IReadOnlyList<DevPlanetResourceBalanceDto> stockpile, PlanetPopulationProfile? population, PlanetBuilding? requiredBuilding, long totalCapacity, int openQueueCount)
    {
        if (openQueueCount > 0) return ("Blocked", "Planet already has an open asset production order.");
        if (population is null) return ("Blocked", "Planet population profile was not found.");
        if (requiredBuilding is null || requiredBuilding.Level < definition.Requirement.RequiredBuildingLevel) return ("Blocked", "MissingRequiredBuilding");
        if (definition.Requirement.PopulationCapacity > totalCapacity) return ("Blocked", "InsufficientPopulationCapacity");
        var credits = stockpile.SingleOrDefault(x => x.ResourceType.ToString() == "Credits")?.Quantity ?? 0;
        var metal = stockpile.SingleOrDefault(x => x.ResourceType.ToString() == "Metal")?.Quantity ?? 0;
        var crystal = stockpile.SingleOrDefault(x => x.ResourceType.ToString() == "Crystal")?.Quantity ?? 0;
        var gas = stockpile.SingleOrDefault(x => x.ResourceType.ToString() == "Gas")?.Quantity ?? 0;
        if (credits < definition.Cost.Credits || metal < definition.Cost.Metal || crystal < definition.Cost.Crystal || gas < definition.Cost.Gas) return ("Blocked", "InsufficientResources");
        return ("Available", "Ready for explicit development confirmation.");
    }
}

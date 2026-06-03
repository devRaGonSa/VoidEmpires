using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Assets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Assets;

public sealed class DevShipyardUiStateService(VoidEmpiresDbContext dbContext) : IDevShipyardUiStateService
{
    private static readonly TimeSpan BaseDurationPerUnit = TimeSpan.FromMinutes(3);

    public async Task<GetDevShipyardUiStateResult> GetAsync(
        GetDevShipyardUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevShipyardUiStateResult(request.CivilizationId, null, [], null, ["Civilization id is required."]);
        }

        var civilization = await dbContext.Set<Civilization>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.CivilizationId, cancellationToken);
        var homePlanetId = civilization?.HomePlanetId;

        var ownedPlanets = await (
            from planet in dbContext.Set<Planet>().AsNoTracking()
            join system in dbContext.Set<SolarSystem>().AsNoTracking() on planet.SolarSystemId equals system.Id
            join ownership in dbContext.Set<PlanetOwnership>().AsNoTracking() on planet.Id equals ownership.PlanetId
            where ownership.CivilizationId == request.CivilizationId && ownership.Status == PlanetControlStatus.Active
            orderby planet.Id == homePlanetId descending, planet.Name, planet.OrbitalSlot
            select new DevShipyardPlanetOptionDto(
                planet.Id,
                planet.Name,
                system.Id,
                system.Name,
                true))
            .ToListAsync(cancellationToken);

        Guid? selectedPlanetId = request.PlanetId ?? ownedPlanets.FirstOrDefault()?.PlanetId;
        if (selectedPlanetId is null)
        {
            return new GetDevShipyardUiStateResult(request.CivilizationId, null, ownedPlanets, null, []);
        }

        var selectedPlanet = await (
            from planet in dbContext.Set<Planet>().AsNoTracking()
            join system in dbContext.Set<SolarSystem>().AsNoTracking() on planet.SolarSystemId equals system.Id
            join ownership in dbContext.Set<PlanetOwnership>().AsNoTracking()
                on planet.Id equals ownership.PlanetId into ownershipGroup
            from ownership in ownershipGroup
                .Where(x => x.Status == PlanetControlStatus.Active)
                .DefaultIfEmpty()
            where planet.Id == selectedPlanetId.Value
            select new
            {
                Planet = planet,
                System = system,
                Ownership = ownership
            })
            .SingleOrDefaultAsync(cancellationToken);

        if (selectedPlanet is null)
        {
            return new GetDevShipyardUiStateResult(
                request.CivilizationId,
                request.PlanetId,
                ownedPlanets,
                null,
                ["Planet was not found."]);
        }

        if (!ownedPlanets.Any(x => x.PlanetId == selectedPlanet.Planet.Id))
        {
            ownedPlanets = [
                .. ownedPlanets,
                new DevShipyardPlanetOptionDto(
                    selectedPlanet.Planet.Id,
                    selectedPlanet.Planet.Name,
                    selectedPlanet.System.Id,
                    selectedPlanet.System.Name,
                    selectedPlanet.Ownership?.CivilizationId == request.CivilizationId)
            ];
        }

        var isOwnedByRequestingCivilization = selectedPlanet.Ownership?.CivilizationId == request.CivilizationId;
        var ownerCivilizationName = selectedPlanet.Ownership?.CivilizationId is Guid ownerCivilizationId
            ? await dbContext.Set<Civilization>()
                .AsNoTracking()
                .Where(x => x.Id == ownerCivilizationId)
                .Select(x => x.Name)
                .SingleOrDefaultAsync(cancellationToken)
            : null;

        var stockpile = isOwnedByRequestingCivilization
            ? await dbContext.PlanetResourceStockpiles
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PlanetId == selectedPlanet.Planet.Id, cancellationToken)
            : null;
        var buildings = isOwnedByRequestingCivilization
            ? await dbContext.Set<PlanetBuilding>()
                .AsNoTracking()
                .Where(x => x.PlanetId == selectedPlanet.Planet.Id)
                .OrderBy(x => x.BuildingType)
                .ToListAsync(cancellationToken)
            : [];
        var populationProfile = isOwnedByRequestingCivilization
            ? await dbContext.Set<PlanetPopulationProfile>()
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PlanetId == selectedPlanet.Planet.Id, cancellationToken)
            : null;
        var queue = isOwnedByRequestingCivilization
            ? await dbContext.Set<AssetProductionOrder>()
                .AsNoTracking()
                .Where(x => x.PlanetId == selectedPlanet.Planet.Id && x.Target == AssetProductionTarget.Orbital && x.SpaceAssetType != null)
                .OrderBy(x => x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active ? 0 : 1)
                .ThenBy(x => x.Sequence)
                .Take(8)
                .ToListAsync(cancellationToken)
            : [];
        var orbitalStock = isOwnedByRequestingCivilization
            ? await dbContext.Set<OrbitalAssetStock>()
                .AsNoTracking()
                .Where(x => x.PlanetId == selectedPlanet.Planet.Id)
                .OrderBy(x => x.AssetType)
                .ToListAsync(cancellationToken)
            : [];

        var shipyardLevel = buildings.SingleOrDefault(x => x.BuildingType == BuildingType.Shipyard)?.Level ?? 0;
        var fleetCommandCenterLevel = buildings.SingleOrDefault(x => x.BuildingType == BuildingType.FleetCommandCenter)?.Level ?? 0;
        var logisticsHubLevel = buildings.SingleOrDefault(x => x.BuildingType == BuildingType.LogisticsHub)?.Level ?? 0;
        var openQueueCount = queue.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active);
        var nowUtc = DateTime.UtcNow;
        var dueQueueCount = queue.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && x.EndsAtUtc <= nowUtc);

        var catalog = Enum.GetValues<SpaceAssetType>()
            .Select(assetType => BuildCatalogItem(
                assetType,
                isOwnedByRequestingCivilization,
                stockpile,
                buildings,
                populationProfile,
                openQueueCount,
                orbitalStock))
            .OrderByDescending(x => x.AvailabilityStatus == "Available")
            .ThenBy(x => x.AssetType)
            .ToArray();

        var shipyardDto = new DevShipyardCockpitDto(
            selectedPlanet.Planet.Id,
            selectedPlanet.Planet.Name,
            selectedPlanet.System.Id,
            selectedPlanet.System.Name,
            isOwnedByRequestingCivilization,
            selectedPlanet.Ownership?.CivilizationId,
            ownerCivilizationName,
            selectedPlanet.Ownership?.Status,
            stockpile is null ? [] : CreateResourceStockpile(stockpile),
            new DevShipyardBuildingReadinessDto(
                shipyardLevel,
                fleetCommandCenterLevel,
                logisticsHubLevel,
                populationProfile is not null),
            catalog,
            queue.Select(x => new DevShipyardQueueItemDto(
                x.Id,
                x.SpaceAssetType!.Value,
                x.Quantity,
                x.Sequence,
                x.Status,
                x.StartsAtUtc,
                x.EndsAtUtc,
                x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && x.EndsAtUtc <= nowUtc))
                .ToArray(),
            orbitalStock.Select(x => new DevShipyardStockItemDto(x.AssetType, x.Quantity)).ToArray(),
            BuildActionSummary(isOwnedByRequestingCivilization, catalog, openQueueCount, dueQueueCount),
            BuildDiagnostics(
                request.PlanetId,
                homePlanetId,
                isOwnedByRequestingCivilization,
                stockpile is not null,
                shipyardLevel > 0,
                populationProfile is not null,
                openQueueCount));

        return new GetDevShipyardUiStateResult(
            request.CivilizationId,
            selectedPlanet.Planet.Id,
            ownedPlanets,
            shipyardDto,
            []);
    }

    private static IReadOnlyList<DevShipyardResourceBalanceDto> CreateResourceStockpile(PlanetResourceStockpile stockpile) =>
    [
        new(ResourceType.Credits, stockpile.Credits),
        new(ResourceType.Metal, stockpile.Metal),
        new(ResourceType.Crystal, stockpile.Crystal),
        new(ResourceType.Gas, stockpile.Gas)
    ];

    private static DevShipyardCatalogItemDto BuildCatalogItem(
        SpaceAssetType assetType,
        bool isOwnedByRequestingCivilization,
        PlanetResourceStockpile? stockpile,
        IReadOnlyList<PlanetBuilding> buildings,
        PlanetPopulationProfile? populationProfile,
        int openQueueCount,
        IReadOnlyList<OrbitalAssetStock> orbitalStock)
    {
        var definition = OrbitalAssetCatalog.Get(assetType);
        var requirement = definition.Requirement;
        var currentStock = orbitalStock.SingleOrDefault(x => x.AssetType == assetType)?.Quantity ?? 0;
        var crewCapacity = populationProfile is null
            ? 0
            : PlanetMilitaryCapacityCalculator.CalculateShipCrewCapacity(populationProfile, buildings);

        var (status, reason) = ResolveAvailability(
            isOwnedByRequestingCivilization,
            stockpile,
            buildings,
            populationProfile,
            openQueueCount,
            requirement,
            definition.Cost,
            crewCapacity);

        return new DevShipyardCatalogItemDto(
            assetType,
            requirement.RequiredBuildingType,
            requirement.RequiredBuildingLevel,
            requirement.OperatorCapacity,
            BaseDurationPerUnit,
            [
                new DevShipyardResourceBalanceDto(ResourceType.Credits, definition.Cost.Credits),
                new DevShipyardResourceBalanceDto(ResourceType.Metal, definition.Cost.Metal),
                new DevShipyardResourceBalanceDto(ResourceType.Crystal, definition.Cost.Crystal),
                new DevShipyardResourceBalanceDto(ResourceType.Gas, definition.Cost.Gas)
            ],
            currentStock,
            status,
            reason);
    }

    private static (string Status, string Reason) ResolveAvailability(
        bool isOwnedByRequestingCivilization,
        PlanetResourceStockpile? stockpile,
        IReadOnlyList<PlanetBuilding> buildings,
        PlanetPopulationProfile? populationProfile,
        int openQueueCount,
        AssetRequirement requirement,
        ConstructionCost cost,
        long crewCapacity)
    {
        if (!isOwnedByRequestingCivilization)
        {
            return ("Blocked", "PlanetIsNotOwned");
        }

        if (openQueueCount > 0)
        {
            return ("Blocked", "OpenProductionOrderExists");
        }

        if (stockpile is null)
        {
            return ("Blocked", "MissingResourceStockpile");
        }

        var requiredBuilding = buildings.SingleOrDefault(x => x.BuildingType == requirement.RequiredBuildingType);
        if (requiredBuilding is null || requiredBuilding.Level < requirement.RequiredBuildingLevel)
        {
            return ("Blocked", "MissingRequiredBuilding");
        }

        if (populationProfile is null)
        {
            return ("Blocked", "MissingPopulationProfile");
        }

        if (!stockpile.CanSpend(cost.Credits, cost.Metal, cost.Crystal, cost.Gas))
        {
            return ("Blocked", "InsufficientResources");
        }

        if (requirement.OperatorCapacity > crewCapacity)
        {
            return ("Blocked", "InsufficientOperatorCapacity");
        }

        return ("Available", "Ready");
    }

    private static DevShipyardActionSummaryDto BuildActionSummary(
        bool isOwnedByRequestingCivilization,
        IReadOnlyList<DevShipyardCatalogItemDto> catalog,
        int openQueueCount,
        int dueQueueCount)
    {
        var anyAvailableCatalogItem = catalog.Any(x => x.AvailabilityStatus == "Available");

        return new DevShipyardActionSummaryDto(
            QueueActionStatus: openQueueCount > 0 ? "Available" : "Blocked",
            QueueActionReason: openQueueCount > 0 ? "OpenProductionQueueVisible" : "NoOpenOrbitalProductionQueue",
            EnqueueSupported: anyAvailableCatalogItem,
            EnqueueActionStatus: anyAvailableCatalogItem ? "Available" : "Blocked",
            EnqueueActionReason: !isOwnedByRequestingCivilization
                ? "PlanetIsNotOwned"
                : anyAvailableCatalogItem
                    ? "Ready"
                    : catalog.FirstOrDefault(x => x.AvailabilityStatus != "Available")?.AvailabilityReason ?? "CatalogUnavailable",
            CompleteDueSupported: dueQueueCount > 0,
            CompleteDueActionStatus: dueQueueCount > 0 ? "Available" : "Blocked",
            CompleteDueActionReason: dueQueueCount > 0 ? "DueOrbitalProductionExists" : "NoDueOrbitalProduction",
            OpenQueueCount: openQueueCount,
            DueQueueCount: dueQueueCount);
    }

    private static DevShipyardDiagnosticsDto BuildDiagnostics(
        Guid? requestPlanetId,
        Guid? homePlanetId,
        bool isOwnedByRequestingCivilization,
        bool hasResourceStockpile,
        bool hasOwnedShipyardBuilding,
        bool hasPopulationProfile,
        int openQueueCount)
    {
        var notes = new List<string>
        {
            "Development-only shipyard read model.",
            "Orbital production queue and stock are read-only from this endpoint.",
            "Orbital stock-to-group handoff stays with fleet development endpoints."
        };

        if (!isOwnedByRequestingCivilization)
        {
            notes.Add("Selected planet is non-owned; shipyard management data stays hidden.");
        }

        if (!hasOwnedShipyardBuilding)
        {
            notes.Add("Current planet does not yet satisfy the minimum local shipyard building requirement.");
        }

        if (!hasPopulationProfile)
        {
            notes.Add("Current planet lacks local population profile data for orbital crew-capacity checks.");
        }

        if (openQueueCount > 0)
        {
            notes.Add("Only one open orbital production order is currently supported per planet.");
        }

        return new DevShipyardDiagnosticsDto(
            requestPlanetId,
            homePlanetId,
            hasResourceStockpile,
            hasOwnedShipyardBuilding,
            hasPopulationProfile,
            notes);
    }
}

using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Population;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Planets;

public sealed class DevDefenseUiStateService(
    IDevPlanetUiStateService planetUiStateService,
    VoidEmpiresDbContext dbContext) : IDevDefenseUiStateService
{
    private static readonly TimeSpan BaseDuration = TimeSpan.FromMinutes(3);
    private static readonly PlanetaryAssetType[] UnitDefenseAssetTypes =
    [
        PlanetaryAssetType.MissileBattery,
        PlanetaryAssetType.LaserTurret,
        PlanetaryAssetType.IonCannon,
        PlanetaryAssetType.PlasmaCannon
    ];

    private static readonly BuildingType[] LevelBasedDefenseTypes =
    [
        BuildingType.DefenseGrid,
        BuildingType.PlanetaryShield
    ];

    public async Task<GetDevDefenseUiStateResult> GetAsync(
        GetDevDefenseUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        var planetUiState = await planetUiStateService.GetAsync(
            new GetDevPlanetUiStateRequest(request.CivilizationId, request.PlanetId),
            cancellationToken);

        if (planetUiState.Planet is null)
        {
            return new GetDevDefenseUiStateResult(
                planetUiState.CivilizationId,
                planetUiState.SelectedPlanetId,
                planetUiState.KnownPlanets,
                null,
                planetUiState.Errors);
        }

        if (!planetUiState.Planet.IsOwnedByRequestingCivilization)
        {
            return new GetDevDefenseUiStateResult(
                planetUiState.CivilizationId,
                planetUiState.SelectedPlanetId,
                planetUiState.KnownPlanets,
                new DevDefenseCockpitDto(
                    planetUiState.Planet.PlanetId,
                    planetUiState.Planet.PlanetName,
                    planetUiState.Planet.SolarSystemId,
                    planetUiState.Planet.SolarSystemName,
                    false,
                    planetUiState.Planet.OwnerCivilizationId,
                    planetUiState.Planet.OwnerCivilizationName,
                    planetUiState.Planet.ControlStatus,
                    [],
                    CreateDefenseCatalog(),
                    [],
                    [],
                    [],
                    new DevDefenseProtectionSummaryDto(0, 0, 0, 0, 0, 0),
                    planetUiState.Planet.ActionSummary,
                    new DevDefenseDiagnosticsDto(
                        request.PlanetId,
                        planetUiState.Planet.SolarSystemId,
                        planetUiState.Planet.OwnerCivilizationId,
                        false,
                        false,
                        ["Selected planet is non-owned; defense management data stays hidden."],
                        ["No combat, interception, damage, bombardment, or invasion behavior is exposed here."])),
                []);
        }

        var realBuildings = await dbContext.Set<PlanetBuilding>()
            .AsNoTracking()
            .Where(x => x.PlanetId == planetUiState.Planet.PlanetId)
            .ToListAsync(cancellationToken);
        var population = await dbContext.Set<PlanetPopulationProfile>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.PlanetId == planetUiState.Planet.PlanetId, cancellationToken);
        var unitStockRows = await dbContext.Set<PlanetaryAssetStock>()
            .AsNoTracking()
            .Where(x => x.PlanetId == planetUiState.Planet.PlanetId && UnitDefenseAssetTypes.Contains(x.AssetType))
            .OrderBy(x => x.AssetType)
            .ToListAsync(cancellationToken);
        var unitQueueRows = await dbContext.Set<AssetProductionOrder>()
            .AsNoTracking()
            .Where(x =>
                x.PlanetId == planetUiState.Planet.PlanetId &&
                x.Target == AssetProductionTarget.Planetary &&
                x.PlanetaryAssetType != null &&
                UnitDefenseAssetTypes.Contains(x.PlanetaryAssetType.Value))
            .OrderBy(x => x.Status == AssetProductionOrderStatus.Pending || x.Status == AssetProductionOrderStatus.Active ? 0 : 1)
            .ThenBy(x => x.Sequence)
            .Take(8)
            .ToListAsync(cancellationToken);

        var defenseStructures = planetUiState.Planet.Buildings
            .Where(x => LevelBasedDefenseTypes.Contains(x.BuildingType))
            .ToArray();
        var specialDefenseOptions = planetUiState.Planet.ConstructionActions
            .Where(x => LevelBasedDefenseTypes.Contains(x.BuildingType))
            .ToArray();
        var specialDefenseQueue = planetUiState.Planet.ConstructionQueue
            .Where(x => LevelBasedDefenseTypes.Contains(x.BuildingType))
            .ToArray();
        var openConstructionBuildingTypes = planetUiState.Planet.ConstructionQueue
            .Where(x => x.Status is ConstructionQueueItemStatus.Pending or ConstructionQueueItemStatus.Active)
            .Select(x => x.BuildingType)
            .ToHashSet();
        var catalog = CreateDefenseCatalog();
        var nowUtc = DateTime.UtcNow;
        var openUnitQueueCount = unitQueueRows.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active);
        var dueUnitQueueCount = unitQueueRows.Count(x => x.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && x.EndsAtUtc <= nowUtc);
        var totalCapacity = population is null ? 0 : PlanetMilitaryCapacityCalculator.CalculateGroundForceCapacity(population, realBuildings);
        var unitDefenseOptions = CreateUnitDefenseOptions(
            planetUiState.Planet.Stockpile,
            realBuildings,
            population,
            openConstructionBuildingTypes,
            totalCapacity,
            openUnitQueueCount,
            unitStockRows);
        var defenseOptions = unitDefenseOptions.Concat(specialDefenseOptions).ToArray();
        var defenseQueue = CreateUnitDefenseQueue(unitQueueRows, nowUtc)
            .Concat(specialDefenseQueue)
            .ToArray();

        var protectionSummary = new DevDefenseProtectionSummaryDto(
            StructureCount: defenseStructures.Length + unitStockRows.Count,
            TotalDefenseLevel: defenseStructures.Sum(x => x.Level) + unitStockRows.Sum(x => x.Quantity),
            AvailableOptionCount: defenseOptions.Count(x => x.AvailabilityStatus == "Available"),
            BlockedOptionCount: defenseOptions.Count(x => x.AvailabilityStatus != "Available"),
            QueueItemCount: defenseQueue.Length,
            DueQueueItemCount: defenseQueue.Count(x => x.IsDue) + dueUnitQueueCount);

        var notes = new List<string>
        {
            "Development-only defenses read model.",
            "This cockpit reflects defensive unit production readiness and special shield construction readiness, not combat behavior."
        };

        if (!planetUiState.Planet.IsOwnedByRequestingCivilization)
        {
            notes.Add("Selected planet is non-owned; defense management data stays hidden.");
        }

        if (planetUiState.Planet.ActionSummary.CompleteDueSupported)
        {
            notes.Add("Due construction completion remains disabled here because the backend completion route is global.");
        }

        var limitations = new List<string>
        {
            "Defensive unit production uses the existing planetary asset queue; combat behavior remains deferred.",
            "Shielding research is not applied as active mitigation in this cockpit.",
            "No combat, interception, damage, bombardment, or invasion behavior is exposed here."
        };

        var cockpit = new DevDefenseCockpitDto(
            planetUiState.Planet.PlanetId,
            planetUiState.Planet.PlanetName,
            planetUiState.Planet.SolarSystemId,
            planetUiState.Planet.SolarSystemName,
            planetUiState.Planet.IsOwnedByRequestingCivilization,
            planetUiState.Planet.OwnerCivilizationId,
            planetUiState.Planet.OwnerCivilizationName,
            planetUiState.Planet.ControlStatus,
            planetUiState.Planet.Stockpile,
            catalog,
            defenseStructures,
            defenseOptions,
            defenseQueue,
            protectionSummary,
            planetUiState.Planet.ActionSummary,
            new DevDefenseDiagnosticsDto(
                request.PlanetId,
                planetUiState.Planet.SolarSystemId,
                planetUiState.Planet.OwnerCivilizationId,
                planetUiState.Planet.Stockpile.Count > 0,
                defenseStructures.Length > 0,
                notes,
                limitations));

        return new GetDevDefenseUiStateResult(
            planetUiState.CivilizationId,
            planetUiState.SelectedPlanetId,
            planetUiState.KnownPlanets,
            cockpit,
            []);
    }

    private static IReadOnlyList<DevPlanetConstructionActionDto> CreateUnitDefenseOptions(
        IReadOnlyList<DevPlanetResourceBalanceDto> stockpile,
        IReadOnlyList<PlanetBuilding> buildings,
        PlanetPopulationProfile? population,
        IReadOnlySet<BuildingType> openConstructionBuildingTypes,
        long totalCapacity,
        int openQueueCount,
        IReadOnlyList<PlanetaryAssetStock> stockRows)
    {
        return UnitDefenseAssetTypes
            .Select(assetType =>
            {
                var definition = PlanetaryAssetCatalog.Get(assetType);
                var buildingType = MapDefenseAssetToBuilding(assetType);
                var buildingDefinition = BuildingCatalog.Get(buildingType);
                var requiredBuilding = buildings.SingleOrDefault(x => x.BuildingType == definition.Requirement.RequiredBuildingType);
                var currentStock = stockRows.SingleOrDefault(x => x.AssetType == assetType)?.Quantity ?? 0;
                var (status, reason) = ResolveUnitAvailability(
                    definition,
                    stockpile,
                    population,
                    requiredBuilding,
                    openConstructionBuildingTypes,
                    totalCapacity,
                    openQueueCount);

                return new DevPlanetConstructionActionDto(
                    ConstructionQueueItemAction.Construct,
                    buildingType,
                    BuildingCategory.Defense,
                    currentStock,
                    currentStock + 1,
                    status,
                    reason,
                    BaseDuration,
                    [
                        new(ResourceType.Credits, definition.Cost.Credits),
                        new(ResourceType.Metal, definition.Cost.Metal),
                        new(ResourceType.Crystal, definition.Cost.Crystal),
                        new(ResourceType.Gas, definition.Cost.Gas)
                    ],
                    new DevPlanetConstructionActionDisplayDto(
                        "Construir",
                        buildingDefinition.DisplayName,
                        "Defensa planetaria",
                        status == "Available" ? "Disponible" : "Bloqueada",
                        GetUnitReasonLabel(reason, definition)),
                    new DevPlanetBuildingMetadataDto(
                        buildingDefinition.DisplayName,
                        buildingDefinition.Description,
                        "Defensa planetaria",
                        buildingDefinition.RoleKey,
                        buildingDefinition.RoleLabel,
                        buildingDefinition.ModuleKey,
                        buildingDefinition.ModuleLabel,
                        buildingDefinition.ImageKey,
                        buildingDefinition.IconKey,
                        buildingDefinition.SortOrder,
                        "unit-production-x-base-3m",
                        "Duracion base de 3 minutos por unidad.",
                        $"Requiere {BuildingCatalog.Get(definition.Requirement.RequiredBuildingType).DisplayName} nivel {definition.Requirement.RequiredBuildingLevel}"));
            })
            .ToArray();
    }

    private static IReadOnlyList<DevPlanetConstructionQueueItemDto> CreateUnitDefenseQueue(
        IReadOnlyList<AssetProductionOrder> queueRows,
        DateTime nowUtc)
    {
        return queueRows.Select(order =>
        {
            var assetType = order.PlanetaryAssetType ?? PlanetaryAssetType.MissileBattery;
            var buildingType = MapDefenseAssetToBuilding(assetType);
            var definition = PlanetaryAssetCatalog.Get(assetType);
            var buildingDefinition = BuildingCatalog.Get(buildingType);

            return new DevPlanetConstructionQueueItemDto(
                order.Id,
                ConstructionQueueItemAction.Construct,
                (ConstructionQueueItemStatus)(int)order.Status,
                buildingType,
                BuildingCategory.Defense,
                order.Quantity,
                order.Sequence,
                order.StartsAtUtc,
                order.EndsAtUtc,
                order.Status is AssetProductionOrderStatus.Pending or AssetProductionOrderStatus.Active && order.EndsAtUtc <= nowUtc,
                [
                    new(ResourceType.Credits, definition.Cost.Credits * order.Quantity),
                    new(ResourceType.Metal, definition.Cost.Metal * order.Quantity),
                    new(ResourceType.Crystal, definition.Cost.Crystal * order.Quantity),
                    new(ResourceType.Gas, definition.Cost.Gas * order.Quantity)
                ],
                new DevPlanetConstructionQueueItemDisplayDto(
                    "Construir",
                    GetQueueStatusLabel(order.Status),
                    buildingDefinition.DisplayName,
                    "Defensa planetaria"));
        }).ToArray();
    }

    private static string GetQueueStatusLabel(AssetProductionOrderStatus status) => status switch
    {
        AssetProductionOrderStatus.Pending => "Pendiente",
        AssetProductionOrderStatus.Active => "Activa",
        AssetProductionOrderStatus.Completed => "Completada",
        AssetProductionOrderStatus.Cancelled => "Cancelada",
        _ => status.ToString()
    };

    private static (string Status, string Reason) ResolveUnitAvailability(
        PlanetaryAssetDefinition definition,
        IReadOnlyList<DevPlanetResourceBalanceDto> stockpile,
        PlanetPopulationProfile? population,
        PlanetBuilding? requiredBuilding,
        IReadOnlySet<BuildingType> openConstructionBuildingTypes,
        long totalCapacity,
        int openQueueCount)
    {
        if (openQueueCount > 0) return ("Blocked", "Planet already has an open asset production order.");
        if (population is null) return ("Blocked", "Planet population profile was not found.");
        if (openConstructionBuildingTypes.Contains(definition.Requirement.RequiredBuildingType)) return ("Blocked", "RequiredBuildingInConstruction");
        if (requiredBuilding is null || requiredBuilding.Level < definition.Requirement.RequiredBuildingLevel) return ("Blocked", "MissingRequiredBuilding");
        if (definition.Requirement.PopulationCapacity > totalCapacity) return ("Blocked", "InsufficientPopulationCapacity");

        var credits = stockpile.SingleOrDefault(x => x.ResourceType == ResourceType.Credits)?.Quantity ?? 0;
        var metal = stockpile.SingleOrDefault(x => x.ResourceType == ResourceType.Metal)?.Quantity ?? 0;
        var crystal = stockpile.SingleOrDefault(x => x.ResourceType == ResourceType.Crystal)?.Quantity ?? 0;
        var gas = stockpile.SingleOrDefault(x => x.ResourceType == ResourceType.Gas)?.Quantity ?? 0;

        if (credits < definition.Cost.Credits || metal < definition.Cost.Metal || crystal < definition.Cost.Crystal || gas < definition.Cost.Gas)
        {
            return ("InsufficientResources", "InsufficientResources");
        }

        return ("Available", "Ready for explicit development confirmation.");
    }

    private static string GetUnitReasonLabel(string reason, PlanetaryAssetDefinition definition) => reason switch
    {
        "Ready for explicit development confirmation." => "Lista para produccion",
        "Planet already has an open asset production order." => "La cola defensiva ya tiene una orden abierta",
        "Planet population profile was not found." => "Falta perfil de poblacion local",
        "RequiredBuildingInConstruction" => $"Requiere {BuildingCatalog.Get(definition.Requirement.RequiredBuildingType).DisplayName} sin obras activas",
        "MissingRequiredBuilding" => $"Requiere {BuildingCatalog.Get(definition.Requirement.RequiredBuildingType).DisplayName} nivel {definition.Requirement.RequiredBuildingLevel}",
        "InsufficientPopulationCapacity" => "Capacidad local insuficiente",
        "InsufficientResources" => "Recursos insuficientes",
        _ => reason
    };

    private static BuildingType MapDefenseAssetToBuilding(PlanetaryAssetType assetType) => assetType switch
    {
        PlanetaryAssetType.MissileBattery => BuildingType.MissileBattery,
        PlanetaryAssetType.LaserTurret => BuildingType.LaserTurret,
        PlanetaryAssetType.IonCannon => BuildingType.IonCannon,
        PlanetaryAssetType.PlasmaCannon => BuildingType.PlasmaCannon,
        _ => BuildingType.MissileBattery
    };

    private static IReadOnlyList<DevDefenseCatalogItemDto> CreateDefenseCatalog()
    {
        return Enum.GetValues<BuildingType>()
            .Select(BuildingCatalog.Get)
            .Where(x => x.Category == BuildingCategory.Defense)
            .OrderBy(x => x.SortOrder)
            .Select(definition => new DevDefenseCatalogItemDto(
                definition.BuildingType,
                definition.DisplayName,
                definition.Description,
                "Defense",
                "Proteccion planetaria",
                definition.RoleKey,
                definition.RoleLabel,
                definition.ModuleKey,
                definition.ModuleLabel,
                definition.ImageKey,
                definition.IconKey,
                definition.SortOrder,
                "ConstructionBackedDefenseReadiness",
                "Depende de la cola de Construccion y de capacidad planetaria visible.",
                "CombatSystemDeferred",
                "La mitigacion, intercepcion y dano real siguen diferidos hasta el sistema de combate.",
                ["Defense", "Readiness", "NonCombat", "ConstructionBacked"]))
            .ToArray();
    }
}

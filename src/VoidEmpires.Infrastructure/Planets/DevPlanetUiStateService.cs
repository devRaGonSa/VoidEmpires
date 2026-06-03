using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Planets;
using VoidEmpires.Domain.Buildings;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Domain.Research;
using VoidEmpires.Infrastructure.Buildings;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Planets;

public sealed class DevPlanetUiStateService(VoidEmpiresDbContext dbContext) : IDevPlanetUiStateService
{
    private static readonly TimeSpan BaseConstructionDuration = TimeSpan.FromMinutes(5);

    public async Task<GetDevPlanetUiStateResult> GetAsync(
        GetDevPlanetUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevPlanetUiStateResult(request.CivilizationId, null, [], null, ["Civilization id is required."]);
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
            select new DevPlanetOptionDto(
                planet.Id,
                planet.Name,
                system.Id,
                system.Name,
                true))
            .ToListAsync(cancellationToken);

        Guid? selectedPlanetId = request.PlanetId ?? ownedPlanets.FirstOrDefault()?.PlanetId;
        if (selectedPlanetId is null)
        {
            return new GetDevPlanetUiStateResult(request.CivilizationId, null, ownedPlanets, null, []);
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
            return new GetDevPlanetUiStateResult(
                request.CivilizationId,
                request.PlanetId,
                ownedPlanets,
                null,
                ["Planet was not found."]);
        }

        if (!ownedPlanets.Any(x => x.PlanetId == selectedPlanet.Planet.Id))
        {
            ownedPlanets = [
                ..ownedPlanets,
                new DevPlanetOptionDto(
                    selectedPlanet.Planet.Id,
                    selectedPlanet.Planet.Name,
                    selectedPlanet.System.Id,
                    selectedPlanet.System.Name,
                    selectedPlanet.Ownership?.CivilizationId == request.CivilizationId)
            ];
        }

        var ownerCivilizationName = selectedPlanet.Ownership?.CivilizationId is Guid ownerCivilizationId
            ? await dbContext.Set<Civilization>()
                .AsNoTracking()
                .Where(x => x.Id == ownerCivilizationId)
                .Select(x => x.Name)
                .SingleOrDefaultAsync(cancellationToken)
            : null;
        var isOwnedByRequestingCivilization = selectedPlanet.Ownership?.CivilizationId == request.CivilizationId;
        var relevantResearch = await dbContext.ResearchProjects
            .AsNoTracking()
            .Where(x =>
                x.CivilizationId == request.CivilizationId &&
                (x.ResearchType == ResearchType.PlanetaryEngineering ||
                    x.ResearchType == ResearchType.ResourceExtraction ||
                    x.ResearchType == ResearchType.ConstructionAutomation))
            .ToDictionaryAsync(x => x.ResearchType, x => x.Level, cancellationToken);

        var stockpile = isOwnedByRequestingCivilization
            ? await dbContext.PlanetResourceStockpiles
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PlanetId == selectedPlanet.Planet.Id, cancellationToken)
            : null;
        var productionProfile = isOwnedByRequestingCivilization
            ? await dbContext.PlanetProductionProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(x => x.PlanetId == selectedPlanet.Planet.Id, cancellationToken)
            : null;
        var buildingCapacity = isOwnedByRequestingCivilization
            ? await dbContext.Set<PlanetBuildingCapacity>()
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
        var constructionQueue = isOwnedByRequestingCivilization
            ? await dbContext.Set<PlanetConstructionOrder>()
                .AsNoTracking()
                .Where(x => x.PlanetId == selectedPlanet.Planet.Id)
                .OrderBy(x => x.Status == ConstructionQueueItemStatus.Pending || x.Status == ConstructionQueueItemStatus.Active ? 0 : 1)
                .ThenBy(x => x.Sequence)
                .Take(8)
                .ToListAsync(cancellationToken)
            : [];

        var usedCapacity = buildings.Sum(x => x.Footprint);
        var planetaryEngineeringLevel = relevantResearch.GetValueOrDefault(ResearchType.PlanetaryEngineering);
        var researchCapacityBonus = ResearchBonusCalculator.GetPlanetaryEngineeringCapacityBonus(planetaryEngineeringLevel);
        var constructionAutomationLevel = relevantResearch.GetValueOrDefault(ResearchType.ConstructionAutomation);
        var productionMultiplier = ResearchBonusCalculator.GetResourceProductionMultiplier(
            relevantResearch.GetValueOrDefault(ResearchType.ResourceExtraction));
        var nowUtc = DateTime.UtcNow;
        var openConstructionOrderCount = constructionQueue.Count(x => x.Status is ConstructionQueueItemStatus.Pending or ConstructionQueueItemStatus.Active);
        var dueConstructionCount = constructionQueue.Count(x =>
            x.Status is ConstructionQueueItemStatus.Pending or ConstructionQueueItemStatus.Active &&
            x.EndsAtUtc <= nowUtc);

        var orbitalContext = new DevPlanetOrbitalContextDto(
            StationedGroups: await dbContext.Set<OrbitalGroup>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.CurrentPlanetId == selectedPlanet.Planet.Id &&
                    x.CivilizationId == request.CivilizationId &&
                    x.Status == OrbitalGroupStatus.Stationed,
                    cancellationToken),
            ActiveDepartures: await dbContext.Set<OrbitalTransfer>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.OriginPlanetId == selectedPlanet.Planet.Id &&
                    x.CivilizationId == request.CivilizationId &&
                    x.Status == OrbitalTransferStatus.Planned,
                    cancellationToken),
            ActiveArrivals: await dbContext.Set<OrbitalTransfer>()
                .AsNoTracking()
                .CountAsync(x =>
                    x.DestinationPlanetId == selectedPlanet.Planet.Id &&
                    x.CivilizationId == request.CivilizationId &&
                    x.Status == OrbitalTransferStatus.Planned,
                    cancellationToken));

        var actionSummary = BuildActionSummary(
            isOwnedByRequestingCivilization,
            stockpile is not null,
            openConstructionOrderCount,
            dueConstructionCount);

        var actions = isOwnedByRequestingCivilization
            ? Enum.GetValues<BuildingType>()
                .Select(buildingType => BuildConstructionAction(
                    buildingType,
                    buildings,
                    stockpile,
                    buildingCapacity,
                    usedCapacity,
                    researchCapacityBonus,
                    openConstructionOrderCount,
                    constructionAutomationLevel))
                .OrderByDescending(x => x.AvailabilityStatus == "Available")
                .ThenBy(x => x.Category)
                .ThenBy(x => x.BuildingType)
                .ToArray()
            : [];

        var planetDto = new DevPlanetCockpitDto(
            selectedPlanet.Planet.Id,
            selectedPlanet.Planet.Name,
            selectedPlanet.System.Id,
            selectedPlanet.System.Name,
            selectedPlanet.Planet.OrbitalSlot,
            selectedPlanet.Planet.PlanetType,
            selectedPlanet.Planet.Size,
            selectedPlanet.Planet.ColonizationStatus,
            isOwnedByRequestingCivilization,
            selectedPlanet.Ownership?.CivilizationId,
            ownerCivilizationName,
            selectedPlanet.Ownership?.Status,
            stockpile is null ? [] : CreateStockpile(stockpile),
            productionProfile is null
                ? null
                : new DevPlanetProductionSummaryDto(
                    productionProfile.CreditsPerHour * productionMultiplier,
                    productionProfile.MetalPerHour * productionMultiplier,
                    productionProfile.CrystalPerHour * productionMultiplier,
                    productionProfile.GasPerHour * productionMultiplier,
                    productionMultiplier),
            buildingCapacity is null
                ? null
                : new DevPlanetBuildingCapacityDto(
                    usedCapacity,
                    buildingCapacity.BaseCapacity,
                    buildingCapacity.BonusCapacity,
                    researchCapacityBonus,
                    buildingCapacity.TotalCapacity + researchCapacityBonus),
            buildings
                .Select(x => new DevPlanetBuildingDto(
                    x.BuildingType,
                    BuildingCatalog.Get(x.BuildingType).Category,
                    x.Level,
                    x.Footprint,
                    CreateBuildingDisplay(x.BuildingType)))
                .ToArray(),
            constructionQueue
                .Select(x => new DevPlanetConstructionQueueItemDto(
                    x.Id,
                    x.Action,
                    x.Status,
                    x.BuildingType,
                    BuildingCatalog.Get(x.BuildingType).Category,
                    x.TargetLevel,
                    x.Sequence,
                    x.StartsAtUtc,
                    x.EndsAtUtc,
                    x.Status is ConstructionQueueItemStatus.Pending or ConstructionQueueItemStatus.Active && x.EndsAtUtc <= nowUtc,
                    CreateConstructionCost(x.BuildingType, x.Action == ConstructionQueueItemAction.Upgrade ? x.TargetLevel : 1),
                    CreateQueueItemDisplay(x.Action, x.Status, x.BuildingType)))
                .ToArray(),
            actionSummary,
            actions,
            orbitalContext,
            new DevPlanetDiagnosticsDto(
                request.PlanetId,
                selectedPlanet.System.Id,
                selectedPlanet.Ownership?.CivilizationId,
                civilization?.HomePlanetId,
                stockpile is not null,
                productionProfile is not null,
                buildingCapacity is not null,
                openConstructionOrderCount,
                BuildDiagnosticsNotes(isOwnedByRequestingCivilization, productionProfile is not null, dueConstructionCount)));

        return new GetDevPlanetUiStateResult(
            request.CivilizationId,
            selectedPlanet.Planet.Id,
            ownedPlanets,
            planetDto,
            []);
    }

    private static IReadOnlyList<DevPlanetResourceBalanceDto> CreateStockpile(PlanetResourceStockpile stockpile) =>
        [
            new(ResourceType.Credits, stockpile.Credits),
            new(ResourceType.Metal, stockpile.Metal),
            new(ResourceType.Crystal, stockpile.Crystal),
            new(ResourceType.Gas, stockpile.Gas)
        ];

    private static DevPlanetBuildingDisplayDto CreateBuildingDisplay(BuildingType buildingType)
    {
        var definition = BuildingCatalog.Get(buildingType);
        return new DevPlanetBuildingDisplayDto(
            FormatBuildingTypeLabel(buildingType),
            FormatBuildingCategoryLabel(definition.Category));
    }

    private static DevPlanetConstructionQueueItemDisplayDto CreateQueueItemDisplay(
        ConstructionQueueItemAction action,
        ConstructionQueueItemStatus status,
        BuildingType buildingType)
    {
        var definition = BuildingCatalog.Get(buildingType);
        return new DevPlanetConstructionQueueItemDisplayDto(
            FormatConstructionActionLabel(action),
            FormatConstructionStatusLabel(status),
            FormatBuildingTypeLabel(buildingType),
            FormatBuildingCategoryLabel(definition.Category));
    }

    private static IReadOnlyList<DevPlanetResourceBalanceDto> CreateConstructionCost(
        BuildingType buildingType,
        int multiplier)
    {
        var definition = BuildingCatalog.Get(buildingType);
        var resolvedMultiplier = Math.Max(1, multiplier);

        return [
            new(ResourceType.Credits, definition.Cost.Credits * resolvedMultiplier),
            new(ResourceType.Metal, definition.Cost.Metal * resolvedMultiplier),
            new(ResourceType.Crystal, definition.Cost.Crystal * resolvedMultiplier),
            new(ResourceType.Gas, definition.Cost.Gas * resolvedMultiplier)
        ];
    }

    private static DevPlanetConstructionActionSummaryDto BuildActionSummary(
        bool isOwnedByRequestingCivilization,
        bool hasStockpile,
        int openConstructionOrderCount,
        int dueConstructionCount)
    {
        if (!isOwnedByRequestingCivilization)
        {
            return new DevPlanetConstructionActionSummaryDto(
                "Blocked",
                "Planet is not controlled by the requesting civilization.",
                false,
                "Unsupported",
                "No disponible en esta build.",
                0,
                CreateActionSummaryDisplay(
                    "Blocked",
                    "La colonia no esta controlada por la civilizacion solicitante.",
                    "Unsupported",
                    "No disponible en esta build."));
        }

        if (!hasStockpile)
        {
            return new DevPlanetConstructionActionSummaryDto(
                "Blocked",
                "Planet resource stockpile was not found.",
                false,
                "Unsupported",
                "No disponible en esta build.",
                dueConstructionCount,
                CreateActionSummaryDisplay(
                    "Blocked",
                    "No se encontro la reserva de recursos del planeta.",
                    "Unsupported",
                    "No disponible en esta build."));
        }

        if (openConstructionOrderCount > 0)
        {
            return new DevPlanetConstructionActionSummaryDto(
                "Blocked",
                "Planet already has an open construction order.",
                false,
                "Unsupported",
                "No disponible en esta build.",
                dueConstructionCount,
                CreateActionSummaryDisplay(
                    "Blocked",
                    "El planeta ya tiene una orden de construccion abierta.",
                    "Unsupported",
                    "No disponible en esta build."));
        }

        return new DevPlanetConstructionActionSummaryDto(
            "Available",
            "Construction enqueue can use the current development endpoint with explicit confirmation.",
            false,
            "Unsupported",
            "No disponible en esta build.",
            dueConstructionCount,
            CreateActionSummaryDisplay(
                "Available",
                "La cola de construccion puede usar el endpoint de desarrollo actual con confirmacion explicita.",
                "Unsupported",
                "No disponible en esta build."));
    }

    private static DevPlanetConstructionActionDto BuildConstructionAction(
        BuildingType buildingType,
        IReadOnlyList<PlanetBuilding> buildings,
        PlanetResourceStockpile? stockpile,
        PlanetBuildingCapacity? buildingCapacity,
        int usedCapacity,
        int researchCapacityBonus,
        int openConstructionOrderCount,
        int constructionAutomationLevel)
    {
        var existingBuilding = buildings.SingleOrDefault(x => x.BuildingType == buildingType);
        var definition = BuildingCatalog.Get(buildingType);
        var action = existingBuilding is null
            ? ConstructionQueueItemAction.Construct
            : ConstructionQueueItemAction.Upgrade;
        var currentLevel = existingBuilding?.Level ?? 0;
        var targetLevel = existingBuilding?.Level + 1 ?? definition.InitialLevel;
        var multiplier = action == ConstructionQueueItemAction.Upgrade ? targetLevel : 1;
        var estimatedDuration = ConstructionDurationCalculator.CalculateDuration(
            BaseConstructionDuration * targetLevel,
            constructionAutomationLevel);
        var cost = CreateConstructionCost(buildingType, multiplier);

        if (openConstructionOrderCount > 0)
        {
            return new DevPlanetConstructionActionDto(
                action,
                buildingType,
                definition.Category,
                currentLevel,
                targetLevel,
                "Blocked",
                "Planet already has an open construction order.",
                estimatedDuration,
                cost,
                CreateActionDisplay(
                    action,
                    buildingType,
                    "Blocked",
                    "El planeta ya tiene una orden de construccion abierta."));
        }

        if (stockpile is null)
        {
            return new DevPlanetConstructionActionDto(
                action,
                buildingType,
                definition.Category,
                currentLevel,
                targetLevel,
                "MissingResourceStockpile",
                "Planet resource stockpile was not found.",
                estimatedDuration,
                cost,
                CreateActionDisplay(
                    action,
                    buildingType,
                    "MissingResourceStockpile",
                    "No se encontro la reserva de recursos del planeta."));
        }

        if (action == ConstructionQueueItemAction.Construct)
        {
            if (buildingCapacity is null)
            {
                return new DevPlanetConstructionActionDto(
                    action,
                    buildingType,
                    definition.Category,
                    currentLevel,
                    targetLevel,
                    "MissingCapacityData",
                    "Planet building capacity was not found.",
                    estimatedDuration,
                    cost,
                    CreateActionDisplay(
                        action,
                        buildingType,
                        "MissingCapacityData",
                        "No se encontro la capacidad de edificios del planeta."));
            }

            if (!buildingCapacity.CanFit(usedCapacity, definition.Footprint, researchCapacityBonus))
            {
                return new DevPlanetConstructionActionDto(
                    action,
                    buildingType,
                    definition.Category,
                    currentLevel,
                    targetLevel,
                    "CapacityExceeded",
                    "Planet building capacity would be exceeded.",
                    estimatedDuration,
                    cost,
                    CreateActionDisplay(
                        action,
                        buildingType,
                        "CapacityExceeded",
                        "La capacidad de edificios del planeta se agotaria."));
            }
        }

        if (!stockpile.CanSpend(
                definition.Cost.Credits * multiplier,
                definition.Cost.Metal * multiplier,
                definition.Cost.Crystal * multiplier,
                definition.Cost.Gas * multiplier))
        {
            return new DevPlanetConstructionActionDto(
                action,
                buildingType,
                definition.Category,
                currentLevel,
                targetLevel,
                "InsufficientResources",
                "Insufficient resources.",
                estimatedDuration,
                cost,
                CreateActionDisplay(
                    action,
                    buildingType,
                    "InsufficientResources",
                    "No hay recursos suficientes."));
        }

        return new DevPlanetConstructionActionDto(
            action,
            buildingType,
            definition.Category,
            currentLevel,
            targetLevel,
            "Available",
            "Ready for explicit development confirmation.",
            estimatedDuration,
            cost,
            CreateActionDisplay(
                action,
                buildingType,
                "Available",
                "Lista para confirmacion explicita de desarrollo."));
    }

    private static DevPlanetConstructionActionSummaryDisplayDto CreateActionSummaryDisplay(
        string queueActionStatus,
        string queueActionReasonLabel,
        string completeDueActionStatus,
        string completeDueActionReasonLabel) =>
        new(
            FormatConstructionAvailabilityLabel(queueActionStatus),
            queueActionReasonLabel,
            FormatConstructionAvailabilityLabel(completeDueActionStatus),
            completeDueActionReasonLabel);

    private static DevPlanetConstructionActionDisplayDto CreateActionDisplay(
        ConstructionQueueItemAction action,
        BuildingType buildingType,
        string availabilityStatus,
        string availabilityReasonLabel)
    {
        var definition = BuildingCatalog.Get(buildingType);
        return new DevPlanetConstructionActionDisplayDto(
            FormatConstructionActionLabel(action),
            FormatBuildingTypeLabel(buildingType),
            FormatBuildingCategoryLabel(definition.Category),
            FormatConstructionAvailabilityLabel(availabilityStatus),
            availabilityReasonLabel);
    }

    private static string FormatBuildingTypeLabel(BuildingType buildingType) => buildingType switch
    {
        BuildingType.CommandCenter => "Centro de mando",
        BuildingType.MetalMine => "Mina de metal",
        BuildingType.CrystalMine => "Mina de cristal",
        BuildingType.GasExtractor => "Extractor de gas",
        BuildingType.SolarPlant => "Planta solar",
        BuildingType.ResearchLab => "Laboratorio de investigacion",
        BuildingType.Shipyard => "Astillero",
        BuildingType.DefenseGrid => "Malla defensiva",
        BuildingType.HabitationDistrict => "Distrito habitacional",
        BuildingType.MedicalCenter => "Centro medico",
        BuildingType.MilitaryAcademy => "Academia militar",
        BuildingType.Barracks => "Barracones",
        BuildingType.CrewAcademy => "Academia de tripulacion",
        BuildingType.FleetCommandCenter => "Mando de flota",
        BuildingType.LogisticsHub => "Centro logistico",
        _ => buildingType.ToString()
    };

    private static string FormatBuildingCategoryLabel(BuildingCategory category) => category switch
    {
        BuildingCategory.Civilian => "Civil",
        BuildingCategory.Industrial => "Industrial",
        BuildingCategory.Research => "Investigacion",
        BuildingCategory.MilitaryGround => "Militar terrestre",
        BuildingCategory.MilitarySpace => "Militar espacial",
        BuildingCategory.Defense => "Defensa",
        BuildingCategory.Logistics => "Logistica",
        _ => category.ToString()
    };

    private static string FormatConstructionActionLabel(ConstructionQueueItemAction action) => action switch
    {
        ConstructionQueueItemAction.Construct => "Construir",
        ConstructionQueueItemAction.Upgrade => "Mejorar",
        _ => action.ToString()
    };

    private static string FormatConstructionStatusLabel(ConstructionQueueItemStatus status) => status switch
    {
        ConstructionQueueItemStatus.Pending => "Pendiente",
        ConstructionQueueItemStatus.Active => "Activa",
        ConstructionQueueItemStatus.Completed => "Completada",
        ConstructionQueueItemStatus.Cancelled => "Cancelada",
        _ => status.ToString()
    };

    private static string FormatConstructionAvailabilityLabel(string availabilityStatus) => availabilityStatus switch
    {
        "Available" => "Disponible",
        "Blocked" => "Bloqueada",
        "MissingResourceStockpile" => "Sin reservas",
        "MissingCapacityData" => "Sin capacidad",
        "CapacityExceeded" => "Capacidad agotada",
        "InsufficientResources" => "Recursos insuficientes",
        "Unsupported" => "No disponible",
        _ => availabilityStatus
    };

    private static IReadOnlyList<string> BuildDiagnosticsNotes(
        bool isOwnedByRequestingCivilization,
        bool hasProductionProfile,
        int dueConstructionCount)
    {
        var notes = new List<string>();

        if (!isOwnedByRequestingCivilization)
        {
            notes.Add("Management data is intentionally limited for non-owned planets.");
        }

        if (!hasProductionProfile)
        {
            notes.Add("No production profile is currently configured for this planet.");
        }

        if (dueConstructionCount > 0)
        {
            notes.Add("Due construction completion stays disabled here because the current backend endpoint is global, not planet-scoped.");
        }

        return notes;
    }
}

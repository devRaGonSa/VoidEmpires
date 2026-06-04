using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Markets;
using VoidEmpires.Domain.Colonization;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;
using VoidEmpires.Domain.Galaxy;
using VoidEmpires.Domain.Players;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Markets;

public sealed class DevMarketUiStateService(VoidEmpiresDbContext dbContext) : IDevMarketUiStateService
{
    public async Task<GetDevMarketUiStateResult> GetAsync(
        GetDevMarketUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevMarketUiStateResult(request.CivilizationId, null, [], null, ["Civilization id is required."]);
        }

        var civilization = await dbContext.Set<Civilization>()
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == request.CivilizationId, cancellationToken);

        if (civilization is null)
        {
            return new GetDevMarketUiStateResult(request.CivilizationId, null, [], null, ["Civilization was not found."]);
        }

        var ownedPlanets = await (
            from planet in dbContext.Set<Planet>().AsNoTracking()
            join system in dbContext.Set<SolarSystem>().AsNoTracking() on planet.SolarSystemId equals system.Id
            join ownership in dbContext.Set<PlanetOwnership>().AsNoTracking() on planet.Id equals ownership.PlanetId
            where ownership.CivilizationId == request.CivilizationId && ownership.Status == PlanetControlStatus.Active
            orderby planet.Id == civilization.HomePlanetId descending, planet.Name, planet.OrbitalSlot
            select new
            {
                planet.Id,
                planet.Name,
                planet.SolarSystemId,
                SolarSystemName = system.Name
            })
            .ToListAsync(cancellationToken);

        Guid? selectedPlanetId = request.PlanetId ?? ownedPlanets.FirstOrDefault()?.Id;
        if (request.PlanetId is not null && ownedPlanets.All(x => x.Id != request.PlanetId.Value))
        {
            return new GetDevMarketUiStateResult(request.CivilizationId, null, [], null, ["Planet was not found."]);
        }

        var ownedPlanetIds = ownedPlanets.Select(x => x.Id).ToArray();
        var stockpiles = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.PlanetResourceStockpiles
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .OrderBy(x => x.PlanetId)
                .ToListAsync(cancellationToken);
        var productionProfiles = ownedPlanetIds.Length == 0
            ? []
            : await dbContext.PlanetProductionProfiles
                .AsNoTracking()
                .Where(x => ownedPlanetIds.Contains(x.PlanetId))
                .OrderBy(x => x.PlanetId)
                .ToListAsync(cancellationToken);

        var knownPlanets = ownedPlanets
            .Select(planet => new DevMarketPlanetOptionDto(
                planet.Id,
                planet.Name,
                planet.SolarSystemId,
                planet.SolarSystemName,
                true,
                CreateReserveRows(stockpiles.SingleOrDefault(x => x.PlanetId == planet.Id)),
                productionProfiles.Any(x => x.PlanetId == planet.Id)))
            .ToArray();

        if (selectedPlanetId is null)
        {
            return new GetDevMarketUiStateResult(request.CivilizationId, null, knownPlanets, null, []);
        }

        var selectedPlanet = ownedPlanets.Single(x => x.Id == selectedPlanetId.Value);
        var selectedStockpile = stockpiles.SingleOrDefault(x => x.PlanetId == selectedPlanetId.Value);
        var selectedProduction = productionProfiles.SingleOrDefault(x => x.PlanetId == selectedPlanetId.Value);

        var stationedGroups = await dbContext.Set<OrbitalGroup>()
            .AsNoTracking()
            .CountAsync(x =>
                x.CivilizationId == request.CivilizationId &&
                x.CurrentPlanetId == selectedPlanetId.Value &&
                x.Status == OrbitalGroupStatus.Stationed,
                cancellationToken);
        var activeDepartures = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .CountAsync(x =>
                x.CivilizationId == request.CivilizationId &&
                x.OriginPlanetId == selectedPlanetId.Value &&
                x.Status == OrbitalTransferStatus.Planned,
                cancellationToken);
        var activeArrivals = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .CountAsync(x =>
                x.CivilizationId == request.CivilizationId &&
                x.DestinationPlanetId == selectedPlanetId.Value &&
                x.Status == OrbitalTransferStatus.Planned,
                cancellationToken);
        var civilizationActiveTransfers = await dbContext.Set<OrbitalTransfer>()
            .AsNoTracking()
            .CountAsync(x =>
                x.CivilizationId == request.CivilizationId &&
                x.Status == OrbitalTransferStatus.Planned,
                cancellationToken);

        var civilizationReserves = AggregateReserveRows(stockpiles);
        var selectedReserves = CreateReserveRows(selectedStockpile);
        var productionSummary = selectedProduction is null
            ? null
            : new DevMarketProductionSummaryDto(
                selectedProduction.CreditsPerHour,
                selectedProduction.MetalPerHour,
                selectedProduction.CrystalPerHour,
                selectedProduction.GasPerHour);

        var market = new DevMarketCockpitDto(
            selectedPlanet.Id,
            selectedPlanet.Name,
            selectedPlanet.SolarSystemId,
            selectedPlanet.SolarSystemName,
            civilizationReserves,
            selectedReserves,
            productionSummary,
            BuildReferenceRatios(selectedReserves, productionSummary),
            BuildSignals(selectedReserves, productionSummary),
            BuildFutureActions(),
            new DevMarketLogisticsSummaryDto(
                stationedGroups,
                activeDepartures,
                activeArrivals,
                civilizationActiveTransfers,
                true),
            new DevMarketDiagnosticsDto(
                request.PlanetId,
                civilization.HomePlanetId,
                ownedPlanets.Count,
                selectedStockpile is not null,
                selectedProduction is not null,
                BuildNotes(selectedStockpile is not null, selectedProduction is not null, civilizationActiveTransfers)),
            [
                "Market remains read-only in this phase.",
                "Civilization reserves are aggregated from owned planet stockpiles rather than a persisted shared treasury.",
                "Reference ratios are advisory only and derived from current reserve and production context.",
                "Trade-route, listing, and transfer execution remain disabled placeholders."
            ]);

        return new GetDevMarketUiStateResult(request.CivilizationId, selectedPlanetId, knownPlanets, market, []);
    }

    private static IReadOnlyList<DevMarketResourceReserveDto> CreateReserveRows(PlanetResourceStockpile? stockpile) =>
    [
        new(ResourceType.Credits, stockpile?.Credits ?? 0),
        new(ResourceType.Metal, stockpile?.Metal ?? 0),
        new(ResourceType.Crystal, stockpile?.Crystal ?? 0),
        new(ResourceType.Gas, stockpile?.Gas ?? 0)
    ];

    private static IReadOnlyList<DevMarketResourceReserveDto> AggregateReserveRows(IReadOnlyList<PlanetResourceStockpile> stockpiles) =>
    [
        new(ResourceType.Credits, stockpiles.Sum(x => x.Credits)),
        new(ResourceType.Metal, stockpiles.Sum(x => x.Metal)),
        new(ResourceType.Crystal, stockpiles.Sum(x => x.Crystal)),
        new(ResourceType.Gas, stockpiles.Sum(x => x.Gas))
    ];

    private static IReadOnlyList<DevMarketReferenceRatioDto> BuildReferenceRatios(
        IReadOnlyList<DevMarketResourceReserveDto> reserves,
        DevMarketProductionSummaryDto? production) =>
        reserves.Select(item =>
        {
            var productionPerHour = GetProduction(production, item.ResourceType);
            var coverageHours = productionPerHour > 0 ? item.Quantity / productionPerHour : (decimal?)null;
            var ratio = coverageHours switch
            {
                null when item.Quantity <= 50 => 1.75m,
                null => 1.15m,
                < 6m => 1.60m,
                < 12m => 1.25m,
                < 24m => 1.00m,
                _ => 0.80m
            };

            return new DevMarketReferenceRatioDto(
                item.ResourceType,
                ratio,
                productionPerHour > 0 ? "Derived" : "Low",
                productionPerHour > 0 ? "SelectedPlanetCoverage" : "SelectedPlanetReservesOnly",
                true);
        }).ToArray();

    private static IReadOnlyList<DevMarketSignalDto> BuildSignals(
        IReadOnlyList<DevMarketResourceReserveDto> reserves,
        DevMarketProductionSummaryDto? production)
    {
        var signals = reserves.Select(item =>
        {
            var productionPerHour = GetProduction(production, item.ResourceType);
            var coverageHours = productionPerHour > 0 ? item.Quantity / productionPerHour : (decimal?)null;

            if (coverageHours is null)
            {
                return new DevMarketSignalDto(
                    item.Quantity >= 100 ? "LocalReserve" : "DemandPressure",
                    item.ResourceType,
                    item.Quantity,
                    null,
                    item.Quantity >= 100 ? "Info" : "Warn",
                    "ProductionProfileUnavailable");
            }

            if (coverageHours < 6m)
            {
                return new DevMarketSignalDto("DemandPressure", item.ResourceType, item.Quantity, productionPerHour, "Warn", "LowCoverage");
            }

            if (coverageHours > 18m && item.Quantity >= 100)
            {
                return new DevMarketSignalDto("VisibleSurplus", item.ResourceType, item.Quantity, productionPerHour, "Good", "HighCoverage");
            }

            return new DevMarketSignalDto("EstimatedProduction", item.ResourceType, item.Quantity, productionPerHour, "Info", "StableFlow");
        }).ToList();

        signals.Add(new DevMarketSignalDto("FutureTradeRoute", null, 0, null, "Info", "RouteExecutionDisabled"));
        return signals;
    }

    private static IReadOnlyList<DevMarketFutureActionDto> BuildFutureActions() =>
    [
        new("market.buy.future", false, "Disabled", "ReadOnlyPhase"),
        new("market.sell.future", false, "Disabled", "ReadOnlyPhase"),
        new("market.transfer.future", false, "Disabled", "FleetExecutionOwnedByFleets"),
        new("market.route.future", false, "Disabled", "RouteExecutionDisabled"),
        new("market.auction.future", false, "Disabled", "AuctionGameplayUnavailable")
    ];

    private static IReadOnlyList<string> BuildNotes(
        bool hasSelectedStockpile,
        bool hasSelectedProduction,
        int civilizationActiveTransfers)
    {
        var notes = new List<string>
        {
            "Development-only market read model.",
            "Primary market numbers are derived from current reserves, production, and logistics context."
        };

        if (!hasSelectedStockpile)
        {
            notes.Add("Selected planet has no persisted stockpile row.");
        }

        if (!hasSelectedProduction)
        {
            notes.Add("Selected planet has no production profile, so advisory ratios fall back to reserve-only context.");
        }

        if (civilizationActiveTransfers > 0)
        {
            notes.Add("Active orbital transfers contribute logistics pressure notes only; they do not move market resources here.");
        }

        return notes;
    }

    private static decimal GetProduction(DevMarketProductionSummaryDto? production, ResourceType resourceType) => resourceType switch
    {
        ResourceType.Credits => production?.CreditsPerHour ?? 0,
        ResourceType.Metal => production?.MetalPerHour ?? 0,
        ResourceType.Crystal => production?.CrystalPerHour ?? 0,
        ResourceType.Gas => production?.GasPerHour ?? 0,
        _ => 0
    };
}

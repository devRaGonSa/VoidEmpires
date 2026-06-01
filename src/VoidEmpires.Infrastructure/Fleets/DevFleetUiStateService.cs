using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Application.StrategicMap;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.StrategicMap;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class DevFleetUiStateService(
    VoidEmpiresDbContext dbContext,
    IFleetOperationalOverviewService fleetOverviewService,
    IDevFleetActionManifestService actionManifestService,
    IInterceptionOpportunityService? interceptionOpportunityService = null) : IDevFleetUiStateService
{
    public async Task<GetDevFleetUiStateResult> GetAsync(
        GetDevFleetUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevFleetUiStateResult(request.CivilizationId, [], [], GetActionHints(), CreateInterceptionNotes());
        }

        var overview = await fleetOverviewService.GetAsync(
            new GetFleetOperationalOverviewRequest(request.CivilizationId),
            cancellationToken);
        var interceptionOpportunities = (await (interceptionOpportunityService ?? new InterceptionOpportunityService(
                dbContext,
                new MapVisibilityService(dbContext),
                new DetectionCoverageService(dbContext, new SensorProfileService(dbContext)),
                fleetOverviewService))
            .GetAsync(new GetInterceptionOpportunitiesRequest(request.CivilizationId), cancellationToken))
            .Opportunities
            .ToDictionary(x => x.TransferId, CreateInterceptionReadinessSummary);

        var planetIds = overview.Groups
            .Select(x => x.CurrentPlanetId)
            .Distinct()
            .ToArray();

        List<PlanetResourceStockpile> stockpiles = [];
        if (planetIds.Length > 0)
        {
            stockpiles = await dbContext.PlanetResourceStockpiles
                .AsNoTracking()
                .Where(x => planetIds.Contains(x.PlanetId))
                .OrderBy(x => x.PlanetId)
                .ToListAsync(cancellationToken);
        }

        var resourceContexts = stockpiles
            .Select(x => new DevFleetUiResourceContextDto(
                x.PlanetId,
                [
                    new DevFleetUiResourceBalanceDto(ResourceType.Credits, x.Credits),
                    new DevFleetUiResourceBalanceDto(ResourceType.Metal, x.Metal),
                    new DevFleetUiResourceBalanceDto(ResourceType.Crystal, x.Crystal),
                    new DevFleetUiResourceBalanceDto(ResourceType.Gas, x.Gas),
                ]))
            .ToArray();

        var groups = overview.Groups
            .Select(x => new DevFleetUiGroupDto(
                x.Id,
                x.CivilizationId,
                x.OriginPlanetId,
                x.CurrentPlanetId,
                x.AssetType,
                x.Quantity,
                x.Status,
                x.IsStationedAwayFromOrigin,
                x.HasActiveTransfer,
                x.ActiveTransfer is null
                    ? null
                    : new DevFleetUiTransferDto(
                        x.ActiveTransfer.Id,
                        x.ActiveTransfer.DestinationPlanetId,
                        x.ActiveTransfer.AbstractDistanceUnits,
                        x.ActiveTransfer.DepartureAtUtc,
                        x.ActiveTransfer.ArrivalAtUtc,
                        x.ActiveTransfer.Status,
                        interceptionOpportunities.GetValueOrDefault(x.ActiveTransfer.Id)),
                new DevFleetUiCommandAvailabilityDto(
                    x.Commands.CanCreateTransfer,
                    x.Commands.CanSplit,
                    x.Commands.CanMerge,
                    x.Commands.CanCancelTransfer),
                CreateRouteFuelReadinessHint(x.Commands.CanCreateTransfer)))
            .ToArray();

        return new GetDevFleetUiStateResult(
            overview.CivilizationId,
            groups,
            resourceContexts,
            GetActionHints(),
            CreateInterceptionNotes());
    }

    private IReadOnlyList<DevFleetUiActionHintDto> GetActionHints() => actionManifestService.Get().Actions
        .Select(x => new DevFleetUiActionHintDto(
            x.ActionKey,
            x.DisplayName,
            x.IsReadOnly,
            x.Method,
            x.Route,
            x.Notes))
        .ToArray();

    private static DevFleetUiRouteFuelReadinessHintDto CreateRouteFuelReadinessHint(bool canCreateTransfer) =>
        new(
            canCreateTransfer,
            true,
            "fleet.travel.estimate",
            "/api/dev/fleets/orbital-travel/estimate",
            OrbitalFuelReadinessPolicy.PlaceholderDerived,
            null,
            null,
            canCreateTransfer
                ? [
                    "Concrete route profile and fuel readiness require a destinationPlanetId.",
                    "Use fleet.travel.estimate to preview route class, risk, fuel readiness, travel costs, and affordability."
                ]
                : [
                    "Travel estimates are only available for stationed groups without an active transfer.",
                    "No destination-specific route profile or fuel readiness is included in UI state."
                ]);

    private static InterceptionReadinessSummaryDto CreateInterceptionReadinessSummary(InterceptionOpportunityDto opportunity) =>
        new(
            opportunity.OpportunityStatus,
            opportunity.BlockReasons,
            opportunity.HasFriendlyInterceptorContext,
            opportunity.DetectionNote,
            opportunity.ReadinessNote);

    private static IReadOnlyList<DevFleetUiInterceptionNoteDto> CreateInterceptionNotes() =>
        [
            new("Interception readiness is read-only metadata only; actual interception execution is not implemented.")
        ];
}

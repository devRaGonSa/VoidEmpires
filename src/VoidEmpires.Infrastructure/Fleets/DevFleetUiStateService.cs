using Microsoft.EntityFrameworkCore;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Infrastructure.Persistence;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class DevFleetUiStateService(
    VoidEmpiresDbContext dbContext,
    IFleetOperationalOverviewService fleetOverviewService,
    IDevFleetActionManifestService actionManifestService) : IDevFleetUiStateService
{
    public async Task<GetDevFleetUiStateResult> GetAsync(
        GetDevFleetUiStateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.CivilizationId == Guid.Empty)
        {
            return new GetDevFleetUiStateResult(request.CivilizationId, [], [], GetActionHints());
        }

        var overview = await fleetOverviewService.GetAsync(
            new GetFleetOperationalOverviewRequest(request.CivilizationId),
            cancellationToken);

        var planetIds = overview.Groups
            .Select(x => x.CurrentPlanetId)
            .Distinct()
            .ToArray();

        var stockpiles = planetIds.Length == 0
            ? []
            : await dbContext.PlanetResourceStockpiles
                .AsNoTracking()
                .Where(x => planetIds.Contains(x.PlanetId))
                .OrderBy(x => x.PlanetId)
                .ToListAsync(cancellationToken);

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
                        x.ActiveTransfer.Status),
                new DevFleetUiCommandAvailabilityDto(
                    x.Commands.CanCreateTransfer,
                    x.Commands.CanSplit,
                    x.Commands.CanMerge,
                    x.Commands.CanCancelTransfer)))
            .ToArray();

        return new GetDevFleetUiStateResult(
            overview.CivilizationId,
            groups,
            resourceContexts,
            GetActionHints());
    }

    private IReadOnlyList<DevFleetUiActionHintDto> GetActionHints() => actionManifestService.Get().Actions
        .Select(x => new DevFleetUiActionHintDto(
            x.ActionKey,
            x.DisplayName,
            x.IsReadOnly,
            x.Method,
            x.Route))
        .ToArray();
}

using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

internal static class DevFleetOperationalOverviewEndpoints
{
    public static void MapDevFleetOperationalOverviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/fleets/overview", async (
            Guid? civilizationId,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (civilizationId is null || civilizationId == Guid.Empty)
            {
                return Results.BadRequest(new FleetOperationalOverviewApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IFleetOperationalOverviewService>();
            var overview = await service.GetAsync(
                new GetFleetOperationalOverviewRequest(civilizationId.Value),
                cancellationToken);

            var groups = overview.Groups
                .Select(x => new FleetOperationalGroupApiResponse(
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
                        : new FleetOperationalTransferApiResponse(
                            x.ActiveTransfer.Id,
                            x.ActiveTransfer.DestinationPlanetId,
                            x.ActiveTransfer.AbstractDistanceUnits,
                            x.ActiveTransfer.DepartureAtUtc,
                            x.ActiveTransfer.ArrivalAtUtc,
                            x.ActiveTransfer.Status),
                    new FleetOperationalCommandAvailabilityApiResponse(
                        x.Commands.CanCreateTransfer,
                        x.Commands.CanSplit,
                        x.Commands.CanMerge,
                        x.Commands.CanCancelTransfer)))
                .ToArray();

            return Results.Ok(new FleetOperationalOverviewApiResponse(
                true,
                new FleetOperationalOverviewPayloadApiResponse(overview.CivilizationId, groups),
                []));
        });
    }
}

internal sealed record FleetOperationalOverviewApiResponse(
    bool Succeeded,
    FleetOperationalOverviewPayloadApiResponse? Overview,
    IReadOnlyList<string> Errors);

internal sealed record FleetOperationalOverviewPayloadApiResponse(
    Guid CivilizationId,
    IReadOnlyList<FleetOperationalGroupApiResponse> Groups);

internal sealed record FleetOperationalGroupApiResponse(
    Guid Id,
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    bool IsStationedAwayFromOrigin,
    bool HasActiveTransfer,
    FleetOperationalTransferApiResponse? ActiveTransfer,
    FleetOperationalCommandAvailabilityApiResponse Commands);

internal sealed record FleetOperationalTransferApiResponse(
    Guid Id,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status);

internal sealed record FleetOperationalCommandAvailabilityApiResponse(
    bool CanCreateTransfer,
    bool CanSplit,
    bool CanMerge,
    bool CanCancelTransfer);

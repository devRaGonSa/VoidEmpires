using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Fleets;

internal static class DevOrbitalTransferLookupEndpoints
{
    public static void MapDevOrbitalTransferLookupEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/fleets/orbital-transfers", async (
            Guid? civilizationId,
            Guid? orbitalGroupId,
            Guid? originPlanetId,
            Guid? destinationPlanetId,
            OrbitalTransferStatus? status,
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
                return Results.BadRequest(new OrbitalTransferListApiResponse(false, [], ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IOrbitalTransferLookupService>();
            var transfers = await service.ListAsync(new OrbitalTransferQueryRequest(
                civilizationId.Value,
                orbitalGroupId,
                originPlanetId,
                destinationPlanetId,
                status), cancellationToken);

            var responseTransfers = transfers
                .Select(x => new OrbitalTransferListItemApiResponse(
                    x.Id,
                    x.CivilizationId,
                    x.OrbitalGroupId,
                    x.OriginPlanetId,
                    x.DestinationPlanetId,
                    x.AbstractDistanceUnits,
                    x.DepartureAtUtc,
                    x.ArrivalAtUtc,
                    x.Status))
                .ToArray();

            return Results.Ok(new OrbitalTransferListApiResponse(true, responseTransfers, []));
        });
    }
}

internal sealed record OrbitalTransferListItemApiResponse(
    Guid Id,
    Guid CivilizationId,
    Guid OrbitalGroupId,
    Guid OriginPlanetId,
    Guid DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime DepartureAtUtc,
    DateTime ArrivalAtUtc,
    OrbitalTransferStatus Status);

internal sealed record OrbitalTransferListApiResponse(
    bool Succeeded,
    IReadOnlyList<OrbitalTransferListItemApiResponse> Transfers,
    IReadOnlyList<string> Errors);

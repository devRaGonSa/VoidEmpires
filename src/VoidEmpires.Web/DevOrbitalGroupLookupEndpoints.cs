using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Assets;
using VoidEmpires.Domain.Fleets;

internal static class DevOrbitalGroupLookupEndpoints
{
    public static void MapDevOrbitalGroupLookupEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/fleets/orbital-groups", async (
            Guid? civilizationId,
            Guid? currentPlanetId,
            Guid? originPlanetId,
            SpaceAssetType? assetType,
            OrbitalGroupStatus? status,
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
                return Results.BadRequest(new OrbitalGroupListApiResponse(false, [], ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IOrbitalGroupLookupService>();
            var groups = await service.ListAsync(new OrbitalGroupQueryRequest(
                civilizationId.Value,
                currentPlanetId,
                originPlanetId,
                assetType,
                status), cancellationToken);

            var responseGroups = groups
                .Select(x => new OrbitalGroupListItemApiResponse(
                    x.Id,
                    x.CivilizationId,
                    x.OriginPlanetId,
                    x.CurrentPlanetId,
                    x.AssetType,
                    x.Quantity,
                    x.Status,
                    x.IsStationedAwayFromOrigin))
                .ToArray();

            return Results.Ok(new OrbitalGroupListApiResponse(true, responseGroups, []));
        });
    }
}

internal sealed record OrbitalGroupListItemApiResponse(
    Guid Id,
    Guid CivilizationId,
    Guid OriginPlanetId,
    Guid CurrentPlanetId,
    SpaceAssetType AssetType,
    int Quantity,
    OrbitalGroupStatus Status,
    bool IsStationedAwayFromOrigin);

internal sealed record OrbitalGroupListApiResponse(
    bool Succeeded,
    IReadOnlyList<OrbitalGroupListItemApiResponse> Groups,
    IReadOnlyList<string> Errors);

using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevFleetUiStateEndpoints
{
    public static void MapDevFleetUiStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/fleets/ui-state", async (
            Guid? civilizationId,
            Guid? planetId,
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
                return Results.BadRequest(new DevFleetUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            if (planetId is not null)
            {
                await CompleteDueArrivalsAsync(services, cancellationToken);
            }

            var service = services.GetRequiredService<IDevFleetUiStateService>();
            var uiState = await service.GetAsync(
                new GetDevFleetUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            return Results.Ok(new DevFleetUiStateApiResponse(true, uiState, []));
        });

        app.MapPost("/api/dev/fleets/orbital-groups/create-from-local-stock", async (
            CreateLocalFleetApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (request.CivilizationId is null || request.CivilizationId == Guid.Empty ||
                request.PlanetId is null || request.PlanetId == Guid.Empty ||
                request.AssetType is null || request.Quantity is null || request.Quantity <= 0)
            {
                return Results.BadRequest(new CreateLocalFleetApiResponse(false, null, ["Civilization, planet, asset type and a positive quantity are required."]));
            }

            var service = services.GetRequiredService<IOrbitalGroupService>();
            var result = await service.CreateFromLocalStockAsync(new CreateOrbitalGroupRequest(
                request.CivilizationId.Value,
                request.PlanetId.Value,
                request.PlanetId.Value,
                request.AssetType.Value,
                request.Quantity.Value), cancellationToken);
            var response = new CreateLocalFleetApiResponse(result.Succeeded, result.OrbitalGroupId, result.Errors);

            return result.Succeeded
                ? Results.Created($"/api/dev/fleets/orbital-groups/{result.OrbitalGroupId}", response)
                : Results.Conflict(response);
        });
    }

    private static async Task CompleteDueArrivalsAsync(IServiceProvider services, CancellationToken cancellationToken)
    {
        try
        {
            await services.GetRequiredService<IOrbitalTransferCompletionService>()
                .CompleteDueAsync(DateTime.UtcNow, cancellationToken);
        }
        catch (Exception exception)
        {
            services.GetService<ILoggerFactory>()?
                .CreateLogger(nameof(DevFleetUiStateEndpoints))
                .LogWarning(exception, "Fleet arrival refresh failed before the fleet UI-state read.");
        }
    }
}

internal sealed record DevFleetUiStateApiResponse(
    bool Succeeded,
    GetDevFleetUiStateResult? UiState,
    IReadOnlyList<string> Errors);

internal sealed record CreateLocalFleetApiRequest(
    Guid? CivilizationId,
    Guid? PlanetId,
    VoidEmpires.Domain.Assets.SpaceAssetType? AssetType,
    int? Quantity);

internal sealed record CreateLocalFleetApiResponse(
    bool Succeeded,
    Guid? OrbitalGroupId,
    IReadOnlyList<string> Errors);

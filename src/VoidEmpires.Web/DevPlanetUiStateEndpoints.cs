using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Planets;

internal static class DevPlanetUiStateEndpoints
{
    public static void MapDevPlanetUiStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/planets/ui-state", async (
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
                return Results.BadRequest(new DevPlanetUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IDevPlanetUiStateService>();
            var uiState = await service.GetAsync(
                new GetDevPlanetUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            if (uiState.Planet is null && uiState.Errors.Count > 0)
            {
                return Results.NotFound(new DevPlanetUiStateApiResponse(false, null, uiState.Errors));
            }

            return Results.Ok(new DevPlanetUiStateApiResponse(true, uiState, []));
        });

        app.MapGet("/api/dev/defenses/ui-state", async (
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
                return Results.BadRequest(new DevDefenseUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IDevDefenseUiStateService>();
            var uiState = await service.GetAsync(
                new GetDevDefenseUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            if (uiState.Defenses is null && uiState.Errors.Count > 0)
            {
                return Results.NotFound(new DevDefenseUiStateApiResponse(false, null, uiState.Errors));
            }

            return Results.Ok(new DevDefenseUiStateApiResponse(true, uiState, []));
        });
    }
}

internal sealed record DevPlanetUiStateApiResponse(
    bool Succeeded,
    GetDevPlanetUiStateResult? UiState,
    IReadOnlyList<string> Errors);

internal sealed record DevDefenseUiStateApiResponse(
    bool Succeeded,
    GetDevDefenseUiStateResult? UiState,
    IReadOnlyList<string> Errors);

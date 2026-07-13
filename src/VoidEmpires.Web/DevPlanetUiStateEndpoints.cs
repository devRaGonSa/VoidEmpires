using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Gameplay;
using VoidEmpires.Application.Planets;
using VoidEmpires.Infrastructure.Persistence;
using VoidEmpires.Infrastructure.Planets;

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

            await RefreshGameplayAsync(services, civilizationId.Value, planetId, cancellationToken);

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

            await RefreshGameplayAsync(services, civilizationId.Value, planetId, cancellationToken);

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

        app.MapGet("/api/dev/ground-army/ui-state", async (
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
                return Results.BadRequest(new DevGroundArmyUiStateApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = new DevGroundArmyUiStateService(
                services.GetRequiredService<IDevPlanetUiStateService>(),
                services.GetRequiredService<VoidEmpiresDbContext>());
            var uiState = await service.GetAsync(
                new GetDevGroundArmyUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            if (uiState.GroundArmy is null && uiState.Errors.Count > 0)
            {
                return Results.NotFound(new DevGroundArmyUiStateApiResponse(false, null, uiState.Errors));
            }

            return Results.Ok(new DevGroundArmyUiStateApiResponse(true, uiState, []));
        });
    }

    private static Task RefreshGameplayAsync(
        IServiceProvider services,
        Guid civilizationId,
        Guid? planetId,
        CancellationToken cancellationToken)
    {
        return RefreshGameplayCoreAsync();

        async Task RefreshGameplayCoreAsync()
        {
            try
            {
                var service = services.GetRequiredService<IGameplayRefreshService>();
                var result = await service.RefreshAsync(new GameplayRefreshRequest(
                    civilizationId,
                    planetId,
                    DateTime.UtcNow,
                    IncludeResources: planetId is not null,
                    IncludeConstruction: true,
                    IncludeResearch: true,
                    IncludeProduction: true), cancellationToken);

                if (!result.Succeeded)
                {
                    services.GetService<ILoggerFactory>()?
                        .CreateLogger(nameof(DevPlanetUiStateEndpoints))
                        .LogWarning("Gameplay refresh did not succeed before the planet UI-state read: {Errors}", string.Join("; ", result.Errors));
                }
            }
            catch (Exception exception)
            {
                services.GetService<ILoggerFactory>()?
                    .CreateLogger(nameof(DevPlanetUiStateEndpoints))
                    .LogWarning(exception, "Gameplay refresh failed before the planet UI-state read.");
            }
        }
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

internal sealed record DevGroundArmyUiStateApiResponse(
    bool Succeeded,
    GetDevGroundArmyUiStateResult? UiState,
    IReadOnlyList<string> Errors);

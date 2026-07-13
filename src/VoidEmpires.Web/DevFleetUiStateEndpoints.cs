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

            var service = services.GetRequiredService<IDevFleetUiStateService>();
            var uiState = await service.GetAsync(
                new GetDevFleetUiStateRequest(civilizationId.Value, planetId),
                cancellationToken);

            return Results.Ok(new DevFleetUiStateApiResponse(true, uiState, []));
        });
    }
}

internal sealed record DevFleetUiStateApiResponse(
    bool Succeeded,
    GetDevFleetUiStateResult? UiState,
    IReadOnlyList<string> Errors);

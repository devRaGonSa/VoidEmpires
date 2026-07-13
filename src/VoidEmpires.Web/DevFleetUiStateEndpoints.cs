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

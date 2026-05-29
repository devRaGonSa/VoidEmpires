using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Visuals;

internal static class DevPlanetVisualStateEndpoints
{
    public static void MapDevPlanetVisualStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/planets/{planetId:guid}/visual-state", async (
            Guid planetId,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (planetId == Guid.Empty)
            {
                return Results.BadRequest(new PlanetVisualStateApiResponse(false, null, ["Planet id is required."]));
            }

            var service = services.GetRequiredService<IPlanetVisualStateService>();
            var result = await service.GetAsync(new GetPlanetVisualStateRequest(planetId), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new PlanetVisualStateApiResponse(true, result.VisualState, []))
                : Results.NotFound(new PlanetVisualStateApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record PlanetVisualStateApiResponse(
    bool Succeeded,
    PlanetVisualStateDto? VisualState,
    IReadOnlyList<string> Errors);

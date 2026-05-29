using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Visuals;

internal static class DevSystemVisualStateEndpoints
{
    public static void MapDevSystemVisualStateEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/solar-systems/{systemId:guid}/visual-state", async (
            Guid systemId,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (systemId == Guid.Empty)
            {
                return Results.BadRequest(new SystemVisualStateApiResponse(false, null, ["System id is required."]));
            }

            var service = services.GetRequiredService<ISystemVisualStateService>();
            var result = await service.GetAsync(new GetSystemVisualStateRequest(systemId), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new SystemVisualStateApiResponse(true, result.VisualState, []))
                : Results.NotFound(new SystemVisualStateApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record SystemVisualStateApiResponse(
    bool Succeeded,
    SystemVisualStateDto? VisualState,
    IReadOnlyList<string> Errors);

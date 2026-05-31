using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevExplorationActionPreviewEndpoints
{
    public static void MapDevExplorationActionPreviewEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/exploration-preview", async (
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
                return Results.BadRequest(new ExplorationActionPreviewApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IExplorationActionPreviewService>();
            var preview = await service.GetAsync(new GetExplorationActionPreviewRequest(civilizationId.Value), cancellationToken);

            return Results.Ok(new ExplorationActionPreviewApiResponse(true, preview, []));
        });
    }
}

internal sealed record ExplorationActionPreviewApiResponse(
    bool Succeeded,
    GetExplorationActionPreviewResult? Preview,
    IReadOnlyList<string> Errors);

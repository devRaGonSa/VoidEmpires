using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevExplorationKnowledgeEndpoints
{
    public static void MapDevExplorationKnowledgeEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/exploration-knowledge", async (
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
                return Results.BadRequest(new ExplorationKnowledgeApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IExplorationKnowledgeQueryService>();
            var result = await service.GetAsync(new GetExplorationKnowledgeRequest(civilizationId.Value), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new ExplorationKnowledgeApiResponse(true, result, []))
                : Results.BadRequest(new ExplorationKnowledgeApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record ExplorationKnowledgeApiResponse(
    bool Succeeded,
    GetExplorationKnowledgeResult? Knowledge,
    IReadOnlyList<string> Errors);

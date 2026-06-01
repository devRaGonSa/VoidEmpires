using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevDetectionCoverageEndpoints
{
    public static void MapDevDetectionCoverageEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/detection-coverage", async (
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
                return Results.BadRequest(new DetectionCoverageApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IDetectionCoverageService>();
            var result = await service.GetAsync(new GetDetectionCoverageRequest(civilizationId.Value), cancellationToken);

            return Results.Ok(new DetectionCoverageApiResponse(true, result, []));
        });
    }
}

internal sealed record DetectionCoverageApiResponse(
    bool Succeeded,
    GetDetectionCoverageResult? DetectionCoverage,
    IReadOnlyList<string> Errors);

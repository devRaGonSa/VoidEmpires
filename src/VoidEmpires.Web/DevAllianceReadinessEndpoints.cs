using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevAllianceReadinessEndpoints
{
    public static void MapDevAllianceReadinessEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/alliances/readiness", async (
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
                return Results.BadRequest(new AllianceReadinessApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IAllianceReadinessQueryService>();
            var result = await service.GetAsync(new GetAllianceReadinessRequest(civilizationId.Value), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new AllianceReadinessApiResponse(true, result, []))
                : Results.BadRequest(new AllianceReadinessApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record AllianceReadinessApiResponse(
    bool Succeeded,
    GetAllianceReadinessResult? AllianceReadiness,
    IReadOnlyList<string> Errors);

using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevAlliancePactReadinessEndpoints
{
    public static void MapDevAlliancePactReadinessEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/alliances/pacts/readiness", async (
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
                return Results.BadRequest(new AlliancePactReadinessApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IAlliancePactReadinessQueryService>();
            var result = await service.GetAsync(new GetAlliancePactReadinessRequest(civilizationId.Value), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new AlliancePactReadinessApiResponse(true, result, []))
                : Results.BadRequest(new AlliancePactReadinessApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record AlliancePactReadinessApiResponse(
    bool Succeeded,
    GetAlliancePactReadinessResult? AlliancePactReadiness,
    IReadOnlyList<string> Errors);

using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevStrategicMapEndpoints
{
    public static void MapDevStrategicMapEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map", async (
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
                return Results.BadRequest(new StrategicMapApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IStrategicMapService>();
            var map = await service.GetAsync(new GetStrategicMapRequest(civilizationId.Value), cancellationToken);

            return Results.Ok(new StrategicMapApiResponse(true, map, []));
        });
    }
}

internal sealed record StrategicMapApiResponse(
    bool Succeeded,
    GetStrategicMapResult? Map,
    IReadOnlyList<string> Errors);

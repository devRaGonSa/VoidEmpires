using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevSensorProfileEndpoints
{
    public static void MapDevSensorProfileEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/sensor-profiles", async (
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
                return Results.BadRequest(new SensorProfilesApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<ISensorProfileService>();
            var result = await service.GetAsync(new GetSensorProfilesRequest(civilizationId.Value), cancellationToken);

            return Results.Ok(new SensorProfilesApiResponse(true, result, []));
        });
    }
}

internal sealed record SensorProfilesApiResponse(
    bool Succeeded,
    GetSensorProfilesResult? SensorProfiles,
    IReadOnlyList<string> Errors);

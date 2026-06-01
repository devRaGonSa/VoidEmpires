using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevDiplomaticContactEndpoints
{
    public static void MapDevDiplomaticContactEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/diplomatic-contacts", async (
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
                return Results.BadRequest(new DiplomaticContactsApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IDiplomaticContactQueryService>();
            var result = await service.GetAsync(new GetDiplomaticContactsRequest(civilizationId.Value), cancellationToken);

            return result.Succeeded
                ? Results.Ok(new DiplomaticContactsApiResponse(true, result, []))
                : Results.BadRequest(new DiplomaticContactsApiResponse(false, null, result.Errors));
        });
    }
}

internal sealed record DiplomaticContactsApiResponse(
    bool Succeeded,
    GetDiplomaticContactsResult? DiplomaticContacts,
    IReadOnlyList<string> Errors);

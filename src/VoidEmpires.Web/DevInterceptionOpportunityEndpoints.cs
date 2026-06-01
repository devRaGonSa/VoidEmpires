using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevInterceptionOpportunityEndpoints
{
    public static void MapDevInterceptionOpportunityEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/interception-opportunities", async (
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
                return Results.BadRequest(new InterceptionOpportunityApiResponse(false, null, ["Civilization id is required."]));
            }

            var service = services.GetRequiredService<IInterceptionOpportunityService>();
            var result = await service.GetAsync(new GetInterceptionOpportunitiesRequest(civilizationId.Value), cancellationToken);

            return Results.Ok(new InterceptionOpportunityApiResponse(true, result, []));
        });
    }
}

internal sealed record InterceptionOpportunityApiResponse(
    bool Succeeded,
    GetInterceptionOpportunitiesResult? InterceptionOpportunities,
    IReadOnlyList<string> Errors);

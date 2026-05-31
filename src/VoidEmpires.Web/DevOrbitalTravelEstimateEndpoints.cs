using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;

internal static class DevOrbitalTravelEstimateEndpoints
{
    public static void MapDevOrbitalTravelEstimateEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-travel/estimate", async (
            EstimateOrbitalTravelApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateEstimateRequest(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(EstimateOrbitalTravelApiResponse.Failure(errors));
            }

            var service = services.GetRequiredService<IOrbitalTravelEstimateService>();
            var result = await service.EstimateAsync(new EstimateOrbitalTravelRequest(
                request.CivilizationId!.Value,
                request.OrbitalGroupId!.Value,
                request.DestinationPlanetId!.Value), cancellationToken);

            var costs = result.ResourceCosts
                .Select(x => new OrbitalTravelCostComponentApiResponse(x.ResourceType, x.Quantity))
                .ToArray();

            var response = new EstimateOrbitalTravelApiResponse(
                result.Succeeded,
                result.OrbitalGroupId,
                result.CurrentPlanetId,
                result.DestinationPlanetId,
                result.AbstractDistanceUnits,
                result.EstimatedDuration,
                costs,
                result.Errors);

            return result.Succeeded ? Results.Ok(response) : Results.Conflict(response);
        });
    }

    private static IReadOnlyList<string> ValidateEstimateRequest(EstimateOrbitalTravelApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.OrbitalGroupId is null || request.OrbitalGroupId == Guid.Empty)
        {
            errors.Add("Orbital group id is required.");
        }

        if (request.DestinationPlanetId is null || request.DestinationPlanetId == Guid.Empty)
        {
            errors.Add("Destination planet id is required.");
        }

        return errors;
    }
}

internal sealed record EstimateOrbitalTravelApiRequest(
    Guid? CivilizationId,
    Guid? OrbitalGroupId,
    Guid? DestinationPlanetId);

internal sealed record EstimateOrbitalTravelApiResponse(
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    IReadOnlyList<OrbitalTravelCostComponentApiResponse> ResourceCosts,
    IReadOnlyList<string> Errors)
{
    public static EstimateOrbitalTravelApiResponse Failure(IReadOnlyList<string> errors) =>
        new(false, null, null, null, 0, null, [], errors);
}

internal sealed record OrbitalTravelCostComponentApiResponse(ResourceType ResourceType, decimal Quantity);

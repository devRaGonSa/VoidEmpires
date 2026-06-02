using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;
using VoidEmpires.Domain.Economy;
using VoidEmpires.Domain.Fleets;

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
            var insufficientResources = result.InsufficientResources
                .Select(x => new OrbitalTravelInsufficientResourceApiResponse(
                    x.ResourceType,
                    x.RequiredQuantity,
                    x.AvailableQuantity))
                .ToArray();

            var response = new EstimateOrbitalTravelApiResponse(
                result.Status,
                result.Succeeded,
                result.OrbitalGroupId,
                result.CurrentPlanetId,
                result.DestinationPlanetId,
                result.AbstractDistanceUnits,
                result.EstimatedDuration,
                result.RouteProfile is null
                    ? null
                    : new OrbitalRouteProfileApiResponse(
                        result.RouteProfile.RouteClass,
                        result.RouteProfile.DistanceBand,
                        result.RouteProfile.RiskBand,
                        result.RouteProfile.FuelMultiplier,
                        result.RouteProfile.ComplexityNotes,
                        result.RouteProfile.IsSupported),
                result.FuelReadiness is null
                    ? null
                    : new OrbitalFuelReadinessApiResponse(
                        result.FuelReadiness.EstimatedFuelUnitsRequired,
                        result.FuelReadiness.EstimatedRangeUnitsAvailable,
                        result.FuelReadiness.IsFuelReady,
                        result.FuelReadiness.NotReadyReason,
                        result.FuelReadiness.Policy),
                costs,
                result.CanAfford,
                insufficientResources,
                result.Errors);

            return result.Status switch
            {
                EstimateOrbitalTravelResultStatus.Succeeded => Results.Ok(response),
                EstimateOrbitalTravelResultStatus.ValidationFailed => Results.BadRequest(response),
                EstimateOrbitalTravelResultStatus.NotFound => Results.NotFound(response),
                _ => Results.Conflict(response)
            };
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
    EstimateOrbitalTravelResultStatus Status,
    bool Succeeded,
    Guid? OrbitalGroupId,
    Guid? CurrentPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    TimeSpan? EstimatedDuration,
    OrbitalRouteProfileApiResponse? RouteProfile,
    OrbitalFuelReadinessApiResponse? FuelReadiness,
    IReadOnlyList<OrbitalTravelCostComponentApiResponse> ResourceCosts,
    bool CanAfford,
    IReadOnlyList<OrbitalTravelInsufficientResourceApiResponse> InsufficientResources,
    IReadOnlyList<string> Errors)
{
    public static EstimateOrbitalTravelApiResponse Failure(IReadOnlyList<string> errors) =>
        new(EstimateOrbitalTravelResultStatus.ValidationFailed, false, null, null, null, 0, null, null, null, [], false, [], errors);
}

internal sealed record OrbitalFuelReadinessApiResponse(
    decimal EstimatedFuelUnitsRequired,
    int EstimatedRangeUnitsAvailable,
    bool IsFuelReady,
    string? NotReadyReason,
    OrbitalFuelReadinessPolicy Policy);

internal sealed record OrbitalRouteProfileApiResponse(
    OrbitalRouteClass RouteClass,
    int DistanceBand,
    OrbitalRouteRiskBand RiskBand,
    decimal FuelMultiplier,
    IReadOnlyList<string> ComplexityNotes,
    bool IsSupported);

internal sealed record OrbitalTravelCostComponentApiResponse(ResourceType ResourceType, decimal Quantity);

internal sealed record OrbitalTravelInsufficientResourceApiResponse(
    ResourceType ResourceType,
    decimal RequiredQuantity,
    decimal AvailableQuantity);

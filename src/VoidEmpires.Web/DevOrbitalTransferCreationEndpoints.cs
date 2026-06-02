using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevOrbitalTransferCreationEndpoints
{
    public static void MapDevOrbitalTransferCreationEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-transfers/create", async (
            CreateOrbitalTransferApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCreateOrbitalTransfer(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(CreateOrbitalTransferApiResponse.Failure(errors));
            }

            var service = services.GetRequiredService<IOrbitalTransferPersistenceService>();
            var result = await service.PersistAsync(new PersistOrbitalTransferRequest(
                request.CivilizationId!.Value,
                request.OrbitalGroupId!.Value,
                request.DestinationPlanetId!.Value,
                request.RequestedAtUtc!.Value), cancellationToken);

            var response = new CreateOrbitalTransferApiResponse(
                result.Status,
                result.Succeeded,
                result.OrbitalTransferId,
                result.OrbitalGroupId,
                result.OriginPlanetId,
                result.DestinationPlanetId,
                result.AbstractDistanceUnits,
                result.DepartureAtUtc,
                result.ArrivalAtUtc,
                result.Errors);

            return result.Status switch
            {
                PersistOrbitalTransferResultStatus.Succeeded => Results.Created($"/api/dev/fleets/orbital-transfers/{result.OrbitalTransferId}", response),
                PersistOrbitalTransferResultStatus.ValidationFailed => Results.BadRequest(response),
                PersistOrbitalTransferResultStatus.NotFound => Results.NotFound(response),
                _ => Results.Conflict(response)
            };
        });
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateCreateOrbitalTransfer(CreateOrbitalTransferApiRequest request)
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

        if (request.RequestedAtUtc is null)
        {
            errors.Add("Requested date is required.");
        }
        else if (request.RequestedAtUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Requested date must be UTC.");
        }

        return errors;
    }
}

internal sealed record CreateOrbitalTransferApiRequest(
    Guid? CivilizationId,
    Guid? OrbitalGroupId,
    Guid? DestinationPlanetId,
    DateTime? RequestedAtUtc);

internal sealed record CreateOrbitalTransferApiResponse(
    PersistOrbitalTransferResultStatus Status,
    bool Succeeded,
    Guid? OrbitalTransferId,
    Guid? OrbitalGroupId,
    Guid? OriginPlanetId,
    Guid? DestinationPlanetId,
    int AbstractDistanceUnits,
    DateTime? DepartureAtUtc,
    DateTime? ArrivalAtUtc,
    IReadOnlyList<string> Errors)
{
    public static CreateOrbitalTransferApiResponse Failure(IReadOnlyList<string> errors) =>
        new(PersistOrbitalTransferResultStatus.ValidationFailed, false, null, null, null, null, 0, null, null, errors);
}

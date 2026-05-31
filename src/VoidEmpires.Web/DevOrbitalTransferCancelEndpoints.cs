using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevOrbitalTransferCancelEndpoints
{
    public static void MapDevOrbitalTransferCancelEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-transfers/cancel", async (
            CancelOrbitalTransferApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCancelOrbitalTransfer(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(CancelOrbitalTransferApiResponse.Failure(errors));
            }

            var service = services.GetRequiredService<IOrbitalTransferCancelService>();
            var result = await service.CancelAsync(new CancelOrbitalTransferRequest(
                request.CivilizationId!.Value,
                request.OrbitalTransferId!.Value), cancellationToken);

            var response = new CancelOrbitalTransferApiResponse(
                result.Succeeded,
                result.OrbitalTransferId,
                result.OrbitalGroupId,
                result.Errors);

            return result.Status switch
            {
                CancelOrbitalTransferResultStatus.Succeeded => Results.Ok(response),
                CancelOrbitalTransferResultStatus.NotFound => Results.NotFound(response),
                CancelOrbitalTransferResultStatus.ValidationFailed => Results.BadRequest(response),
                _ => Results.Conflict(response)
            };
        });
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateCancelOrbitalTransfer(CancelOrbitalTransferApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.OrbitalTransferId is null || request.OrbitalTransferId == Guid.Empty)
        {
            errors.Add("Orbital transfer id is required.");
        }

        return errors;
    }
}

internal sealed record CancelOrbitalTransferApiRequest(
    Guid? CivilizationId,
    Guid? OrbitalTransferId);

internal sealed record CancelOrbitalTransferApiResponse(
    bool Succeeded,
    Guid? OrbitalTransferId,
    Guid? OrbitalGroupId,
    IReadOnlyList<string> Errors)
{
    public static CancelOrbitalTransferApiResponse Failure(IReadOnlyList<string> errors) =>
        new(false, null, null, errors);
}

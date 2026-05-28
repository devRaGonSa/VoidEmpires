using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevOrbitalTransferCompletionEndpoints
{
    public static void MapDevOrbitalTransferCompletionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-transfers/complete-due", async (
            CompleteOrbitalTransfersApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateCompleteOrbitalTransfers(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(new CompleteOrbitalTransfersApiResponse(false, 0, [], [], errors));
            }

            var service = services.GetRequiredService<IOrbitalTransferCompletionService>();
            var result = await service.CompleteDueAsync(request.NowUtc!.Value, cancellationToken);

            return Results.Ok(new CompleteOrbitalTransfersApiResponse(
                true,
                result.CompletedCount,
                result.CompletedTransferIds,
                result.CompletedOrbitalGroupIds,
                []));
        });
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateCompleteOrbitalTransfers(CompleteOrbitalTransfersApiRequest request)
    {
        var errors = new List<string>();

        if (request.NowUtc is null)
        {
            errors.Add("Current date is required.");
        }
        else if (request.NowUtc.Value.Kind != DateTimeKind.Utc)
        {
            errors.Add("Current date must be UTC.");
        }

        return errors;
    }
}

internal sealed record CompleteOrbitalTransfersApiRequest(DateTime? NowUtc);

internal sealed record CompleteOrbitalTransfersApiResponse(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedTransferIds,
    IReadOnlyList<Guid> CompletedOrbitalGroupIds,
    IReadOnlyList<string> Errors);

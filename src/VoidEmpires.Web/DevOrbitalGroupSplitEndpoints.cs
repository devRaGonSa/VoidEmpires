using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevOrbitalGroupSplitEndpoints
{
    public static void MapDevOrbitalGroupSplitEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-groups/split", async (
            SplitOrbitalGroupApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateSplitOrbitalGroup(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(SplitOrbitalGroupApiResponse.Failure(errors));
            }

            var service = services.GetRequiredService<IOrbitalGroupSplitService>();
            var result = await service.SplitAsync(new SplitOrbitalGroupRequest(
                request.CivilizationId!.Value,
                request.SourceOrbitalGroupId!.Value,
                request.Quantity!.Value), cancellationToken);

            var response = new SplitOrbitalGroupApiResponse(
                GetStatus(result),
                result.Succeeded,
                result.SourceOrbitalGroupId,
                result.NewOrbitalGroupId,
                result.SourceQuantity,
                result.NewQuantity,
                result.Errors);

            return response.Status switch
            {
                SplitOrbitalGroupApiResultStatus.Succeeded => Results.Created($"/api/dev/fleets/orbital-groups/{result.NewOrbitalGroupId}", response),
                SplitOrbitalGroupApiResultStatus.ValidationFailed => Results.BadRequest(response),
                SplitOrbitalGroupApiResultStatus.NotFound => Results.NotFound(response),
                _ => Results.Conflict(response)
            };
        });
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateSplitOrbitalGroup(SplitOrbitalGroupApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.SourceOrbitalGroupId is null || request.SourceOrbitalGroupId == Guid.Empty)
        {
            errors.Add("Source orbital group id is required.");
        }

        if (request.Quantity is null || request.Quantity <= 0)
        {
            errors.Add("Quantity must be positive.");
        }

        return errors;
    }

    private static SplitOrbitalGroupApiResultStatus GetStatus(SplitOrbitalGroupResult result)
    {
        if (result.Succeeded)
        {
            return SplitOrbitalGroupApiResultStatus.Succeeded;
        }

        return result.Errors.Any(error => error.Contains("was not found", StringComparison.Ordinal))
            ? SplitOrbitalGroupApiResultStatus.NotFound
            : SplitOrbitalGroupApiResultStatus.Conflict;
    }
}

internal enum SplitOrbitalGroupApiResultStatus
{
    Succeeded = 0,
    ValidationFailed = 1,
    NotFound = 2,
    Conflict = 3
}

internal sealed record SplitOrbitalGroupApiRequest(
    Guid? CivilizationId,
    Guid? SourceOrbitalGroupId,
    int? Quantity);

internal sealed record SplitOrbitalGroupApiResponse(
    SplitOrbitalGroupApiResultStatus Status,
    bool Succeeded,
    Guid? SourceOrbitalGroupId,
    Guid? NewOrbitalGroupId,
    int SourceQuantity,
    int NewQuantity,
    IReadOnlyList<string> Errors)
{
    public static SplitOrbitalGroupApiResponse Failure(IReadOnlyList<string> errors) =>
        new(SplitOrbitalGroupApiResultStatus.ValidationFailed, false, null, null, 0, 0, errors);
}

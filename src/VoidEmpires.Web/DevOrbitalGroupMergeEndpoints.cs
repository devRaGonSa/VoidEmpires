using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.Fleets;

internal static class DevOrbitalGroupMergeEndpoints
{
    public static void MapDevOrbitalGroupMergeEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/fleets/orbital-groups/merge", async (
            MergeOrbitalGroupsApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (!IsPersistenceConfigured(configuration))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var errors = ValidateMergeOrbitalGroups(request);
            if (errors.Count > 0)
            {
                return Results.BadRequest(MergeOrbitalGroupsApiResponse.Failure(errors));
            }

            var service = services.GetRequiredService<IOrbitalGroupMergeService>();
            var result = await service.MergeAsync(new MergeOrbitalGroupsRequest(
                request.CivilizationId!.Value,
                request.TargetOrbitalGroupId!.Value,
                request.SourceOrbitalGroupId!.Value), cancellationToken);

            var response = new MergeOrbitalGroupsApiResponse(
                result.Succeeded,
                result.TargetOrbitalGroupId,
                result.SourceOrbitalGroupId,
                result.TargetQuantity,
                result.Errors);

            return result.Succeeded
                ? Results.Ok(response)
                : Results.Conflict(response);
        });
    }

    private static bool IsPersistenceConfigured(IConfiguration configuration) =>
        !string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection"));

    private static IReadOnlyList<string> ValidateMergeOrbitalGroups(MergeOrbitalGroupsApiRequest request)
    {
        var errors = new List<string>();

        if (request.CivilizationId is null || request.CivilizationId == Guid.Empty)
        {
            errors.Add("Civilization id is required.");
        }

        if (request.TargetOrbitalGroupId is null || request.TargetOrbitalGroupId == Guid.Empty)
        {
            errors.Add("Target orbital group id is required.");
        }

        if (request.SourceOrbitalGroupId is null || request.SourceOrbitalGroupId == Guid.Empty)
        {
            errors.Add("Source orbital group id is required.");
        }

        if (request.TargetOrbitalGroupId == request.SourceOrbitalGroupId)
        {
            errors.Add("Target and source orbital groups must be different.");
        }

        return errors;
    }
}

internal sealed record MergeOrbitalGroupsApiRequest(
    Guid? CivilizationId,
    Guid? TargetOrbitalGroupId,
    Guid? SourceOrbitalGroupId);

internal sealed record MergeOrbitalGroupsApiResponse(
    bool Succeeded,
    Guid? TargetOrbitalGroupId,
    Guid? SourceOrbitalGroupId,
    int TargetQuantity,
    IReadOnlyList<string> Errors)
{
    public static MergeOrbitalGroupsApiResponse Failure(IReadOnlyList<string> errors) =>
        new(false, null, null, 0, errors);
}

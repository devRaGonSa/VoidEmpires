using Microsoft.AspNetCore.Mvc;
using VoidEmpires.Application.StrategicMap;

internal static class DevExplorationMissionEndpoints
{
    public static void MapDevExplorationMissionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/dev/strategic-map/exploration-missions/create", async (
            CreateExplorationMissionApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (request.CivilizationId is null ||
                request.TargetSystemId is null ||
                request.RequestedAtUtc is null)
            {
                return Results.BadRequest(new CreateExplorationMissionApiResponse(
                    false,
                    null,
                    ["Civilization id, target system id, and requested date are required."]));
            }

            var service = services.GetRequiredService<IExplorationMissionCreateService>();
            var result = await service.CreateAsync(new CreateExplorationMissionRequest(
                request.CivilizationId.Value,
                request.TargetSystemId.Value,
                request.TargetPlanetId,
                request.RequestedAtUtc.Value), cancellationToken);

            if (result.Succeeded)
            {
                return Results.Created(
                    $"/api/dev/strategic-map/exploration-missions/{result.Mission!.ExplorationMissionId}",
                    new CreateExplorationMissionApiResponse(true, result.Mission, []));
            }

            var response = new CreateExplorationMissionApiResponse(false, null, result.Errors);
            return result.IsConflict ? Results.Conflict(response) : Results.BadRequest(response);
        });

        app.MapPost("/api/dev/strategic-map/exploration-missions/complete-due", async (
            CompleteDueExplorationMissionsApiRequest request,
            [FromServices] IServiceProvider services,
            [FromServices] IConfiguration configuration,
            CancellationToken cancellationToken) =>
        {
            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("DefaultConnection")))
            {
                return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            if (request.NowUtc is null)
            {
                return Results.BadRequest(new CompleteDueExplorationMissionsApiResponse(false, 0, [], ["Now is required."]));
            }

            var service = services.GetRequiredService<IExplorationMissionCompletionService>();
            var result = await service.CompleteDueAsync(
                new CompleteDueExplorationMissionsRequest(request.NowUtc.Value),
                cancellationToken);

            return result.Succeeded
                ? Results.Ok(new CompleteDueExplorationMissionsApiResponse(true, result.CompletedCount, result.CompletedMissionIds, []))
                : Results.BadRequest(new CompleteDueExplorationMissionsApiResponse(false, 0, [], result.Errors));
        });
    }
}

internal sealed record CreateExplorationMissionApiRequest(
    Guid? CivilizationId,
    Guid? TargetSystemId,
    Guid? TargetPlanetId,
    DateTime? RequestedAtUtc);

internal sealed record CreateExplorationMissionApiResponse(
    bool Succeeded,
    CreatedExplorationMissionDto? Mission,
    IReadOnlyList<string> Errors);

internal sealed record CompleteDueExplorationMissionsApiRequest(DateTime? NowUtc);

internal sealed record CompleteDueExplorationMissionsApiResponse(
    bool Succeeded,
    int CompletedCount,
    IReadOnlyList<Guid> CompletedMissionIds,
    IReadOnlyList<string> Errors);

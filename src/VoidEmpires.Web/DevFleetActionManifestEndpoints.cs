using VoidEmpires.Infrastructure.Fleets;

internal static class DevFleetActionManifestEndpoints
{
    public static void MapDevFleetActionManifestEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/fleets/action-manifest", () =>
        {
            var manifest = new DevFleetActionManifestService().Get();
            return Results.Ok(new DevFleetActionManifestApiResponse(true, manifest, []));
        });
    }
}

internal sealed record DevFleetActionManifestApiResponse(
    bool Succeeded,
    object Manifest,
    IReadOnlyList<string> Errors);

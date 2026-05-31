using VoidEmpires.Infrastructure.StrategicMap;

internal static class DevStrategicMapActionManifestEndpoints
{
    public static void MapDevStrategicMapActionManifestEndpoints(this WebApplication app)
    {
        app.MapGet("/api/dev/strategic-map/action-manifest", () =>
        {
            var manifest = new DevStrategicMapActionManifestService().Get();
            return Results.Ok(new DevStrategicMapActionManifestApiResponse(true, manifest, []));
        });
    }
}

internal sealed record DevStrategicMapActionManifestApiResponse(
    bool Succeeded,
    object Manifest,
    IReadOnlyList<string> Errors);

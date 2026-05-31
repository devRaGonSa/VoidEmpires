using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DevStrategicMapActionManifestService : IDevStrategicMapActionManifestService
{
    public GetDevStrategicMapActionManifestResult Get() => new(GetActions());

    private static IReadOnlyList<DevStrategicMapActionManifestItem> GetActions() =>
    [
        Read("strategicMap.read", "Read strategic map", "GET", "/api/dev/strategic-map", [Field("civilizationId", "Guid")], "Returns civilization-scoped relevant systems, planet summaries, fleet presence, transfer overlays, command availability metadata, exploration preview metadata, and route/fuel capability notes."),
        Read("strategicMap.explorationPreview.read", "Read exploration preview", "GET", "/api/dev/strategic-map/exploration-preview", [Field("civilizationId", "Guid")], "Returns read-only exploration readiness metadata derived from map visibility. It does not create missions, sensors, persisted fog-of-war, espionage, diplomacy, combat, or route graph state."),
        Read("visual.system.read", "Read system visual state", "GET", "/api/dev/solar-systems/{systemId}/visual-state", [Field("systemId", "Guid")], "Returns renderer-facing star, coordinate, layout, planet visual state, marker, and transfer overlay data for one solar system."),
        Read("visual.planet.read", "Read planet visual state", "GET", "/api/dev/planets/{planetId}/visual-state", [Field("planetId", "Guid")], "Returns renderer-facing visual state for one planet without meshes, textures, binary assets, or shader data."),
        Read("fleet.uiState.read", "Read fleet UI state", "GET", "/api/dev/fleets/ui-state", [Field("civilizationId", "Guid")], "Returns fleet screen state and route/fuel readiness capability hints related to map tooling."),
        Read("fleet.actionManifest.read", "Read fleet action manifest", "GET", "/api/dev/fleets/action-manifest", [], "Returns deterministic metadata for current fleet development actions and contracts."),
        Read("strategicMap.actionManifest.read", "Read strategic map action manifest", "GET", "/api/dev/strategic-map/action-manifest", [], "Returns this deterministic strategic map metadata surface for future UI prototypes.")
    ];

    private static DevStrategicMapActionManifestItem Read(
        string key,
        string displayName,
        string method,
        string route,
        IReadOnlyList<DevStrategicMapActionFieldDto> fields,
        string notes) => new(key, displayName, method, route, true, fields, 200, [400, 404, 503], notes);

    private static DevStrategicMapActionFieldDto Field(string name, string type) => new(name, type, true);
}

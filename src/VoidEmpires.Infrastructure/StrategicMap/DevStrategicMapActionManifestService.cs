using VoidEmpires.Application.StrategicMap;

namespace VoidEmpires.Infrastructure.StrategicMap;

public sealed class DevStrategicMapActionManifestService : IDevStrategicMapActionManifestService
{
    public GetDevStrategicMapActionManifestResult Get() => new(GetActions());

    private static IReadOnlyList<DevStrategicMapActionManifestItem> GetActions() =>
    [
        Read("strategicMap.read", "Read strategic map", "GET", "/api/dev/strategic-map", [Field("civilizationId", "Guid")], "Returns civilization-scoped relevant systems, planet summaries, fleet presence, transfer overlays, command availability metadata, exploration preview metadata, and route/fuel capability notes."),
        Read("strategicMap.explorationPreview.read", "Read exploration preview", "GET", "/api/dev/strategic-map/exploration-preview", [Field("civilizationId", "Guid")], "Returns read-only exploration readiness metadata derived from map visibility. It does not create missions, sensors, persisted fog-of-war, espionage, diplomacy, combat, or route graph state."),
        Read("exploration.preview.read", "Read exploration preview", "GET", "/api/dev/strategic-map/exploration-preview", [Field("civilizationId", "Guid")], "Returns read-only exploration readiness metadata for mission tooling. It does not create missions, sensors, persisted fog-of-war, espionage, diplomacy, combat, or route graph state."),
        Mutating("exploration.mission.create", "Create exploration mission", "POST", "/api/dev/strategic-map/exploration-missions/create", [Field("civilizationId", "Guid"), Field("targetSystemId", "Guid"), Field("targetPlanetId", "Guid", false), Field("requestedAtUtc", "DateTime")], 201, "Creates a minimal planned exploration mission for a preview-eligible unknown target with deterministic placeholder duration. It does not complete missions, reveal visibility, assign fleets, charge resources, or create sensor/fog-of-war state."),
        Mutating("exploration.mission.completeDue", "Complete due exploration missions", "POST", "/api/dev/strategic-map/exploration-missions/complete-due", [Field("nowUtc", "DateTime")], 200, "Development operation that marks due planned exploration missions completed and records exploration knowledge. It does not assign rewards, run sensors, or mutate fog-of-war."),
        Read("exploration.mission.list", "List exploration missions", "GET", "/api/dev/strategic-map/exploration-missions", [Field("civilizationId", "Guid"), Field("status", "ExplorationMissionStatus", false)], "Returns read-only, civilization-scoped planned and completed exploration missions with an optional status filter. It does not create, complete, cancel, or mutate missions."),
        Read("exploration.knowledge.read", "Read exploration knowledge", "GET", "/api/dev/strategic-map/exploration-knowledge", [Field("civilizationId", "Guid")], "Returns ids-only, civilization-scoped exploration knowledge rows for development inspection. It does not create missions, reveal visibility, mutate map state, or execute gameplay."),
        Read("sensor.profile.read", "Read sensor profiles", "GET", "/api/dev/strategic-map/sensor-profiles", [Field("civilizationId", "Guid")], "Returns read-only placeholder sensor profile metadata. It does not reveal visibility, scan targets, detect objects, or create sensor state."),
        Read("detection.coverage.read", "Read detection coverage", "GET", "/api/dev/strategic-map/detection-coverage", [Field("civilizationId", "Guid")], "Returns read-only placeholder detection coverage metadata. It does not reveal unknown targets, trigger detection, interception, or combat, or create detection state."),
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

    private static DevStrategicMapActionManifestItem Mutating(
        string key,
        string displayName,
        string method,
        string route,
        IReadOnlyList<DevStrategicMapActionFieldDto> fields,
        int successStatus,
        string notes) => new(key, displayName, method, route, false, fields, successStatus, [400, 404, 409, 503], notes);

    private static DevStrategicMapActionFieldDto Field(string name, string type, bool isRequired = true) => new(name, type, isRequired);
}

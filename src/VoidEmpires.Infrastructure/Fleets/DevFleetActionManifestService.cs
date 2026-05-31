using VoidEmpires.Application.Fleets;

namespace VoidEmpires.Infrastructure.Fleets;

public sealed class DevFleetActionManifestService : IDevFleetActionManifestService
{
    public GetDevFleetActionManifestResult Get() => new(GetActions());

    private static IReadOnlyList<DevFleetActionManifestItem> GetActions() =>
    [
        Read("fleet.overview.read", "Read fleet overview", "GET", "/api/dev/fleets/overview", [Field("civilizationId", "Guid")], "Returns operational group state and command availability."),
        Read("fleet.uiState.read", "Read fleet UI state", "GET", "/api/dev/fleets/ui-state", [Field("civilizationId", "Guid")], "Returns a UI-ready aggregate payload for fleet screen prototypes."),
        Mutating("fleet.travel.estimate", "Estimate orbital travel", "POST", "/api/dev/fleets/orbital-travel/estimate", 200, [Field("civilizationId", "Guid"), Field("orbitalGroupId", "Guid"), Field("destinationPlanetId", "Guid")], "Read-only estimate of duration, resource costs, and affordability."),
        Mutating("fleet.transfer.create", "Create orbital transfer", "POST", "/api/dev/fleets/orbital-transfers/create", 201, [Field("civilizationId", "Guid"), Field("orbitalGroupId", "Guid"), Field("destinationPlanetId", "Guid"), Field("requestedAtUtc", "DateTime UTC")], "Charges estimated travel costs, reserves the group, and creates a transfer."),
        Mutating("fleet.transfer.cancel", "Cancel orbital transfer", "POST", "/api/dev/fleets/orbital-transfers/cancel", 200, [Field("civilizationId", "Guid"), Field("orbitalTransferId", "Guid")], "Cancels an active transfer, releases the group, and does not refund resources."),
        Mutating("fleet.transfer.complete", "Complete due orbital transfers", "POST", "/api/dev/fleets/orbital-transfers/complete-due", 200, [Field("nowUtc", "DateTime UTC")], "Completes due transfers and moves groups to destination planets."),
        Mutating("fleet.group.split", "Split orbital group", "POST", "/api/dev/fleets/orbital-groups/split", 201, [Field("civilizationId", "Guid"), Field("sourceOrbitalGroupId", "Guid"), Field("quantity", "int")], "Creates a new compatible group from part of a stationed source group."),
        Mutating("fleet.group.merge", "Merge orbital groups", "POST", "/api/dev/fleets/orbital-groups/merge", 200, [Field("civilizationId", "Guid"), Field("targetOrbitalGroupId", "Guid"), Field("sourceOrbitalGroupId", "Guid")], "Merges compatible stationed groups at the same current planet."),
    ];

    private static DevFleetActionManifestItem Read(
        string key,
        string displayName,
        string method,
        string route,
        IReadOnlyList<DevFleetActionFieldDto> fields,
        string notes) => new(key, displayName, method, route, true, fields, 200, [400, 404, 503], notes);

    private static DevFleetActionManifestItem Mutating(
        string key,
        string displayName,
        string method,
        string route,
        int successStatus,
        IReadOnlyList<DevFleetActionFieldDto> fields,
        string notes) => new(key, displayName, method, route, false, fields, successStatus, [400, 404, 409, 503], notes);

    private static DevFleetActionFieldDto Field(string name, string type) => new(name, type, true);
}

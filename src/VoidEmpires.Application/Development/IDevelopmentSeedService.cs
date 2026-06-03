namespace VoidEmpires.Application.Development;

public sealed record ApplyDevelopmentSeedRequest(string Profile);

public sealed record DevelopmentSeedProfileKeyId(
    string Key,
    string Value,
    string Note);

public sealed record DevelopmentSeedProfileMetadata(
    string Name,
    bool IsImplemented,
    string AvailabilityNote,
    string ReapplyBehavior,
    IReadOnlyList<string> IntendedCockpits,
    IReadOnlyList<string> RecommendedQaUrls,
    IReadOnlyList<DevelopmentSeedProfileKeyId> KeyIds);

public sealed record ApplyDevelopmentSeedResult(
    bool Succeeded,
    string Profile,
    IReadOnlyList<string> AppliedSteps,
    IReadOnlyList<string> Errors,
    DevelopmentSeedProfileMetadata? ProfileMetadata,
    IReadOnlyList<DevelopmentSeedProfileMetadata> KnownProfiles)
{
    public static ApplyDevelopmentSeedResult Success(
        string profile,
        IReadOnlyList<string> appliedSteps,
        DevelopmentSeedProfileMetadata profileMetadata,
        IReadOnlyList<DevelopmentSeedProfileMetadata> knownProfiles) =>
        new(true, profile, appliedSteps, [], profileMetadata, knownProfiles);

    public static ApplyDevelopmentSeedResult Failure(
        string profile,
        IReadOnlyList<string> errors,
        IReadOnlyList<DevelopmentSeedProfileMetadata> knownProfiles,
        DevelopmentSeedProfileMetadata? profileMetadata = null) =>
        new(false, profile, [], errors, profileMetadata, knownProfiles);
}

public static class DevelopmentSeedProfiles
{
    public static readonly DevelopmentSeedProfileMetadata MinimalValidation = new(
        "minimal-validation",
        true,
        "Implemented today. Use this for the current deterministic QA baseline.",
        "Additive and idempotent. Inserts missing baseline rows, tops up the Aurelia stockpile to documented minimums, and preserves existing queues or extra mutated rows.",
        ["Galaxy", "Planet", "Construction", "Research", "Shipyard", "Fleets"],
        [
            "/?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
        ],
        [
            new("playerProfileId", "90000000-0000-0000-0000-000000000001", "Validation Commander / seed-user-minimal-validation"),
            new("civilizationId", "00000000-0000-0000-0000-000000000001", "Void Seed Civilization"),
            new("galaxyId", "10000000-0000-0000-0000-000000000001", "Validation Galaxy"),
            new("systemId", "20000000-0000-0000-0000-000000000001", "Helios Gate"),
            new("ownedPlanetId", "40000000-0000-0000-0000-000000000001", "Aurelia"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000002", "Cinder Reach"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000003", "Aether Crown")
        ]);

    public static readonly DevelopmentSeedProfileMetadata CockpitValidation = new(
        "cockpit-validation",
        true,
        "Implemented today. Use this for a richer cross-cockpit QA baseline.",
        "Additive and idempotent. Builds on minimal-validation, preserves existing queues, and adds non-blocking completed history plus richer stockpile and stock context.",
        ["Galaxy", "Planet", "Construction", "Research", "Shipyard", "Fleets"],
        [
            "/?civilizationId=00000000-0000-0000-0000-000000000001&systemId=20000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/planet?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/construction?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001",
            "/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
        ],
        [
            new("playerProfileId", "90000000-0000-0000-0000-000000000001", "Validation Commander / seed-user-minimal-validation"),
            new("civilizationId", "00000000-0000-0000-0000-000000000001", "Void Seed Civilization"),
            new("galaxyId", "10000000-0000-0000-0000-000000000001", "Validation Galaxy"),
            new("systemId", "20000000-0000-0000-0000-000000000001", "Helios Gate"),
            new("ownedPlanetId", "40000000-0000-0000-0000-000000000001", "Aurelia"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000002", "Cinder Reach"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000003", "Aether Crown")
        ]);

    public static readonly DevelopmentSeedProfileMetadata ShipyardValidation = new(
        "shipyard-validation",
        true,
        "Implemented today. Use this for a shipyard-focused richer QA baseline.",
        "Additive and idempotent. Builds on minimal-validation, preserves open queue safety, and adds richer shipyard stock plus completed production history without seeding an active order.",
        ["Shipyard"],
        [
            "/shipyard?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
        ],
        [
            new("civilizationId", "00000000-0000-0000-0000-000000000001", "Void Seed Civilization"),
            new("ownedPlanetId", "40000000-0000-0000-0000-000000000001", "Aurelia")
        ]);

    public static readonly DevelopmentSeedProfileMetadata FleetValidation = new(
        "fleet-validation",
        true,
        "Implemented today. Use this for a fleet-focused richer QA baseline.",
        "Additive and idempotent. Builds on minimal-validation, preserves current fleet command rules, and adds one extra stationed logistics example plus one due active transfer.",
        ["Fleets"],
        [
            "/fleets?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001"
        ],
        [
            new("civilizationId", "00000000-0000-0000-0000-000000000001", "Void Seed Civilization"),
            new("ownedPlanetId", "40000000-0000-0000-0000-000000000001", "Aurelia"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000002", "Cinder Reach"),
            new("visiblePlanetId", "40000000-0000-0000-0000-000000000003", "Aether Crown")
        ]);

    public static readonly DevelopmentSeedProfileMetadata ResearchValidation = CreatePlannedProfile(
        "research-validation",
        ["Research"]);

    public static readonly DevelopmentSeedProfileMetadata PlanetFullValidation = CreatePlannedProfile(
        "planet-full-validation",
        ["Planet", "Construction"]);

    public static IReadOnlyList<DevelopmentSeedProfileMetadata> All { get; } =
    [
        MinimalValidation,
        CockpitValidation,
        ShipyardValidation,
        FleetValidation,
        ResearchValidation,
        PlanetFullValidation
    ];

    public static bool TryGetImplementedProfile(string profile, out DevelopmentSeedProfileMetadata metadata)
    {
        metadata = All.SingleOrDefault(x =>
            x.IsImplemented &&
            string.Equals(x.Name, profile, StringComparison.OrdinalIgnoreCase)) ?? MinimalValidation;

        return string.Equals(metadata.Name, profile, StringComparison.OrdinalIgnoreCase) && metadata.IsImplemented;
    }

    private static DevelopmentSeedProfileMetadata CreatePlannedProfile(
        string name,
        IReadOnlyList<string> intendedCockpits) =>
        new(
            name,
            false,
            "Planned profile name only. Not executable in the current build.",
            "No runtime behavior yet because this profile is not implemented.",
            intendedCockpits,
            [],
            []);
}

public interface IDevelopmentSeedService
{
    Task<ApplyDevelopmentSeedResult> ApplyAsync(
        ApplyDevelopmentSeedRequest request,
        CancellationToken cancellationToken = default);
}

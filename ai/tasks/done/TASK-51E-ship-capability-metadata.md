# TASK-51E

---
id: TASK-51E
title: Ship capability metadata
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 5
dependencies: ["TASK-51A-fleet-domain-and-existing-system-audit.md"]
---

## Goal

Make OrbitalAssetCatalog the authoritative backend source of static ship capabilities for the new FleetMission engine. Extend the existing definition with base movement speed, a Gas-consumption coefficient and typed exploration/colonization flags so later calculations do not create competing per-ship tables. Preserve production, UI metadata, existing storage/range values and legacy movement behavior. This task supplies metadata only, not movement calculations or launch authorization.

## Current behavior / problem

SpaceAssetType has four stable identities: ScoutCraft=1, CargoCraft=2, EscortCraft=3, ColonyCraft=4. OrbitalAssetDefinition is a positional domain record; OrbitalAssetCatalog owns its four definitions and Get(SpaceAssetType) lookup. It already supplies production requirements/costs, StorageCapacity, OperatingRange, display/category/role metadata, descriptions, image/icon keys, sorting and policy metadata. It lacks speed, fuel and typed mission capabilities.

AssetProductionQueueService consumes Requirement/Cost; DevShipyardUiStateService explicitly projects existing fields into its DTO. Repository discovery found no external definition construction or deconstruction: only the four catalog entries construct the record. No API, DTO, frontend, persistence or DI changes are required.

## Desired behavior

Every defined SpaceAssetType resolves to its matching catalog definition with deterministic static capabilities. Future F/I/J/N consumers use the same catalog. Base speed and fuel coefficients are decimal, independent of quantity, research, route and mutable mission state. Exploration and colonization capability are typed flags, not RoleLabel or arbitrary gameplay strings. StorageCapacity and OperatingRange remain the sole cargo/range metadata properties.

## Architectural authority

TASK-51A's audit at docs/dev/fleet-mission-engine-v1.md requires one FleetMission movement authority and reuse of the existing orbital catalog. B owns the lifecycle/version seam, C owns normalized composition and D owns the in-flight cargo ledger; E leaves all three untouched. The dependency remains exactly TASK-51A-fleet-domain-and-existing-system-audit.md. Static metadata belongs to Domain, without DI, DbSet, EF configuration or database rows.

Dependency direction: SpaceAssetType -> OrbitalAssetCatalog -> ship metadata -> F (effective speed), I (fuel), J (cargo capacity), N (full launch eligibility). G owns distance and H owns duration. ResearchCatalog.Propulsion currently describes readiness; do not invent a research modifier here. Current-state/architecture-index historical descriptions do not override the completed A-D contracts or current source.

## Existing conflicting / placeholder metadata

OrbitalFuelReadinessService.GetFuelUnitsPerDistance currently uses Scout=1, Cargo=2, Escort=2.5, Colony=4. These coefficients become the initial v1 catalog values for a traceable future migration; the legacy method itself is unchanged.

Its GetRangeUnitsAvailable table instead uses Scout=6, Cargo=4, Escort=5, Colony=3, conflicting with catalog ranges 3/2/4/5. Those service values are legacy placeholders and must not be copied into new FleetMission calculations. I and later integration/cutover tasks must consume the catalog and retire or bypass legacy tables safely, without creating a third source. Existing legacy readiness tests intentionally protect current behavior until that migration.

OrbitalTravelEstimator still uses placeholder distance=1, one hour per distance unit, Credits+Gas prices and its own cost multipliers; OrbitalRouteProfileService adapts coarse route bands. This task must not change them or silently activate new movement through the Block 54 UI.

## Exact capability fields and semantics

- Reuse int StorageCapacity: authoritative cargo-capacity metadata per ship. J owns checked composition summation, resource-to-cargo-unit conversion and fuel occupying capacity. No CargoCapacity/TransportCapacity/FleetCargoCapacity alias.
- Reuse int OperatingRange: authoritative v1 catalog range metadata. Range is not speed or duration; no replacement range property.
- Add decimal BaseMovementSpeed: positive abstract v1 speed coefficient, with no real-world unit label. F owns effective fleet speed and modifiers; H owns its use in travel time.
- Add decimal FuelConsumptionPerDistanceUnit: nonnegative Gas coefficient per ship per distance unit before future route/speed modifiers; all four current ships have positive values. I owns actual fuel requirements and rounding; no spending/reservation here.
- Add SpaceAssetCapability Capabilities, with [Flags] enum None=0, Exploration=1, Colonization=2. Declare the enum alongside OrbitalAssetDefinition, consistent with small related domain enums already colocated in FleetMission.cs. Unknown bits are invalid static catalog data; validate with a bit mask, not Enum.IsDefined on combined flags.
- Do not add Attack, Combat, Raid, Intercept or Espionage flags. Cargo has no Transport flag: full Transport eligibility belongs to N. Escort has no combat behavior.
- Keep RoleKey/RoleLabel unchanged for human-readable catalog roles; do not add FleetRole or authorize gameplay from translated text. No redundant stored booleans or capability helper service is needed.

## Authoritative v1 values

| Ship | StorageCapacity | OperatingRange | BaseMovementSpeed | FuelConsumptionPerDistanceUnit | Capabilities |
|---|---:|---:|---:|---:|---|
| ScoutCraft | 0 | 3 | 120m | 1.0m | Exploration |
| CargoCraft | 1000 | 2 | 80m | 2.0m | None |
| EscortCraft | 100 | 4 | 100m | 2.5m | None |
| ColonyCraft | 500 | 5 | 60m | 4.0m | Colonization |

Speed order is a v1 design contract: Scout > Escort > Cargo > Colony. Scout alone supports exploration; Colony alone supports colonization. Capabilities are necessary static facts, not sufficient authorization of a complete mission.

## Scope

### In scope

- Three new typed definition fields and the two-bit capability enum.
- Exact v1 catalog values and documentation of existing storage/range semantics.
- Focused catalog validity, exact balance/capability and compatibility tests.
- This task's detailed implementation contract, lifecycle and completion evidence, including the legacy-source warning.

### Out of scope

- Fleet speed, distance, duration, fuel formulas/spending/reservation, cargo summation or unit conversion.
- Mission launch eligibility/transactions, research effects, destination policy, fleet/colony slots, arrival/return, combat and rewards.
- FleetMission, FleetMissionShip, FleetMissionCargo, OrbitalTransfer or legacy movement-service changes.
- UI/API/DTO changes, EF/schema/DbSet, migrations, SQL, seeds, other task execution, Block 52, PRs and merge.

## Compatibility expectations and static validity

Preserve OrbitalAssetCatalog.Get and existing callers. Keep the positional record pattern and explicitly supply new fields at its four catalog construction sites; no default zero-speed profile or second dictionary. The positional constructor/deconstruction signature expands, but repository discovery found no callers outside those four construction sites and no deconstruction consumers. Production and UI continue reading their existing fields unchanged.

Catalog definitions must have known AssetType, StorageCapacity >= 0, OperatingRange > 0, BaseMovementSpeed > 0, FuelConsumptionPerDistanceUnit >= 0 (strictly positive for these four ships) and no unknown capability bits. Static validity is enforced by catalog tests, following the existing code-backed record/catalog convention, without redesigning constructors of unrelated metadata records. A future ship must receive an explicit tested profile. Unknown Get inputs retain the current KeyNotFoundException behavior.

Preserve every existing requirement, construction cost, category, role, description, module, image/icon, sort, policy, prerequisite, requirement-key and tag value. No unrelated rebalancing. No eager use of the new fields by legacy Infrastructure or frontend.

## Detailed implementation steps

1. Verify clean branch/upstream at 85e398228fcd39ff4787008e592f5b9d996fd87a, synchronize, verify exact A-D predecessor files done and E-CZ pending. Move only E to in-progress and expand this contract before source changes.
2. Read existing catalog/types/tests and required audit/discovery/context; inspect legacy formulas and future F/G/H/I/J/N contracts. Trace production/UI consumers to confirm no wiring or DTO changes are needed.
3. Extend OrbitalAssetDefinition with the typed enum and explicit decimal fields, documenting coefficient units and existing storage/range authority. Populate only the new values in all four catalog entries, preserving all old arguments exactly.
4. Expand AssetCatalogTests while keeping existing assertions. Test all types, exact v1 table, speed order, known flags/absence of combat, resource-independent coefficients and regression metadata/costs. Do not calculate fleet behavior in tests or production.
5. Run fresh restore/build, focused catalog tests, full suite, integration-hook inspection and secret scan. Inspect unstaged/staged diffs and compare every path against the allowlist; confirm later tasks remain untouched.
6. Record evidence, move only E to done, commit and push the configured feature branch. Stop before F or any other task; no PR or merge.

## Files to read first

- AGENTS.md; ai/task-template.md; ai/current-state.md; ai/architecture-index.md; ai/orchestrator/component-discovery.md; ai/orchestrator/di-analysis.md: authoring, workflow and existing architectural boundaries.
- docs/dev/fleet-mission-engine-v1.md and the exact completed A/B/C/D fleet task contracts: one authority, compatible cutover and untouched predecessor behavior.
- src/VoidEmpires.Domain/Assets/SpaceAssetType.cs; OrbitalAssetDefinition.cs; OrbitalAssetCatalog.cs; tests/VoidEmpires.Tests/AssetCatalogTests.cs: current types, values and test contracts.
- src/VoidEmpires.Domain/Research/ResearchCatalog.cs: existing propulsion readiness metadata, without applying it here.
- src/VoidEmpires.Infrastructure/Fleets/OrbitalFuelReadinessService.cs; src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs; src/VoidEmpires.Infrastructure/Fleets/OrbitalRouteProfileService.cs: legacy duplicate values that remain untouched.
- Pending TASK-51F-fleet-speed-calculation.md; TASK-51G-galactic-distance-calculation.md; TASK-51H-travel-time-calculation.md; TASK-51I-fuel-consumption-calculation.md; TASK-51J-fleet-cargo-capacity-calculation.md; TASK-51N-fleet-launch-validation.md: future consumers, read for boundaries only.

## Expected files to modify

- src/VoidEmpires.Domain/Assets/OrbitalAssetDefinition.cs: capability enum and documented movement/fuel/capability fields; reuse storage/range/roles.
- src/VoidEmpires.Domain/Assets/OrbitalAssetCatalog.cs: four explicit v1 profiles, all existing values preserved.
- tests/VoidEmpires.Tests/AssetCatalogTests.cs: focused static metadata and regression coverage.
- This exact task's expanded body and pending -> in-progress -> done lifecycle. No extra enum file is needed.

## Edge cases

- Scout storage remains zero; this is valid metadata, not missing capacity.
- None is valid for Cargo/Escort and does not imply every mission is forbidden or Transport automatically allowed.
- Combined known flags are representable for future profiles; undefined bits are not permitted. No combat flags are introduced.
- Fuel 2.5 for Escort must remain exact decimal. Zero fuel is semantically permitted for future metadata, but all current profiles are positive.
- Undefined SpaceAssetType lookup fails as before. No mutable inputs, persistence failures, timing, authorization or concurrency path is added by this static task.
- Legacy range divergence is deliberately retained until safe integration; tests must not overwrite the canonical 3/2/4/5 range to match legacy readiness.

## Test plan

- Preserve the four existing catalog tests and their requirement/operator/building/UI assertions.
- For every enum asset, Get returns the matching known type and nonnegative storage, positive range/speed, nonnegative fuel (positive for v1), and only Exploration/Colonization flag bits.
- Assert all six columns of the v1 table for every ship and Scout > Escort > Cargo > Colony speed order.
- Explicitly assert exploration only on Scout and colonization only on Colony using typed flags; protect the enum's None/Exploration/Colonization contract and ability to combine the two known bits.
- Preserve requirements, production costs and role key/label for all ships, plus existing nonempty descriptive/UI/requirement/tag contracts. Verify decimal property types and retain StorageCapacity/OperatingRange as documented sole authorities.
- Unknown enum lookup retains its exception. Full regression suite covers unchanged B/C/D, production, shipyard and legacy movement behavior.

## Acceptance criteria

- All four catalog entries expose the exact documented values through the existing Get API; no duplicate catalog or cargo/range/role properties.
- Typed flags contain only Exploration and Colonization; no combat or premature launch eligibility appears.
- Storage/range, production requirements/costs and UI metadata stay unchanged; legacy readiness/travel services and FleetMission models are untouched.
- Task body is implementation-ready, identity metadata unchanged, and only E progresses to done after successful validation.
- Fresh focused/full tests, build and secret scan succeed; no schema, SQL, seed, other tasks or PR are produced.

## Validation

Run dotnet restore; dotnet build --no-restore; dotnet test --no-build --filter FullyQualifiedName~AssetCatalogTests; dotnet test --no-build. Set VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false only in test processes and report the guard's early return as no live SQL coverage, even when xUnit counts it passing. Inspect scripts/run-integration-tests.ps1; if still a placeholder, report exactly: No integration tests configured.

Run powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1. Review git diff --check, --stat, --name-only and status, including staged equivalents. Record actual fresh results, not the historical 986 baseline.

## Commit and push

After successful validation, append evidence and move this task to done, stage only its three source/test files and lifecycle, commit feat(fleets): add authoritative ship capability metadata, push the configured branch, verify clean/upstream state and stop. Do not execute F, create Block 52, open a PR or merge.

## Change Budget

Keep the static capability model, catalog, tests and detailed lifecycle together as one cohesive responsibility. File/line counts are review signals under current AGENTS.md, not hard limits. Split only for a separable responsibility or concrete regression/review risk; no follow-up task is needed for already planned F-J/N integration work.

## Completion evidence

Completed 2026-10-08 on codex/block-51a-51cz-fleet-movement-mission-engine-v1 from clean synchronized HEAD/upstream 85e398228fcd39ff4787008e592f5b9d996fd87a. Fetch and fast-forward pull confirmed no remote advancement. Exact A-D predecessor contracts were done and 100 Block 51 tasks were pending at start.

- Expanded this task before source implementation, preserving its id, title, roadmap, type/team/supporting teams, priority, execution_order=5 and dependency identity. Only E changes lifecycle pending -> in-progress -> done.
- Added [Flags] SpaceAssetCapability (None=0, Exploration=1, Colonization=2), decimal BaseMovementSpeed and decimal FuelConsumptionPerDistanceUnit to the existing positional definition. Four catalog entries supply explicit profiles from the authoritative v1 table above; no helpers, duplicate dictionary or duplicate cargo/range/role properties.
- Only Scout is exploration-capable and only Colony is colonization-capable. Speeds are abstract units, fuel coefficients are Gas per ship per distance unit. StorageCapacity and OperatingRange remain the authoritative existing fields. RoleKey/RoleLabel stay unchanged and are not gameplay authorization.
- Production diff adds only the new capability metadata/values and semantic comments. Every old catalog argument, production requirement/cost, visual/UI field and policy is preserved. Read-only independent review verified the production diff and compatibility boundaries; Get callers need no changes.
- OrbitalFuelReadinessService and both legacy travel/profile sources are unchanged. Their hardcoded values remain compatibility placeholders, documented above as unsuitable authorities for new calculations. I and later integration/cutover work own migration. FleetMission, StateVersion, composition, cargo, OrbitalTransfer, UI/API, DI and persistence are untouched.
- dotnet restore: passed; all projects up to date.
- dotnet build --no-restore: passed, 0 warnings, 0 errors.
- dotnet test --no-build --filter FullyQualifiedName~AssetCatalogTests: 18 passed, 0 failed, 0 skipped (four existing cases plus 14 new cases; existing assertions retained and extended).
- dotnet test --no-build: 1000 passed, 0 failed, 0 skipped (986 predecessor baseline plus 14 new cases), including unchanged B/C/D and legacy behavior regressions.
- Both test processes explicitly used VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false. SQL smoke returned early; its counted pass is not live SQL Server or relational coverage.
- scripts/run-integration-tests.ps1 inspected and remains an unadapted placeholder. No integration tests configured.
- Repository secret scan passed. Changed source/test/task text reviewed; no secrets introduced. No migrations generated/applied, SQL executed, seed executed or manual/browser QA.
- Unstaged and staged git diff --check, --stat, --name-only and status reviewed against the exact three source/test files plus this lifecycle. Final staged scope: 302 insertions and 93 deletions across five Git paths (three source/test files and the task deletion/addition); no unstaged changes. Line/file counts are cohesion signals; no follow-up tasks were created. LF-to-CRLF notices are Git checkout normalization, not build warnings.
- All 99 original TASK-51F through TASK-51CZ files remain unchanged and pending, checked against the starting Git tree. No Block 52 task, PR or merge was created. No later task was executed.
- Final action: commit this completed cycle as feat(fleets): add authoritative ship capability metadata, push the configured branch, verify clean/upstream state and stop. The execution response reports the final SHA rather than embedding a commit's own hash in its contents.

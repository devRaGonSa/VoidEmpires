# TASK-51F

---
id: TASK-51F
title: Fleet speed calculation
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 6
dependencies: ["TASK-51C-fleet-mission-composition-model.md", "TASK-51E-ship-capability-metadata.md"]
---

## Goal

Introduce the authoritative, pure and deterministic fleet-speed calculation for FleetMission v1. Future preview and launch callers must receive the slowest participating catalog speed, its limiting ship type, the supplied propulsion modifier and the effective speed without persisting a mission or duplicating ship balance constants. Preserve every existing lifecycle, composition, cargo and legacy transfer behavior.

## Current behavior / problem

TASK-51C provides FleetMissionShip rows with AssetType and positive Quantity, normalized by mission/type. TASK-51E supplies OrbitalAssetCatalog.BaseMovementSpeed for every ship but no fleet-level rule. Preparing missions may legitimately have no ships yet; B's domain launch behavior deliberately does not implement complete launch validation. F must reject an empty calculation input without modifying that lifecycle contract.

OrbitalTravelEstimator still uses fixed abstract distance=1 and one hour per unit plus legacy Credits/Gas costs. OrbitalTravelEstimateService composes that estimator with route, fuel and affordability services. Those are compatibility paths, not FleetMission speed authority, and remain unchanged until later integration/cutover.

## Desired behavior

A caller supplies immutable typed composition rows before or after persistence. The calculator validates every row, reads ship speed exclusively from OrbitalAssetCatalog, chooses the minimum and multiplies it by a positive decimal modifier (default 1.0m). It returns immutable numeric/typed data with no presentation strings. Input order, positive quantity magnitude, valid duplicate rows and repeated calls cannot change the result. The operation neither reads nor writes research, missions, stocks or any database.

## Architectural authority and selected layer

Use src/VoidEmpires.Application/Fleets/FleetSpeedCalculator.cs, not the provisional Domain path in the old task. Input/result records live together in Application/Fleets/FleetSpeedResult.cs. Application already references Domain; it can read OrbitalAssetCatalog and expose results to H/N without creating a Domain -> Application reference or circular dependency. Existing PlanetVisualIntensityCalculator is a pure static Application calculation precedent. No DI or Infrastructure dependency is needed.

The A audit at docs/dev/fleet-mission-engine-v1.md governs single-authority/cutover behavior. Preserve C's normalization, E's catalog authority and B/D lifecycle/cargo invariants. Existing legacy services and DTOs are inspected for compatibility only. Only this task's expected calculator path is reconciled; no future task file is edited.

## Static catalog inputs

Every production lookup must be OrbitalAssetCatalog.Get(assetType).BaseMovementSpeed after explicit SpaceAssetType validation. Do not copy balance numbers to production switches, dictionaries, services, UI or another catalog.

| AssetType | Current catalog base speed |
|---|---:|
| ScoutCraft | 120m |
| CargoCraft | 80m |
| EscortCraft | 100m |
| ColonyCraft | 60m |

These are abstract speed coefficients, not km/h, range, duration, fuel or cargo units. Each resolved speed must be strictly positive, including metadata for a row that would not otherwise limit the fleet. Invalid future catalog data fails closed.

## Fleet-speed formula and deterministic limiter

BaseSpeed = MIN(catalog BaseMovementSpeed for each participating ship type).
EffectiveSpeed = checked(BaseSpeed * PropulsionModifier).

Every input row must have Quantity > 0. Quantity determines valid participation only: do not sum, average, weight or multiply speed by quantity. Accept duplicate valid types without merging quantities; validate each duplicate independently. This avoids irrelevant quantity-sum overflow and leaves payload normalization to N.

If multiple types share the lowest speed, choose the lowest numeric SpaceAssetType among them. This tie policy is independent of enumeration order. Select the base limiter before applying the common positive modifier. Enumerate input once, using local state; do not sort or mutate the caller's collection, cache results or return a default limiting enum on empty input.

## Modifier policy and research decision

Expose decimal propulsionModifier = 1.0m as a pure backend parameter. Reject zero/negative values. Permit positive fractions below one (future slowdown) as well as values above one. Do not persist the modifier or expose it through a player API here. No explicit rounding or float/double conversion is introduced; checked CLR decimal multiplication defines representability and throws on overflow. Reject a computed nonpositive effective speed, including possible underflow to zero if future catalog speeds become extremely small.

ResearchType.Propulsion and ResearchCatalog's fleet_speed BonusKey exist, and ResearchProject stores CivilizationId/ResearchType/Level. There is no authoritative propulsion-level-to-speed formula. ResearchBonusCalculator's generic fallback returns a level for otherwise unmapped research; that display/readiness value is not a propulsion multiplier and must not be used as one. Do not invent percentages, curves, caps or research queries.

Future flow: ResearchProject -> authoritative bonus provider -> decimal multiplier -> FleetSpeedCalculator. F implements only the final calculation boundary. V1 uses identity 1.0 unless a trusted backend caller explicitly supplies a valid multiplier derived elsewhere.

## In scope

- Immutable calculation-only FleetSpeedShipInput and FleetSpeedResult records.
- Catalog-driven slowest-ship selection, deterministic tie handling, duplicate/quantity semantics and validation.
- Positive decimal modifier seam, checked effective speed and fail-closed metadata handling.
- Focused calculation/edge/purity tests and this detailed lifecycle contract.

## Out of scope

- Research bonus formulas, research persistence/provider implementation, distance, slots, orbital coordinates, travel duration/arrival, fuel, route profiles and cargo capacity.
- Launch validation/transactions, spending, FleetMission mutation, UI/API, workers, DI and Infrastructure changes.
- EF, schema, DbSet, migrations, SQL and seed actions.
- Replacing OrbitalTravelEstimator, OrbitalFuelReadinessService or Block 54 behavior; executing other tasks, creating Block 52, PRs or merges.

## Detailed implementation requirements

- Public API: FleetSpeedCalculator.Calculate(IEnumerable<FleetSpeedShipInput> composition, decimal propulsionModifier = 1.0m).
- FleetSpeedShipInput is a sealed positional record (SpaceAssetType AssetType, int Quantity), with immutable value semantics and no identity/lifecycle/inventory state. Persisted Ships or future preview requests project into it; no database entity is required.
- FleetSpeedResult is a sealed positional record (decimal BaseSpeed, decimal PropulsionModifier, decimal EffectiveSpeed, SpaceAssetType LimitingAssetType). Do not add distance, time, fuel, capacity or UI copy.
- Null composition fails with ArgumentNullException. A null row or empty sequence fails with ArgumentException. Undefined type, nonpositive quantity and nonpositive modifier fail with ArgumentOutOfRangeException, rather than leaking a catalog KeyNotFoundException.
- Check every resolved base speed > 0; otherwise throw InvalidOperationException. Validate all rows before success, including invalid entries after a known slowest ship and duplicate invalid quantities.
- Use checked decimal multiplication; overflow propagates as OverflowException. A nonpositive computed result fails with InvalidOperationException. The operation has no partial external state to roll back.
- Keep the public path bound to OrbitalAssetCatalog. A private calculation core receives a speed resolver solely to separate the fold from catalog resolution and allow isolated fault/tie tests. No public alternate-catalog parameter or DI registration. Tests can bind that private core once to a strongly typed reflection delegate; no InternalsVisibleTo or global catalog mutation is needed.

## Implementation steps

1. Verify clean configured feature branch/upstream at 583307c49effdfe116be2068ae62d18ffeb87e17, exact A-E done files and F-CZ pending. Move only F to in-progress and enrich this contract before source implementation.
2. Read required architecture/discovery/predecessor/catalog/research/legacy sources. Confirm no propulsion gameplay formula, valid Application -> Domain direction and no existing speed calculator to duplicate.
3. Add input/result records and pure single-pass calculator using the catalog and the formula above. Keep all quantities/types/modifier/metadata guards local and preserve existing mission/legacy code.
4. Add table-driven examples, quantity/order/duplicate independence, modifier boundaries/overflow, single-use enumeration, null/invalid inputs and purity tests. Exercise future cross-type ties and bad metadata through isolated resolver inputs without changing the static catalog.
5. Run fresh restore/build, focused FleetSpeedCalculatorTests, full suite and secret scan; inspect integration configuration and all diff scopes. Record actual results rather than historical baseline.
6. Add completion evidence, move only F to done, commit and push the feature branch after staged review, verify clean/upstream and stop before G or any other task.

## Files to read first

- AGENTS.md; ai/task-template.md; ai/current-state.md; ai/architecture-index.md; ai/orchestrator/component-discovery.md; ai/orchestrator/di-analysis.md; docs/dev/fleet-mission-engine-v1.md: workflow, task quality and authoritative architecture/cutover.
- Completed TASK-51C-fleet-mission-composition-model.md and TASK-51E-ship-capability-metadata.md: exact prerequisites, composition and catalog contracts.
- src/VoidEmpires.Domain/Assets/SpaceAssetType.cs; OrbitalAssetDefinition.cs; OrbitalAssetCatalog.cs; src/VoidEmpires.Domain/Fleets/FleetMissionShip.cs; FleetMission.cs: current ship metadata, projection and unchanged aggregate invariants.
- src/VoidEmpires.Domain/Research/ResearchType.cs; ResearchDefinition.cs; ResearchCatalog.cs; ResearchProject.cs; ResearchBonusCalculator.cs: confirm metadata/readiness versus actual implemented research formulas.
- src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs; src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs; src/VoidEmpires.Application/Fleets/IOrbitalTravelEstimateService.cs; EstimateOrbitalTravelResult.cs: untouched compatibility flow.
- src/VoidEmpires.Application/VoidEmpires.Application.csproj; src/VoidEmpires.Application/Visuals/PlanetVisualIntensityCalculator.cs: existing dependency direction and pure-calculator style.
- Pending TASK-51G-galactic-distance-calculation.md; TASK-51H-travel-time-calculation.md; TASK-51I-fuel-consumption-calculation.md; TASK-51N-fleet-launch-validation.md: future boundaries only.

## Expected files to modify

- src/VoidEmpires.Application/Fleets/FleetSpeedCalculator.cs (new): reconciled placement for pure catalog-driven speed calculation.
- src/VoidEmpires.Application/Fleets/FleetSpeedResult.cs (new): immutable input/result value records together.
- tests/VoidEmpires.Tests/FleetSpeedCalculatorTests.cs (new): exhaustive deterministic calculation and failure/purity evidence.
- This exact task's expanded body and pending -> in-progress -> done lifecycle. No project references, catalog constants, mission classes, legacy services or other task edits.

## Edge cases

Empty or null input is not success; no zero/max/default sentinel result. Unknown enum, null row, zero/negative quantity or modifier are rejected explicitly. Every duplicate must be valid even when another row of that type already participated. Quantities up to int.MaxValue are accepted without irrelevant summation/multiplication. Input enumerators are consumed once and their failures propagate without mutation.

Ties select the numerically lowest type; synthetic tests cover different tied types because current catalog speeds are unique. Tiny positive decimal modifiers remain valid if the result is representable and positive. Extreme positive modifiers throw on overflow. Invalid future nonpositive base metadata and positive-value multiplication underflow fail closed. No default modifier is inferred from a persisted Propulsion level.

## Test plan

- Every single type: exact current speed and limiter; default modifier equals 1 and effective equals base/catalog.
- Mixed examples: Scout+Escort=100, Scout+Cargo=80, Escort+Colony=60, Scout+Escort+Cargo=80 and all four=60, with correct limiters.
- Compare small/large quantities (including int.MaxValue), every permutation of a mixed fleet, repeated calls, lazy/single-use enumeration and valid duplicated types versus normalized rows.
- Reject null/empty inputs, null rows, zero/negative quantities including duplicate/late invalid entries, and undefined enums with validation exceptions.
- Modifier cases: 80*1=80, *1.10=88, *1.25=100, *0.50=40; reject <=0; throw OverflowException for decimal.MaxValue; accept smallest positive decimal when representable.
- Isolated private-core tests cover equal speeds with opposite/all input orderings, nonpositive catalog metadata even on nonlimiting rows, and multiplication underflow. Public tests always consume the real catalog, and test fixtures never replace its dictionary or values.
- Assert input, full catalog snapshots and a projected FleetMission's state/composition/cargo/version are unchanged on success and rejected calculations. No persistence setup needed.
- Verify no invented research rule: all default calls use identity, with documentation of future trusted provider flow. Full unchanged B/C/D/E, production and legacy suite must pass.

## Acceptance criteria

- Pure Application calculation reads authoritative catalog speeds and exposes the four result fields with correct dependency direction.
- Minimum ship speed, quantity/duplicate/order independence, numeric tie break and positive modifier behavior match the documented contract.
- All invalid inputs/metadata, overflow and nonpositive results fail without mutating anything; no inventory, research query or legacy activation appears.
- Only the three implementation/test paths and detailed F lifecycle change; other tasks remain pending. Fresh required validation succeeds, with no schema, SQL, seed, PR or subsequent-task execution.

## Validation

Run dotnet restore; dotnet build --no-restore; dotnet test --no-build --filter FullyQualifiedName~FleetSpeedCalculatorTests; dotnet test --no-build. Set VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false only in test processes and report no live SQL coverage even though its early-return case counts as passing.

Inspect scripts/run-integration-tests.ps1; when it remains the unadapted placeholder, report exactly: No integration tests configured. Run powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1. Check unstaged/staged git diff --check, --stat, --name-only and git status against the expected paths; report actual fresh totals.

## Commit and push

After validation, append evidence, move only F to done and stage its exact scope. Commit feat(fleets): add authoritative fleet speed calculation, push configured feature branch and verify clean synchronized status. Stop after F; no G, AA, other task, Block 52, PR or merge.

## Change Budget

Keep this pure calculation, immutable contracts, tests and lifecycle as one cohesive change. File/line totals are review signals, not hard limits. No arbitrary split or duplicate follow-up for work already assigned to G/H/I/N.

## Completion evidence

Completed 2026-10-08 on codex/block-51a-51cz-fleet-movement-mission-engine-v1 from clean synchronized HEAD/upstream 583307c49effdfe116be2068ae62d18ffeb87e17. Fetch and fast-forward pull confirmed no advancement. Exact A-E predecessor files were done; all 99 Block 51 pending files had pending status before implementation.

- Expanded this task before implementation, preserving id/title/roadmap/type/team/supporting teams/priority/execution_order=6 and the exact C/E dependencies. Only F transitions pending -> in-progress -> done.
- Reconciled the old Domain calculator proposal into Application/Fleets alongside its immutable input/result records. Existing Application -> Domain reference is unchanged; no circular dependency, project change, DI or Infrastructure reference.
- Public input is IEnumerable<FleetSpeedShipInput> (AssetType, Quantity) plus propulsionModifier=1.0m. Result has BaseSpeed, PropulsionModifier, EffectiveSpeed, LimitingAssetType. Preview rows and projected persisted composition share the same pure calculation.
- Production resolves all speeds from OrbitalAssetCatalog.Get(type).BaseMovementSpeed. Observed defaults: Scout120, Cargo80, Escort100, Colony60. Minimum participating speed wins; equal minimum types use lowest numeric enum. Positive quantities never scale/weight speed. Valid duplicates are accepted without quantity summation; every row is validated.
- EffectiveSpeed uses checked decimal multiplication, supports positive sub-1 modifiers and rejects nonpositive inputs, invalid/empty/null composition, unknown types, bad base metadata, overflow and nonpositive computed results. Input is enumerated once and nothing external is mutated.
- Propulsion research remains descriptive/readiness metadata with no authoritative level-to-speed formula. Default is identity; a future trusted provider may supply a multiplier. No research reads, percentages, formula, persistence or player-controlled modifier API added.
- Isolated private-core tests verify all-order cross-type ties, invalid future metadata and positive-decimal underflow without changing global catalog data. Public API always uses the catalog; no public alternate source or assembly-wide test visibility added. Independent read-only review found no implementation/contract defects.
- dotnet restore: passed; all projects up to date.
- dotnet build --no-restore: passed, 0 warnings, 0 errors.
- dotnet test --no-build --filter FullyQualifiedName~FleetSpeedCalculatorTests: 49 passed, 0 failed, 0 skipped.
- dotnet test --no-build: 1049 passed, 0 failed, 0 skipped (1000 prior baseline plus 49 new cases), including unchanged B/C/D/E and legacy regressions.
- Both test processes used VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false. Its passing early return is not live SQL Server or relational coverage. No database connection or persistence setup was needed by the new tests.
- scripts/run-integration-tests.ps1 inspected and still an unadapted placeholder. No integration tests configured.
- Repository secret scan passed; changed code/tests/task reviewed with no secrets introduced. No persistence/schema changes, migrations generated/applied, SQL executed, seeds executed or browser/manual QA.
- Unstaged and staged git diff --check, --stat, --name-only and status reviewed against the exact allowlist. Staged total: 477 insertions and 92 deletions across five Git paths (three new implementation/test files plus task deletion/addition); no unstaged edits. Git LF-to-CRLF notices concern checkout normalization, not build warnings. Counts are cohesion signals, not hard limits; no split or follow-up task was needed.
- All 98 original TASK-51G through TASK-51CZ files remain unchanged and pending, checked against the starting Git tree. No catalog, FleetMission, composition/cargo, legacy movement or other existing source was modified. No Block 52 tasks, PR, merge or subsequent task execution.
- Final action: commit feat(fleets): add authoritative fleet speed calculation, push the configured branch, verify clean/upstream and stop. The final commit SHA is reported in the execution response rather than embedded in its own commit contents.

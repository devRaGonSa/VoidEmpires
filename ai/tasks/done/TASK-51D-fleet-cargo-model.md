# TASK-51D

---
id: TASK-51D
title: Fleet cargo model
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 4
dependencies: ["TASK-51B-fleet-mission-domain-model.md", "TASK-51C-fleet-mission-composition-model.md"]
---

## Goal

Introduce the authoritative mission-owned, normalized ledger for Credits, Metal, Crystal and Gas physically in transit. FleetMission owns initial loading and subsequent delivery/return accounting, preserving exact conservation without manipulating stockpiles. This extends the lifecycle/version seam from B and the child-collection pattern from C so later launch and settlement transactions have one ledger to use.

## Problem statement / current behavior

FleetMission already owns its UTC journey, state machine, recall/return schedule and checked StateVersion. FleetMissionShip stores normalized composition with a composite key and field-backed read-only collection. Neither component currently tracks cargo. PlanetResourceStockpile owns planetary resources, mapped as decimal(18,4); its Increase helper clamps at capacity and cannot serve as an in-flight ledger. OrbitalAssetCatalog exposes ship StorageCapacity (Scout 0, Cargo 1000, Escort 100, Colony 500), but there is no authoritative resource-to-cargo-unit conversion yet.

## Desired behavior

A Preparing mission accepts positive resource loads, merges repeated resource types and exposes immutable collection views. Zero cargo is valid and creates no row. A Processing mission records accepted destination delivery; a Returning mission records settled origin returns. These operations only update accounting, never balances or the mission phase. Every real mutation advances StateVersion once; rejected input and recognized accounting retries leave the whole aggregate unchanged. A recalled mission retains all undelivered cargo on the same mission until settlement. Backend callers supply authoritative availability, fuel and capacity values to pure validation seams.

## Context and architectural decisions

- Completed predecessors: TASK-51B-fleet-mission-domain-model.md and TASK-51C-fleet-mission-composition-model.md; governing audit: docs/dev/fleet-mission-engine-v1.md (TASK-51A). Preserve their lifecycle, UTC, recall, empty-composition launch and ship normalization contracts.
- Mission-owned typed cargo is the only in-flight resource ledger. No OrbitalGroup inventory, JSON/blob, duplicated stockpile or independent cargo aggregate.
- Domain owns invariants; Infrastructure maps the child through ApplyConfigurationsFromAssembly. No Application/Web/worker/DI changes or child DbSet are needed.
- Existing ResourceType numeric identities remain Credits=1, Metal=2, Crystal=3, Gas=4. Mission-type cargo eligibility belongs to N, not this entity.
- I calculates fuel; J calculates fleet capacity and resource-unit conversion; N supplies full launch validation; O/Q own atomic stockpile/fuel deduction and concurrency; T owns destination credit; U/AB own origin return credit. Those transactions must combine ledger/version updates with balance changes atomically.
- Provider-independent metadata/InMemory tests prove mapping and domain behavior only. BH owns final relational CHECK hardening; BI/BJ/BK own migrations/SQL review. No database actions here.

## Scope

### In scope

- FleetMissionCargo entity and mission-owned collection.
- Preparing load/merge, Processing delivery, Returning return accounting, precision and conservation checks.
- StateVersion integration and documented replay preconditions.
- Pure supplied-resource, Gas-plus-fuel and supplied-capacity validation.
- EF key/relationship/precision configuration, focused tests and this task's detailed lifecycle record.

### Out of scope

- Capacity or fuel formulas/reservations, stockpile deduction/credit, transactions or concurrent resource spending.
- Slots, distance, duration, launch/arrival/return services, transport handlers, rewards, colonization, market, APIs and frontend.
- OrbitalTransfer changes, migrations, generated SQL, seeds, real database operations, PRs or subsequent tasks.

## Detailed implementation requirements

### Domain model and loading

- FleetMissionCargo has private setters for MissionId, ResourceType, LoadedAmount, DeliveredAmount and ReturnedAmount; no random row Id. Empty mission IDs and undefined resource enums are rejected.
- LoadedAmount is strictly positive for every row. DeliveredAmount and ReturnedAmount start at zero. RemainingAmount is derived, not stored.
- FleetMission uses a private _cargo list and a cached AsReadOnly view with private setter, matching Ships and existing reflection regression tests. No lazy loading or public child mutators.
- AddCargo(type, amount) validates the resource and representable nonnegative amount. Zero is a no-op in any phase, including an existing row: no row creation, version change or phase change. Unknown resource zero still fails.
- Positive loading is allowed only in Preparing. Duplicate resource loads merge into the same row; checked addition and storage-range checks run before any mutation or version increment.
- All persisted amounts use decimal(18,4): maximum 99_999_999_999_999.9999; values with nonzero digits beyond four decimal places fail, never round. Numerically representable trailing zeros are accepted. Negative/zero-invalid inputs throw ArgumentOutOfRangeException; unsupported scale throws ArgumentException; storage or arithmetic overflow throws OverflowException.

### Delivery, return and replay contract

- RecordCargoDelivered(type, amount, expectedDeliveredAmount = 0) is allowed only in Processing. RecordCargoReturned(type, amount, expectedReturnedAmount = 0) is allowed only in Returning.
- The amount is a positive delta; the expected argument is the previously observed cumulative counter for that resource and effect kind. Both must be representable decimal(18,4), and expected must be nonnegative. Missing rows, wrong phases and unknown resources fail without mutation.
- Compute target = checked(expected + amount). If the current counter equals target, return false without mutation: that accounting interval is already represented. An exact retry is thus safe while the phase and counter still match. This is an accounting precondition, not a persistent request/event ID.
- Otherwise require current == expected and amount <= RemainingAmount. Reject stale/conflicting expectations or over-consumption. Return true only after applying a new delta and incrementing StateVersion once.
- Additional partial installments must explicitly supply the current cumulative counter, even for equal amounts. Older retries after further progress fail closed; retries after a phase change fail the phase guard. Callers cannot infer new stockpile credit from false or an exception.
- Future T/U/AB must credit only the newly applied delta in the same persistence transaction as the versioned ledger update. This domain contract alone does not provide relational exactly-once processing or request identity.
- All validations precede the existing checked IncrementVersion seam, which precedes child writes. Version overflow therefore leaves every amount, collection and phase intact. No second cargo version is introduced.

### Pure validation seams

- ValidateCargoAvailability(type, availableAmount, reservedFuelGas = 0) compares the row's total loaded amount (zero if absent) against caller-supplied availability; it never spends or credits resources.
- Gas requires loadedGas + reservedFuelGas <= availableGas using checked addition. Nonzero fuel supplied for a non-Gas resource is rejected to avoid silently ignoring a cost. Supplied resource/fuel amounts are nonnegative and decimal(18,4)-representable. Fuel never creates a cargo row.
- ValidateCargoCapacity(requiredCargoUnits, availableCargoCapacityUnits) accepts caller-calculated nonnegative decimal units and rejects required > available. Units are not persisted here and have no resource-unit conversion or decimal(18,4) restriction imposed by this ledger.
- Neither validation changes any entity or StateVersion; both require authoritative runtime inputs from future N/O and capacity units from J. Validation is deliberately not automatically performed by AddCargo without that context.

## Cargo accounting invariants

For every row at every public operation boundary:

- LoadedAmount > 0; DeliveredAmount >= 0; ReturnedAmount >= 0; RemainingAmount >= 0.
- RemainingAmount = LoadedAmount - DeliveredAmount - ReturnedAmount.
- LoadedAmount = DeliveredAmount + ReturnedAmount + RemainingAmount.
- Example: load 1000 Metal, deliver 700, retain 300, then return 300: (1000,700,300,0). No operation can consume the same remaining amount twice.
- Recall schedules a return without changing cargo: Delivered=0, Returned=0, Remaining=Loaded for an undelivered mission. Complete/Cancel retain B's existing orchestration contract; future handlers must settle authoritative effects before calling Complete.

## Persistence model

- Composite primary key (MissionId, ResourceType); required scalar columns and decimal precision 18,4 for all three persisted amounts.
- Required MissionId FK to FleetMission, one-to-many Cargo navigation, cascade delete and _cargo field access matching FleetMissionShipConfiguration.
- Explicitly ignore RemainingAmount. Discover configuration through the existing assembly scan; no separate DbSet or context edit.
- Preserve FleetMission.StateVersion as a provider-independent concurrency token. No provider-specific CHECK syntax; database positivity/conservation checks remain TASK-51BH.

## Implementation steps

1. Verify the clean feature branch at 4f989a72 (or explain legitimate advancement), synchronize upstream, and verify full predecessor filenames and remaining Block 51 tasks. Move only this task pending -> in-progress and enrich its contract before coding.
2. Extend the existing aggregate/child conventions with precision-safe cargo and replay-aware accounting. Preserve lifecycle and Ships behavior; test each mutation and failure atomically.
3. Map the normalized child using existing EF conventions; verify keys, decimal metadata, field navigation, cascade and round-trip accounting.
4. Run fresh restore/build, focused and full tests, inspect integration configuration and run the secret scan. Compare all diff paths to the allowlist and review actual size as a cohesion signal.
5. Record completion evidence, move only D to done, commit and push the configured branch. Stop; do not execute E or any other task.

## Files to read first

- AGENTS.md; ai/task-template.md; ai/current-state.md; ai/architecture-index.md; ai/orchestrator/component-discovery.md; ai/orchestrator/di-analysis.md: workflow, authoring, architecture and discovery.
- docs/dev/fleet-mission-engine-v1.md; completed TASK-51B-fleet-mission-domain-model.md and TASK-51C-fleet-mission-composition-model.md: authoritative audit and accepted predecessor contracts.
- src/VoidEmpires.Domain/Fleets/FleetMission.cs; FleetMissionShip.cs; src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionShipConfiguration.cs; tests/VoidEmpires.Tests/FleetMissionCompositionTests.cs: aggregate and EF conventions.
- src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs; ResourceType.cs; src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetResourceStockpileConfiguration.cs; tests/VoidEmpires.Tests/PlanetResourceStockpileDomainTests.cs: resources and precision.
- src/VoidEmpires.Domain/Assets/OrbitalAssetDefinition.cs; OrbitalAssetCatalog.cs: existing capacity metadata, without implementing its calculation.
- Pending TASK-51J-fleet-cargo-capacity-calculation.md; TASK-51N-fleet-launch-validation.md; TASK-51O-fleet-launch-transaction.md; TASK-51Q-resource-cargo-reservation-hardening.md; TASK-51T-transport-mission-arrival.md; TASK-51U-transport-return-arrival.md; TASK-51AB-return-materialization.md: deferred integration boundaries only.

## Expected files to modify

- src/VoidEmpires.Domain/Fleets/FleetMissionCargo.cs: new typed ledger and amount/accounting validation.
- src/VoidEmpires.Domain/Fleets/FleetMission.cs: collection, versioned loading/accounting and pure validation seams.
- src/VoidEmpires.Infrastructure/Persistence/Configurations/FleetMissionCargoConfiguration.cs: normalized child mapping.
- tests/VoidEmpires.Tests/FleetMissionCargoTests.cs: focused domain and EF evidence.
- This exact task's body and pending -> in-progress -> done lifecycle. No other task metadata changes.

## Edge cases and failure modes

- Zero requests, every negative resource, undefined enums, missing rows, excess decimal scale/range, duplicate merge overflow and version overflow leave the entire aggregate unchanged.
- Over-delivery, over-return, stale expected counters, wrong phases and terminal mutation attempts fail closed; valid exact replays do not advance version.
- Test CLR checked addition overflow by explicitly fault-injecting an impossible persisted amount: two valid decimal(18,4) amounts cannot overflow CLR decimal, but storage-range overflow is tested through normal APIs. This does not legitimize corrupt stored values.
- Resource and mission isolation must hold. Availability is a read-only snapshot validation, not a reservation or authorization check; fresh stock/ownership and transaction retries belong to N/O/Q.
- No clamping or resource loss: destination/origin overflow decisions remain T/U/AB, which must retain uncredited remainder.

## Test plan

- All four normalized resources; 600+400 merge; zero no-op; negative and unknown values; valid .0001, 1.2345, 123.4567 and trailing zeros; rejected scale/range; checked merge overflow and full-state preservation.
- Load only Preparing; delivery only Processing; return only Returning. Test normal and recalled missions, terminal phases, no phase changes during accounting and conservation after each successful mutation.
- Test 1000 -> 700 delivered -> 300 returned; zero/negative/missing/over-consumption; exact retry, stale expectation, equal partial installments with distinct expected counters and resource isolation.
- Check every successful mutation increments once; rejected/no-op/replay operations do not. Inject int.MaxValue StateVersion for add, merge, delivery and return failure atomicity.
- Exact and .0001-over availability for each resource; Gas70+fuel30 against100, then fuel30.0001 failure; fuel-only without a row; unchanged stockpile and mission snapshots. Supplied capacity exact/over/negative and fractional units, without a conversion assumption.
- Read-only collection and private scalar setters; EF key/FK/required/precision/ignored-derived/navigation metadata; four-resource round trip and delivery/return reload, persisted retry and tracked cascade. These tests do not prove SQL constraints/races.
- Full existing B/C and repository regression suite must pass unchanged; no weakening empty-composition or UTC tests.

## Acceptance criteria

- Exactly one positive loaded row per known resource on a mission, no zero rows or mutable external collection.
- Conservation holds through load, partial delivery, normal/recall return and retries; invalid input and overflow cannot partially mutate state/version.
- Pure resource/Gas-fuel/capacity seams enforce supplied bounds without modifying balances or calculating capacity/fuel.
- EF persists exact decimal values and accounting via the composite child relationship, with cascade and existing parent concurrency metadata intact.
- Only the four implementation paths and this task lifecycle are changed; all fresh validation succeeds; no migrations/SQL/seeds, other tasks, Block 52, PR or merge.

## Constraints

Preserve layered .NET 8 backend authority, existing provider selection/SQL Server support and ordinary provider-independent tests. No unrelated refactor or secrets. Do not equate passing disabled SQL smoke or EF InMemory tests with real relational evidence.

## Validation

- dotnet restore; dotnet build --no-restore.
- Focused FleetMissionCargoTests and dotnet test --no-build, with process-scoped VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false; report fresh counts and build warnings/errors.
- Inspect scripts/run-integration-tests.ps1. If still a placeholder, do not run it as integration evidence; report exactly: No integration tests configured.
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1.
- git diff --stat; git diff --name-only; git status; corresponding staged diff checks and git diff --check. Verify all remaining E-CZ task files unchanged/pending.

## Commit and push

After validation succeeds, record the evidence below and move this exact task to done. Stage only the allowlist, commit with feat(fleets): add fleet mission cargo ledger, and push the configured feature branch. Verify clean status/upstream. Stop after D; no PR or merge.

## Change Budget

This is one cohesive cargo ledger responsibility. File/line/commit counts are review signals, not hard limits; keep related EF/tests/specification/lifecycle together under current AGENTS.md. Split only for separable responsibilities or concrete review/regression risk, not arbitrary counts. No follow-up task is needed for the named deferred work already planned.

## Completion evidence

Completed 2026-10-08 on codex/block-51a-51cz-fleet-movement-mission-engine-v1, starting from verified clean HEAD/upstream 4f989a72ea4872a2baa5f206de7ca60c6c11c306 after fast-forward synchronization. A/B/C predecessor files were already done. The original metadata identity/dependencies/order/team/title/roadmap were preserved; the body was expanded before implementation.

- Implemented exactly the four allowlisted source/test paths and this task lifecycle. No DbContext, OrbitalTransfer, stockpile, service, UI, migration, SQL artifact, seed or other task edits. No new tasks or PR. Current numeric budget policy keeps this cohesive model/configuration/test/specification change together.
- Domain: one row per resource, positive loaded values, zero no-op in every phase, exact decimal(18,4), checked merge, private fields/read-only collection, versioned delivery/return and pure availability/Gas-fuel/capacity validation. Accounting uses delta plus expected prior counter; true applies a new effect, false acknowledges an interval already represented, stale/conflicting requests fail. StateVersion overflow is checked before mutation.
- Conservation demonstrated for all four resources, partial destination acceptance, retained remainder, normal return and recall; no balance mutation. Explicit fault injection exercises CLR decimal overflow; public input tests enforce the narrower storage boundary.
- EF composite PK, required parent FK, cascade, field access and precision verified through production model discovery. Four-resource and fractional delivery/return reload tests pass; RemainingAmount is ignored. Existing B/C lifecycle and composition tests pass unchanged. Parent StateVersion remains a concurrency token; relational CHECKs remain BH and schema generation remains BI/BJ/BK.
- dotnet restore: passed; all projects up to date.
- dotnet build --no-restore: passed, 0 warnings, 0 errors.
- dotnet test --no-build --filter FullyQualifiedName~FleetMissionCargoTests: 58 passed, 0 failed, 0 skipped.
- dotnet test --no-build: 986 passed, 0 failed, 0 skipped (928 predecessor baseline plus 58 new cases).
- Tests used process-scoped VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false. The SQL smoke guard returned early; no live SQL Server was exercised. InMemory evidence covers mapping/materialization/tracked cascade only, not relational races, constraints or transactions.
- Inspected scripts/run-integration-tests.ps1: still the unadapted placeholder. No integration tests configured.
- powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1: Repository secret scan passed. Changed source/test/task text also reviewed; no secrets added.
- Reviewed unstaged/staged git diff --stat, --name-only, --check and status against the exact allowlist. Staged total: 876 insertions and 96 deletions across six Git paths (four implementation/test files plus task deletion/addition); the enriched task move is one lifecycle responsibility. No unstaged edits remain. Git's LF-to-CRLF notices are checkout normalization notices, not build warnings.
- All 100 original TASK-51E through TASK-51CZ files remain present, unchanged and pending, verified against the starting Git tree. Only D transitions to done. Block 52 was not created. No migrations generated/applied, SQL, seeds or manual browser/database QA were run.
- Final action: commit this completed task cycle and push the configured feature branch, verify clean/upstream state, then stop without starting any subsequent task. Final commit SHA is reported in the execution response (not embedded self-referentially in its own commit).

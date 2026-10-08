# TASK-51H

---
id: TASK-51H
title: Travel time calculation
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 8
dependencies: ["TASK-51F-fleet-speed-calculation.md", "TASK-51G-galactic-distance-calculation.md"]
---

## Goal

Provide authoritative FleetMission v1 travel duration and UTC arrival by composing the independent speed and distance calculations delivered by F and G. Future preview and launch orchestration must share one pure deterministic backend balance rule, without queries or frontend duplication. Actual composition speed must affect travel; every valid journey stays positive and cannot arrive early through fractional-second truncation.

## Problem statement / current behavior

OrbitalTravelEstimator assigns every distinct planet pair abstract distance 1 and one hour per legacy distance unit. OrbitalTravelEstimateService combines that compatibility estimate with route, fuel and affordability services. Those paths remain unchanged and are not the authority for new mission timing.

TASK-51F returns FleetSpeedResult.EffectiveSpeed from the slowest participating catalog ship and a supplied decimal propulsion modifier. Default modifier is 1.0m; no authoritative Propulsion-level formula exists. TASK-51G returns positive long GalacticDistanceResult.DistanceUnits and SameSystem/IntraGalaxy scope from spatial snapshots. H consumes these results exactly once; it must not recalculate either input or apply propulsion again.

## Desired behavior

The caller supplies typed F/G results and an explicit DateTime UTC departure. H validates its public input boundary, calculates whole-second positive duration/arrival and returns immutable evidence. Equal inputs produce value-equal results, independent of locale, clock, random state, database and frontend. Invalid or unrepresentable inputs fail deliberately before a result; no external state is mutated.

## Context and architectural decisions

- Application already references Domain. F's result is Application.Fleets; G's result is Domain.Fleets. Both H calculator and result belong in Application.Fleets. This explicitly reconciles the old provisional Domain calculator path without introducing Domain -> Application or project-reference changes.
- Reuse FleetSpeedResult and GalacticDistanceResult directly. No DbContext, DI, catalog/planet/research lookup or FleetMission mutation is needed.
- FleetMission persists DateTime UTC lifecycle deadlines. Preserve this convention, without DateTimeOffset conversion or silent time normalization.
- A requires launch snapshots to survive later research changes. H returns calculation evidence and persists nothing; O later stores the schedule/required metadata. Finishing research after launch does not change an active mission's deadline.
- I owns fuel; N owns authorization, ownership, visibility, range and complete launch eligibility; M/O own request/server-clock authority and launch persistence. Preview is advisory and O later recalculates/rechecks at launch. Browser time never becomes gameplay authority.
- Z later owns recall's elapsed-travel rule and retains positive timing semantics. H implements no recall, arrival, return or worker behavior.

## Scope

### In scope

Pure F + G duration/arrival calculation; explicit v1 constants; public guards; checked decimal arithmetic; whole-second ceiling/minimum; safe ticks/UTC arrival; immutable result; focused unit/composition tests; one architecture policy section; this expanded task and lifecycle evidence.

### Out of scope

Ship/research/planet queries, new balance catalogs/research formulas, mission-speed selection, distance recalculation, fuel, cargo, range, slots, affordability, destination ownership, launch validation/creation, active-mission recalculation, FleetMission mutation, persistence, EF/DbSet/configuration, migrations, SQL/seeds, API/frontend/countdown, recall/arrival/return workers, legacy cutover, other tasks, Block 52, PRs and merges.

## Detailed implementation requirements

### Policy constants and exact formula

Expose named policy constants on FleetTravelTimeCalculator:

- ReferenceSpeed = 100m.
- BaseSecondsPerDistanceUnitAtReferenceSpeed = 3600m, or 60 minutes.
- MinimumTravelDurationSeconds = 1L.

Use checked decimal arithmetic throughout the gameplay formula:

```text
RawDurationSeconds = DistanceUnits * 3600m * 100m / EffectiveSpeed
RoundedDurationSeconds = max(1, decimal.Ceiling(RawDurationSeconds))
Duration = TimeSpan.FromTicks(checked((long)RoundedDurationSeconds * TimeSpan.TicksPerSecond))
ArrivalAtUtc = DepartureAtUtc + Duration
```

Never use float/double or TimeSpan.FromSeconds(double). Scope is preserved in output and does not apply a second multiplier; G already incorporates route geometry. Reference speed 100 reproduces one hour/unit for Escort and lets actual composition speed determine movement:

| Ship | Effective speed | Distance 1 | Distance 2 | Distance 6 |
|---|---:|---|---|---|
| ScoutCraft | 120 | 50 minutes | 1h40m | 5h |
| EscortCraft | 100 | 1 hour | 2h | 6h |
| CargoCraft | 80 | 1h15m | 2h30m | 7h30m |
| ColonyCraft | 60 | 1h40m | 3h20m | 10h |

Whole seconds always round upward, never to nearest or by truncation: 3600 stays 3600; 3600.0001 becomes 3601. Distance3 / effective 88 yields 12272.7272... seconds -> 12273 (3h24m33s). Every valid sub-second duration becomes one second, including decimal.MaxValue speed; no instant teleport. This is the new mission policy, not a legacy estimator modification.

### Input, UTC and exception contract

Public API: FleetTravelTimeCalculator.Calculate(GalacticDistanceResult distance, FleetSpeedResult fleetSpeed, DateTime departureAtUtc).

- Null FleetSpeedResult throws ArgumentNullException.
- DistanceUnits <= 0, undefined GalacticDistanceScope and EffectiveSpeed <= 0 throw ArgumentOutOfRangeException. Both predecessor results can be manually constructed; validate the fields H consumes.
- Use EffectiveSpeed directly. Do not reinterpret BaseSpeed, PropulsionModifier or LimitingAssetType, inspect composition/catalog or apply modifiers twice.
- departureAtUtc.Kind must be Utc. Local/Unspecified throw ArgumentException and are never silently normalized. Preserve fractional departure ticks; duration itself is integral seconds.
- Do not call DateTime.UtcNow/Now, a clock provider, database or Random. Future orchestration supplies authoritative server UTC.
- Before integer conversion/tick multiplication, rounded seconds must fit long.MaxValue / TimeSpan.TicksPerSecond. Decimal arithmetic or TimeSpan capacity overflow throws OverflowException with no clamp, saturation or wrap.
- Compare duration ticks with DateTime.MaxValue.Ticks - departureAtUtc.Ticks before adding. Excess throws OverflowException; exact maximum arrival is valid. Successful arrival remains Utc and strictly later than departure.
- Full long distance * 3600m * 100m is at most approximately 3.32e24, below decimal.MaxValue; multiplication overflow is impossible here. Extremely small positive speed can overflow division. Test that achievable path, not impossible multiplication coverage.

### Immutable result, research and snapshot semantics

Use a sealed positional record matching F's Application style:

```text
FleetTravelTimeResult(
    long DistanceUnits,
    GalacticDistanceScope DistanceScope,
    decimal EffectiveSpeed,
    DateTime DepartureAtUtc,
    TimeSpan Duration,
    DateTime ArrivalAtUtc)
```

Keep immutable init-only value semantics and no mutable setters, fuel, cargo, research level, eligibility or UI strings. Output snapshots the supplied speed and schedule; later research affects future calculations only. O owns persisted deadlines and metadata, not H.

V1 has no 25/50/75/100% throttle or manual mission-speed selector. There is no product rule for its fuel/recall/request implications. Use F's EffectiveSpeed directly. A future authoritative research provider may supply a modifier to F; H invents no formula, queries no research and does not apply Propulsion again.

## Implementation steps

1. Verify clean working tree, fetch the existing feature branch and safely select its local tracking checkout. Confirm HEAD 00a5f7b85c9c43cf535d1bb6b5d93870ca2c924c; pull configured upstream with --ff-only. Verify exact A-G done and all H-CZ pending independently of lexical order.
2. Read governance/template/current-state/architecture/discovery/DI, F/G contracts/source, mission UTC tests, legacy source/tests and I/M/N/O/Z/AA for boundary awareness. Move only H to in-progress and expand this contract before implementation.
3. Add pure Application calculator/result with the exact constants, guards, checked decimal, ceiling and tick/date range checks. Keep F/G, mission, legacy and dependency wiring unchanged.
4. Add exhaustive timing, invalid input, precision, overflow, determinism and real F/G/H composition tests. Add one finalized-H architecture section next to G, preserving historical audit evidence.
5. Run fresh restore/build, focused tests, full suite and repository secret scan. Inspect integration configuration, state SQL smoke limits and review all unstaged/staged changed paths.
6. After validation, append actual evidence, move only H to done, stage scoped files, commit and push configured feature branch. Verify clean synchronized Git and remaining task states, then stop.

## Files to read first

- AGENTS.md; ai/task-template.md; ai/current-state.md; ai/architecture-index.md; ai/orchestrator/component-discovery.md; ai/orchestrator/di-analysis.md; docs/dev/fleet-mission-engine-v1.md: governance, authority, layers and cutover.
- ai/tasks/done/TASK-51F-fleet-speed-calculation.md; ai/tasks/done/TASK-51G-galactic-distance-calculation.md: exact prerequisite contracts and numeric/snapshot policy.
- src/VoidEmpires.Application/Fleets/FleetSpeedCalculator.cs; FleetSpeedResult.cs; src/VoidEmpires.Domain/Fleets/GalacticDistanceCalculator.cs: actual typed inputs and dependency direction.
- src/VoidEmpires.Domain/Fleets/FleetMission.cs; tests/VoidEmpires.Tests/FleetMissionTests.cs: UTC/lifecycle invariants.
- src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs; OrbitalTravelEstimate.cs; src/VoidEmpires.Infrastructure/Fleets/OrbitalTravelEstimateService.cs; tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs; OrbitalTravelEstimateServiceTests.cs: unchanged compatibility behavior.
- Pending TASK-51I-fuel-consumption-calculation.md; TASK-51M-fleet-launch-request-contract.md; TASK-51N-fleet-launch-validation.md; TASK-51O-fleet-launch-transaction.md; TASK-51Z-fleet-recall-domain-rules.md; TASK-51AA-fleet-recall-service.md: deferred boundaries, never implementation targets here.

## Expected files to modify

- src/VoidEmpires.Application/Fleets/FleetTravelTimeCalculator.cs (new): pure timing policy/guards; reconciled from old provisional Domain path.
- src/VoidEmpires.Application/Fleets/FleetTravelTimeResult.cs (new): immutable typed timing evidence.
- tests/VoidEmpires.Tests/FleetTravelTimeCalculatorTests.cs (new): formula, composition, precision and boundary coverage.
- docs/dev/fleet-mission-engine-v1.md: one finalized-H section adjacent to G.
- This exact expanded task and pending -> in-progress -> done lifecycle. No other task/source updates.

## Edge cases and failure modes

Reject default/zero/negative distance, unknown scopes, null/nonpositive speed and non-UTC departure. Accept very high speed with one-second minimum. Reject tiny speed causing decimal or TimeSpan overflow. A huge manual distance may fit decimal but exceed TimeSpan; a representable duration may still exceed DateTime's smaller range. Preserve sub-second departure ticks and day rollover. Exact maximum UTC arrival is allowed; one tick beyond it is not. Inputs remain unchanged on success and failure; no mission/persistence state is involved.

## Test plan

- Exact policy constants and all Scout/Escort/Cargo/Colony examples at distances 1/2/6 through real F/catalog inputs.
- Reference speed 100 gives exactly 3600 seconds/unit. Fractional 3/88 gives 12273; fractions below half a second and immediately above a whole-second boundary distinguish ceiling from nearest/truncation.
- UTC departure 2026-10-08 12:00 + Cargo distance 1 -> 13:15. Long routes cross dates; Utc Kind and fractional departure ticks are retained.
- Equal repeated results, all six output fields, unchanged inputs on success/failure and equal duration across scopes for equal distance/speed.
- Default/zero/negative distance, unknown scopes, null/zero/negative speed, Local/Unspecified departure fail deliberately.
- Sub-second/decimal.MaxValue speed yields one second. Tiny speed and huge distance test separate TimeSpan/decimal overflow; exact/overflowing maximum arrival tests date bounds separately.
- Real GalacticPlanetLocation snapshots same-system slots 1->8 -> G distance 2, real Cargo composition -> F effective 80 -> H duration 2h30m. Mixed-fleet and supplied modifier tests prove F's effective result is consumed without a second application.
- Source/scope review confirms no clock, float/double, DB, research/catalog queries or FleetMission mutation. Full B-G and unchanged legacy estimator/service regressions pass.

## Acceptance criteria

The exact decimal formula/constants, ceiling and minimum produce all documented examples. F/G compose through typed inputs in Application without dependency inversion. Guards and deliberate overflow match this contract. Every successful arrival is later than UTC departure and retains Kind. Result has exactly six immutable fields without adjacent behavior. Architecture/task document finalized timing and deferred ownership. Fresh validation passes and only H reaches done; no persistence, migrations, SQL, seed, PR, merge, Block 52 or next-task execution.

## Constraints

Preserve backend authority, layered .NET 8 conventions, ordinary provider-independent tests, legacy compatibility and SQL Server support. No secrets, unrelated refactors or automatic database changes. File/line totals are review signals under current AGENTS.md, never grounds to split this cohesive calculation or create new tasks.

## Validation

Run dotnet restore; dotnet build --no-restore; dotnet test --no-build --filter FullyQualifiedName~FleetTravelTimeCalculatorTests; dotnet test --no-build. Explicitly disable optional SQL smoke in test processes and report no live SQL Server coverage; its early return counts as passing in xUnit. Report actual fresh totals rather than historical 1111.

Inspect scripts/run-integration-tests.ps1. If unadapted, report exactly: No integration tests configured. Run scripts/check-repo-secret-scan.ps1 with PowerShell. Review git diff --check/--stat/--name-only, git status and staged equivalents; compare paths to the allowlist and verify remaining pending contracts against starting HEAD.

## Commit and push

After required validation append evidence, set done and move this exact task to done. Stage only calculator/result/tests/doc/H lifecycle. Commit feat(fleets): add authoritative travel time calculation, push configured feature branch and verify clean synchronized Git. Stop after H; no PR/merge, I/AA/other task or Block 52.

## Change Budget

One cohesive timing rule includes immutable result, meaningful tests, finalized documentation and lifecycle evidence. Review counts honestly and justify all paths. Split only for separable responsibilities or concrete regression risk; no arbitrary numeric splits/follow-ups.

## Completion evidence

- Completed 2026-10-08 on codex/block-51a-51cz-fleet-movement-mission-engine-v1 from verified starting HEAD 00a5f7b85c9c43cf535d1bb6b5d93870ca2c924c. The initially clean work branch was preserved. Its checkout tracked only main, so an explicit feature-branch fetch/refspec and local tracking checkout were required; --ff-only pull then confirmed no advancement. Exact A-G predecessor contracts were done and all 97 Block 51 contracts were pending before implementation.
- Expanded this exact task before source implementation, preserving id/title/roadmap/type/team/supporting teams/priority/execution_order=8 and both exact dependencies. Only H follows pending -> in-progress -> done; the detailed body is historical technical documentation, not a short checklist.
- Added pure Application FleetTravelTimeCalculator and sealed immutable FleetTravelTimeResult. Correct Application -> Domain direction consumes F's FleetSpeedResult and G's GalacticDistanceResult without new references, DI, clocks, DB, research or catalog queries. Result contains exactly DistanceUnits, DistanceScope, EffectiveSpeed, DepartureAtUtc, Duration and ArrivalAtUtc.
- Constants are ReferenceSpeed=100m, BaseSecondsPerDistanceUnitAtReferenceSpeed=3600m (60 minutes), MinimumTravelDurationSeconds=1L. Formula is max(1, ceil(distance * 3600m * 100m / EffectiveSpeed)) seconds. Decimal arithmetic is checked, ceiling is always upward, and exact integer ticks avoid float/double. All four ship examples at distances 1/2/6 pass: one-unit Scout 50m, Escort 60m, Cargo 75m, Colony 100m; distance 3/speed 88 produces 12273 seconds.
- Positive distance/effective speed, defined scope, non-null F result and UTC departure are validated. Local/Unspecified timestamps are rejected without normalization. Successful arrival is later than departure and retains UTC Kind plus fractional departure ticks. Decimal division, TimeSpan capacity and DateTime arrival overflow deliberately throw OverflowException without saturation; exact maximum arrival is accepted and one tick beyond is rejected. Full long distance multiplication remains below decimal capacity, so the arithmetic-overflow test honestly exercises tiny-speed division.
- No V1 mission-speed selector or invented Propulsion rule. EffectiveSpeed is consumed directly without modifier reapplication. Immutable preview/launch results stay unchanged after later calculations/research inputs; O owns future persistence of launch snapshots, I fuel, N eligibility and Z recall rules.
- Updated only the focused finalized-H architecture section adjacent to finalized G. Legacy OrbitalTravelEstimator and its services/tests remain unchanged. Full-run evidence verifies all 14 OrbitalTravelEstimatorTests and all 9 OrbitalTravelEstimateServiceTests passed.
- Fresh dotnet restore VoidEmpires.sln passed. Fresh dotnet build VoidEmpires.sln --no-restore -m:4 passed: 0 warnings, 0 errors. Official local .NET SDK 8.0.425/runtime 8.0.31 and PowerShell 7.6.6 were installed outside the checkout with Microsoft SHA512/PowerShell SHA256 verification and TLS preserved.
- Fresh focused dotnet test VoidEmpires.sln --no-build --filter FullyQualifiedName~FleetTravelTimeCalculatorTests passed: 53 passed, 0 failed, 0 skipped. Real G same-system slots 1->8 -> distance 2 and mixed Scout/Cargo F -> effective 80 -> H duration 9000 seconds/2h30m passed; supplied modifier 1.25 -> effective 100 -> 7200 seconds also passed without reapplication.
- Initial full regression run executed all 1164 tests with 1118 passed/46 failed/0 skipped; every failure was the cloud container's 128-instance inotify limit during test-host startup, not a timing assertion. Supported DOTNET_USE_POLLING_FILE_WATCHER=true resolved the environment issue without changing tests or assertions. Fresh complete rerun passed: 1164 passed, 0 failed, 0 skipped (historical 1111 plus 53 new cases). TRX files outside the checkout confirm nonzero execution, counters and the composed/legacy test outcomes.
- VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false was set only for test commands. SQL smoke's disabled early-return case is counted in the passing suite total; no live SQL Server coverage. scripts/run-integration-tests.ps1 remains the inspected unadapted placeholder. No integration tests configured.
- Repository secret scan passed with pwsh -NoProfile -File scripts/check-repo-secret-scan.ps1. Independent read-only implementation/contract review found no actionable defects.
- Reusable cloud install_script and start_skill were saved for tool installation, shell activation, the polling-watcher correction and provider-independent test/scan commands. The complete installation script was exercised again successfully with both official checksums and up-to-date restore. Draft saving does not publish the environment or prove restoration in a new task.
- Persistence/schema/EF/DbSet changes: NO. Migrations: NO. SQL executed: NO. Seed executed: NO. Frontend/API/worker changes: NO. Browser/manual QA: NOT EXECUTED.
- Unstaged and staged git diff --check/--stat/--name-only and status reviews passed. Final staged scope: 5 logical files / 6 Git paths (pending deletion plus expanded done file), 603 insertions and 94 deletions: 53 production lines, 336 test lines, 24 architecture lines and the 190-line completed task replacing its 94-line predecessor. Every path matches H's allowlist; this cohesive responsibility warrants no count-based split or follow-up task.
- All 96 remaining Block 51 pending task files were verified byte-for-byte unchanged against starting HEAD, with status pending preserved. TASK-51I through TASK-51CZ remain pending; Block 52 was not created. No PR, merge or next-task execution is part of this cycle. Commit/push and clean synchronized Git are verified after final staging.

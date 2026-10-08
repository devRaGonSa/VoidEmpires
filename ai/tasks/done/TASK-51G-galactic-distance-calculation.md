# TASK-51G

---
id: TASK-51G
title: Galactic distance calculation
status: done
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 7
dependencies: ["TASK-51A-fleet-domain-and-existing-system-audit.md"]
---

## Goal and product rationale

Introduce the authoritative FleetMission v1 galactic-distance rule using existing persisted galaxy/system coordinates and planet orbital slots. Return deterministic positive integer GalacticDistanceUnits for valid same-system or intra-galaxy travel. The scale makes orbital movement and nearby systems accessible with current ship ranges while leaving some distant generated routes out of range. Distance remains a pure spatial fact for later time, fuel and launch calculations, without activating the new movement engine.

## Current placeholder behavior

OrbitalTravelEstimator.EstimateAbstractDistanceUnits currently returns 1 for any two distinct nonempty planet IDs, regardless of systems, galaxies, XYZ or slots. Its fixed-duration/cost path still serves Block 54 OrbitalTransfer and must remain unchanged. TASK-51A requires the new FleetMission authority to replace placeholders only through later safe integration/cutover. G provides the new pure calculation; it does not reroute legacy services/endpoints.

## Existing galaxy-coordinate model

Planet supplies Id, SolarSystemId and positive OrbitalSlot. SolarSystem supplies Id, GalaxyId and integer CoordinateX/Y/Z, exposed as the readonly GalaxyCoordinates record struct. Galaxy.AddSolarSystem rejects duplicate coordinates within a galaxy; SolarSystem.AddPlanet rejects repeated orbital slots. The calculator uses snapshots of these existing facts, without modifying persistence or requiring entity instances.

Strategic-map contracts expose system coordinateX/Y/Z separately from orbitalSlot and visual orbitRadius/orbitAngleDegrees/scale. Visual layout, screen positions and visibility-sanitized DTOs are not authoritative distance inputs. Future backend callers load Planet plus SolarSystem and construct validated snapshots after their own authorization checks.

## Existing generation ranges

GalaxyGenerator.GenerateUniqueCoordinates uses inclusive X/Y ranges -10000..10000 and Z -1000..1000, with a HashSet ensuring coordinate uniqueness. Planets are created sequentially at slots 1..planetCount; planetCount is bounded by the generation request's MinPlanetsPerSystem/MaxPlanetsPerSystem. Neither the domain nor generator defines a universal 12-slot cap (Romanize has a fallback beyond XII). G supports the entire int coordinate domain and every positive int slot, not just current generation examples.

Generated systems therefore have theoretical maximum Manhattan separation 20000 + 20000 + 2000 = 42000. Valid generated inter-system routes lie within 2..6 distance units under the policy below; this is a scale/bound, not a promise that each generated galaxy contains every possible distance.

## Exact v1 units and formulas

GalacticDistanceUnits are abstract integer backend gameplay distance, not kilometres, astronomical units, light years, screen pixels or frontend map distance.

### Same solar system

When SolarSystemId matches, both GalaxyId and SystemCoordinates must match. Distinct planet IDs must also have distinct slots. Let slotDelta = abs((long)origin.OrbitalSlot - destination.OrbitalSlot), requiring slotDelta > 0.

- OrbitalSlotsPerDistanceUnit = 6.
- DistanceUnits = ceil(slotDelta / 6), using integer quotient/remainder arithmetic.
- Deltas 1, 5, 6 map to 1; 7, 11, 12 map to 2; 13 maps to 3.
- Shared system coordinates do not affect this branch beyond validating snapshot consistency.

### Different systems in the same galaxy

Let coordinateSpan = abs((long)origin.X - destination.X) + abs((long)origin.Y - destination.Y) + abs((long)origin.Z - destination.Z). Require coordinateSpan > 0: different systems in one galaxy cannot share coordinates.

- SystemCoordinateUnitsPerDistanceUnit = 10000.
- InterSystemBaseDistanceUnits = 1.
- DistanceUnits = 1 + ceil(coordinateSpan / 10000).
- Spans 1/9999/10000 -> 2; 10001/20000 -> 3; 20001/30000 -> 4; 30001/40000 -> 5; 40001/42000 -> 6.
- OrbitalSlot remains validated as positive but is deliberately omitted from the inter-system formula. The base crossing unit includes orbital ingress/egress at v1 scale; raw slots must not distort strategic movement. No route/pathfinding model is introduced.

### Cross-galaxy policy

For different system IDs with different GalaxyId, throw NotSupportedException with an explicit v1 unsupported-route message. Do not compare unrelated galaxy coordinates, return zero or invent a huge route. GalaxyId remains in the input for later topology extensions. A shared system ID with conflicting galaxies is instead inconsistent snapshot data and fails validation before the ordinary cross-galaxy branch.

## Architectural authority and input/result contracts

Place GalacticDistanceCalculator and focused value types in src/VoidEmpires.Domain/Fleets/GalacticDistanceCalculator.cs. Domain can reference Domain.Galaxy.GalaxyCoordinates directly; no Application/Infrastructure dependency, DI or database access is needed. This preserves A's single-authority plan, E's independent catalog metadata and F's independent Application speed calculator.

- GalacticPlanetLocation: public readonly record struct with Guid PlanetId, Guid GalaxyId, Guid SolarSystemId, GalaxyCoordinates SystemCoordinates, int OrbitalSlot. It contains only spatial facts; construction/loading belongs to future callers.
- GalacticDistanceScope: SameSystem and IntraGalaxy enum values.
- GalacticDistanceResult: public readonly record struct with long DistanceUnits and GalacticDistanceScope Scope. The calculator guarantees positive distance on success. No speculative path, fuel, time or range fields; local raw deltas are not duplicated in the result because H/I/N need units and scope only.
- GalacticDistanceCalculator.Calculate(origin, destination) is pure, static and symmetric. The three named policy constants are public long constants with the exact values above.

## Validation and consistency rules

Validate both snapshots first: PlanetId, GalaxyId and SolarSystemId must be nonempty (ArgumentException); OrbitalSlot must be positive (ArgumentOutOfRangeException). Default snapshots are invalid. Negative, zero and positive XYZ are all valid.

Then reject equal PlanetId (ArgumentException), regardless of otherwise valid location differences. Special same-planet actions belong to future mission policy, not fake travel distance.

For matching SolarSystemId require matching GalaxyId/coordinates and distinct slots; reject all inconsistencies with ArgumentException. For distinct systems reject different galaxies explicitly; within one galaxy reject zero coordinate span with ArgumentException. No database lookup or authorization assertion is implied by validating caller-supplied snapshots.

## Determinism, symmetry and overflow strategy

Use only integer arithmetic and immutable value inputs. No Random, clocks, float/double, database ordering or frontend geometry. The same snapshots always return the same value, and reversing endpoints preserves DistanceUnits and Scope.

Promote operands to long before each coordinate or slot subtraction, then Math.Abs on long. Maximum absolute axis delta is 4294967295; maximum XYZ sum is 12884901885, safely within Int64. Use checked subtraction/summation and checked final addition. Integer ceiling division uses numerator/divisor + (numerator%divisor == 0 ? 0 : 1), avoiding numerator+divisor-1. Divisors are positive constants and both branch numerators are validated positive.

Full int-range endpoints yield intra-galaxy distance 1288492. Positive slots 1 and int.MaxValue yield same-system distance 357913941. No artificial upper bound or clamping is introduced; those large routes remain geometry results even if future ship range validation rejects them.

## Relationship to OperatingRange and later tasks

Catalog OperatingRange remains Scout3, Cargo2, Escort4, Colony5. G does not read the catalog or enforce range. N will compare this abstract distance with the authoritative ship/fleet range policy; nearby systems cost at least 2 and the generated extent can cost 6, so current 2..5 values remain meaningful.

F owns fleet speed and is not called by G. H combines distance with effective speed to calculate duration/arrival. I combines distance/composition/catalog fuel rates to calculate Gas. J owns cargo capacity. N owns full eligibility, ownership, visibility and range. None of those responsibilities is implemented here.

## Legacy compatibility and documentation

Keep OrbitalTravelEstimator and its tests/services untouched. Legacy distinct-planet distance remains 1; only future FleetMission integrations use GalacticDistanceCalculator. No Block 54 endpoint switches or partial activation.

Add one focused finalized-distance section to docs/dev/fleet-mission-engine-v1.md after the audit's balance discussion. Record formulas/constants, generator scale, snapshot validation, integer safety, cross-galaxy rejection, omitted inter-system slots and G/F/H/I/N boundaries. Retain unrelated audit/history sections intact.

## In scope

- Immutable spatial snapshot, scope/result types and pure v1 distance calculator.
- Same-system and intra-galaxy formulas, consistency guards and cross-galaxy rejection.
- Long arithmetic, integer ceiling division, symmetry and full int-domain support.
- Comprehensive domain tests, focused architecture documentation and this detailed lifecycle record.

## Out of scope

Database loading, visibility, authorization, range eligibility, speed, duration/arrival, fuel, cargo, research, route graphs/pathfinding, wormholes/gates, launch/mission mutations, frontend/API/worker/DI, EF/schema/migrations/SQL/seeds, legacy replacement, other task execution, Block 52, PR or merge.

## Detailed implementation plan

1. Verify clean configured branch at dbfe3ed017ec281d402c9132811d5c8d4ebff414 after fetch/pull, A-F done and exact G pending with 98 total Block 51 tasks. Move only G to in-progress and expand this contract before coding.
2. Read current domain/generation/map sources, A/E/F contracts and future H/I/J/N boundaries. Confirm positive unique slots, unique coordinates, generation ranges and pure Domain placement; reuse GalaxyCoordinates.
3. Implement snapshot/result types and constants in the focused calculator file. Validate input identities/slots and pair consistency before choosing the formula; widen arithmetic before subtraction and return a positive long result.
4. Add threshold/sign/axis/symmetry, inconsistent input, cross-galaxy, extreme integer/slot, determinism and immutability tests. Include a compatibility assertion that new long-distance geometry does not change legacy fixed-distance behavior.
5. Document the finalized policy in the existing architecture audit without unrelated rewrites. Run restore/build, focused tests, full regression suite, integration-hook inspection and secret scan; review all diffs and remaining task states.
6. Record completion evidence, move only G to done, stage the allowlist, commit/push configured branch, verify clean upstream and stop before H or any other task.

## Files to read first

- AGENTS.md; ai/task-template.md; ai/current-state.md; ai/architecture-index.md; ai/orchestrator/component-discovery.md; ai/orchestrator/di-analysis.md; docs/dev/fleet-mission-engine-v1.md: workflow, authoring, existing architecture and audit.
- Exact completed TASK-51A-fleet-domain-and-existing-system-audit.md; TASK-51E-ship-capability-metadata.md; TASK-51F-fleet-speed-calculation.md: authority, ranges and independent speed calculation.
- src/VoidEmpires.Domain/Galaxy/GalaxyCoordinates.cs; Galaxy.cs; SolarSystem.cs; Planet.cs; tests/VoidEmpires.Tests/GalaxyDomainTests.cs: immutable coordinates and uniqueness/slot invariants.
- src/VoidEmpires.Infrastructure/GalaxyGeneration/GalaxyGenerator.cs; src/VoidEmpires.Application/Galaxy/GenerateGalaxyRequest.cs: generation ranges and request-bounded slots.
- src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs; tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs: compatibility-only placeholder and unchanged regressions.
- docs/dev/strategic-map-api-contract.md and relevant coordinate projections: distinguish persisted XYZ/slot from rendered orbital layout and visibility masks.
- Pending TASK-51H-travel-time-calculation.md; TASK-51I-fuel-consumption-calculation.md; TASK-51J-fleet-cargo-capacity-calculation.md; TASK-51N-fleet-launch-validation.md: downstream ownership only.

## Expected files to modify

- src/VoidEmpires.Domain/Fleets/GalacticDistanceCalculator.cs (new): pure calculation plus focused snapshot/scope/result types.
- tests/VoidEmpires.Tests/GalacticDistanceCalculatorTests.cs (new): deterministic formulas, validation and compatibility coverage.
- docs/dev/fleet-mission-engine-v1.md: one new finalized-v1-distance section.
- This exact task's expanded body and pending -> in-progress -> done lifecycle. No other task or source updates.

## Edge cases and test plan

- Same-system slot deltas 1,5,6,7,11,12,13 and both endpoint directions; include shared nonzero/extreme coordinates, which cannot affect slot-only distance.
- Inter-system spans 1,9999,10000,10001,20000,20001,30000,30001,40000,40001; assert exact units, scope and symmetry.
- XYZ Manhattan example (100,-200,300) -> (-400,500,-600) has span2100 and distance2. Also use multi-axis spans that cross thresholds so ignoring Y/Z or using Euclidean/max-axis metrics cannot pass.
- Negative-to-negative, negative-to-positive and reversed routes; coordinate translation/axis permutations retain distance. Inter-system orbital slots do not affect distance even at large positive values.
- Empty PlanetId/GalaxyId/SolarSystemId on either endpoint, default snapshots, zero/negative slots, same planet despite other location differences, shared-system conflicting galaxy/coordinates/slot, and distinct-system identical coordinates all fail explicitly without changing inputs.
- Cross-galaxy distinct systems throw NotSupportedException symmetrically; shared-system mismatched galaxy throws validation error instead.
- Opposite int.MinValue/int.MaxValue XYZ returns 1288492 units, with the expected span calculated in Int64 in test evidence. Generated extreme corners span42000 -> distance6. Large positive slots (including 1..int.MaxValue) remain correct and positive.
- Repeated calculations are equal and both input values stay unchanged. No mutable clock/random/DB/input collections exist.
- Legacy OrbitalTravelEstimatorTests remain unchanged and full B-F/repository suite passes; changed-file review confirms no frontend formula, schema or integration activation.

## Acceptance criteria

- The three exact constants and formulas produce positive long distances with correct SameSystem/IntraGalaxy scope, symmetry and no overflow over the full supported int domain.
- Invalid/inconsistent snapshots and cross-galaxy routes fail closed; arbitrary coordinate signs and large positive slots remain valid.
- Input/result types stay immutable and Domain-only, independent of ships, player/civilization state, range, speed, cargo, clocks and databases.
- Architecture documentation and this task specify the selected policy and downstream boundaries; legacy behavior is unchanged.
- Fresh required validation passes; only the allowlist changes and G reaches done. H-CZ remain pending; no Block 52, PR, merge or next-task execution.

## Validation

Run dotnet restore; dotnet build --no-restore; dotnet test --no-build --filter FullyQualifiedName~GalacticDistanceCalculatorTests; dotnet test --no-build. Set VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false only in the test processes and report its counted early return as no live SQL Server coverage.

Inspect scripts/run-integration-tests.ps1. If still the unadapted placeholder, report exactly: No integration tests configured. Run powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1. Review unstaged/staged git diff --check, --stat, --name-only and git status; verify scope and original remaining task states against the starting Git tree. Report fresh actual totals instead of reusing the 1049 baseline.

## Commit and push

After validation succeeds, append evidence and move G to done. Stage only the three implementation/test/doc paths and G lifecycle. Commit feat(fleets): add deterministic galactic distance calculation, push configured feature branch, verify clean/upstream and stop. No H, AA, other task, Block 52, PR or merge.

## Change Budget

One cohesive distance rule includes its immutable contracts, tests, architecture note and task lifecycle. Counts are review signals, not hard limits under current AGENTS.md. No split solely for documentation/tests or duplicate follow-up for already planned integration work.

## Completion evidence

- Completed on 2026-10-08 from verified clean starting HEAD dbfe3ed017ec281d402c9132811d5c8d4ebff414 on codex/block-51a-51cz-fleet-movement-mission-engine-v1. Fetch and configured-upstream fast-forward pull confirmed no remote advancement; A-F were already done.
- Expanded this exact task into the historical implementation contract above, preserving its original header metadata and responsibility. Only its lifecycle status changes from pending through in-progress to done.
- Added pure GalacticDistanceCalculator with immutable GalacticPlanetLocation and GalacticDistanceResult plus SameSystem/IntraGalaxy scope. Exact constants are 6 orbital slots, 10,000 coordinate units and 1 inter-system base unit; invalid and inconsistent snapshots fail closed, and cross-galaxy travel is explicitly unsupported.
- Long promotion precedes subtraction and absolute value; checked arithmetic and quotient/remainder ceiling division cover all int coordinates and every positive int orbital slot. Full three-axis extremes yield 1,288,492 units; maximum slot separation yields 357,913,941 units. Tests confirm symmetry, repeatability, immutable inputs, threshold behavior and axis/sign handling.
- Added the finalized policy section to docs/dev/fleet-mission-engine-v1.md, including generator distance range 2..6, omitted inter-system slot costs, future OperatingRange eligibility in N, and separate speed/duration/fuel/cargo responsibilities. No caller, catalog, frontend, dependency wiring or legacy movement behavior changed; OrbitalTravelEstimator still returns fixed distance 1.
- Independent read-only review found no functional defects; two documentation wording/formatting issues were corrected before completion.
- Fresh dotnet restore succeeded. Fresh dotnet build --no-restore succeeded with 0 warnings and 0 errors.
- Fresh focused dotnet test --no-build --filter FullyQualifiedName~GalacticDistanceCalculatorTests: 62 passed, 0 failed, 0 skipped.
- Fresh full dotnet test --no-build: 1111 passed, 0 failed, 0 skipped (1049 pre-existing plus 62 new cases). Existing B-F and legacy regression tests remain passing and unchanged.
- VOIDEMPIRES_SQLSERVER_SMOKE_ENABLED=false was set for both test processes. The optional SQL Server smoke test's disabled early return is included in the reported passing total; no live SQL Server coverage is claimed.
- Inspected scripts/run-integration-tests.ps1: it remains the unadapted placeholder. No integration tests configured.
- Repository secret scan passed via powershell -NoProfile -ExecutionPolicy Bypass -File scripts/check-repo-secret-scan.ps1.
- Persistence changes: NO. EF configuration/DbSet changes: NO. Migrations: NO. SQL execution: NO. Seed execution: NO.
- Unstaged and staged git diff --check, --stat, --name-only and status review passed. Staged scope: 4 logical files / 5 Git paths (task deletion plus expanded done file), 544 insertions and 95 deletions: 72 production lines, 263 test lines, 22 architecture-note lines and the 187-line completed task replacing its 95-line pending predecessor. Every path matches the expected G scope; no follow-up split is justified solely by counts.
- All 97 original remaining Block 51 pending files were verified unchanged against the starting Git tree, with pending status preserved. TASK-51H through TASK-51CZ remain pending; no Block 52 task, PR, merge or next-task execution is part of this cycle.

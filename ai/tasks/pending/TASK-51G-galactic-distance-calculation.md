# TASK-51G

---
id: TASK-51G
title: Galactic distance calculation
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 7
dependencies: ["TASK-51A-fleet-domain-and-existing-system-audit.md"]
---

## Goal

Create deterministic distance calculation between planets.

Use existing:
- galaxy;
- solar-system coordinates;
- orbital slot.

Do not invent arbitrary frontend distances.

Define distance rules clearly.

Support:
- same system;
- different systems in same galaxy;
- future cross-galaxy extensibility.

Tests required.

## Context

Planets expose OrbitalSlot; systems expose GalaxyId and integer CoordinateX/Y/Z. Current travel ignores these.

Prerequisites: `TASK-51A-fleet-domain-and-existing-system-audit.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Document deterministic units/formulas/rounding for same-system slots and different systems; valid distinct planets have positive distance.
2. Make cross-galaxy policy extensible; reject unsupported cross-galaxy routes explicitly until defined rather than inventing coordinates.
3. Keep calculation pure, symmetric and overflow-safe; database lookups stay outside domain and no frontend formula is added.

## Files to read first

- `src/VoidEmpires.Domain/Galaxy/Planet.cs`
- `src/VoidEmpires.Domain/Galaxy/SolarSystem.cs`
- `src/VoidEmpires.Domain/Galaxy/GalaxyCoordinates.cs`
- `src/VoidEmpires.Domain/Fleets/OrbitalTravelEstimator.cs`
- `tests/VoidEmpires.Tests/OrbitalTravelEstimatorTests.cs`

## Expected files to modify

- `src/VoidEmpires.Domain/Fleets/GalacticDistanceCalculator.cs` (new/proposed; reconcile with A audit)
- `tests/VoidEmpires.Tests/GalacticDistanceCalculatorTests.cs` (new/proposed; reconcile with A audit)
- `docs/dev/fleet-mission-engine-v1.md` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test same-system, different-system, negative coordinates, same planet, symmetry, overflow and unsupported cross-galaxy routes.
- Every original Goal requirement is covered by this slice or a named later integration task; missing prerequisites are explicit, never fake success.
- Relevant deterministic tests/validation pass; no build artifacts or secrets are committed.

## Constraints

- Backend owns state, time, accounting and results in the .NET 8 layered persistent multiplayer universe.
- No combat, attack/espionage execution, loot, market/trade routes or final assets. Keep Spanish-first UI and authenticated shell.
- Never automatically apply migrations, SQL or seeds. Preserve SQL Server support and provider-independent ordinary tests; do not change default provider to match a local setting.
- In-process locks and InMemory-only tests do not prove cross-process SQL correctness; document provider limits and use existing conditional-write/transaction patterns.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build` (focused cases above plus suite; report actual count).
- For storage/services/workers inspect configured integration tests. Current `scripts/run-integration-tests.ps1` is an unadapted placeholder: do not use it as evidence; log `No integration tests configured.` If later adapted, run configured suite and require success.
- Run `git diff --stat` and `git diff --name-only` (also staged equivalents after staging), compare allowlist and budget. Never claim manual SQL/browser QA without execution.

## Commit and push

1. Check `git status`; pull configured upstream per workflow before implementation.
2. Move this task to in-progress, implement/validate, stage intended files and commit a focused change.
3. Move the completed task to done, include lifecycle update in task cycle, and push configured feature branch after validation. Preserve unrelated legacy in-progress tasks.

## Change Budget

- Prefer fewer than 5 implementation files, under 200 changed lines and fewer than 3 commits; count generated changes honestly.
- Stop before exceeding budget and refine/split minimum follow-ups during implementation under AGENTS.md (at most 3 at once). Do not duplicate existing tasks. This planning pass creates only the 104 requested files.

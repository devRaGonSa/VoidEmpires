# TASK-51Q

---
id: TASK-51Q
title: Resource cargo reservation hardening
status: pending
type: platform
team: platform
supporting_teams: [gameplay]
roadmap_item: "Block 51A-51CZ Fleet Movement, Mission Engine & Galactic Expansion v1"
priority: high
execution_order: 17
dependencies: ["TASK-51D-fleet-cargo-model.md", "TASK-51I-fuel-consumption-calculation.md", "TASK-51O-fleet-launch-transaction.md"]
---

## Goal

Make resource cargo/fuel deduction concurrency-safe.

Prevent:
- duplicate resource use;
- negative resources;
- double deduction on retry.

Tests required.

## Context

Transport Gas and fuel share the origin balance and must be protected together against other spends/accrual.

Prerequisites: `TASK-51D-fleet-cargo-model.md`, `TASK-51I-fuel-consumption-calculation.md`, `TASK-51O-fleet-launch-transaction.md`

Created in the planning-only pass; execute later. Old completed TASK-51A/B/C describe unrelated UI work: identify this plan by full filename and roadmap. Read `ai/architecture-index.md`, `ai/orchestrator/component-discovery.md` and, before service/wiring changes, `ai/orchestrator/di-analysis.md`. The A audit at `docs/dev/fleet-mission-engine-v1.md` governs proposed names and compatibility. Reuse verified existing behavior. Supplemental order/dependencies guide execution; verify runner selection before dependent work.

## Implementation steps

1. Protect current balances across contexts with conditional/serializable writes compatible with existing resource spenders and accrual.
2. Preserve request-key idempotency on retries; no helper may independently save or split fuel/cargo into separate commits.
3. On conflict refresh authoritative balances and retry the whole unit or return structured insufficiency.

## Files to read first

- `src/VoidEmpires.Domain/Economy/PlanetResourceStockpile.cs`
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetResourceStockpileConfiguration.cs`
- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionLaunchService.cs` (predecessor output; resolve actual agreed path)
- `src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs`
- `tests/VoidEmpires.Tests/PlanetResourceStockpileDomainTests.cs`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/Fleets/FleetMissionLaunchService.cs` (new/proposed; reconcile with A audit)
- `src/VoidEmpires.Infrastructure/Persistence/Configurations/PlanetResourceStockpileConfiguration.cs`
- `tests/VoidEmpires.Tests/FleetResourceReservationConcurrencyTests.cs` (new/proposed; reconcile with A audit)

Own task status/location metadata is also expected. Resolve proposed paths against audit/predecessors before editing; document justified allowlist refinements in this task/commit without unrelated changes.

## Acceptance criteria

- Test competing launches, combined Gas, duplicate retries, accrual/spend interaction and failed-save rollback: no negatives/partial or duplicate debit.
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

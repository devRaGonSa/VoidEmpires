# TASK-31K

---
id: TASK-31K
title: Orbital production QA state preparation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add Development-only QA preparation for manual Shipyard and any safe Defenses production testing without relying on SQL.

## Context
Construction and Research now have Development-only manual QA preparation flows for reused databases. Orbital production needs the same explicit preparation pattern so repeated manual Shipyard, and optionally Defenses, QA can clear only scoped blockers and restore minimum supported resources without affecting unrelated state or globally completing queues.

## Implementation steps

1. Audit the current Shipyard and any real Defenses queue persistence so the preparation scope matches actual backend contracts.
2. Add Development-only preparation endpoint or service logic for the seeded civilization and planet defaults.
3. Ensure the preparation path neutralizes only blocking open production orders, restores enough targeted resources for at least one supported candidate, and does not affect unrelated civilizations or run global due processing.
4. Add tests covering Development-only gating, idempotent repeated preparation, unrelated civilization safety, and enqueue readiness after preparation.

## Files to read first

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md

## Acceptance criteria

- Development-only orbital production QA preparation exists without SQL.
- The default target uses civilization `00000000-0000-0000-0000-000000000001` and planet `40000000-0000-0000-0000-000000000001`.
- Repeated preparation is idempotent and scoped safely.
- Unrelated civilizations remain unaffected.
- Production enqueue becomes possible after preparation for supported systems.
- `dotnet build --no-restore` and `dotnet test --no-build` pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Keep the path Development-only and explicit

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- no new warnings or obvious regressions are introduced

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

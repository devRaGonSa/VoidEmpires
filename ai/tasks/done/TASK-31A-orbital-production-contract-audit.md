# TASK-31A

---
id: TASK-31A
title: Orbital production contract audit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Audit the current backend and read-model contracts for Shipyard, Defenses, Fleets, and the Planet cockpit so the orbital or military gameplay-v1 scope is documented before any UI mutation work begins.

## Context
The repository already has accepted real enqueue patterns for Construction and Research, plus a Development-safe Shipyard foundation and read-only cockpit companions. Before extending this into a broader orbital or military preparation layer, the team needs a written contract audit that identifies which behaviors are already real, which remain read-only, and which require future backend support rather than frontend invention.

## Implementation steps

1. Inspect Shipyard enqueue, queue persistence, request and response DTOs, and current validation behavior in the backend and tests.
2. Inspect Defenses read or write support and determine whether real persisted defense production exists, is partial, or is still absent.
3. Inspect Fleets and Planet read models to determine how orbital stock, production readiness, and military or orbital summaries can be reflected safely without movement or combat.
4. Update the cockpit and persisted-flow docs with a clear implementation boundary for this block:
   - what can become a real persisted mutation now;
   - what must stay read-only;
   - what requires future backend work.

## Files to read first

- AGENTS.md
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md
- docs/dev/fleets-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Domain/
- tests/VoidEmpires.Tests/

## Expected files to modify

- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md
- docs/dev/fleets-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- The current backend or persistence scope for Shipyard, Defenses, Fleets, and Planet orbital or military summaries is documented.
- The block boundary is explicit about what is real now, what stays read-only, and what needs later backend work.
- No production behavior or frontend mutation is introduced in this task.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not implement UI mutation or backend mutation in this task

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

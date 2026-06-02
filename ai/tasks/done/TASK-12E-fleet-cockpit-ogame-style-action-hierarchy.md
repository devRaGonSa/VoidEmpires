# TASK-12E

---
id: TASK-12E
title: Phase 12E - Fleet cockpit OGame-style action hierarchy
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12E"
priority: medium
---

## Goal
Rework the Fleet action areas so the screen feels closer to a simple OGame-style fleet command UI with a small command-cockpit layer on top.

## Context
The Fleet cockpit layout direction is already accepted, but manual review found that the guarded mutation manifest still feels too prominent and the main playable action flow could be clearer. This task should strengthen the hierarchy of estimate, create, and cancel actions while keeping prototype-only actions discoverable but visually secondary.

## Implementation steps

1. Inspect the current Fleet page action layout, selected-group context, command execution panel, mutation manifest area, and current action labels.
2. Rework the primary action panel so it clearly focuses on selected group, destination selection, estimate trigger, latest estimate summary, and create or cancel flows with explicit confirmation.
3. Make active usable actions visually distinct from prototype-only actions through placement, copy, and styling hierarchy.
4. Compact, move, or visually demote the guarded mutation manifest so it remains discoverable and clearly prototype-only without reading as the main playable control surface.
5. Keep the overall UI simple and readable, avoiding modals, drag and drop, polling-driven timelines, combat, or interception behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/fleet-controlled-mutation-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- The primary Fleet action area clearly presents selected group context, destination, estimate action, estimate summary, and create or cancel flows with explicit confirmation requirements.
- Create and cancel remain the only executable mutation actions, while complete-due, split, and merge stay visible only as prototype or future actions.
- The guarded mutation manifest remains discoverable but is visually secondary and not easily confused with primary playable actions.
- Spanish labels such as `Calcular estimación`, `Crear transferencia`, `Cancelar transferencia`, `Acción protegida`, `Requiere confirmación`, and `Solo prototipo` are used where appropriate.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the work frontend-focused and simple.
- Do not add execution for complete-due, split, or merge.
- Do not add modals, drag and drop, polling, combat, interception, or backend contract changes.
- Split follow-up work if action hierarchy improvements exceed the repository AI change budget.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single commit for this task.
- If hierarchy cleanup starts requiring broader page redesign, stop and create a narrower follow-up task.

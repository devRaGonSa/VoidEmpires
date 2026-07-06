# TASK-41AB1

---
id: TASK-41AB1
title: Module action catalog product labels
status: done
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Apply the product action label vocabulary to the remaining module action catalogs.

## Context
TASK-41AB introduced shared product action labels and updated construction, research, and action-state labels. Shipyard, Defense, and Ground Army action catalogs still use older labels such as "Preparar" or "Cerrar" and should be aligned in a separate small change.

## Implementation steps

1. Inspect the shared product action labels in `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`.
2. Update Shipyard, Defense, and Ground Army action catalog labels to use Construir, Producir, Revisar, Abrir, and Confirmar language.
3. Preserve existing click behavior and disabled states.
4. Do not add new actions or change mutation semantics.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Frontend/src/utils/defensePresentation.ts
- src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/shipyardPresentation.ts
- src/VoidEmpires.Frontend/src/utils/defensePresentation.ts
- src/VoidEmpires.Frontend/src/utils/groundArmyPresentation.ts

## Acceptance criteria

- Shipyard, Defense, and Ground Army action labels use the shared product action vocabulary.
- Technical or implementation-oriented labels are absent from these primary action catalogs.
- Existing behavior and disabled states are unchanged.

## Constraints

- Do not add actions that lack backend support.
- Do not change mutation semantics.
- Keep labels Spanish-first.

## Validation

Before completing the task ensure:

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
- Split the work into additional tasks if limits are exceeded.

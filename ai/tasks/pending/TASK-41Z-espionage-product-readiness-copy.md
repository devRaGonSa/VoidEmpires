# TASK-41Z

---
id: TASK-41Z
title: Espionage product readiness copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Espionage page product-facing.

## Context
Espionage should use intelligence network language while keeping active mission execution unavailable.

## Implementation steps

1. Inspect Espionage page copy, target cards, passive signals, disabled actions, and diagnostics.
2. Use intelligence network language.
3. Describe missions as pending activation without dev references.
4. Keep active espionage execution unavailable.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Espionage page uses intelligence product language.
- Product mode avoids Development/read-only primary copy.
- No active espionage execution is added.

## Constraints

- Do not add sabotage, infiltration, counter-espionage, or mission mutations.
- Do not fake intelligence state.
- Preserve existing read behavior.

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

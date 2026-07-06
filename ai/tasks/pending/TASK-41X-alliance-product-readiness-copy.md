# TASK-41X

---
id: TASK-41X
title: Alliance product readiness copy
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Make the Alliance page product-facing.

## Context
Alliance should use diplomacy/federation language while keeping alliance mutations unavailable.

## Implementation steps

1. Inspect Alliance page copy, contacts, disabled actions, empty states, and diagnostics.
2. Replace development/read-only primary copy with diplomacy and federation language.
3. Keep mutation boundaries honest and product-facing.
4. Do not add alliance, pact, invitation, role, treasury, or messaging mutations.
5. Hide technical diagnostics from product mode.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Alliance page uses diplomacy/federation product language.
- Product mode avoids Development/read-only primary copy.
- No alliance mutation is added.

## Constraints

- Do not add alliance mutations.
- Do not fake diplomacy state.
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

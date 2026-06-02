# TASK-13A

---
id: TASK-13A
title: Phase 13A - Fleet primary action language polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 12Y-13D"
priority: medium
---

## Goal
Replace unclear primary action wording with direct game-like labels while preserving the cockpit flavor in secondary labels.

## Context
The Fleet screen already supports the right actions, but some button and heading labels still sound too technical or ambiguous. This task makes the next action obvious without removing the cockpit identity entirely.

## Implementation steps

1. Inspect the Fleet buttons, badges, headings, and helper text.
2. Replace unclear primary action labels with concise gameplay wording.
3. Keep cockpit-flavored wording only in secondary descriptive areas.
4. Preserve the existing action behavior and confirmation flow.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet button and heading helpers

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSummaryPanel.tsx`
- `src/VoidEmpires.Frontend/src/components/FleetSelectedGroupPanel.tsx`
- Fleet action label helpers, if needed

## Acceptance criteria

- Primary Fleet actions read clearly in Spanish.
- Cockpit flavor remains limited to secondary labels or descriptive copy.
- Existing estimate, create, cancel, and complete-due behavior is preserved.
- Split and merge remain disabled or prototype-only.
- Backend behavior is unchanged.

## Constraints

- Do not change backend.
- Do not apply EF migrations.
- Do not enable split or merge execution.
- Keep the wording concise and game-like.

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
- Split the work into additional tasks if limits are exceeded.

# TASK-13K

---
id: TASK-13K
title: Phase 13K - Strategic map Spanish copy and capability language
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Polish the Galaxia cockpit primary copy so the visible strategic surface reads mostly in Spanish and feels player-facing instead of API-facing.

## Context
Visual QA for the 13E-13J block confirmed the cockpit structure is valid, but several first-viewport labels still read like backend or capability diagnostics. This task should improve language only and preserve the current read-only Galaxy behavior.

## Implementation steps

1. Review the current strategic map page, labels, helper text, and capability rendering in the first viewport.
2. Replace English or API-oriented primary copy with Spanish gameplay-oriented language.
3. Keep raw capability keys and technical wording only inside collapsed diagnostics or secondary technical sections.
4. Preserve accessibility labels and keep backend behavior unchanged.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `src/VoidEmpires.Frontend/README.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- frontend strategic map helpers or label-mapping utilities, if needed

## Acceptance criteria

- Primary Galaxy copy is mostly Spanish and player-readable.
- Raw capability keys no longer dominate the main cards or panels.
- Player-facing labels such as system inspection, planet detail, and fleet-route intent are readable in Spanish.
- Raw technical capability names remain available only in collapsed diagnostics.
- No backend behavior changes are introduced.

## Constraints

- Keep Galaxy read-only.
- Do not add backend endpoints.
- Do not expose technical DTO or enum names in the primary UI.
- Preserve existing accessibility intent where present.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA on `/galaxy` confirms the visible copy is mostly Spanish and no longer reads like an API console

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

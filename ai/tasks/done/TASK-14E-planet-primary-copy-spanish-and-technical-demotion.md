# TASK-14E

---
id: TASK-14E
title: Phase 14E - Planet primary copy Spanish and technical demotion
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Remove English and overly technical copy from the primary Planet UI.

## Context
Visual QA found several primary strings that still read like backend or development notes. The Planet screen should feel like a Spanish-first gameplay cockpit, with technical details collapsed into secondary diagnostics.

## Implementation steps

1. Review the visible Planet strings in headers, cards, action hints, and availability messages.
2. Replace English primary copy with Spanish player-facing text.
3. Demote technical or capability wording into collapsed diagnostics.
4. Preserve safety notes, but rewrite them as gameplay/dev-safe Spanish.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/README.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- Planet presentation helpers or styles, if needed

## Acceptance criteria

- Primary Planet UI reads mostly in Spanish.
- English capability or backend phrasing no longer dominates the main cards.
- Technical details remain only in collapsed diagnostics.
- The page no longer feels like an API console.

## Constraints

- Keep useful safety notes.
- Do not remove technical diagnostics entirely.
- Do not change backend behavior.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA confirms the Planet primary copy is mostly Spanish and gameplay-readable

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

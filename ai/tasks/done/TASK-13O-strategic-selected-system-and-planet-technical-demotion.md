# TASK-13O

---
id: TASK-13O
title: Phase 13O - Strategic selected system and planet technical demotion
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Demote technical system and planet detail in Galaxy so gameplay hierarchy is clearer than implementation detail.

## Context
Selected system and planet cards still surface ids, capability names, and technical state too early. The cockpit should prioritize strategic identity, control, counts, and navigation intent, while keeping diagnostics available for development.

## Implementation steps

1. Review the current selected system and selected planet presentation hierarchy.
2. Reorder or reframe the cards so names, coordinates, control, counts, and gameplay-facing context come first.
3. Move ids, capability keys, and lower-level visibility or request details into collapsed diagnostics.
4. Preserve the current data mapping and read-only behavior.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- `src/VoidEmpires.Frontend/README.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMapSystemCard.tsx`
- strategic map helper utilities, if needed

## Acceptance criteria

- Selected system cards prioritize name, coordinates, control or visibility, star type, and meaningful counts.
- Selected planet cards prioritize name, type, ownership or control, and colonization state.
- Full ids, capability keys, and lower-level technical details move into collapsed diagnostics.
- Existing API data is preserved without backend changes.

## Constraints

- Keep diagnostics available for development.
- Keep Galaxy read-only.
- Do not change backend response contracts.
- Do not let ids dominate primary headings or callouts.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA on `/galaxy` confirms gameplay-first hierarchy and secondary diagnostics

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

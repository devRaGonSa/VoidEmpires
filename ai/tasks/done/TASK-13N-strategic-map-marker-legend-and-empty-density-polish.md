# TASK-13N

---
id: TASK-13N
title: Phase 13N - Strategic map marker legend and empty density polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Improve Galaxy marker clarity, legend usefulness, and sparse-map presentation so minimal data still feels intentional.

## Context
The deterministic 2D map is valid, but sparse seeds can leave the stage feeling empty or placeholder-like. This task should clarify what is being shown and why without inventing fake data or changing the deterministic projection.

## Implementation steps

1. Review the current 2D map markers, labels, counts, and legend behavior.
2. Make system, own, visible, unknown, fleet, and transfer cues more distinguishable in the current 2D design language.
3. Add contextual map metadata such as visible system count, current focus, fleet or transfer counts, grid hints, and read-only status.
4. Add a short Spanish operational hint when visibility is intentionally limited to very few systems.

## Files to read first

- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance criteria

- Marker categories are easier to distinguish visually or by labeling.
- The legend communicates the current map state clearly.
- Sparse datasets still look like intentional strategic intelligence rather than an empty placeholder.
- Contextual map details show counts, focus, and read-only status without inventing gameplay data.
- The deterministic 2D map remains intact.

## Constraints

- Do not add fake systems, planets, fleets, or transfers.
- Do not add 3D.
- Keep Galaxy read-only.
- Preserve the deterministic 2D projection.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA on `/galaxy` confirms sparse-map presentation still looks intentional

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

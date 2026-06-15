# TASK-33H

---
id: TASK-33H
title: Research session context integration
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Integrate session banner/context into Research without changing accepted research behavior.

## Context
Research already has persisted enqueue behavior and safe open-order handling. This task only makes the page session-aware and improves navigation handoffs.

## Implementation steps

1. Read Research page behavior and accepted checklist notes.
2. Display the reusable playable session banner/context.
3. Use the route context fallback for missing ids and offer continuation from local playable session.
4. Ensure handoffs back to Planet preserve `civilizationId` and `planetId`.
5. Preserve accepted behavior:
   - selection does not mutate;
   - modal open does not mutate;
   - confirm mutates;
   - open-order 409/no-op remains safe;
   - refresh is backend-first;
   - no auto-complete is added.
6. Keep raw ids secondary or diagnostic only.

## Files to read first

- docs/dev/research-cockpit-checklist.md
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/PlayableSessionBanner.tsx
- src/VoidEmpires.Frontend/src/utils/usePlayableRouteContext.ts
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- Optional: src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- Optional: docs/dev/research-cockpit-checklist.md

## Acceptance criteria

- Research is session-aware without behavior regression.
- Missing-id entry offers local-session continuation.
- Handoffs to Planet preserve ids.
- Raw ids remain secondary/diagnostic only.

## Constraints

- Do not add auto-complete.
- Do not mutate on selection or modal open.
- Do not fake resources or research state.
- Do not claim visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33H message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

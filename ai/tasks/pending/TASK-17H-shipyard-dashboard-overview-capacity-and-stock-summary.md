# TASK-17H-shipyard-dashboard-overview-capacity-and-stock-summary

---
id: TASK-17H-shipyard-dashboard-overview-capacity-and-stock-summary
title: Shipyard dashboard overview capacity and stock summary
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: high
---

## Goal
Add the top-level Shipyard overview that summarizes production capability, local resources, current stock, and readiness before the player scans the production catalog.

## Purpose
Give the cockpit a strategic dashboard layer so the player can quickly understand what the selected planet can produce and what orbital assets already exist locally.

## Current Problem
Jumping straight into a production list would make Shipyard feel like a debug table. The user needs context first: whether the planet is ready, what resources are available, whether there is current orbital stock, and whether the queue is empty or active.

## Context
- Planet, Construction, and Research already open with summary-style sections.
- Shipyard must align structurally with those cockpits while keeping its own focus on orbital production rather than buildings or technology.
- Any unavailable backend field must degrade gracefully.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard presentation helpers
- `src/VoidEmpires.Frontend/src/styles.css`
- Shipyard UI-state contracts or view-model files

## Implementation Requirements
1. Add a dashboard overview section near the top of the Shipyard page.
2. Surface the most important cockpit signals, such as:
   - selected civilization and planet;
   - shipyard readiness or capacity summary;
   - relevant local resources;
   - current queue count;
   - stock or orbital reserve summary;
   - recommended next action.
3. If the backend exposes readiness blockers, show them in a readable summary form.
4. If stock exists, summarize it without forcing the user to inspect the detailed queue or Fleet page.
5. If the backend does not expose one of the expected signals, render an honest limitation instead of a fake zero.
6. Keep technical diagnostics secondary.

## UI/UX Requirements
- The overview should feel like a cockpit dashboard, not a bare list.
- Spanish-first labels and section titles.
- Prioritize readability and quick scanning over dense tables.
- Recommended action copy should not claim that production is available if it is actually blocked.

## Backend/API Requirements
- Reuse the Shipyard read model.
- Only change backend contracts if a small missing summary field prevents a truthful dashboard.

## Safety Constraints
- No mutation.
- No fake capacity values.
- No Fleet behavior leakage into the Shipyard dashboard.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/` Shipyard mapping or presentation helpers
- `src/VoidEmpires.Frontend/src/styles.css`
- Narrow read-model files only if summary fields are missing

## Acceptance Criteria
- Shipyard opens with a meaningful summary of readiness, resources, queue, and stock.
- Missing backend support is displayed honestly.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend files are changed.

## Notes / Residual Risks
- Keep the summary modular so later tasks can refine layout without reworking the data contract.
- Avoid overscoping into catalog cards, queue tables, or mutation handling in this task.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Split styling follow-ups if layout polish grows too large.

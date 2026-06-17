# TASK-36K

---
id: TASK-36K
title: Shipyard page decluttering
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Declutter Shipyard around production queue, stock, and buildable units.

## Context
Shipyard should focus on production queue, available production, and backend-backed stock/readiness while keeping Development tools and diagnostics secondary.

## Implementation steps

1. Prioritize primary Shipyard UI:
   - compact context/resource strip;
   - orbital production queue;
   - available production;
   - current stock/readiness if exposed;
   - handoff to Fleets.
2. Remove repeated session, planet, and Development cards if redundant.
3. Keep `GameModal` confirmation.
4. Keep backend-first enqueue/refresh/fallback behavior.
5. Do not add fleet missions, movement, or combat.
6. Move diagnostics and dev tools to collapsed secondary areas.
7. Do not change gameplay behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/components/PageHeader.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Shipyard page is clearer and less repetitive.
- Shipyard remains functionally equivalent.
- No missions, movement, or combat are introduced.
- Frontend build and copy guard pass.

## Constraints

- Do not change backend enqueue/refresh semantics.
- Do not fake stock.
- Do not add fleet gameplay.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36K message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

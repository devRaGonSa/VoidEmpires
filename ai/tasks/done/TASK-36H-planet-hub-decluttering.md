# TASK-36H

---
id: TASK-36H
title: Planet hub decluttering
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Declutter Planet so it acts as a clear hub instead of a diagnostic dashboard.

## Context
Planet should prioritize the current playable loop: resources, production, active queues, and primary cockpit links. Development and diagnostic content should be secondary.

## Implementation steps

1. Prioritize primary Planet UI:
   - planet identity;
   - resources;
   - production per hour;
   - active/open queues summary;
   - links to Construcción, Investigación, Astillero, Defensas, and Flotas.
2. Remove repeated planet/session cards when redundant with the shared header/context strip.
3. Move backend endpoints, ids, and raw diagnostics to collapsed `DevDiagnosticsPanel`.
4. Move resource accrual/materialization actions into `DevelopmentToolsPanel`.
5. Keep resource materialization backend-first.
6. Preserve route/query handoffs.
7. Keep Spanish-first copy and avoid backend behavior changes.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Planet is cleaner and more hub-like.
- Dev/diagnostic content is secondary and collapsed where appropriate.
- Resource materialization remains backend-first.
- Frontend build and copy guard pass.

## Constraints

- Do not fake resources.
- Do not optimistic-update authoritative state.
- Do not change route/query handoffs.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36H message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

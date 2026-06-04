# TASK-19Z-ground-army-dashboard-readiness-and-garrison-summary

---
id: TASK-19Z-ground-army-dashboard-readiness-and-garrison-summary
title: Ground Army dashboard readiness and garrison summary
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 19U-20J - Ground Army cockpit playable foundation v1"
priority: high
---

## Goal
Create the top-level Ground Army dashboard showing garrison posture, troop readiness, structures, resources, and the recommended next step.

## Purpose
Give the cockpit an at-a-glance overview so the page does not open into a raw list of technical items with no explanation of current terrestrial posture.

## Current Problem
The Ground Army cockpit should not open directly into a raw list. It needs an overview that explains current planet-side force posture and what can safely be done next.

## Context
- Planet, Research, Shipyard, and Defenses already use summary dashboards.
- Ground Army should align visually while staying specialized for terrestrial readiness rather than orbital production or combat.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx` if already created
- shared cockpit components
- Ground Army presentation and view-model helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Add a dashboard or overview section showing, where supported:
   - selected planet
   - system
   - ownership or control
   - garrison readiness
   - troop and structure count
   - available Ground Army options count
   - blocked options count
   - queue count
   - local resources
   - manpower or population
   - recommended next Ground Army action
2. If no dedicated Ground Army backend exists yet, show honest readiness copy such as `Lectura terrestre preparada para esta build.`
3. If capability exists, show copy such as `Guarnicion terrestre disponible` or equivalent player-facing wording.
4. Include an explicit note such as `Esta cabina no resuelve combate ni invasiones.`

## UI/UX Requirements
- Spanish-first copy.
- Compact summary cards.
- No raw ids in the main dashboard.
- No wording that implies active battles or invasions.

## Backend/API Requirements
- No backend change is expected unless the read model lacks required counts or summary fields.

## Safety Constraints
- No combat.
- No mutation in this task.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/GroundArmyPage.tsx`
- Ground Army view-model or presentation helpers
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- `/ground-army` has a useful overview.
- A player can understand the current terrestrial posture without opening diagnostics.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Real battle-readiness scoring can remain a future block; this task only needs a truthful readiness summary.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on the top-level dashboard only.

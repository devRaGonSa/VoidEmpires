# TASK-17N-shipyard-complete-due-safe-placeholder-or-dev-action

---
id: TASK-17N-shipyard-complete-due-safe-placeholder-or-dev-action
title: Shipyard complete due safe placeholder or dev action
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 17C-17T - Shipyard cockpit playable foundation v1"
priority: medium
---

## Goal
Expose Shipyard production completion only if the repository already supports a safe development-only completion path; otherwise render a truthful disabled placeholder.

## Purpose
Keep due production handling honest and conservative because completion can create stock or other gameplay-relevant effects.

## Current Problem
Asset production may have due orders, but completing them is a real mutation. If the completion path is global, unscoped, or otherwise unsafe, Shipyard must not offer it as a live cockpit action.

## Context
- Construction and Research already established the pattern of leaving complete-due disabled or placeholder when the backend path is not safely scoped.
- Shipyard completion could affect stock and future Fleet handoff readiness.
- This behavior is optional for v1.

## Files to Inspect First
- Existing asset production completion or process-due endpoint or service
- Tests for asset production processors
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- Shipyard API client files
- Relevant dev frontend checklists

## Implementation Requirements
1. Inspect whether a safe dev-only completion endpoint exists and whether it is scoped narrowly enough for Shipyard.
2. If it is safe:
   - expose a secondary completion action;
   - show due count;
   - require confirmation;
   - refresh Shipyard state after success;
   - add tests.
3. If it is not safe:
   - render a disabled or placeholder action such as `Completar produccion vencida no disponible`;
   - explain in Spanish that safe closure is not yet bounded to this cockpit.
4. Do not add background workers, auto-completion, or hidden completion triggers.

## UI/UX Requirements
- The action must look secondary.
- Disabled state should be clearly intentional, not broken.
- Spanish copy should explain the limitation calmly and specifically.

## Backend/API Requirements
- Add tests if completion is enabled.
- Do not introduce a global unsafe completion route just for convenience.

## Safety Constraints
- No hidden stock creation without confirmation.
- No unsafe global completion from Shipyard.
- No worker toggles or background magic.

## Expected Files to Modify
- `src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx`
- `src/VoidEmpires.Frontend/src/api/` Shipyard client files if completion is supported
- Backend endpoint or service files only if a safe dev-only completion path already exists and needs thin exposure
- `tests/VoidEmpires.Tests/` if backend completion is enabled

## Acceptance Criteria
- Complete-due state is truthful: enabled only if safe, otherwise disabled with explanation.
- Validation passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore` and `dotnet test --no-build` if backend changes are made.

## Notes / Residual Risks
- It is acceptable for Shipyard v1 to leave completion disabled.
- Do not let the existence of processor classes alone justify enabling the button.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code when possible.
- Keep the task narrow and conservative.

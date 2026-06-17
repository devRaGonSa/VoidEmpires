# TASK-36G

---
id: TASK-36G
title: Development actions modal confirmation
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Require clearer confirmation for Development-only mutating tools.

## Context
Actions such as applying resource accrual or materializing due queues mutate Development database state. They should be inside Development tools and confirmed before execution.

## Implementation steps

1. Identify Development-only mutating UI actions:
   - apply 15 min / 30 min / 1 h resource accrual;
   - update/materialize due queues;
   - any similar dev-only action.
2. Move these actions into `DevelopmentToolsPanel` where appropriate.
3. Use `GameModal` or equivalent confirmation before invoking mutating Development actions.
4. Modal copy must say:
   - this is Development-only;
   - it mutates the Development database;
   - it only materializes backend-authoritative state;
   - it does not complete non-due orders unless the backend says they are due.
5. After success, refresh backend state.
6. Keep normal gameplay actions separate.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- src/VoidEmpires.Frontend/src/components/GameModal.tsx
- src/VoidEmpires.Frontend/src/api/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx

## Acceptance criteria

- Dev-only mutating actions are no longer presented as ordinary buttons.
- Confirmation is explicit before mutations.
- No fake updates or optimistic state changes are introduced.
- Frontend build and copy guard pass.

## Constraints

- Do not change backend semantics.
- Do not add instant-complete normal UI.
- Do not auto-complete on page load.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36G message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

# TASK-36F

---
id: TASK-36F
title: Development tools panel foundation
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Create a reusable collapsed Development tools panel for QA/materialization actions.

## Context
Development/QA actions such as resource accrual and queue materialization are currently mixed into normal gameplay UI. They should remain available but clearly secondary.

## Implementation steps

1. Review existing DevDiagnosticsPanel and GameModal patterns.
2. Add or reuse a component such as `DevelopmentToolsPanel` or `DevToolsPanel`.
3. Ensure the panel:
   - is collapsed by default or visually secondary;
   - clearly labels Development/QA scope;
   - can contain mutating QA actions;
   - supports explanatory text;
   - supports action buttons;
   - supports last result summary;
   - can link to diagnostics where useful.
4. Ensure it does not look like normal gameplay.
5. Keep Spanish-first copy.
6. Do not change gameplay behavior yet.

## Files to read first

- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- src/VoidEmpires.Frontend/src/components/GameModal.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Reusable Development tools panel exists.
- Panel is clearly secondary/Development scoped.
- No gameplay behavior changes are introduced.
- Frontend build passes.

## Constraints

- Do not remove Development tools.
- Do not add hidden mutations.
- Do not present QA tools as normal gameplay.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36F message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

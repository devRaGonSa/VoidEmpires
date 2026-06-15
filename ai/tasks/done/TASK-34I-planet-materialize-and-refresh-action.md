# TASK-34I

---
id: TASK-34I
title: Planet materialize and refresh action
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: high
---

## Goal
Add a safe Planet hub action to explicitly materialize due queues and refresh backend state.

## Context
Planet is the playable hub. A clearly Development-scoped action can ask the backend to materialize due orders, then refresh the hub from the backend without fake or optimistic updates.

## Implementation steps

1. Read Planet page state loading and refresh behavior.
2. Add a clearly Development-scoped action such as `Actualizar colas vencidas`.
3. Copy must explain:
   - the backend materializes due orders;
   - this is Development/manual QA scope if the endpoint is dev-only;
   - not-due orders are not completed.
4. On click, call the typed materialization API with the current session ids and requested queue types.
5. After success, re-fetch Planet state from the backend.
6. Show processed/skipped counts.
7. Keep raw response in diagnostics or secondary technical areas.
8. Do not use instant-complete wording or normal gameplay cheating semantics.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/api/
- Optional: src/VoidEmpires.Frontend/src/styles.css
- Optional: scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Planet can trigger explicit backend materialization safely.
- Planet re-fetches backend state after materialization.
- Processed counts are displayed.
- No optimistic updates or fake completion are introduced.
- Copy guard passes.

## Constraints

- Do not auto-call materialization on page load.
- Do not add instant-complete buttons to normal UI.
- Do not perform or claim visual QA.
- Keep Spanish-first copy and raw ids/payloads secondary.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34I message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

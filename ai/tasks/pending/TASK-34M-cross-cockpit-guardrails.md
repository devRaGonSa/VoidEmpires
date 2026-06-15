# TASK-34M

---
id: TASK-34M
title: Cross-cockpit guardrails
status: pending
type: frontend
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 34A-34P - Queue Progression & Completion Materialization v1"
priority: medium
---

## Goal
Ensure queue materialization does not leak forbidden gameplay actions or eager route imports.

## Context
Development materialization copy is allowed, but it must not present cheating as normal gameplay or introduce combat, movement, missions, or auto-complete semantics.

## Implementation steps

1. Review existing frontend guard scripts and smoke checklist.
2. Update or add lightweight guards/checks for:
   - no visible normal UI action for `completar instantáneamente`;
   - no attack, move, or mission creation introduced;
   - no auto-complete on page load wording;
   - route lazy imports preserved.
3. Allow copy that clearly mentions Development materialization.
4. Update the frontend smoke checklist with the guardrail notes.
5. Keep guard failures actionable.

## Files to read first

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/

## Expected files to modify

- scripts/check-frontend-route-lazy-imports.ps1
- scripts/check-frontend-copy-regressions.ps1
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Forbidden actions remain absent.
- Materialization is not framed as normal gameplay cheating.
- Lazy route checks remain green.
- Guards pass.

## Constraints

- Do not perform browser/visual QA.
- Do not add new gameplay actions.
- Do not make materialization automatic on page load.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-34M message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

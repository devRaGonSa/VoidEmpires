# TASK-36J

---
id: TASK-36J
title: Research page decluttering
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Declutter Research around technology queue and catalog.

## Context
Research should focus on current/open research, available technologies, and blocked reasons while keeping Development diagnostics secondary.

## Implementation steps

1. Prioritize primary Research UI:
   - compact context/resource strip;
   - current/open research;
   - available technologies;
   - blocked technologies and reasons.
2. Remove repeated session, planet, and Development cards if redundant.
3. Keep `GameModal` confirmation.
4. Keep backend-first enqueue and refresh behavior.
5. Preserve 409 open-order no-op behavior.
6. Move diagnostics and dev-only explanations to collapsed secondary areas.
7. Do not change gameplay behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/PageHeader.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- docs/dev/research-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Research page is clearer and less repetitive.
- Research remains functionally equivalent.
- Diagnostics and dev-only explanations are secondary.
- Frontend build and copy guard pass.

## Constraints

- Do not change backend enqueue or 409 behavior.
- Do not auto-complete.
- Do not fake resources or research state.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36J message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

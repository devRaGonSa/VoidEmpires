# TASK-36I

---
id: TASK-36I
title: Construction page decluttering
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Declutter Construction so the catalog and queue are the main content.

## Context
Construction currently places several context and Development cards before the action catalog. The page should surface the queue and available construction catalog earlier while preserving behavior.

## Implementation steps

1. Prioritize primary Construction UI:
   - compact context/resource strip;
   - construction queue/current order;
   - available construction catalog;
   - blocked construction reasons.
2. Remove or demote repeated load cabin cards, duplicated planet/session cards, long Development explanations, and unrelated specialized cabin summaries.
3. Keep modal confirmation for construction enqueue.
4. Keep backend-first enqueue and refresh behavior.
5. Preserve open-order/no-op behavior.
6. Move materialization, diagnostics, and dev notes to Development tools or diagnostics panels.
7. Do not change gameplay semantics.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/components/PageHeader.tsx
- src/VoidEmpires.Frontend/src/components/DevelopmentToolsPanel.tsx
- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- docs/dev/construction-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Construction catalog appears earlier and with less noise.
- Construction remains functionally equivalent.
- Development/diagnostic information is secondary.
- Frontend build and copy guard pass.

## Constraints

- Do not change backend enqueue behavior.
- Do not add auto-complete or fake updates.
- Preserve GameModal confirmation.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36I message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

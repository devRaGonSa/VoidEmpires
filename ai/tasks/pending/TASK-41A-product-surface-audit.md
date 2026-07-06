# TASK-41A

---
id: TASK-41A
title: Product surface audit
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Audit every normal UI page and identify visible Development, QA, test, prototype, raw ids, diagnostics, localhost/backend details, and non-product copy.

## Context
This is the discovery task for Block 41A-41AZ. The output should guide later UI copy and visibility changes without changing runtime behavior.

## Implementation steps

1. Read the frontend route, page, component, utility, and API files that shape normal UI.
2. List each offending visible phrase or category, including raw ids, raw payloads, backend/provider details, localhost details, and dev/test wording.
3. Define whether each item should be removed, renamed, hidden, collapsed, or gated behind operator mode.
4. Document Spanish-first product terminology rules for the follow-up tasks.
5. Do not change behavior in this task unless only documentation is needed.

## Files to read first

- ai/architecture-index.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- docs/dev/product-surface-audit.md

## Acceptance criteria

- All normal UI routes are covered by the audit.
- Offending terms and categories are listed with target remediation.
- Product-facing terminology rules are documented.
- No gameplay, backend, SQL Server, auth, market, alliance, fleet movement, combat, or asset behavior is added.

## Constraints

- Keep this task documentation-only unless a minimal supporting doc file is required.
- Do not claim browser, screenshot, or manual visual QA.
- Keep Spanish-first product language.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

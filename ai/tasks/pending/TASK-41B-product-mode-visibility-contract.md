# TASK-41B

---
id: TASK-41B
title: Product mode visibility contract
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Define a product-mode visibility contract for normal UI versus hidden operator/internal mode.

## Context
Normal UI must default to product mode. Diagnostics and development tools may remain only when explicitly hidden behind a safe operator/internal mode.

## Implementation steps

1. Review existing UI surfaces that expose diagnostics, development tools, raw ids, backend details, or technical payloads.
2. Define product mode as the default for all routes.
3. Define an operator/internal visibility contract, including query flag or local internal toggle behavior only if the existing architecture supports it safely.
4. Document that no normal user route should show development diagnostics by default.
5. Keep this task documentation-oriented unless a tiny utility stub is clearly needed by follow-up tasks.

## Files to read first

- docs/dev/product-surface-audit.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/utils/

## Expected files to modify

- docs/dev/product-mode-visibility-contract.md

## Acceptance criteria

- Product mode is explicitly documented as default.
- Debug/operator tools are documented as hidden by default.
- Allowed reveal mechanisms are local, explicit, and non-sensitive.
- No normal product route is allowed to show development diagnostics.

## Constraints

- Do not persist sensitive state.
- Do not remove internal endpoints or scripts needed for local operation.
- Do not claim manual visual QA.

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

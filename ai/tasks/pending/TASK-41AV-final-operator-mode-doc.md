# TASK-41AV

---
id: TASK-41AV
title: Final operator mode doc
status: pending
type: docs
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Document how to access hidden operator/dev tools if they remain available.

## Context
Operator/dev tools must remain documented only in `docs/dev`, not product user documentation.

## Implementation steps

1. Inspect the implemented operator-mode gate and hidden tooling.
2. Create or update a `docs/dev` document that explains safe local access.
3. Document that operator mode is hidden by default and not part of normal player flow.
4. Avoid secrets, real connection strings, and unsafe production instructions.
5. Run backend validation.

## Files to read first

- docs/dev/product-mode-visibility-contract.md
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/components/
- docs/dev/

## Expected files to modify

- docs/dev/operator-mode.md
- docs/dev/product-readiness-report.md
- ai/current-state.md

## Acceptance criteria

- Operator mode access is documented under `docs/dev`.
- Documentation does not appear as product user docs.
- No secrets are included.

## Constraints

- Do not expose operator mode by default.
- Do not commit real credentials or connection strings.
- Do not claim production operational readiness.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`

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

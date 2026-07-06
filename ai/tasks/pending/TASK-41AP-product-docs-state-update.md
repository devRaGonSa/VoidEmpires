# TASK-41AP

---
id: TASK-41AP
title: Product docs state update
status: pending
type: docs
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 41 product-facing app finalization"
priority: high
---

## Goal
Update product readiness docs and `ai/current-state.md`.

## Context
Documentation should reflect that normal UI no longer exposes development/test language by default while internal tools remain hidden and technical.

## Implementation steps

1. Review the current product readiness report, SQL Server checklist, and current-state notes.
2. Record that product-facing UI no longer exposes dev/test language by default.
3. Record that internal operator/dev tools remain technical and hidden.
4. Preserve the accepted SQL Server seeded validation status.
5. Record that manual visual QA remains deferred and no gameplay expansion was added.

## Files to read first

- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/sql-server-user-checklist.md
- docs/dev/deferred-visual-qa-master-checklist.md

## Expected files to modify

- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/sql-server-user-checklist.md

## Acceptance criteria

- Docs reflect product-facing UI status and hidden operator tooling.
- SQL Server seeded validation status remains accurate.
- Manual visual QA remains explicitly deferred.
- No gameplay expansion is claimed.

## Constraints

- Do not claim browser/manual QA.
- Do not add final images or assets.
- Do not change backend behavior.

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

# TASK-11J

---
id: TASK-11J
title: Phase 11J - Fleet prototype mutation controls guarded and disabled
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11J"
priority: medium
---

## Goal
Add guarded, disabled prototype controls for mutation fleet commands so users can see the future command surface without being able to execute mutations accidentally.

## Context
Once the metadata and confirmation model are in place, the Fleet page can expose the future action surface more concretely. This task is about discoverability only: the controls must stay visibly disabled, clearly explained, and fully non-executable so the page remains a safe development prototype.

## Implementation steps

1. Review the Fleet page, command readiness display, and any reusable UI primitives already used for badges, rows, or action hints.
2. Add disabled or prototype-only controls for create transfer, cancel transfer, complete due, split group, and merge groups.
3. Show each control's label, read-only versus mutation classification, readiness if known, disabled reason, and a clear `Prototype only` or equivalent Spanish warning.
4. Verify that no control calls a mutation endpoint, no hidden submit behavior exists, and no keyboard shortcut or form side effect can trigger execution.
5. Run the required build and test validation commands.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/components/ui/UiCard.tsx
- src/VoidEmpires.Frontend/src/components/ui/UiBadge.tsx
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/utils/fleetCommandPresentation.ts
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/components/ui/UiBadge.tsx
- docs/dev/fleet-api-contracts.md

## Acceptance criteria

- Disabled prototype controls are visible for the mutation fleet commands.
- The UI clearly explains that mutation controls are prototype-only and non-executable.
- No click handler, form submission, or shortcut calls create, cancel, complete, split, or merge endpoints.
- Backend code, migrations, and persistence behavior remain unchanged.
- Validation succeeds with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Constraints

- Keep the UI simple and aligned with the existing frontend style.
- Do not change backend code or apply EF migrations.
- Do not remove the existing separation between manifest metadata and action execution.
- Do not add hidden interactive paths that could mutate development data.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
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
- Prefer a single commit for this task.
- If safe disabled controls require a broader layout refactor, stop and split that work into a follow-up task.

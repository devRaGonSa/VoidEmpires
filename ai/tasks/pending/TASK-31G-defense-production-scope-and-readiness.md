# TASK-31G

---
id: TASK-31G
title: Defense production scope and readiness
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Determine and implement the safest Defenses scope for this phase, using real enqueue only if narrow backend support already exists.

## Context
Defenses currently sits near the boundary between future military preparation and out-of-scope combat systems. This task decides whether Defenses can join the accepted persisted enqueue pattern safely or whether it must remain read-only with explicit readiness and limitation messaging.

## Implementation steps

1. Audit the backend and tests to confirm whether a real persisted Defenses enqueue path exists and is scoped safely enough for this block.
2. If safe backend support exists, implement the same selection -> review -> confirm -> submit pattern with conservative scope.
3. If safe backend support does not exist, keep Defenses read-only and add honest readiness or limitation copy that explains the missing backend contract.
4. Update the Defenses checklist to describe the accepted scope for this block.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/
- docs/dev/defenses-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Web/
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- tests/VoidEmpires.Tests/
- docs/dev/defenses-cockpit-checklist.md

## Acceptance criteria

- Defenses is either safely real-enqueue capable or clearly read-only and prepared with documented limitations.
- Blocked or unavailable defenses never look executable by accident.
- No fake production success is shown.
- No combat, auto-complete, or unrelated military expansion is introduced.
- `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend` pass if touched.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not invent production semantics when backend support is absent

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- no new warnings or obvious regressions are introduced

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

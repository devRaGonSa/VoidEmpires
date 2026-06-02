# TASK-11W

---
id: TASK-11W
title: Phase 11W - Cancel transfer controlled mutation docs and validation
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11W"
priority: medium
---

## Goal
Document the controlled cancel-transfer frontend mutation path and provide non-visual validation steps.

## Context
The cancel path is the second controlled frontend mutation flow after create-transfer. The repository docs need to spell out that cancellation mutates development data, does not refund resources if that remains the documented rule, and must stay clearly separate from the still-disabled complete-due, split, and merge controls.

## Implementation steps

1. Review `docs/dev/fleet-api-contracts.md`, `docs/dev/fleet-controlled-mutation-checklist.md`, the frontend README, and any other fleet-command documentation already present.
2. Update the most appropriate docs to explain that read-only estimate can execute from the frontend dev UI, create-transfer can execute after explicit confirmation, and cancel-transfer can now also execute after explicit confirmation.
3. Document that cancel-transfer mutates development data and that it does not refund resources if that is still the current rule.
4. Document that complete-due, split, and merge remain prototype-only and non-executable from the frontend UI.
5. Record the non-visual validation steps for backend build, backend tests, frontend build, and optional seed or fleet-state checks.
6. Keep the documentation free of secrets, real credentials, private IPs, and local-only connection strings.

## Files to read first

- docs/dev/fleet-api-contracts.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/README.md
- README.md
- ai/current-state.md

## Expected files to modify

- docs/dev/fleet-api-contracts.md
- docs/dev/fleet-controlled-mutation-checklist.md
- src/VoidEmpires.Frontend/README.md
- README.md

## Acceptance criteria

- Documentation clearly states that cancel-transfer can execute from the frontend dev UI only after explicit confirmation.
- Documentation clearly states that cancel-transfer mutates development data.
- Documentation clearly states whether cancel-transfer refunds resources or does not refund them, using the current rule.
- Documentation clearly states that complete-due, split, and merge remain disabled or prototype-only in the frontend.
- The non-visual validation flow is documented with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- Manual visual validation is explicitly deferred unless a clear frontend regression appears.

## Constraints

- Keep this task documentation-focused.
- Only allow small frontend or docs consistency fixes if strictly necessary.
- Do not apply EF migrations.
- Do not touch PostgreSQL directly.
- Use placeholders for any connection strings.

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
- If documentation updates begin to require broader product-state rewrites, stop and create a follow-up task.

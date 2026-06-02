# TASK-12R

---
id: TASK-12R
title: Phase 12R - Fleet cockpit final visual QA docs
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12R"
priority: medium
---

## Goal
Update visual validation documentation for Fleet cockpit v1 and prepare final manual QA.

## Context
This six-phase block is intended to finish Fleet cockpit v1 as a readable, mostly Spanish gameplay screen with the safe command boundary intact. The repository needs updated documentation that captures the final visual expectations and the standard build and test baseline so manual QA can confirm the experience without guessing the intended result.

## Implementation steps

1. Review the current frontend smoke checklist, fleet controlled mutation checklist, fleet API contract docs, frontend README, and root README for existing Fleet validation guidance.
2. Update the most relevant docs with a final manual visual QA checklist for Fleet cockpit v1.
3. Document the expected final visual state: mostly or fully Spanish UI, Spanish shell labels, readable squad list, primary names with secondary IDs, gameplay-oriented readiness labels, readable estimate card, explicit create and cancel confirmations, readable active transfers, readable resources, and secondary technical panels.
4. Document that complete-due, split, and merge remain prototype-only, and that raw enum numbers or `NetworkError` should not dominate the screen.
5. Include the non-visual validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md
- README.md

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/README.md
- README.md

## Acceptance criteria

- The documentation includes a final visual QA checklist for Fleet cockpit v1.
- The checklist covers Spanish shell labels, readable squads, primary names with secondary IDs, gameplay-oriented readiness, readable estimate results, explicit confirmations, active transfers, resources, and secondary technical panels.
- The documentation includes the non-visual validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- No screenshots, secrets, real credentials, private IPs, or local-only credentials are added.

## Constraints

- Keep this task documentation-focused, with only small frontend or docs consistency fixes if strictly necessary.
- Do not add new gameplay behavior, screenshots, or production credentials.
- Do not expand the executable command surface beyond estimate, create transfer, and cancel transfer.
- Split follow-up work if the documentation needs broader state rewrites.

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
- If the final QA documentation grows beyond the block, stop and create a smaller follow-up task.

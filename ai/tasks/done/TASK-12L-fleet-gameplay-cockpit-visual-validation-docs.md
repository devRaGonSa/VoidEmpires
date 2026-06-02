# TASK-12L

---
id: TASK-12L
title: Phase 12L - Fleet gameplay cockpit visual validation docs
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12L"
priority: medium
---

## Goal
Update documentation and the manual validation checklist for the completed Fleet gameplay cockpit milestone.

## Context
This six-phase block is intended to finish the Fleet cockpit direction by making the screen feel like a gameplay view rather than a debug or API dashboard. The repository needs an updated validation checklist that captures the new visual expectations and keeps the non-visual build and test baseline explicit, without exposing secrets or environment-specific credentials.

## Implementation steps

1. Review the current frontend smoke checklist, fleet controlled mutation checklist, fleet API contract docs, frontend README, and root README for existing validation guidance.
2. Update the most relevant docs with a manual visual QA checklist for the gameplay cockpit milestone.
3. Document the expected visual outcomes: mostly Spanish UI, scannable squad list, readable selected squad panel, simple estimate or create or cancel flow, visible active transfers, readable resources, and secondary technical panels.
4. Document that complete-due, split, and merge remain prototype-only, IDs are secondary metadata, and raw enum numbers or NetworkError should not dominate the screen.
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

- The documentation includes a visual QA checklist for the Fleet gameplay cockpit milestone.
- The checklist covers gameplay-screen feel, mostly Spanish UI, scannable squad list, readable selected squad panel, simple estimate or create or cancel flow, visible active transfers, readable resources, secondary technical panels, secondary IDs, and disabled prototype commands.
- The documentation includes the non-visual validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- No screenshots, secrets, real credentials, private IPs, or local-only credentials are added.

## Constraints

- Keep this task documentation-focused, with only small frontend or docs consistency fixes if strictly necessary.
- Do not add new gameplay behavior, screenshots, secrets, or production credentials.
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
- If the documentation update starts to grow beyond the block, stop and create a smaller follow-up task.

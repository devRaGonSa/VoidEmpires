# TASK-12F

---
id: TASK-12F
title: Phase 12F - Fleet cockpit visual validation checklist update
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 12F"
priority: medium
---

## Goal
Update the visual validation checklist for the polished OGame-style Fleet cockpit milestone.

## Context
The next Fleet block is meant to polish the cockpit into a mostly Spanish, simpler, more playable fleet screen while preserving the current safe prototype execution boundary. Documentation should capture the next manual visual QA expectations and the unchanged technical validation baseline so reviewers can verify polish without guessing the intended outcome.

## Implementation steps

1. Review the current frontend smoke checklist, fleet controlled mutation checklist, and frontend README for existing Fleet validation guidance.
2. Update the most relevant docs with a checklist for the next manual visual review covering Spanish copy, readable fleet list, readable selected-group panel, understandable estimate or create or cancel flow, secondary IDs, resource readability, and readable result or error feedback.
3. Explicitly document that create and cancel still require confirmation, complete-due, split, and merge remain prototype-only or disabled, and no raw enum numbers or `NetworkError` should dominate the review.
4. Include the technical validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
5. Keep the docs free of screenshots, secrets, real connection strings, passwords, private IPs, local-only credentials, or new gameplay behavior.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- src/VoidEmpires.Frontend/README.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/fleet-controlled-mutation-checklist.md
- src/VoidEmpires.Frontend/README.md

## Acceptance criteria

- The documentation includes a manual visual validation checklist for the polished Fleet cockpit milestone.
- The checklist explicitly covers mostly Spanish copy, a readable simple playable screen feel, readable fleet list and selected-group panel, understandable estimate or create or cancel flow, explicit confirmation requirements, secondary IDs, readable resource context, readable result or error feedback, no dominant raw enum numbers, and no `NetworkError`.
- The documentation includes the technical validation baseline with `dotnet build --no-restore`, `dotnet test --no-build`, and `npm run build --prefix src/VoidEmpires.Frontend`.
- No screenshots, secrets, real credentials, private IPs, or new gameplay behavior are added.

## Constraints

- Keep this task documentation-focused, with only very small frontend or docs consistency fixes if strictly necessary.
- Do not add screenshots, secrets, new gameplay behavior, or broader product-state rewrites.
- Do not expand the executable command surface beyond estimate, create transfer, and cancel transfer.
- Split follow-up work if documentation maintenance grows beyond this milestone.

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
- If the checklist update starts requiring broader documentation rewrites, stop and create a smaller follow-up task.

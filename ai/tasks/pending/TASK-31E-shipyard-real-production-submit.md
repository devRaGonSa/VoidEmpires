# TASK-31E

---
id: TASK-31E
title: Shipyard real production submit
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Wire Shipyard confirmation to the real backend production enqueue endpoint using backend-first mutation rules.

## Context
Once Shipyard has selection and confirmation state, it needs the same safe real enqueue behavior that Construction and Research already use: POST only on explicit confirm, no optimistic success, no fake queue or stock changes, no double-submit, and honest surfaced backend failures.

## Implementation steps

1. Connect the Shipyard confirmation action to the typed production API contract.
2. Add pending or loading state that prevents duplicate submits and protects against double clicks.
3. Handle success, known 409 or validation cases, and unexpected failures with Spanish-first primary copy and diagnostics-only raw detail.
4. Preserve useful local selection state on failure and avoid introducing any fleet movement or mission side effects.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- docs/dev/shipyard-cockpit-checklist.md
- tests/VoidEmpires.Tests/

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/
- tests/VoidEmpires.Tests/

## Acceptance criteria

- A real Shipyard production order can be created from the UI if backend support exists.
- The frontend submits only on explicit confirmation.
- No optimistic queue, stock, or resource mutation is shown before backend success.
- Known no-op or conflict states are shown honestly and do not fabricate success.
- No fleet movement, missions, or combat behavior is introduced.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not invent backend behavior that does not already exist

## Validation

Before completing the task ensure:

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

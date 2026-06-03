# TASK-13M

---
id: TASK-13M
title: Phase 13M - Strategic map transfer route readable summary
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Make Galaxy transfer and route information understandable to a player instead of exposing vague technical metadata.

## Context
The current transfer area can report information such as metadata availability without telling the player what route or transfer context actually exists. This task should translate existing transfer data into readable summaries while preserving the read-only boundary.

## Implementation steps

1. Review how the current strategic map page reads and presents transfer overlays and route metadata.
2. Replace vague transfer copy with readable Spanish summaries that explain count, origin, destination, and visible transfer state when available.
3. Provide graceful fallbacks when data is partial and keep the `Flotas` navigation intent visible.
4. Preserve the rule that Galaxy cannot create, cancel, or complete transfers.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/api/strategicMapTypes.ts`
- `src/VoidEmpires.Frontend/src/api/fleetTypes.ts`, if route labels are shared
- `docs/dev/fleet-api-contracts.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- frontend strategic map label helpers or transfer-summary helpers, if needed

## Acceptance criteria

- Transfer area shows readable route summaries in Spanish.
- Route count or transfer count is visible when available.
- Origin and destination names are shown when available.
- Fallback labels such as detected route and inspect in Fleets remain graceful when data is partial.
- Galaxy stays strictly read-only.

## Constraints

- Do not execute transfer mutations from Galaxy.
- Do not add new backend endpoints.
- Keep technical metadata secondary.
- Keep any route toward `Flotas` clearly non-mutating.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- manual QA on `/galaxy` confirms the transfer area explains what exists and where to inspect it

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

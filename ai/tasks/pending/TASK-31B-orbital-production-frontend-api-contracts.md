# TASK-31B

---
id: TASK-31B
title: Orbital production frontend API contracts
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add or extend typed frontend API contracts for supported orbital or military production flows so Shipyard and any safe Defenses path can consume backend results without swallowing validation failures.

## Context
The frontend already uses typed API helpers for read models and real enqueue patterns in Construction and Research. This block needs the same typed discipline for orbital or military production: safe POST contracts where backend support exists, typed readiness-only contracts where it does not, and parseable 400 or 409 failures that the UI can present without exposing raw payloads in the primary experience.

## Implementation steps

1. Audit the existing frontend API utilities, Shipyard page data flow, and any current orbital or defense API helpers.
2. Add typed request or response models for Shipyard production enqueue, including known error handling and the refreshed state fields the UI needs after success.
3. If Defenses has a safe real backend production contract, add the matching typed request or response models; otherwise add typed readiness-only models and avoid inventing a fake POST client.
4. Ensure the API layer preserves raw response details for diagnostics while returning typed UI-safe results for known validation cases.

## Files to read first

- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Web/DevEndpointMappings.cs
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/utils/

## Acceptance criteria

- Typed frontend API contracts exist for supported orbital or military production flows.
- Defenses uses a readiness-only contract if no safe backend enqueue exists.
- Non-2xx backend responses are parsed and surfaced intentionally rather than swallowed.
- Raw backend payloads remain secondary or diagnostic-only.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not add fake backend endpoints or invented production semantics

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

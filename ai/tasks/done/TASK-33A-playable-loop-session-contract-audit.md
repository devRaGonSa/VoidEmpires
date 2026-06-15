# TASK-33A

---
id: TASK-33A
title: Playable loop session contract audit
status: pending
type: frontend
team: gameplay
supporting_teams: [platform]
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: high
---

## Goal
Audit the current frontend navigation and session behavior, then document the safe playable-loop contract without changing runtime behavior.

## Context
Block 33 should reduce reliance on hardcoded seeded URLs while avoiding any production authentication claims. This first task defines the session/navigation boundaries so later tasks can add local navigation convenience safely.

## Implementation steps

1. Read the listed context files and identify where `civilizationId` and `planetId` are read from query parameters.
2. Audit how these pages behave when ids are missing: Planet, Construction, Research, Shipyard, Defenses, and Fleets.
3. Inspect `/onboarding` and document what the playable-start endpoint returns after success.
4. Identify any existing helper that builds cockpit or page URLs.
5. Document the safe session persistence contract in the two checklist docs:
   - localStorage may store only non-sensitive navigation context returned by the Development-safe playable start.
   - Allowed fields are `civilizationId`, `planetId`, player/display name, civilization name, planet name, and client timestamps if useful.
   - localStorage is a navigation convenience only and is never authoritative game state.
   - Backend remains source of truth.
   - No production auth, login, token, or session claims are introduced.
6. Do not change application behavior in this task.

## Files to read first

- ai/architecture-index.md
- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Acceptance criteria

- Safe session/navigation scope is documented.
- Query-parameter dependencies and missing-id behavior are summarized.
- `/onboarding` success response shape is documented.
- Existing route helper usage is documented.
- No production auth overclaim is introduced.
- No behavior changes are made.

## Constraints

- Do not implement localStorage behavior yet.
- Do not perform or claim browser/visual QA.
- Keep UI behavior unchanged.
- Preserve lazy loading.

## Validation

Before completing the task run:

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33A message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split follow-up work into later Block 33 tasks if needed.

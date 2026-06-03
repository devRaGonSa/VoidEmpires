# TASK-15K

---
id: TASK-15K
title: Current state update module boundaries
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: medium
---

## Purpose
Update `ai/current-state.md` to reflect the corrected frontend module architecture.

## Current problem
The current-state document still describes the earlier mixed cockpit shape. After this block, the important new state is that module boundaries are explicit and Planet/Construction no longer act as catch-all screens.

## Context from current implementation
`ai/current-state.md` is a key continuity document for the orchestrator. It must reflect accepted rules and intentional exclusions so future chats do not accidentally undo the boundary decision.

## Files to inspect first
- `ai/current-state.md`
- `docs/dev/planet-module-boundaries.md` if created
- `docs/dev/frontend-foundation-smoke-checklist.md`

## Expected files to modify
- `ai/current-state.md`

## Implementation requirements
- Update the phase line to include:
  - `Phase 15L - Planet module boundaries and construction scope separation`
  - or equivalent wording.
- Add frontend state bullets:
  - Planet is now a dashboard/resumen surface.
  - Construction is scoped to general/civil/economic/infrastructure construction.
  - Research, Ground Army, Shipyard and Defenses have dedicated route boundaries or placeholders.
  - Module-specific catalog duplication has been reduced.
  - Query-context helpers protect `civilizationId` and `planetId` navigation.
- Preserve existing backend foundations.
- Keep the intentional exclusions list accurate:
  - no 3D/WebGL;
  - no combat;
  - no real interception execution;
  - no espionage;
  - no production auth;
  - no real specialized module execution yet.

## UI/UX requirements
- The current-state file should make it hard for future chats to forget the boundary decision.

## Backend/API requirements
- No backend change.

## Safety constraints
- Do not remove important historical state unless it is clearly obsolete.

## Acceptance criteria
- `ai/current-state.md` accurately summarizes the new boundaries.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Keep this concise; current-state is already large.

# TASK-26R Update Build Output Docs And Performance Note

---
id: TASK-26R
title: Document bundle splitting results and frontend performance guardrails
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Document the lazy-loading decision, the resulting build output, and the guardrails future developers should follow when evaluating bundle warnings.

## Current problem
The Vite chunk warning has recurred across validations. Without an explicit note, future contributors will not know whether the issue was resolved, reduced, or intentionally accepted.

## Context from current implementation
This block is performance-architecture work only. The accepted cockpit behavior remains unchanged, but the build and documentation should make the architectural outcome clear.

## Goal
Produce durable developer documentation that explains the new loading strategy, the actual build result, and how to interpret future chunk warnings.

## Implementation steps
1. Update the frontend smoke checklist or create a dedicated performance note.
2. Record the previous bundle warning context and the new route-lazy-loading strategy.
3. Record whether the warning disappeared or remained after the changes.
4. Explain how to run the frontend build and how to interpret chunk output.
5. Make clear that this work changes architecture and loading strategy, not gameplay behavior.

## Files to inspect first
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/market-cockpit-checklist.md
- ai/current-state.md
- src/VoidEmpires.Frontend/vite.config.ts

## Expected files to modify
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/frontend-performance-notes.md
- ai/current-state.md

## Implementation requirements
- Update `docs/dev/frontend-foundation-smoke-checklist.md` or create `docs/dev/frontend-performance-notes.md`.
- Record:
- previous main bundle warning context
- new lazy-loading strategy
- whether the warning disappeared
- if it still exists, why and whether it is acceptable
- how to run `npm run build --prefix src/VoidEmpires.Frontend`
- how to interpret chunk warnings
- Mention explicitly that the change does not alter gameplay behavior.
- Mention that accepted cockpit routes should still be checked after route-loading changes.

## Frontend requirements
- None beyond accurate documentation.

## Backend/API requirements
- None.

## Safety constraints
- Do not overclaim performance improvements without actual build evidence.
- Keep documentation aligned with observed outputs and implemented behavior.

## Acceptance criteria
- A performance/build note exists.
- The bundle warning outcome is documented accurately.
- Repository validation remains green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- If the outcome is only a partial improvement, document the remaining risk clearly instead of phrasing it as full resolution.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

# TASK-22N

---
id: TASK-22N
title: Phase 22N - Espionage cockpit docs and smoke checklist
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - qa
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Document Espionage cockpit behavior, seed URL, manual QA flow, and explicit non-goals.

## Purpose

The cockpit needs repeatable validation guidance so future work does not accidentally claim sabotage, active missions, combat, or real-time intelligence features that are still out of scope.

## Current problem

Every accepted cockpit now has supporting docs and smoke guidance. Espionage needs the same continuity layer so later task runs can validate it quickly and consistently.

## Context

The repository already uses `docs/dev/frontend-foundation-smoke-checklist.md`, `docs/dev/development-seed-profiles.md`, and module-specific cockpit checklists as the durable QA source of truth.

## Files to read first

- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/strategic-map-cockpit-checklist.md`
- `ai/current-state.md`

## Component discovery

Inspect how existing cockpit checklist docs define seeded URLs, validation commands, manual checks, and explicit non-goals. Reuse the same documentation structure so Espionage is easy to audit later.

## Implementation requirements

1. Create `docs/dev/espionage-cockpit-checklist.md`.
2. Include the canonical seeded URL:
   - `/espionage?civilizationId=00000000-0000-0000-0000-000000000001`
3. Include manual checks for:
   - page loads
   - civilization context visible
   - intelligence summary visible
   - targets visible
   - visibility and confidence levels clear
   - future mission actions disabled
   - handoff links to Galaxy, Planet, Fleets, and Research
   - diagnostics collapsed
4. Include explicit exclusions:
   - no sabotage
   - no active spy mission execution
   - no combat
   - no WebSockets
5. Update `docs/dev/frontend-foundation-smoke-checklist.md` with Espionage coverage.
6. Update `docs/dev/development-seed-profiles.md` with Espionage seed expectations.
7. Update `docs/dev/planet-module-boundaries.md` if the route-boundary matrix needs an Espionage entry or clarification.

## UI/UX requirements

- Docs should be practical, screenshot-friendly, and easy to follow during manual QA
- Copy should use the same Spanish module names the UI exposes

## Backend/API requirements

- No backend behavior change required
- Document only supported read surfaces

## Expected files to modify

- `docs/dev/espionage-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/development-seed-profiles.md`
- `docs/dev/planet-module-boundaries.md` only if needed

## Safety constraints

- Do not document unsupported espionage actions as implemented
- Do not imply active missions, sabotage, theft, or combat

## Acceptance criteria

- A user can open the docs and run a repeatable Espionage QA pass.
- The exclusions are explicit and hard to misread.
- Shared smoke docs now include Espionage.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Keep the checklist honest if some sections are conditionally empty because the current read model is conservative.
- If route-boundary docs already clearly imply Espionage behavior, update them minimally instead of rewriting the whole matrix.

## Commit and push

1. Run `git status`.
2. Verify only intended docs changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep doc updates focused and avoid broad copy cleanup outside Espionage.

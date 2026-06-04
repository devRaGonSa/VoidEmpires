# TASK-22I

---
id: TASK-22I
title: Phase 22I - Espionage observation history and signals panel
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: medium
---

## Goal

Show passive observation signals or a truthful empty-state panel so the cockpit feels like an intelligence module even without active missions.

## Purpose

Espionage should surface what the civilization has recently observed or can currently infer from existing route, fleet, and strategic signals, while staying fully read-only.

## Current problem

The module risks feeling static if it only shows target cards. Existing fleet markers, transfer overlays, or last-known strategic hints may already provide enough material for a useful secondary panel, but that panel does not exist yet.

## Context

Current backend foundations already expose fleet markers, transfer context, and strategic observations in `Galaxy` and `Fleets`. Espionage should reuse that read-only context if available and otherwise stay honest about the absence of recent readings.

## Files to read first

- Espionage read model or service
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- Espionage page component

## Component discovery

Inspect how current strategic-map and fleet screens present transfer, marker, or route information. Reuse existing signal terminology and avoid inventing a new persistence concept.

## Implementation requirements

1. Add a secondary panel such as:
   - `Senales observadas`
   - `Lecturas recientes`
2. If current read-state supports it, show items such as:
   - transfer route signals
   - fleet marker signals
   - observed system or planet signal summaries
   - timestamp or status only if already available from existing DTOs
3. If the data is not available, show a truthful empty state such as:
   - `No hay lecturas recientes para este contexto.`
4. Do not introduce a new event-history table or new persistence model just to support this panel.
5. Do not imply real-time updates or background streaming.

## UI/UX requirements

- Secondary panel, not the primary hero
- Read-only and easy to scan
- Spanish-first
- The panel should feel useful when populated and honest when empty

## Backend/API requirements

- No new persistence
- Read model may derive signal entries from existing data if that can be done conservatively
- Keep any backend additions read-only and covered by tests

## Expected files to modify

- Espionage page component
- Espionage view-model helpers
- backend read-model files only if lightweight signal derivation is needed
- seed tests only if signal support depends on deterministic seeded visibility

## Safety constraints

- No WebSockets
- No polling requirement
- No active mission execution
- No fabricated historical timeline

## Acceptance criteria

- The cockpit exposes a useful passive-signals panel when data exists, or a truthful empty state when it does not.
- The panel does not imply real-time tracking or active surveillance gameplay.
- Frontend build passes, and backend tests pass if backend files changed.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

Run backend validation only if backend files are touched.

## Notes / residual risks

- Current data may be too coarse for a rich timeline; that is acceptable as long as the panel remains honest and reusable.
- A future dedicated intelligence-history system can replace this without invalidating the read-only v1 boundary.

## Commit and push

1. Run `git status`.
2. Confirm changed files match the intended signal panel scope.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Avoid creating a broad history subsystem.

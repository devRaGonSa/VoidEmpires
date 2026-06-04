# TASK-22L

---
id: TASK-22L
title: Phase 22L - Espionage error taxonomy and diagnostics
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

Normalize Espionage failures into clear Spanish primary messages with collapsed diagnostics for technical detail.

## Purpose

The cockpit should fail gracefully when context is missing, the endpoint is unavailable, or the read model is empty, without surfacing raw backend wording in the primary UI.

## Current problem

Espionage can fail for multiple reasons: invalid `civilizationId`, missing context, empty intelligence, Development-only endpoint gating, unsupported action attempts, or unexpected API errors. Without a taxonomy, the page will drift toward generic or technical errors.

## Context

Accepted cockpits already use Spanish-first error mapping with technical drawers. Espionage must follow that standard and keep diagnostics available but secondary.

## Files to read first

- Espionage page component
- Espionage API client
- Espionage presentation helpers
- error and diagnostics patterns in `ResearchPage`, `ShipyardPage`, `DefensesPage`, and `GroundArmyPage`

## Component discovery

Inspect current error mappers and diagnostics panels. Reuse shared error-state layout and avoid introducing a one-off taxonomy style.

## Implementation requirements

1. Add an error-mapping layer for cases such as:
   - invalid civilization id
   - civilization not found
   - no visible intelligence
   - strategic read unavailable
   - endpoint unavailable outside Development
   - unsupported mission or action
   - unexpected error
2. Provide Spanish-first primary messages, for example:
   - `No se pudo cargar la lectura de inteligencia.`
   - `No hay objetivos visibles para esta civilizacion.`
   - `Aplica cockpit-validation para cargar el escenario demo.`
   - `Las misiones activas no estan disponibles en esta version.`
3. Keep technical details available in collapsed diagnostics.
4. Do not hide backend rejections; reframe them clearly and surface raw detail secondarily.
5. If backend result codes are needed to distinguish cases safely, add them conservatively and cover them with tests.

## UI/UX requirements

- Error state must be readable and visually stable
- Diagnostics collapsed by default
- Spanish-first and gameplay-facing
- Empty intelligence must look intentional, not broken

## Backend/API requirements

- Prefer frontend-only error mapping
- Add backend result codes only if truly needed and covered by tests
- No new mutation behavior

## Expected files to modify

- Espionage page component
- Espionage API client or presentation helpers
- backend endpoint or tests only if response-shape refinements are necessary

## Safety constraints

- No mutation
- No espionage execution
- Do not swallow backend failures silently

## Acceptance criteria

- Espionage errors are clear and localized.
- Technical details remain available but secondary.
- Empty or unavailable intelligence states do not break layout or expose raw backend text as primary UI.
- Frontend build passes, and backend tests pass if backend changes were required.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

Run backend validation only if backend files are touched.

## Notes / residual risks

- Future active mission flows will need a separate error taxonomy; this task should stay focused on read-only cockpit behavior.
- Avoid overfitting the mapper to seed-only phrasing if generic failure wording is safer.

## Commit and push

1. Run `git status`.
2. Confirm changed files match the intended error-handling scope.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep backend response changes minimal if they become necessary.

# TASK-23L

---
id: TASK-23L
title: Phase 23L - Market error taxonomy and diagnostics
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Normalize Market failures into Spanish primary messages with collapsed diagnostics.

## Purpose

The cockpit should surface clear, calm user-facing errors while still preserving enough technical detail in secondary diagnostics for development and QA.

## Current problem

Market can fail for several reasons: missing civilization context, invalid planet context, empty economy data, Development-only endpoint gating, unsupported actions, or unexpected API failures. Those errors must not leak raw backend text into the primary UI.

## Context

Accepted cockpits already use error taxonomies and collapsed diagnostics. Market should follow the same standard and keep technical details visible but secondary.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx`
- Market presentation helpers
- Market API client files
- diagnostics patterns in accepted cockpit pages

## Component discovery

Inspect current cockpit error mapping, secondary diagnostics UI, and missing-context state patterns. Prefer reusing those helpers instead of inventing a Market-only alert structure.

## Dependency analysis

Expected flow:

- API error or invalid route context -> error mapper
- error mapper -> Spanish primary message plus technical diagnostics
- page render -> visible error state with collapsed details

If backend result codes are already available, prefer mapping those rather than string matching raw messages.

## Implementation requirements

1. Add or extend an error mapper for cases such as:
   - invalid civilization id
   - civilization not found
   - invalid planet id
   - no resource data
   - market read unavailable
   - endpoint unavailable outside Development
   - unsupported transaction or action
   - unexpected error
2. Use primary Spanish messages such as:
   - `No se pudo cargar la lectura economica.`
   - `No hay reservas visibles para esta civilizacion.`
   - `Aplica cockpit-validation para cargar el escenario demo.`
   - `Las operaciones de mercado no estan disponibles en esta version.`
3. Keep technical details collapsed by default.
4. Do not hide backend rejection entirely; preserve it in diagnostics where helpful.
5. Ensure the error state does not break page layout.
6. Add backend result codes only if they are truly needed and can be covered by tests.

## UI/UX requirements

- Error state must be visible and understandable
- Diagnostics collapsed by default
- Spanish-first
- No layout break or giant raw stack traces in the main panel

## Backend/API requirements

- Prefer frontend mapping only
- Add backend result codes only if necessary and tested

## Expected files to modify

- Market page or helper files
- Market API client files
- backend files only if a tiny result-code addition is needed
- tests if backend error-shaping changes are introduced

## Safety constraints

- No mutation
- No transactions
- No silent swallowing of meaningful failures

## Acceptance criteria

- Market errors are clear in Spanish.
- Technical details remain available but secondary.
- Frontend build passes.
- Backend tests pass if backend changes are introduced.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

Run backend validation if backend files are changed:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Future transaction errors will need a separate taxonomy once active market gameplay exists; this task should remain limited to read-only Market behavior.

## Commit and push

1. Run `git status`.
2. Run `git diff --stat`.
3. Confirm the change is focused on Market error presentation or narrow result-code support.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer mapping existing error shapes instead of adding a broad backend exception taxonomy.
- If many backend surfaces need alignment, stop and create a small follow-up task.

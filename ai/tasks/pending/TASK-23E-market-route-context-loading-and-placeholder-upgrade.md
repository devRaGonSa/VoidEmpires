# TASK-23E

---
id: TASK-23E
title: Phase 23E - Market route context loading and placeholder upgrade
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 23A-23P - Market cockpit read-only economy foundation v1"
priority: high
---

## Goal

Upgrade `/market` from a placeholder module into a context-aware cockpit shell that loads Market read-state from `civilizationId` and optional `planetId`.

## Purpose

Market should stop behaving like a generic future-module screen and start behaving like a real cockpit entrypoint that understands current route context and accepted navigation conventions.

## Current problem

The `Mercado` route exists as a future or placeholder module. It does not yet load read-state, preserve known cockpit context, or explain its read-only economy boundary in a usable way.

## Context

Route helpers already preserve context across accepted cockpits. Market should fit the same module architecture and should not fall back to a generic placeholder when valid civilization context exists.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect how Planet, Research, Shipyard, Defenses, Ground Army, and Espionage parse route context, show loading and error states, and preserve handoff links. Prefer matching those patterns instead of introducing a bespoke route shell.

## Dependency analysis

Expected route flow:

- `App.tsx` route -> Market page
- route helpers -> parse `civilizationId` and optional `planetId`
- Market API client -> typed read-state fetch
- page state -> loading, error, missing-context, and success rendering

## Implementation requirements

1. Ensure `/market` accepts:
   - required `civilizationId`
   - optional `planetId`
2. Use existing route helpers wherever possible.
3. Load Market UI state through the typed client from Task 23D or the existing frontend API pattern.
4. Show loading, error, and missing-context states in Spanish.
5. Preserve suspicious-context warning behavior if that pattern already exists in accepted cockpit pages.
6. Add handoff links such as:
   - `Volver a Planeta` if selected planet exists
   - `Abrir Construccion`
   - `Abrir Astillero`
   - `Abrir Flotas`
   - `Volver a Galaxia`
7. Explain the cockpit boundary in visible copy:
   - Market reads economy and trade potential
   - this version does not execute buy or sell orders
8. Do not leave the page looking like an empty future placeholder when valid context exists.

## UI/UX requirements

- Page title should be `Mercado`
- The shell should look like a real cockpit, not a generic placeholder
- Spanish-first
- Diagnostics collapsed by default
- Missing-context and error states should stay explicit and calm

## Backend/API requirements

- No backend change is expected if the previous read-model task provided a stable endpoint.

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx` if route wiring needs adjustment
- `src/VoidEmpires.Frontend/src/pages/MarketPage.tsx` or the equivalent Market page file
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- Market API or helper files only if the shell needs small integration changes

## Safety constraints

- No buy or sell actions
- No resource mutation
- No fleet movement
- No generic fallback that masks real data-loading errors

## Acceptance criteria

- `/market` loads seeded context when a valid `civilizationId` is present.
- `/market` no longer looks like an empty placeholder.
- Missing or invalid context shows an explicit state rather than a broken or blank page.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If backend support is still intentionally limited, the shell should present an honest read-only state rather than implying richer data exists.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended route, page, and helper files changed.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer integrating with current shared route helpers instead of creating a separate context parser.
- If the route shell starts absorbing too much dashboard content, stop and leave the remaining UI for the next tasks.

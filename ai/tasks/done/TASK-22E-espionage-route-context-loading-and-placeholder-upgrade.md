# TASK-22E

---
id: TASK-22E
title: Phase 22E - Espionage route context loading and placeholder upgrade
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Upgrade `/espionage` from a placeholder surface to a context-aware cockpit shell that loads read-only intelligence state for a selected civilization.

## Purpose

The route should stop looking like a future-module stub and start behaving like a real cockpit entrypoint that understands `civilizationId` and related handoff context.

## Current problem

The Espionaje route currently exists only as a placeholder or readiness surface. That breaks the accepted module pattern where real cockpits load seeded context, show explicit missing-context states, and preserve navigation intent across handoffs.

## Context

Other accepted routes already:

- parse query context through route helpers
- load typed UI state
- render Spanish loading, error, and empty states
- keep diagnostics secondary
- explain module boundaries clearly

Espionage must now join that baseline without claiming active mission gameplay.

## Files to read first

- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx`
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Frontend/src/api/`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect the route setup, shared module-cabin wrapper, and the way `Planet`, `Research`, `Shipyard`, `Defenses`, and `Ground Army` load query context and render safe boundary messaging.

## Implementation requirements

1. Ensure `/espionage` accepts `civilizationId` as required route context.
2. Optionally accept `systemId` or `planetId` only if they help preserve safe handoffs from Galaxy or Planet.
3. Use shared route helpers instead of rebuilding query strings inline.
4. Load the Espionage UI state from the typed client introduced in earlier tasks.
5. Render:
   - loading state in Spanish
   - missing-context state in Spanish
   - API error state in Spanish
   - seeded success state with a real cockpit shell
6. Preserve suspicious-context warning behavior if a shared pattern already exists.
7. Add clear links for:
   - `Volver a Galaxia`
   - `Abrir Planeta` when a target has planet context
   - `Abrir Flotas` when signals or fleet markers are relevant
   - `Abrir Investigacion` when future intelligence technology is mentioned
8. Include visible boundary copy explaining that Espionage reads intelligence and target coverage only in this build.

## UI/UX requirements

- Page title must be `Espionaje`
- The first viewport must feel like a real cockpit, not a generic placeholder
- Spanish-first copy
- Diagnostics collapsed or clearly secondary
- Read-only and limitation guidance visible without dominating the screen

## Backend/API requirements

- No backend change is expected if the earlier read-model task landed cleanly
- Do not add mutation endpoints from this route

## Expected files to modify

- `src/VoidEmpires.Frontend/src/App.tsx`
- Espionage page component under `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- supporting styles only if the shell needs small cockpit-specific layout support

## Safety constraints

- No spy actions
- No sabotage
- No combat
- No fleet movement
- No hidden auto-navigation to mutating flows

## Acceptance criteria

- `/espionage` loads seeded civilization context when `civilizationId` is present.
- The route no longer behaves like an empty placeholder.
- Missing or invalid context shows a clear Spanish state.
- The cockpit boundary is visible and does not imply active mission execution.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If the backend currently returns limited state, the shell should still load and explain the limitation honestly rather than falling back to a generic future-module card.
- If optional `systemId` or `planetId` create noisy edge cases, keep them as passive context only.

## Commit and push

1. Run `git status`.
2. Verify only intended route and page files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the route upgrade focused on context loading and shell rendering.
- Split any large design refactor into later presentation tasks.

# TASK-26I

---
id: TASK-26I
title: Phase 26I - Market handoff and sidebar status polish
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 26C-26L - Market visual QA and read-only polish"
priority: medium
---

## Goal

Ensure Market navigation, handoffs, and sidebar status match the accepted cockpit suite and reflect Market as a real read-only cockpit rather than a future placeholder.

## Current problem

Market was added after earlier cockpit foundations, so the navigation language and sidebar state may still reflect transitional copy or uneven route emphasis.

## Context from current implementation

- The cockpit suite already includes shared route helpers and implemented-versus-future module cues.
- Market should preserve context toward Planet, Construction, Shipyard, Fleets, and Galaxy.
- Future modules such as Alliance and Ranking should remain visibly future-oriented.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/market-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/styles.css
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements

1. Verify the sidebar status and route state for `/market`.
2. Ensure the active-state treatment for Market is correct and consistent with other implemented cockpits.
3. Ensure handoffs preserve the existing civilization and planet context for:
   - `Planeta`
   - `Construccion`
   - `Astillero`
   - `Flotas`
   - `Galaxia`
4. Ensure clearly future modules remain future-facing and do not look newly available:
   - `Alianza`
   - `Ranking`
5. Keep the work navigation-focused; do not broaden into new route behavior or features.

## UI/UX requirements

- Market should feel fully part of the current cockpit suite.
- Handoffs should be easy to understand and should not lose seeded context.
- Future modules should remain visually distinct from implemented ones.

## Backend/API requirements

- No backend changes are expected.
- Route preservation should rely on current frontend URL helpers and page patterns.

## Safety constraints

- No route mutations beyond current context-preserving navigation.
- No feature expansion.
- No unintended breakage to other cockpit links.

## Acceptance criteria

- Sidebar and handoffs treat Market as an implemented read-only cockpit.
- Navigation preserves context correctly.
- Future modules remain clearly marked as future.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Because shared route helpers affect multiple pages, any helper adjustment should be minimal and validated against existing cockpit patterns.


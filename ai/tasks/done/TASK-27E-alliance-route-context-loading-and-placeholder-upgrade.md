# TASK-27E Alliance Route Context Loading And Placeholder Upgrade

---
id: TASK-27E
title: Upgrade /alliance from placeholder to context-aware read-only cockpit
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Turn /alliance from a placeholder route into a context-aware read-only cockpit entry point.

## Current problem
Alliance route currently lacks real context loading and reads like an unfinished placeholder.

## Context from current implementation
All accepted cockpits use route helpers and lazy loading. Alliance should reuse that flow and preserve global route behavior.

## Goal
Add a lazy-loaded Alliance page with required route context and safe no-context states.

## Files to inspect first
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts (if helper missing)

## Implementation requirements
- Add or verify /alliance route registration.
- Ensure route supports civilizationId query param.
- Lazy-load AlliancePage with existing Suspense loading pattern.
- Ensure App.tsx does not eager-import AlliancePage.
- Include no-context fallback messaging.
- Add links:
- Volver a Galaxia
- Abrir Mercado
- Abrir Espionaje
- Abrir Ranking if placeholder route exists
- Make page title "Alianzas".

## UI/UX requirements
- Page should feel like a real cockpit scaffold, not a generic placeholder.
- Spanish-first copy.
- Diagnostics collapsed by default.

## Backend/API requirements
- Consume existing or newly added Alliance read endpoint.
- No backend mutation changes for routing.

## Safety constraints
- No alliance creation, no messaging, no mutation hooks.
- Preserve lazy route guard compliance.

## Acceptance criteria
- /alliance resolves with civilizationId.
- Route is lazy-loaded.
- Shared loading fallback is preserved.
- Existing route guards still pass.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If /alliance route helper is missing, add it in a way consistent with existing helper naming and casing.

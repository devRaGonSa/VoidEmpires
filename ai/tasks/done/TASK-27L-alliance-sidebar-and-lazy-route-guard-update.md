# TASK-27L Alliance Sidebar And Lazy Route Guard Update

---
id: TASK-27L
title: Update sidebar state and lazy-route checks for Alliance
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Wire /alliance into navigation and guard rails without breaking existing lazy route guarantees.

## Current problem
Route-level lazy loading and sidebar status are expected to reflect the new Alliance cockpit and keep future modules untouched.

## Context from current implementation
Sidebar status/visibility and route import checks are used across cockpit changes and should remain deterministic.

## Goal
Mark Alliance as implemented read-only cockpit and keep /alliance lazy-loaded in App shell checks.

## Files to inspect first
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/utils/routeUrls.ts
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify
- src/VoidEmpires.Frontend/src/components/ui/SidebarNav.tsx
- src/VoidEmpires.Frontend/src/App.tsx
- scripts/check-frontend-route-lazy-imports.ps1

## Implementation requirements
- Ensure Sidebar reflects Alliance as read-only implemented cockpit if current model supports status badges.
- Ensure active nav state works for /alliance.
- Ensure /alliance remains lazy-loaded in route setup.
- Update route-lazy guard exceptions/allowlist if needed for Alliance.
- Keep Ranking as future/placeholder in nav or doc as currently intended.

## UI/UX requirements
- Spanish-first nav copy.
- Alliance should appear available for navigation while clearly non-transactional.

## Backend/API requirements
- None.

## Safety constraints
- Preserve route helper behavior.
- No eager loading regression.
- No gameplay action links in sidebar.

## Acceptance criteria
- Sidebar and route checks include Alliance appropriately.
- Lazy import guard passes.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If sidebar data schema cannot encode foundation vs active, add the smallest safe marker and document it.

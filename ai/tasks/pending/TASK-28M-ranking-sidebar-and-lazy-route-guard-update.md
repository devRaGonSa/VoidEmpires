# TASK-28M Ranking Sidebar And Lazy Route Guard Update

---
id: TASK-28M
title: Reflect Ranking in sidebar and guard scripts without lazy-load regression
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: medium
---

## Purpose
Align sidebar navigation and lazy route guard expectations with Ranking becoming an implemented read-only cockpit.

## Current problem
The system currently marks Ranking as placeholder-only and needs read-only implementation status updates.

## Context from current implementation
Sidebar and guard checks are part of existing route health and should be updated consistently.

## Goal
Update UI navigation state and lazy-import guard metadata for Ranking.

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
- Mark Ranking as implemented/read-only cockpit in sidebar metadata where supported.
- Ensure active state for `/ranking` works.
- Keep `RankingPage` lazy-loaded in App route config.
- Update lazy-import guard allowlist if needed for the new route.
- Do not reintroduce eager `RankingPage` imports.

## UI/UX requirements
- Spanish-first label.
- Make Ranking discoverable but clearly foundational.
- No action-like treatment for disabled module.

## Backend/API requirements
- None.

## Safety constraints
- No route removal.
- No lazy-load regression.
- No gameplay mutation links.

## Acceptance criteria
- Sidebar and route guard checks pass.
- Existing route behavior unchanged.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1

## Notes / residual risks
- If sidebar schema cannot represent read-only state, document this in task 28N and keep minimal marker additions.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

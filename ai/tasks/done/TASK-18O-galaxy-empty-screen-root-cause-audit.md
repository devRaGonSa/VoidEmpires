# TASK-18O-galaxy-empty-screen-root-cause-audit

---
id: TASK-18O-galaxy-empty-screen-root-cause-audit
title: Galaxy empty screen root cause audit
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - backend
  - docs
roadmap_item: "Block 18O-19D - Galaxy cockpit regression fix after cockpit-validation seeds"
priority: high
---

## Goal
Find and document the exact cause of the current Galaxy empty-screen regression after `cockpit-validation`.

## Purpose
Remove ambiguity before implementation so follow-up tasks fix the real defect instead of layering fallback UI over an unknown failure mode.

## Current Problem
The Galaxy route now renders only the shared shell and generic development framing instead of the accepted strategic cockpit. The break could be route wiring, query parsing, read-state fetch, render guards, missing seed data, endpoint mismatch, or a hidden runtime exception.

## Context
- Other `cockpit-validation` routes still load, so the seed profile is not globally broken.
- Current docs and helpers still describe Galaxy primarily on `/`, while the reported regression references `/galaxy`; this mismatch must be audited explicitly.
- `StrategicMapPage.tsx` already contains rich cockpit content, so a route or state path may now be bypassing it.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/App.tsx`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/components/StrategicMap2DView.tsx`
- `src/VoidEmpires.Frontend/src/utils/routeUrls.ts`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `docs/dev/strategic-map-cockpit-checklist.md`

## Implementation Requirements
1. Reproduce the empty render condition by code inspection and, if possible, by a targeted automated check.
2. Trace the full Galaxy flow:
   - route match in `App.tsx`
   - `/` versus `/galaxy` behavior
   - `civilizationId` query parsing
   - default or fallback civilization handling
   - frontend API call and endpoint selection
   - response success/error handling
   - loading, missing-context, error, empty, and success render branches
   - final cockpit render path
3. Determine whether a React runtime error or thrown request failure is being swallowed behind the shell.
4. Determine whether `/galaxy` is supposed to be an alias, the new canonical route, or an unsupported path that now needs normalization.
5. Determine whether `cockpit-validation` currently returns valid strategic data for the seeded civilization.
6. Record the confirmed root cause and reproduction notes in `docs/dev/strategic-map-cockpit-checklist.md` or an equivalent short development note.
7. Do not redesign the Galaxy page in this task unless the safe fix is truly trivial.

## UI/UX Requirements
- If an invisible error state exists, identify exactly where the later task should surface it.
- Distinguish between blank output caused by missing context and blank output caused by failed data load.

## Backend/API Requirements
- No backend change unless the audit proves the strategic read model is empty or broken.

## Safety Constraints
- Keep Galaxy read-only.
- No mutation endpoints.
- No 3D or renderer-scope expansion.

## Expected Files to Modify
- `docs/dev/strategic-map-cockpit-checklist.md`
- audit-adjacent tests or docs only if needed to prove the root cause

## Acceptance Criteria
- The exact regression cause is known.
- `/`, `/galaxy`, and query-context behavior are explicitly distinguished.
- The next implementation tasks can proceed from documented facts instead of guesswork.
- Validation remains green.

## Validation
- `dotnet build --no-restore` if backend code changes
- `dotnet test --no-build` if backend or tests change
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend code changes

## Notes / Residual Risks
- Do not treat a route mismatch alone as the full answer until the seeded strategic read model is also verified.
- The current docs still point to `/`; if that is stale, update later tasks accordingly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep this task audit-first and narrowly scoped.

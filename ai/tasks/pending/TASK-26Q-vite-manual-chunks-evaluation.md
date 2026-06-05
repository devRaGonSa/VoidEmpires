# TASK-26Q Vite Manual Chunks Evaluation

---
id: TASK-26Q
title: Evaluate whether Vite manual chunks are needed after lazy loading
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Evaluate whether route-level lazy loading alone resolves the bundle warning, and only introduce `manualChunks` if the build output still needs a stable additional split.

## Current problem
Manual chunking can help, but it can also overcomplicate builds and future maintenance if route-based code splitting already provides enough improvement.

## Context from current implementation
Vite currently warns about an oversized main chunk. After cockpit routes are lazy-loaded, the build output should be re-evaluated based on observed results rather than assumptions.

## Goal
Make an evidence-based decision on whether to keep Vite defaults or add a minimal `manualChunks` configuration.

## Implementation steps
1. Run the frontend build after route-level lazy loading is in place.
2. Check whether the chunk-size warning still appears.
3. If the warning is gone, document that no manual chunking is needed.
4. If the warning remains, inspect the output and introduce only minimal, stable chunk rules where clearly beneficial.
5. Document the decision and rationale.

## Files to inspect first
- src/VoidEmpires.Frontend/vite.config.ts
- src/VoidEmpires.Frontend/package.json
- src/VoidEmpires.Frontend/src/App.tsx
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/frontend-performance-notes.md

## Expected files to modify
- src/VoidEmpires.Frontend/vite.config.ts
- docs/dev/frontend-performance-notes.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Run `npm run build --prefix src/VoidEmpires.Frontend` after lazy loading.
- Check whether the chunk warning above 500 kB remains.
- If the warning is gone:
- do not add `manualChunks`
- document the outcome
- If the warning remains:
- inspect the build output
- add minimal `manualChunks` only if stable and useful
- Possible chunk themes include vendor React dependencies, cockpit pages, or API-related code, but only if the observed output supports it.
- Avoid brittle per-file chunk rules when route splitting already provides the main benefit.

## Frontend requirements
- Preserve current source-map and build behavior unless a change is genuinely needed.
- No runtime behavior change beyond chunk boundaries.

## Backend/API requirements
- None.

## Safety constraints
- Do not silence the warning merely by increasing `chunkSizeWarningLimit` unless that is explicitly documented as the final fallback and justified.
- Prefer real splitting over cosmetic warning suppression.

## Acceptance criteria
- A documented decision exists for whether `manualChunks` is needed.
- Build output is improved or the remaining warning is explicitly justified.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- Chunk naming and grouping should remain stable enough for developers to reason about build output in future validations.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

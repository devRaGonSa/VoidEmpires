# TASK-26M Frontend Bundle Baseline And Import Audit

---
id: TASK-26M
title: Audit frontend bundle baseline and eager route imports
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Audit the current frontend bundle structure, eager page imports, and the exact source of the repeated Vite chunk-size warning before changing the route-loading architecture.

## Current problem
The frontend build passes, but Vite repeatedly warns that the main chunk is larger than 500 kB. With the accepted cockpit suite now spanning many pages, the current route layer likely imports too much code eagerly into the initial bundle.

## Context from current implementation
The current frontend already ships multiple accepted cockpit routes, but users do not need every cockpit at first load. This task establishes the baseline needed to apply route-level lazy loading without changing gameplay behavior, API behavior, Spanish-first UX, or accepted QA flows.

## Goal
Produce a precise baseline of the current bundle output, current eager imports, and likely lazy-loading candidates so follow-up tasks can split code safely and measurably.

## Implementation steps
1. Run or inspect the current frontend production build output.
2. Record the main chunk size, CSS output size, and whether the chunk-size warning appears.
3. Inspect `App.tsx`, route setup, and global shell imports to list page-level components that are currently loaded eagerly.
4. Distinguish globally shared shell dependencies from page-specific modules that should remain page-local.
5. Document the baseline and target splitting approach in the existing frontend smoke checklist or a focused performance note.

## Files to inspect first
- ai/architecture-index.md
- ai/current-state.md
- src/VoidEmpires.Frontend/package.json
- src/VoidEmpires.Frontend/vite.config.ts
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/main.tsx

## Expected files to modify
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/frontend-performance-notes.md
- ai/current-state.md

## Implementation requirements
- Run or inspect `npm run build --prefix src/VoidEmpires.Frontend`.
- Identify the current main chunk size and whether the warning appears.
- Identify all page-level imports that are eager in `App.tsx` or equivalent route setup.
- Identify shared modules that should stay in the synchronous shell.
- Identify obvious heavy page-specific modules that are imported globally today.
- Document the recommended target approach for the following tasks.
- Do not change runtime behavior in this task unless the change is trivial and purely documentary.

## Frontend requirements
- No UI redesign.
- No route-path changes.
- No query-param behavior changes.

## Backend/API requirements
- None.

## Safety constraints
- No gameplay changes.
- No backend changes.
- Do not hide the warning without evidence-driven follow-up work.
- Keep the change within documentation and low-risk audit scope.

## Acceptance criteria
- The current bundle warning context is documented.
- Candidate route-lazy pages are listed explicitly.
- Shared synchronous shell modules are identified.
- Frontend build remains green.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`
- `dotnet build --no-restore`
- `dotnet test --no-build` if `ai/current-state.md` or other validated state docs are updated

## Notes / residual risks
- This task is intentionally baseline-only and should not overclaim performance improvements.
- If build output differs from the attached request, record the actual observed numbers rather than the historical reference values.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.
- Split follow-up documentation if the baseline write-up grows too large.

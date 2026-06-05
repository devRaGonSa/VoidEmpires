# TASK-26T Add Lightweight Route Import Regression Check If Feasible

---
id: TASK-26T
title: Add a lightweight guard against reintroducing eager route imports
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Add a lightweight static guard, or a documented equivalent, so future tasks do not accidentally reintroduce direct eager cockpit page imports into the route layer.

## Current problem
After lazy loading is introduced, a later change could quietly reintroduce direct page imports in `App.tsx` and undo the main bundle improvement.

## Context from current implementation
The repository already includes PowerShell-based validation helpers. A small script could fit the existing workflow if it remains simple and not overly brittle.

## Goal
Create either a lightweight PowerShell route-import check or a clearly documented manual guardrail for keeping cockpit route imports lazy-loaded.

## Implementation steps
1. Evaluate whether a small script can reliably detect direct imports of protected route pages.
2. If feasible, add `scripts/check-frontend-route-lazy-imports.ps1`.
3. Keep the script narrow, readable, and aligned with the current route file structure.
4. If the script would be too brittle, document the guardrail in developer docs instead.
5. Validate any added script manually.

## Files to inspect first
- scripts/check-dev-qa-scripts.ps1
- src/VoidEmpires.Frontend/src/App.tsx
- docs/dev/frontend-foundation-smoke-checklist.md
- ai/current-state.md

## Expected files to modify
- scripts/check-frontend-route-lazy-imports.ps1
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1

## Implementation requirements
- If feasible without heavy dependencies, add `scripts/check-frontend-route-lazy-imports.ps1`.
- The script should check `App.tsx` for direct imports of large page components that should remain lazy-loaded.
- It should not be overly brittle or require complex parsing.
- It should allow accepted direct imports such as `AppShell`, shared fallback components, and route helpers.
- If a script is added, include it in validation docs.
- Do not integrate it into package scripts unless that is trivial and consistent with repo conventions.
- If a script is not feasible, document the guardrail instead and explain why.

## Frontend requirements
- No runtime behavior change.

## Backend/API requirements
- None.

## Safety constraints
- No heavy dependencies.
- No CI assumptions unless the repository already has an obvious place for the check.
- Keep the check understandable for future maintainers.

## Acceptance criteria
- A lightweight guard script exists, or an explicit documented guardrail exists if scripting is not feasible.
- Existing checks continue to pass.

## Validation
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `npm run build --prefix src/VoidEmpires.Frontend`
- Run the new route-import script if it is added

## Notes / residual risks
- Regex-based checks can become brittle if the route file is heavily refactored. Prefer a narrow rule set over a false sense of safety.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

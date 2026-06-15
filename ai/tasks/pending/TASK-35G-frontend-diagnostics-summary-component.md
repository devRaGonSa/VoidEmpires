# TASK-35G

---
id: TASK-35G
title: Frontend diagnostics summary component
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: medium
---

## Goal
Add a reusable collapsed diagnostics summary component for Development pages.

## Context
Raw ids and backend payloads should remain secondary. A reusable diagnostics panel can keep technical information accessible without polluting primary gameplay UI.

## Implementation steps

1. Review existing component and style conventions.
2. Add or reuse a component such as `src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx`.
3. Ensure the component:
   - is collapsed by default;
   - shows an explicit Development/diagnostics label;
   - supports key-value summaries;
   - optionally shows raw payload only when expanded;
   - avoids primary gameplay styling.
4. Use Spanish-first copy.
5. Do not add the component to pages yet unless required for compile.
6. Do not change gameplay behavior.

## Files to read first

- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/package.json

## Expected files to modify

- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Reusable diagnostics component compiles.
- Component is collapsed by default.
- Raw payload support is secondary/expanded only.
- No gameplay behavior changes.
- Frontend build passes.

## Constraints

- Do not redesign pages broadly.
- Do not expose diagnostics as primary UI.
- Do not mutate backend state.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35G message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

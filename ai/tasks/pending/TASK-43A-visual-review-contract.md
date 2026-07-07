# TASK-43A-visual-review-contract

---
id: TASK-43A
title: Visual review contract
status: pending
type: docs
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Document the user's visual review findings and the corrected UI contract for the OGame-like core game interface.

## Context
The current UI still feels too much like an internal orchestrator or productivity dashboard. Block 43 should make the existing game pages feel like a browser strategy game without adding advanced gameplay systems.

## Implementation steps

1. Read the architecture index, component discovery guide, current state, and product readiness docs.
2. Document that login and registration must be standalone public pages outside the game shell.
3. Document that Inicio becomes the authenticated current planet overview.
4. Document whether Planeta should be merged, redirected, or aliased to Inicio when appropriate.
5. Document that Construction, Research, Shipyard, Defense, and Ground Army pages must be focused action catalogs.
6. Record forbidden player-facing concepts: `cabina`, `contexto guardado`, `dar contexto`, `cargar mando`, `siguientes cabinas`, `registrar comandante` inside gameplay pages, generic `continuar` gameplay CTA, and duplicated module navigation cards.

## Files to read first

- AGENTS.md
- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/no-visible-development-report.md
- docs/dev/cockpit-copy-guidelines.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/components/ui/AppShell.tsx

## Expected files to modify

- docs/dev/product-readiness-report.md
- docs/dev/no-visible-development-report.md
- docs/dev/cockpit-copy-guidelines.md
- docs/dev/visual-state-sandbox.md

## Acceptance criteria

- The visual review findings are documented.
- Public auth layout, authenticated game shell, Inicio/Planeta decision, and action catalog requirements are documented.
- Forbidden normal UI concepts are recorded.
- No behavior change is made in this task.

## Constraints

- Documentation only.
- Do not claim browser or manual QA.
- Keep UI language direction Spanish-first.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

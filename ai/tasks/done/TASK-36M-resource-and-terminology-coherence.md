# TASK-36M

---
id: TASK-36M
title: Resource and terminology coherence
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: medium
---

## Goal
Normalize visible terminology for resources and state labels.

## Context
The UI currently mixes or has historically shown terms such as Credits, Créditos, Metal, Crystal, Cristal, Gas, Deuterio, Energía, and Población. Primary UI should be coherent and avoid fake resource bars.

## Implementation steps

1. Audit visible resource labels across frontend, docs, and scripts.
2. Decide and document current v1 terminology.
3. If Credits and Energy are different concepts, make that distinction clear in UI/docs.
4. If the header cannot show real resources, avoid showing fake resource bars.
5. Ensure scripts/docs/frontend do not expose raw resource ids in normal UI.
6. Keep raw technical info in diagnostics only.
7. Map common ids to readable names in diagnostics where feasible.
8. Update `resourceDisplay` helper if needed.
9. Update copy regression guard if useful.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- Optional: src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Resource terminology is consistent in primary UI.
- No backend value changes occur.
- Fake resource bars are removed or avoided.
- Frontend build and copy guard pass.

## Constraints

- Do not change backend resource values or queue semantics.
- Do not fake resources in frontend.
- Keep raw resource ids secondary/diagnostic.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36M message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

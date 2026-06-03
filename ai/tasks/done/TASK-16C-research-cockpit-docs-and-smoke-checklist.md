# TASK-16C-research-cockpit-docs-and-smoke-checklist

---
id: TASK-16C-research-cockpit-docs-and-smoke-checklist
title: Research cockpit docs and smoke checklist
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 15M-16D - Research cockpit playable foundation v1"
priority: medium
---

## Goal
Document Research cockpit behavior, manual QA and intentional exclusions.

## Purpose
Future tasks should not have to guess what Research supports. The docs need to state the expected behavior and the safe boundaries clearly.

## Current Problem
Without docs, later tasks may overclaim support or accidentally wire unsupported effects.

## Context
- The repo already has smoke checklists and module boundary docs.
- Research needs the same practical documentation layer.

## Files to Inspect First
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Create or update `docs/dev/research-cockpit-checklist.md`.
2. Include a seeded QA URL:
   - `/research?civilizationId=00000000-0000-0000-0000-000000000001&planetId=40000000-0000-0000-0000-000000000001`
3. Include checks for:
   - page loads;
   - context shows Aurelia and Helios Gate;
   - catalog shows Spanish technology names;
   - categories are meaningful;
   - available research is visible;
   - blocked research shows reason;
   - queue panel works;
   - confirmation appears before enqueue if enabled;
   - success refreshes queue if enqueue is enabled;
   - complete-due is enabled only if safe, otherwise placeholder;
   - diagnostics collapsed;
   - no 3D or WebGL;
   - no production auth;
   - no real effects over fleets, combat or espionage.
4. Update the frontend foundation smoke checklist with Research.
5. Update the module boundaries doc only if needed.

## UI/UX Requirements
- Docs should be practical, not theoretical.
- Include exact URLs and exact expectations when possible.
- Keep phrasing aligned with the user-facing Spanish cockpit terms.

## Backend/API Requirements
- No backend change.

## Safety Constraints
- Do not document unsupported effects as implemented.
- Do not imply production auth or real gameplay progression beyond the safe dev scope.

## Expected Files to Modify
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-module-boundaries.md` only if a small context note is needed

## Acceptance Criteria
- Research QA can be performed by the user.
- Docs reflect the real state.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Browser QA may remain manual if the Codex browser is unavailable.
- Keep the checklist short enough to use during a live smoke test.
- The docs should help future blocks avoid unsafe assumptions.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer one new checklist plus small cross-doc updates.

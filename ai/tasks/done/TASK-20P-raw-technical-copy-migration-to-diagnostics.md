# TASK-20P-raw-technical-copy-migration-to-diagnostics

---
id: TASK-20P-raw-technical-copy-migration-to-diagnostics
title: Raw technical copy migration to diagnostics
status: done
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Move raw technical phrases from primary cockpit UI into collapsed diagnostics.

## Purpose
Keep the technical signal available for developers without letting raw DTO, endpoint, payload, or capability wording dominate gameplay-facing cards and callouts.

## Current Problem
Primary UI still exposes raw technical wording in places, including DTOs, payloads, endpoint, mutation, dev route, affordance, build, and raw technical ids. These are useful for development but should not dominate the experience of a first demo.

## Context
- Every accepted cockpit already has, or can have, diagnostics sections.
- The goal is not to remove technical information, but to move it into a consistent secondary layer.
- This task should reuse existing diagnostics patterns rather than inventing a new debug surface.

## Files to Inspect First
- all accepted cockpit pages
- diagnostics patterns in Research, Shipyard, Defenses, and Ground Army
- `src/VoidEmpires.Frontend/src/styles.css`

## Implementation Requirements
1. Search frontend source for terms such as:
   - `DTO`
   - `payload`
   - `endpoint`
   - `mutation`
   - `affordance`
   - `raw`
   - `dev`
   - `build`
   - `capability`
2. For each meaningful occurrence, decide whether it should:
   - stay in diagnostics
   - be rewritten as gameplay copy
   - remain only in documentation or a clearly secondary dev panel
3. Apply safe replacements such as:
   - `mutacion` -> `accion`
   - `endpoint` -> `servicio` or `ruta de desarrollo` only when still needed
   - `payload` -> `datos tecnicos`
   - `build` -> `esta version`
   - `raw` -> remove or diagnostics-only
4. Keep exact technical details in collapsed diagnostics where they still help QA.
5. Do not remove safety warnings that explain why an action is unavailable.

## UI/UX Requirements
- Primary UI should become noticeably less technical.
- Diagnostics must remain available and useful.
- Spanish-first wording throughout the visible primary layer.

## Backend/API Requirements
- None.

## Safety Constraints
- No behavior changes.
- No mutation-surface changes.

## Expected Files to Modify
- targeted accepted cockpit pages
- possibly a shared diagnostics helper or styles file
- `src/VoidEmpires.Frontend/src/styles.css`

## Acceptance Criteria
- Major raw technical terms are removed from primary cockpit surfaces where safe.
- Diagnostics still preserve the relevant developer details.
- Frontend build passes.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Some development route text may remain intentionally technical if it is clearly secondary.
- Avoid forcing euphemistic copy that hides real limitations.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on migration of visible wording, not broad layout work.

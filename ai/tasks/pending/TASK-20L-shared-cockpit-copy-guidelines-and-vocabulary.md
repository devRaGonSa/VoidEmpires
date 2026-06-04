# TASK-20L-shared-cockpit-copy-guidelines-and-vocabulary

---
id: TASK-20L-shared-cockpit-copy-guidelines-and-vocabulary
title: Shared cockpit copy guidelines and vocabulary
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Create a shared vocabulary and copy rules for cockpit UI language.

## Purpose
Give all accepted cockpits one common language so similar states and actions stop reading differently depending on the page where the user happens to be.

## Current Problem
Different cockpits currently use different wording for similar concepts, including available versus preparada, blocked versus no disponible, queue versus cola versus ordenes, complete-due versus cierre, and multiple handoff terms. This makes the current demo feel assembled module by module instead of designed as one surface.

## Context
- Research, Shipyard, Defenses, and Ground Army each evolved their own terminology during earlier blocks.
- The app now needs one consistent vocabulary that future cockpit work can reuse.
- This task should establish standards first, not force a risky mass rewrite in one pass.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/utils/`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/*cockpit-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Add or update a shared documentation section with preferred terms such as:
   - `Disponible`
   - `Bloqueada`
   - `En cola`
   - `En curso`
   - `Completada`
   - `Cierre no disponible`
   - `Preparar orden`
   - `Revisar orden`
   - `Confirmar`
   - `Abrir cabina`
   - `Volver a Galaxia`
   - `Diagnostico secundario`
2. Define terms to avoid in primary UI, including:
   - `endpoint`
   - `DTO`
   - `payload`
   - `affordance`
   - `raw`
   - `mutation`
   - `build`
   - `dev route`
   except when they appear in diagnostics or documentation.
3. Define allowed limitation language examples, such as:
   - `Esta accion todavia no esta habilitada desde esta cabina.`
   - `El cierre automatico permanece fuera de esta superficie.`
   - `La resolucion de combate no forma parte de esta version.`
4. Clarify how gameplay copy, limitation copy, diagnostics, and docs should differ.
5. Avoid forcing every replacement in this task; the main deliverable is the vocabulary reference.

## UI/UX Requirements
- Spanish-first.
- Clear distinction between gameplay copy and diagnostics.
- Copy rules should optimize for player comprehension without hiding useful developer context.

## Backend/API Requirements
- None.

## Safety Constraints
- No behavior changes.
- No backend changes.

## Expected Files to Modify
- shared UX vocabulary note under `docs/dev/` or an expanded section in `docs/dev/frontend-foundation-smoke-checklist.md`
- optionally a small related doc reference update if needed for discoverability

## Acceptance Criteria
- Shared cockpit vocabulary is documented in the repo.
- Later UI tasks can reference a stable set of preferred terms.
- Validation passes for any touched files.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched
- `dotnet build --no-restore` and `dotnet test --no-build` only if required by repo workflow around touched docs or tests

## Notes / Residual Risks
- This task is documentation-first.
- Some older wording may remain until later cleanup tasks apply the vocabulary page by page.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task focused on vocabulary and guidance.

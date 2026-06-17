# TASK-36A

---
id: TASK-36A
title: UI information architecture audit
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Audit the current UI information architecture across the playable loop and document what must be removed, consolidated, or moved.

## Context
Recent manual inspection found repeated session, resource, diagnostics, navigation, and Development status information across Planet and cockpit pages. This task defines the cleanup target before code changes.

## Implementation steps

1. Inspect Onboarding, Planet, Construction, Research, Shipyard, Defenses, Fleets, and the sidebar/header/app shell.
2. Identify duplicated information:
   - session;
   - planet;
   - civilization;
   - resources;
   - Development status;
   - backend endpoint details;
   - diagnostics;
   - navigation links.
3. Identify obsolete copy:
   - `solo lectura` on pages that now mutate;
   - `no ejecutan mutaciones`;
   - old backend/prototype language;
   - old mock resource/header copy.
4. Define target page hierarchy:
   - global shell;
   - compact session/header context;
   - page-specific primary action;
   - queue/progress;
   - secondary Development tools;
   - collapsed diagnostics.
5. Document the decluttering scope in the frontend and persisted gameplay checklists.
6. Make no code behavior changes.

## Files to read first

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md
- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify

- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/persisted-gameplay-flow-checklist.md

## Acceptance criteria

- UI decluttering scope is documented.
- Duplicated and obsolete primary UI content is identified.
- Target page hierarchy is documented.
- No gameplay behavior is changed.
- Validation passes.

## Constraints

- Do not perform or claim full visual QA.
- Do not add gameplay systems or change backend rules.
- Do not remove diagnostics entirely; plan to move them secondary/collapsed.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36A message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

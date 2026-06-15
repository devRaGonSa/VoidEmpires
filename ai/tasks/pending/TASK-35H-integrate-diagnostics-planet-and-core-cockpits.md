# TASK-35H

---
id: TASK-35H
title: Integrate diagnostics into Planet and core cockpits
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Integrate collapsed diagnostics safely into core loop pages.

## Context
Planet, Construction, Research, Shipyard, and possibly Onboarding should expose technical context in a secondary collapsed area while keeping primary UI Spanish-first and user-facing.

## Implementation steps

1. Add the diagnostics component to:
   - Planet;
   - Construction;
   - Research;
   - Shipyard;
   - Onboarding if useful after success.
2. Keep diagnostics secondary and collapsed by default.
3. Do not display raw ids in primary UI.
4. Do not mutate state from diagnostics.
5. Do not fetch diagnostics in a way that breaks normal seeded flow.
6. Prefer existing page payloads where possible instead of unnecessary requests.
7. If calling the diagnostics endpoint, handle failure quietly in the secondary diagnostics area.

## Files to read first

- src/VoidEmpires.Frontend/src/components/DevDiagnosticsPanel.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx

## Acceptance criteria

- Core pages have safer and clearer collapsed diagnostics.
- Primary UI is not polluted by raw ids or raw payloads.
- Diagnostics do not mutate state.
- Frontend build and copy guard pass.

## Constraints

- Do not perform or claim visual QA.
- Do not add optimistic updates.
- Preserve lazy loading.
- Keep raw payloads collapsed/secondary.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35H message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

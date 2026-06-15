# TASK-35I

---
id: TASK-35I
title: Empty error loading state hardening
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 35A-35P - Playable Loop Hardening, Diagnostics & Deferred Visual QA Prep v1"
priority: high
---

## Goal
Harden empty, loading, and error states across the playable loop.

## Context
Pages should communicate missing ids, local session recovery, backend failures, blocked orders, no-op materialization, and read-only scope clearly in Spanish without hiding backend errors.

## Implementation steps

1. Review state handling in:
   - Onboarding;
   - Planet;
   - Construction;
   - Research;
   - Shipyard;
   - Defenses;
   - Fleets.
2. Ensure each page has clear Spanish states for:
   - missing ids;
   - local session available;
   - backend unavailable;
   - no available actions;
   - open order blocking;
   - materialization no-op;
   - read-only scope.
3. Keep raw technical details in diagnostics.
4. Do not hide backend errors.
5. Avoid broad visual redesign.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx

## Acceptance criteria

- User-facing states are clearer.
- Backend errors remain visible in appropriate UI/diagnostics.
- Raw details remain secondary.
- Frontend build and copy guard pass.

## Constraints

- Do not fake resources, buildings, research, queues, or stock.
- Do not optimistic-update authoritative game state.
- Do not perform broad visual redesign.
- Do not claim visual QA.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-35I message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split page groups if the change exceeds budget.

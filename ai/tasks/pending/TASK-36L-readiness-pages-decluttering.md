# TASK-36L

---
id: TASK-36L
title: Readiness pages decluttering
status: pending
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: medium
---

## Goal
Declutter read-only/readiness pages so they do not look like unfinished noisy dashboards.

## Context
Defenses and Fleets remain read-only/readiness. Related preparation pages should be honest about scope without presenting primary-looking unavailable actions.

## Implementation steps

1. Apply decluttering to Defenses and Fleets.
2. Optionally include Espionage, Market, Alliance, and Ranking if the same shell copy is repeated.
3. Ensure each read-only page clearly shows:
   - what it currently displays;
   - what is intentionally not active;
   - next expected gameplay integration if useful;
   - no primary-looking unavailable action.
4. Remove repeated obsolete dev copy.
5. Keep read-only status where true.
6. Keep diagnostics secondary.
7. Do not add mutations.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/pages/RankingPage.tsx
- scripts/check-frontend-route-lazy-imports.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Frontend/src/pages/FleetsPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- Optional: src/VoidEmpires.Frontend/src/pages/RankingPage.tsx

## Acceptance criteria

- Readiness pages are clearer and honestly scoped.
- Read-only labels remain accurate.
- No new mutations are added.
- Frontend build and route/copy guards pass.

## Constraints

- Do not add defense production, fleet movement, missions, combat, market transactions, or alliance mutations.
- Preserve lazy loading.
- Keep diagnostics secondary.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36L message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

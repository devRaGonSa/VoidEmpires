# TASK-52C

---
id: TASK-52C
title: Correct queue countdown and finalization presentation
status: done
type: full-stack
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 52"
priority: high
---

## Goal
Render one live Construction/Research countdown per active order and refresh due orders once so backend materialization removes completed rows.

## Context
Construction and Research rows can duplicate `finalizando...` or retain expired rows. The shared LiveQueueCountdown must own countdown/finalization text and invoke the existing safe reload at most once per order.

## Implementation steps

1. Audit shared countdown behavior and Construction/Research row composition.
2. Remove duplicate due text and keep status separate from remaining time.
3. Verify GameplayRefreshService materializes due orders before active UI-state reads return.
4. Add focused timer/guard and backend due-materialization coverage.
5. Record final Block 52 behavior and validation state.

## Files to read first

- src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs
- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- scripts/check-frontend-copy-regressions.ps1
- src/VoidEmpires.Frontend/src/utils/countdown.ts
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Infrastructure/Gameplay/GameplayRefreshService.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx
- src/VoidEmpires.Frontend/src/styles.css
- scripts/check-frontend-copy-regressions.ps1
- ai/tasks/done/TASK-52C-queue-countdown-finalization.md

## Acceptance criteria

- Future timestamps render a once-per-second HH:MM:SS countdown with steady digits.
- Expiry renders exactly one `finalizando...` and invokes reload at most once per order.
- Status remains Pendiente/Activa and is not replaced by countdown text.
- Due Construction and Research reads materialize orders and omit them from active queues.
- Research completion upgrades or creates the ResearchProject.
- Refresh/materialization failure is not silently hidden in automated tests.

## Constraints

- Keep LiveQueueCountdown as the shared timer.
- Do not add continuous polling.
- Do not redesign Home or change independent queue-family rules.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-repo-secret-scan.ps1

## Commit and push

At the end, stage only intended files and commit with a focused message. Push the branch after full validation.

## Change Budget

- Keep each implementation slice focused and split follow-ups if the preferred limits are exceeded.
- Prefer fewer than 3 commits for this task.

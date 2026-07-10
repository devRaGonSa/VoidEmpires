# TASK-51A-frontend-enum-live-queues

---
id: TASK-51A
title: Frontend enum normalization and live queues
status: done
type: frontend
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 51 - Live queues and visible resource capacity"
priority: high
---

## Goal
Normalize frontend enum values and make active queue summaries/countdowns update reliably from backend-provided queue data.

## Context
Manual QA found Home missing construction activity and module countdowns staying static when enum values arrive numerically or as numeric strings.

## Implementation steps

1. Add shared frontend helpers for enum/status normalization.
2. Use the helpers in Home and queue summary rendering.
3. Replace static countdown text with a shared live countdown component/hook.
4. Ensure expiration triggers at most one safe reload callback.

## Files to read first

- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/utils/countdown.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/countdown.ts
- src/VoidEmpires.Frontend/src/utils/enumNormalization.ts
- src/VoidEmpires.Frontend/src/components/QueueSummaryPanels.tsx
- src/VoidEmpires.Frontend/src/pages/HomePage.tsx
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx

## Acceptance criteria

- Pending and Active are recognized as strings, numeric strings, and numbers.
- Completed and Cancelled are never treated as active.
- Home shows construction when the selected planet has active construction.
- Countdown text updates once per second and shows `finalizando...` at expiry.
- Expiry reload callbacks run at most once per expiring item.

## Constraints

- Do not create fake queues.
- Do not add polling loops beyond the live countdown timer.
- Preserve existing layout and copy style.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

## Commit and push

Handled by the final Block 51 closure task.

## Change Budget

- Prefer fewer than 5 modified frontend files for this task.
- Split unrelated fixes into other Block 51 task files.

# TASK-52E

---
id: TASK-52E
title: Fix research active countdown and due materialization
status: done
type: full-stack
team: gameplay
supporting_teams: [frontend]
roadmap_item: "Block 52"
priority: critical
---

## Goal
Ensure future Research orders expose unambiguous UTC timestamps and live countdowns, while due orders materialize and disappear from the active queue response.

## Context
Manual browser QA found a future-looking active Research row rendering `finalizando...`. The fix must identify whether enqueue timestamps, backend materialization, serialization, or frontend parsing is responsible and preserve the one-expiration-callback guard.

## Implementation steps

1. Inspect the live Research UI-state response, persisted timestamps, UTC clock, and refresh warnings.
2. Trace enqueue, materialization, serialization, and countdown parsing to identify the concrete case.
3. Implement the smallest backend/frontend correction and a recoverable one-shot refresh failure state if needed.
4. Add focused regression coverage for future and due Research orders and countdown presentation.
5. Run the complete Block 52 validation gate and update current-state documentation.

## Files to read first

- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- src/VoidEmpires.Infrastructure/Gameplay/GameplayQueueMaterializationService.cs
- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx
- src/VoidEmpires.Frontend/src/utils/countdown.ts
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Web/DevResearchUiStateEndpoints.cs
- tests/VoidEmpires.Tests/ResearchQueueServiceTests.cs
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs
- ai/tasks/done/TASK-52E-research-active-countdown-materialization.md

## Acceptance criteria

- A known UTC enqueue produces a future UTC EndsAtUtc.
- A future order remains active and returns an unambiguous EndsAtUtc.
- A due order materializes before the UI-state query, reaches its ResearchProject target, becomes Completed, and is absent from the active queue.
- Future countdown formatting returns HH:MM:SS; duplicate or permanent `finalizando...` is prevented.
- Refresh/materialization failure is logged and has a concise recoverable UI path without polling.

## Constraints

- Stay on the existing Block 52 branch and update PR #65.
- Do not create a new branch or PR.
- Keep Home unchanged unless a shared timestamp correction naturally applies.
- Do not add repeated polling or weaken the one-expiration-callback guard.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-repo-secret-scan.ps1

## Commit and push

Commit the focused fix and push the existing branch so PR #65 updates.

## Change Budget

- Prefer modifying fewer than 5 implementation/test files.
- Prefer changes under 200 lines.
- Split a follow-up task if the investigation requires a broader correction.

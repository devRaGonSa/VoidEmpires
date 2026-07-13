# TASK-52F

---
id: TASK-52F
title: Harden research countdown parsing and expiry retry
status: done
type: frontend
team: frontend
supporting_teams: [gameplay]
roadmap_item: "Block 52"
priority: high
---

## Goal
Interpret UTC queue timestamps consistently and provide a concise manual retry when one-shot expiry refresh fails.

## Context
Task 52E fixes Research UI-state serialization. This follow-up keeps the frontend resilient to timezone-less legacy UTC values and prevents a failed one-shot refresh from leaving an unexplained permanent `finalizando...` row.

## Implementation steps

1. Parse timezone-less queue timestamps as UTC while preserving explicit offsets.
2. Surface a concise Research queue-refresh failure with a manual retry action.
3. Extend the existing frontend guard for future countdown formatting and duplicate-finalization prevention.
4. Update Block 52 current-state documentation and run full validation.

## Files to read first

- src/VoidEmpires.Frontend/src/utils/countdown.ts
- src/VoidEmpires.Frontend/src/components/LiveQueueCountdown.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/countdown.ts
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- scripts/check-frontend-copy-regressions.ps1
- ai/current-state.md
- ai/tasks/done/TASK-52F-research-countdown-retry-guard.md

## Acceptance criteria

- Future UTC timestamps format as HH:MM:SS, including timezone-less legacy values.
- Explicit UTC/offset timestamps retain their correct instant.
- Expiry still invokes at most one automatic callback per order.
- Failed expiry refresh exposes one concise manual retry without polling.
- Duplicate `finalizando...` row text remains forbidden.

## Constraints

- Stay on the existing branch and update PR #65.
- Do not create a branch or PR.
- Keep Home unchanged except for naturally shared UTC parsing.

## Validation

- Run all Block 52 validation commands.

## Commit and push

Commit and push to the existing Block 52 branch.

## Change Budget

- Prefer fewer than 5 implementation/guard files and under 200 lines.
- The shared Research view-model timestamp parser is included because it derives the same due state as the countdown.
- Prefer one commit.

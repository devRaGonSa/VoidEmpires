# TASK-36B

---
id: TASK-36B
title: App shell header reality alignment
status: pending
type: frontend
team: frontend
supporting_teams: []
roadmap_item: "Block 36A-36P - UI Information Architecture Audit & Decluttering v1"
priority: high
---

## Goal
Align the global app header with the current Development-safe playable-loop reality.

## Context
The header still shows disconnected mock economy/auth values such as `128.4k Metal`, `84.1k Cristal`, `Deuterio`, `Población`, `Energía +480`, and `RaulG`. These should not contradict backend-sourced playable session state.

## Implementation steps

1. Find the global header/top bar component or repeated header code.
2. Remove or clearly demote mock-only resource and identity values.
3. If current real playable session/resource context is already available safely, show concise real session values.
4. If real global context is not available, show compact status such as `Modo Development` or `Prototipo jugable local` instead of fake resources.
5. Avoid global heavy fetching or hidden mutations.
6. Do not imply production auth.
7. Keep Spanish-first copy.

## Files to read first

- src/VoidEmpires.Frontend/src/App.tsx
- src/VoidEmpires.Frontend/src/components/
- src/VoidEmpires.Frontend/src/utils/playableSession.ts
- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/App.tsx
- Optional: src/VoidEmpires.Frontend/src/components/
- Optional: src/VoidEmpires.Frontend/src/styles.css

## Acceptance criteria

- Header no longer looks like a disconnected mock economy/auth state.
- Header copy does not contradict current gameplay.
- No production auth claim is introduced.
- Frontend build and copy guard pass.

## Constraints

- Do not add hidden backend fetches or mutations.
- Do not fake resources.
- Preserve lazy loading and existing route behavior.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-36B message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.

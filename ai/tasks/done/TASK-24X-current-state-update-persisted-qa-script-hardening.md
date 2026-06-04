# TASK-24X

---
id: TASK-24X
title: Phase 24X - Current state update persisted QA script hardening
status: pending
type: platform
team: platform
supporting_teams:
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: medium
---

## Goal

Update `ai/current-state.md` after fixing the persisted QA runtime scripts.

## Purpose

Future orchestration needs to know that the persisted QA helpers were hardened against the real endpoint DTO shapes and that the baseline helper no longer crashes on the missing `.amount` assumption.

## Current problem

`ai/current-state.md` currently records the previous 24P state but does not yet reflect the runtime contract hardening work that follows from the real script failure.

## Files to read first

- `ai/current-state.md`
- `docs/dev/persisted-gameplay-flow-checklist.md`
- final script changes from Tasks 24R through 24W

## Implementation requirements

1. Record that persisted QA scripts were hardened against real endpoint DTO shapes.
2. Record the current validated test count.
3. Record that the baseline script no longer crashes on missing `.amount`.
4. Preserve the `Phase 24P` state while extending it accurately.
5. Do not overclaim production readiness.

## Backend/API requirements

- None.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not overstate runtime coverage if the scripts were only parser-checked
- Do not claim production readiness

## Acceptance criteria

- `ai/current-state.md` accurately reflects the script hardening result.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- The current-state entry should distinguish between parser validation and a real backend runtime check if only one of those was performed.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm the change is limited to `ai/current-state.md` and any tightly related doc alignment.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a concise continuity update over broader doc cleanup.

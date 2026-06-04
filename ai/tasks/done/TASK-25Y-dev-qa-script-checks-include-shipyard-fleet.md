# TASK-25Y

---
id: TASK-25Y
title: Phase 25Y - Dev QA script checks include Shipyard and Fleet
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: medium
---

## Goal

Extend the local PowerShell QA script checker so the Shipyard and Fleet helpers are parser-checked and lightly validated like the existing Construction and Research helpers.

## Current problem

New QA scripts are part of the repository workflow. If the checker does not include them, regressions can slip in even when backend code is unchanged.

## Context from current implementation

- `scripts/check-dev-qa-scripts.ps1` already parser-checks the current persisted QA helpers.
- The helper layer already includes resource-format logic and known no-op messaging that should stay consistent.
- This task should remain lightweight and not require a live backend.

## Files to read first

- scripts/check-dev-qa-scripts.ps1
- scripts/dev-qa-common.ps1
- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-fleet-read-state.ps1

## Expected files to modify

- scripts/check-dev-qa-scripts.ps1
- scripts/dev-qa-common.ps1
- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-fleet-read-state.ps1
- scripts/dev-qa-create-orbital-group-from-stock.ps1

## Implementation requirements

1. Update `scripts/check-dev-qa-scripts.ps1` so it parses:
   - `dev-qa-baseline.ps1`
   - `dev-qa-create-construction-order.ps1`
   - `dev-qa-create-research-order.ps1`
   - `dev-qa-create-shipyard-production-order.ps1` when added
   - `dev-qa-fleet-read-state.ps1` when added
   - `dev-qa-create-orbital-group-from-stock.ps1` when added
2. Add lightweight checks for helper behavior that does not require a backend, such as:
   - resource formatting
   - payload summary formatting
   - known queue or open-order messages
   - expected no-op shape handling when feasible
3. Keep the checker PowerShell 5.1 compatible.
4. Avoid turning the checker into a second implementation of the real scripts.

## Backend/API requirements

- None beyond preserving script compatibility with current Development contracts.
- Do not add backend runtime dependencies to the checker.

## Script/QA requirements

- The checker must succeed locally without a running backend.
- It should fail clearly when a new script has parser issues or breaks the expected helper surface.

## Safety constraints

- No live database access.
- No mutation.
- No reliance on shell features unavailable in PowerShell 5.1.

## Acceptance criteria

- The Shipyard and Fleet QA helpers are included in local script validation.
- The checker remains lightweight and backend-free.
- Validation passes.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If optional allocation is documented as out of scope, the checker should handle the script being absent without reporting a false failure.


# TASK-24Q

---
id: TASK-24Q
title: Phase 24Q - Baseline script runtime contract audit
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 24Q-24Z - Persisted QA scripts runtime contract hardening"
priority: high
---

## Goal

Audit the real JSON and PowerShell object shape returned by the Development endpoints consumed by `dev-qa-baseline.ps1`.

## Purpose

The baseline script currently assumes resource rows expose `.resourceType` and `.amount`, but the real response shape no longer matches that assumption. The repository needs an explicit audited contract before changing the scripts.

## Current problem

`dev-qa-baseline.ps1` fails at runtime because it formats Construction resources by directly reading `.amount` from rows that no longer expose that property in the expected path.

## Context

The persisted QA helpers were introduced in the previous block and are now part of the documented backend-only runbook. This corrective block should adapt the scripts to the real endpoint DTO shape instead of forcing backend DTOs to match an outdated script expectation.

## Files to read first

- `scripts/dev-qa-baseline.ps1`
- `scripts/dev-qa-create-construction-order.ps1`
- `scripts/dev-qa-create-research-order.ps1`
- `src/VoidEmpires.Web/DevPlanetUiStateEndpoints.cs`
- application or infrastructure DTOs used by planet, construction, and research UI state
- `tests/VoidEmpires.Tests/`

## Implementation requirements

1. Identify the actual object paths for:
   - Construction resources, stockpile, or reserve rows
   - Construction queue count
   - Construction available actions
   - Research resources or stockpile if present
   - Research queue count
   - Research available actions
2. Confirm the real property names used by the current DTOs and by `Invoke-RestMethod` materialization in PowerShell.
3. Record the findings in `docs/dev/persisted-gameplay-flow-checklist.md`.
4. Do not change behavior yet unless the fix is trivial and directly part of the audit outcome.

## Backend/API requirements

- Prefer docs and tests only for this task.
- Do not change backend DTOs just to fit the script unless the DTO is clearly wrong.

## Frontend/UI requirements

- None.

## Safety constraints

- Do not weaken endpoint validation
- Do not mutate read endpoints
- Do not introduce production behavior

## Acceptance criteria

- The real DTO and PowerShell object paths used by the QA scripts are documented.
- The root cause of the missing `.amount` access is explicit.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

If a PowerShell parser check exists during implementation, run it as part of the task.

## Notes / residual risks

- The backend response may be internally consistent while still being awkward for scripts. Later tasks should adapt the scripts first unless a backend contract bug is undeniable.

## Commit and push

1. Run `git status`.
2. Run `git diff --name-only`.
3. Confirm only intended docs or test-supporting files changed.
4. Commit clearly.
5. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer documentation and inspection over contract churn.

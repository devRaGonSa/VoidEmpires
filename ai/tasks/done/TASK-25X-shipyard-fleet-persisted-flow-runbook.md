# TASK-25X

---
id: TASK-25X
title: Phase 25X - Shipyard and Fleet persisted flow runbook
status: done
type: platform
team: platform
supporting_teams:
  - docs
  - backend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Create a copy-pasteable backend-only runbook for real persisted Shipyard and Fleet QA.

## Current problem

The operator needs the same level of guidance already available for Construction and Research, but focused on Shipyard enqueue, Fleet read-state, and the reused-database safety boundaries for this block.

## Context from current implementation

- The central persisted-flow checklist already covers Construction and Research.
- This block is explicitly backend-only and should not rely on frontend clicks or manual SQL.
- The runbook must match the exact script and endpoint behavior validated by the other tasks in this block.

## Files to read first

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/fleet-api-contracts.md
- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-create-shipyard-production-order.ps1
- scripts/dev-qa-fleet-read-state.ps1

## Expected files to modify

- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/shipyard-fleet-persisted-qa.md
- docs/dev/development-seed-profiles.md

## Implementation requirements

1. Add a dedicated Shipyard and Fleets persisted QA section, either in the main checklist or in a clearly linked companion document.
2. Include copy-pasteable commands for:
   - starting the backend
   - applying `cockpit-validation` twice
   - running the baseline script
   - creating one Shipyard production order
   - inspecting Fleet read-state
   - running optional stock-to-fleet allocation only if the earlier audit allowed it
3. Include expected success output and expected controlled no-op output.
4. Include clear warnings that:
   - rows persist in the Development database
   - no production environment is involved
   - no visual QA is required for this block
   - no movement, combat, split, or merge is part of this flow
5. Keep the commands aligned with the final script names and defaults.

## Backend/API requirements

- Do not introduce new backend behavior in this documentation task.
- Reflect only the validated behavior and safe endpoint boundaries established by prior tasks.

## Script/QA requirements

- The runbook must stay consistent with the final PowerShell script parameters and output wording.
- If some step is intentionally unsupported, document the limitation directly instead of implying support.

## Safety constraints

- No destructive reset guidance.
- No secrets or connection strings.
- No manual SQL instructions.

## Acceptance criteria

- A user can perform backend-only Shipyard and Fleet persisted QA from the docs without guessing missing steps.
- The docs explain both the happy path and the expected reused-database no-op path.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- If optional allocation remains out of scope, the runbook should still mention it briefly so operators do not assume the omission is accidental.


# TASK-25S

---
id: TASK-25S
title: Phase 25S - Extend dev QA baseline with Shipyard and Fleets
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Extend the baseline PowerShell QA script so it prints Shipyard and Fleet summary snapshots alongside the existing Construction and Research summaries.

## Current problem

`scripts/dev-qa-baseline.ps1` currently centers on Construction and Research. The Shipyard and Fleet persisted QA flow needs the same quick baseline snapshot before running more specific commands.

## Context from current implementation

- Shared script helpers already handle multiple DTO shapes and resource formatting.
- Shipyard and Fleets both have Development read contracts that can be queried without mutation.
- The baseline script should remain resilient on reused databases and should not fail hard when one auxiliary endpoint is unavailable.

## Files to read first

- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-common.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/fleet-api-contracts.md
- src/VoidEmpires.Web/DevEndpointMappings.cs

## Expected files to modify

- scripts/dev-qa-baseline.ps1
- scripts/dev-qa-common.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md

## Implementation requirements

1. Add a Shipyard baseline snapshot that prints, when available:
   - selected planet
   - resources or reserves
   - available production option count
   - blocked production option count
   - queue count
   - stock count
2. Add a Fleet baseline snapshot that prints, when available:
   - orbital group or squad count
   - stationed count
   - active transfer count
   - local reserve or stock context
3. Reuse shared helper functions instead of duplicating DTO parsing logic.
4. If a Shipyard or Fleet endpoint is unavailable while the rest of the baseline is still usable, print a controlled warning instead of crashing.
5. Keep existing Construction and Research baseline behavior intact.

## Backend/API requirements

- No backend behavior changes are expected unless a small contract mismatch is discovered.
- Prefer adapting the script to the real DTO shapes rather than changing the endpoints.

## Script/QA requirements

- Must stay PowerShell 5.1 compatible.
- Must remain non-mutating.
- Must be included in the local parser and helper check script.
- User-facing output should remain mostly Spanish and operationally readable.

## Safety constraints

- No seed destruction.
- No auto-mutation of Shipyard or Fleet state.
- No backend dependency beyond already documented Development endpoints.

## Acceptance criteria

- `dev-qa-baseline.ps1` prints meaningful Shipyard and Fleet snapshots in addition to existing sections.
- Existing baseline behavior remains intact.
- PowerShell checks and backend validation pass.

## Validation

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Fleet and Shipyard DTOs may expose optional fields or null collections; the baseline script must treat those shapes defensively.


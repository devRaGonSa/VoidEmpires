# TASK-25V

---
id: TASK-25V
title: Phase 25V - Optional stock to Fleet allocation script or docs
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: medium
---

## Goal

Either add a narrowly safe stock-to-fleet allocation QA command or document clearly why allocation remains out of scope for this block.

## Current problem

Allocation would be useful after Shipyard production, but it is also a real persisted mutation that can consume stock and create orbital groups in a reused Development database. The block needs a clear decision instead of ambiguity.

## Context from current implementation

- The preceding audit task determines whether allocation is already safe and documented.
- If allocation is safe, the command must be more conservative than baseline read-state commands.
- If allocation is not safe, the runbook should say so plainly instead of leaving an implied gap.

## Files to read first

- ai/tasks/pending/TASK-25R-stock-to-fleet-allocation-safe-scope-audit.md
- scripts/dev-qa-common.ps1
- docs/dev/fleet-api-contracts.md
- docs/dev/persisted-gameplay-flow-checklist.md
- tests/VoidEmpires.Tests/*Fleet*.cs

## Expected files to modify

- scripts/dev-qa-create-orbital-group-from-stock.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/fleet-api-contracts.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-fleet-persisted-qa.md

## Implementation requirements

1. Use the audit result from `TASK-25R` as the gate for implementation.
2. If the current allocation path is safe:
   - add `scripts/dev-qa-create-orbital-group-from-stock.ps1`
   - require an explicit parameter set or confirmation switch
   - print stock before and after
   - print the created orbital group summary
   - never move the created group
3. If the current allocation path is not safe:
   - do not add a script
   - document the exclusion, the reason, and the exact boundary for this block
4. In either branch, update the persisted QA docs so later users know whether allocation is supported.

## Backend/API requirements

- Do not introduce a new allocation endpoint just to satisfy this task.
- If the existing allocation path lacks the safety guarantees needed for reused-database QA, document that and stop.

## Script/QA requirements

- Any added script must be PowerShell 5.1 compatible.
- Any added script must be parser-checked.
- Allocation must never be triggered automatically from the baseline or Shipyard enqueue scripts.

## Safety constraints

- No split or merge.
- No movement.
- No combat.
- No accidental stock depletion.
- No manual SQL.

## Acceptance criteria

- Either a safe allocation command exists with strong guardrails, or the docs clearly state why allocation is excluded.
- The decision is reflected in the runbook and API documentation.
- Validation passes.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1
```

## Notes / residual risks

- Because allocation is a real mutation, documentation-only closure is acceptable if that is the safer outcome.


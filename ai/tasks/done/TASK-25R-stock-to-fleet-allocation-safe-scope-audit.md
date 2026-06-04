# TASK-25R

---
id: TASK-25R
title: Phase 25R - Stock to Fleet allocation safe scope audit
status: done
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: medium
---

## Goal

Audit whether stock-to-orbital-group allocation is safe enough to include in backend-only persisted QA for this block or must remain explicitly out of scope.

## Current problem

Shipyard production naturally points toward Fleet allocation, but allocation consumes real stock and may create new orbital groups. That can be unsafe for repeated QA if the current implementation is not sufficiently scoped or idempotent.

## Context from current implementation

- Earlier project history includes orbital stock and orbital group foundations.
- The current repository may already have a Development endpoint for allocation, but the exact safety boundaries need verification before any script or runbook includes it.
- This block must stay conservative and avoid silently broadening into Fleet mutation work.

## Files to read first

- src/VoidEmpires.Application/Fleets/*
- src/VoidEmpires.Infrastructure/Fleets/*
- src/VoidEmpires.Domain/Fleets/*
- src/VoidEmpires.Domain/Assets/*
- src/VoidEmpires.Web/DevEndpointMappings.cs
- docs/dev/fleet-api-contracts.md

## Expected files to modify

- docs/dev/fleet-api-contracts.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-fleet-persisted-qa.md
- tests/VoidEmpires.Tests/*Fleet*.cs

## Implementation requirements

1. Identify the current stock-to-fleet allocation endpoint or service, if any.
2. Determine whether it:
   - consumes `OrbitalAssetStock`
   - creates `OrbitalGroup`
   - validates civilization and origin planet ownership
   - is Development-only
   - is deterministic enough for QA
   - is idempotent or intentionally non-idempotent
3. Decide one of three outcomes for this block:
   - read-state only, allocation excluded
   - controlled allocation included with strict safeguards
   - allocation deferred to a future task with documented reasons
4. Document the decision and the rationale clearly.
5. Do not implement allocation in this audit unless the change is trivial, already safe, and already covered by tests.

## Backend/API requirements

- Prefer docs and tests only.
- If read-only metadata or one narrow test is enough to make the decision safely, keep changes minimal.
- Do not introduce new mutation surfaces.

## Script/QA requirements

- If allocation is excluded, say so explicitly in the runbook and script planning notes.
- If allocation is allowed later, note the exact parameters and confirmation requirements the script must use.

## Safety constraints

- No split or merge.
- No movement.
- No combat.
- No destructive stock consumption unless already proven safe and intentional.

## Acceptance criteria

- The stock-to-fleet QA scope is explicitly documented.
- Later tasks do not need to guess whether allocation is safe to automate.
- Validation passes without broadening gameplay behavior.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
```

## Notes / residual risks

- Even if allocation exists, it may still be too risky for a reused Development database unless the script path can clearly limit quantity, origin, and confirmation behavior.


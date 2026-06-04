# TASK-25P

---
id: TASK-25P
title: Phase 25P - Shipyard validation failure scenarios
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - frontend
roadmap_item: "Block 25M-26B - Real persisted gameplay flow QA for Shipyard and Fleets"
priority: high
---

## Goal

Add negative-path coverage proving that invalid Shipyard production requests fail safely and do not mutate persisted state.

## Current problem

Successful enqueue coverage is necessary but incomplete. The backend also needs proof that invalid Development requests are rejected cleanly without creating orders or consuming resources.

## Context from current implementation

- Shipyard cards already expose available and blocked option state in the cockpit.
- The backend must remain authoritative for civilization, planet, asset, affordability, and unlock validation.
- Scripts and frontend error summaries will be safer if known backend failures are mapped clearly.

## Files to read first

- src/VoidEmpires.Application/Assets/*
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/api/*
- docs/dev/shipyard-cockpit-checklist.md

## Expected files to modify

- tests/VoidEmpires.Tests/DevShipyardEnqueueEndpointTests.cs
- tests/VoidEmpires.Tests/*Shipyard*.cs
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/*
- scripts/dev-qa-common.ps1

## Implementation requirements

1. Add deterministic negative-path tests for:
   - invalid civilization id
   - invalid planet id
   - planet not controlled by the civilization
   - unknown or invalid asset type
   - insufficient resources when deterministic
   - unmet requirement when supported by the current backend
   - duplicate or open queue restriction when applicable
   - disabled or unavailable production option
2. For each failing scenario, assert:
   - non-success status or failure result
   - no new persisted order
   - no resource mutation
3. Reuse existing test infrastructure and keep the data setup explicit.
4. If the backend returns known error codes or messages that the frontend or PowerShell layer does not currently map clearly, add a small Spanish-first mapping update.
5. Do not broaden the task into general Shipyard UX redesign.

## Backend/API requirements

- Tests are required.
- Do not weaken validation.
- If response metadata must be clarified, keep changes minimal and Development-safe.

## Script/QA requirements

- Any newly recognized known-failure message should be representable in script output without dumping only raw JSON.
- The later Shipyard script should be able to distinguish expected no-op states from real failures.

## Safety constraints

- No validation bypasses.
- No optimistic order creation followed by rollback.
- No manual SQL.

## Acceptance criteria

- Invalid Shipyard requests are covered by automated tests.
- Failed requests do not mutate orders or resources.
- Any touched UI or script error copy remains conservative and clear.

## Validation

```powershell
dotnet build --no-restore
dotnet test --no-build
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- Some negative scenarios may depend on the exact seeded availability matrix, so the tests should isolate state rather than relying on incidental blocked options from unrelated profiles.


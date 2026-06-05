# TASK-27C Alliance Cockpit Read Model Or Dev Endpoint

---
id: TASK-27C
title: Provide stable Alliance read model for cockpit consumption
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Ensure the Alliance cockpit has a stable read model source for identity, status, contacts, and placeholders without adding mutable gameplay behavior.

## Current problem
Frontend assembly would be fragile if it directly composes multiple payloads; a dedicated read model minimizes coupling and keeps v1 scope clear.

## Context from current implementation
Existing accepted cockpits use endpoint-driven state where needed. Alliance should follow that pattern and stay read-only, with development boundaries where appropriate.

## Goal
Reuse existing read sources when sufficient; otherwise add a development-only endpoint for Alliance UI state.

## Files to inspect first
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/

## Expected files to modify
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/

## Implementation requirements
- Start by evaluating existing read surfaces for:
- civilization identity
- player context
- ownership visibility
- diplomatic context
- Add development-only endpoint if missing, such as:
- GET /api/dev/alliance/ui-state?civilizationId={id}
- Ensure response includes:
- civilization id and display identity
- alliance status in read-only mode
- known contacts if derivable
- future pact/invitation placeholders
- disabled action summary
- diagnostics/limitations block
- Add tests for:
- development gating
- invalid civilizationId
- success response shape
- no mutation path

## UI/UX requirements
- Response fields should support Spanish labels and placeholders.

## Backend/API requirements
- Prefer read-only query/service changes only.
- Keep production route behavior unchanged.
- No authentication model changes.
- No migrations unless strictly required by existing model changes, which should be avoided.

## Safety constraints
- Endpoint remains read-only.
- Do not enable alliance mutation, messaging, invitations, roles, or pact execution.
- No real-time communication dependencies.

## Acceptance criteria
- Alliance frontend can call one stable read model.
- Error and boundary behavior is tested.
- Backend build/tests remain green.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if payload consumers are added

## Notes / residual risks
- If no backend changes are needed, document explicit consumption path clearly to avoid accidental divergence across tasks.

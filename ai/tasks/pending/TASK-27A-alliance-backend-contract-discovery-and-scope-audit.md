# TASK-27A Alliance Backend Contract Discovery And Scope Audit

---
id: TASK-27A
title: Audit safe backend scope for Alliance read-only diplomacy v1
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Identify existing backend and domain capabilities for alliance-like diplomacy concepts and define the exact safe scope for the Alliance v1 read-only cockpit.

## Current problem
Alliance remains a placeholder module. We need to confirm what civilization, player, ownership, and relationship data already exists before adding a cockpit so we do not accidentally introduce real alliance gameplay.

## Context from current implementation
The product already has accepted read-only cockpits and a lazy route pattern. This task must define where Alliance can read from existing data without mutating any alliance state.

## Goal
Document what backend/domain services and read surfaces can support:
1. own diplomatic identity,
2. current alliance status as non-authoritative/read-only,
3. future/potential diplomacy placeholders,
4. no mutation guarantees.

## Files to inspect first
- src/VoidEmpires.Domain/
- src/VoidEmpires.Domain/Players/
- src/VoidEmpires.Domain/Planets/
- src/VoidEmpires.Application/
- src/VoidEmpires.Infrastructure/
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- src/VoidEmpires.Web/DevEndpointMappings.cs
- tests/VoidEmpires.Tests/
- ai/current-state.md

## Expected files to modify
- docs/dev/alliance-cockpit-checklist.md
- ai/current-state.md

## Implementation requirements
- Confirm whether PlayerProfile and Civilization models can be reused for Alliance read-state.
- Confirm availability and safety of any existing relationship or diplomacy naming.
- Confirm any endpoint or service that can supply read-only alliance or diplomatic context for a given civilizationId.
- Define explicit Alliance v1 safe scope:
- own diplomatic identity read,
- alliance status represented as None/Future/ReadOnly,
- known contacts from existing data where safe,
- pact/invitation/membership placeholders only,
- no create/join/leave/mutate actions.
- Note any backend gaps that require a tiny development read endpoint.
- Keep all findings in a docs file, not by altering behavior in this task.

## UI/UX requirements
- No direct UI changes in this task.
- The audit must provide clear guidance for safe front-end copy and placeholders.

## Backend/API requirements
- Prefer investigation and documentation only.
- If any service/endpoint is touched, add tests and maintain development-only boundaries.

## Safety constraints
- No alliance creation, pact creation, invitations, roles/permissions, or messaging logic.
- No production behavior changes.
- No endpoint changes outside documented scope unless justified by later tasks.

## Acceptance criteria
- Alliance read model boundaries are documented.
- Clear v1 read-only boundaries are stated.
- Future implementation tasks have unambiguous source-of-truth references.
- Validation remains green.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if documentation ties into frontend contracts

## Notes / residual risks
- If no relationship model exists, tasks later must add a minimal development-only endpoint rather than inventing persisted alliance state.
- Keep raw technical names in diagnostics-only output.

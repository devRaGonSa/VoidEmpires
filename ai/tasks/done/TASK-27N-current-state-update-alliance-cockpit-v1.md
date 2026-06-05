# TASK-27N Current State Update Alliance Cockpit V1

---
id: TASK-27N
title: Update current state for Alliance read-only diplomacy foundation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Record the v1 Alliance milestone in architecture state with honest scope boundaries.

## Current problem
`ai/current-state.md` does not yet reflect that Alliance moved from placeholder to a read-only cockpit implementation.

## Context from current implementation
Frontend architecture must remain lazy-loaded and accepted cockpit behavior unchanged while this new read-only diplomatic module is added.

## Goal
Update current-state with:
- phase milestone text
- Alliance v1 features
- read-only constraints
- validation and seed status

## Files to inspect first
- ai/current-state.md
- src/VoidEmpires.Frontend/src/App.tsx
- docs/dev/alliance-cockpit-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- tests/VoidEmpires.Tests

## Expected files to modify
- ai/current-state.md

## Implementation requirements
- Add/update entry:
- Block 27A-27P / Alliance v1 foundation
- /alliance now read-only diplomacy cockpit
- show alliance status, contacts/readiness, disabled future actions, handoffs
- explicitly state no alliance creation/pact execution/invitations/messages/roles
- preserve accepted cockpit list
- keep lazy-route architecture note
- ensure test count remains accurate.

## UI/UX requirements
- No runtime changes.

## Backend/API requirements
- If validations changed, align recorded counts with test results.

## Safety constraints
- No overclaim of gameplay expansion.
- No ambiguity around non-active diplomacy.

## Acceptance criteria
- current-state reflects v1 scope and limits.
- No unsupported behavior is described as shipped.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep wording consistent with the existing style in current-state to avoid context drift.

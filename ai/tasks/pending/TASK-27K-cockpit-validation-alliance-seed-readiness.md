# TASK-27K Cockpit Validation Alliance Seed Readiness

---
id: TASK-27K
title: Extend cockpit-validation support for Alliance read model
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Keep seeded QA deterministic and include Alliance read-state in development cockpit-validation workflows.

## Current problem
Without validation seed support, Alliance checks can be flaky or unavailable in reused development database workflows.

## Context from current implementation
Cockpit-validation is already accepted for prior modules. Alliance should be added as read-only foundation without breaking idempotency.

## Goal
Ensure development seed and QA scripts can produce stable Alliance demo state and diagnostics.

## Files to inspect first
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests
- docs/dev/development-seed-profiles.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/
- docs/dev/development-seed-profiles.md

## Implementation requirements
- Add or extend Alliance seed state for UI validation:
- own civilization identity
- no active alliance state
- known contacts or readiness placeholders
- future pact/placeholder state
- disabled action state
- Ensure replay/idempotent seed behavior.
- Add/extend tests:
- Alliance UI state loads with validation seed
- status summary exists
- future actions are disabled
- repeated seed does not duplicate values
- Keep no real membership mutation logic.

## UI/UX requirements
- Not direct UI task, but seed should support screenshot and state checks.

## Backend/API requirements
- Development-only seed adjustments only.
- No schema migration.

## Safety constraints
- Do not create real alliances in seed.
- No membership mutation or invitation records.
- Do not add manual SQL for normal QA.

## Acceptance criteria
- Validation seed supports deterministic Alliance smoke checks.
- Tests verify read-only state and stability.
- Build/test pass.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if frontend contracts depend on seed output

## Notes / residual risks
- If seed service lacks place for placeholder diplomacy context, add the minimal safe shape and document it clearly.

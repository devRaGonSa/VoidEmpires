# TASK-28L Cockpit Validation Ranking Seed Readiness

---
id: TASK-28L
title: Add deterministic Ranking coverage to cockpit-validation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 28A-28P - Ranking cockpit read-only power index foundation v1"
priority: high
---

## Purpose
Ensure development seed and validation flow can reliably prepare Ranking scenario state for deterministic QA.

## Current problem
Ranking cannot be accepted without seed-backed validation and deterministic data guarantees.

## Context from current implementation
cockpit-validation already supports accepted cockpits; Ranking should integrate as read-only foundational check.

## Goal
Seed and validate Ranking-ready context for power categories and comparison rows without introducing real ladder data.

## Files to inspect first
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/
- docs/dev/development-seed-profiles.md
- docs/dev/frontend-foundation-smoke-checklist.md

## Expected files to modify
- src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs
- tests/VoidEmpires.Tests/
- docs/dev/development-seed-profiles.md

## Implementation requirements
- Ensure seeded state includes:
- own civilization identity
- category score baseline values
- demo comparison rows or placeholders
- future leaderboard placeholders as disabled state
- action disabled markers
- Keep seed logic idempotent and reusable.
- Add tests:
- Ranking UI state reachable after validation
- power summary exists
- category scores populated
- future actions remain disabled
- no duplicate records on reseed
- Keep no real ranking persistence.

## UI/UX requirements
- No direct UI changes.
- Seed should support screenshot-friendly verification.

## Backend/API requirements
- Development-only seed/read model.
- No migrations.

## Safety constraints
- No global ladder table writes.
- No rewards or matchmaking data.
- No public profile data.

## Acceptance criteria
- cockpit-validation supports Ranking QA deterministically.
- Tests pass and reseed remains stable.

## Validation
- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend if tied to UI payload

## Notes / residual risks
- If seed data is derived from existing modules, document the dependency chain clearly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

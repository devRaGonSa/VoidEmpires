# TASK-27D Alliance Frontend Api Types And View Models

---
id: TASK-27D
title: Add typed Alliance API client and normalized view models
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Build a typed, normalized frontend model layer for Alliance state, actions, contacts, and diagnostics.

## Current problem
Alliance should avoid rendering backend DTOs directly and keep placeholder semantics consistent with Spanish-first copy and disabled action states.

## Context from current implementation
Frontend patterns in other cockpits already separate API DTOs, view models, and presentation formatting. Alliance should reuse this approach.

## Goal
Create explicit Alliance types and mapping helpers for stable rendering.

## Files to inspect first
- src/VoidEmpires.Frontend/src/api/
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify
- src/VoidEmpires.Frontend/src/api/allianceApi.ts
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/pages/alliance/ (or relevant folder)

## Implementation requirements
- Add typed models:
- AllianceUiState
- AllianceDiplomaticIdentity
- AllianceStatusSummary
- AllianceContact
- AlliancePactPlaceholder
- AllianceFutureAction
- AllianceDiagnostics
- Add API function:
- fetchAllianceUiState(...)
- Add view-model helpers:
- mapAllianceUiStateToViewModel
- groupAllianceContacts
- selectRecommendedDiplomacyFocus
- getAlliancePrimaryAction
- Normalize labels, disabled action states, contact confidence, future placeholders.
- Keep technical details in diagnostics.

## UI/UX requirements
- Data structures must support:
- top summary cards
- contacts catalog
- disabled future action section
- diagnostics panel

## Backend/API requirements
- If endpoint shape mismatches, coordinate with backend team via follow-up task 27C.
- No write endpoints.

## Safety constraints
- Do not call mutation endpoints.
- No optimistic mutation state.
- No alliance execution hooks.

## Acceptance criteria
- Alliance page can consume typed state cleanly.
- Frontend compiles with consistent view-model usage.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep naming stable for future extension into real diplomacy without breaking v1.

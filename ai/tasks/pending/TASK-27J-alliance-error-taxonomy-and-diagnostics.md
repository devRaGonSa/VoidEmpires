# TASK-27J Alliance Error Taxonomy And Diagnostics

---
id: TASK-27J
title: Normalize Alliance errors and collapsed diagnostics
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Define consistent Spanish user-facing errors for Alliance read-state failures while keeping technical details available in diagnostics.

## Current problem
Alliance endpoint or context failures can confuse users if technical error details leak into primary UI.

## Context from current implementation
Existing pages use human-readable messages plus collapsed diagnostic sections. Alliance should follow this same model.

## Goal
Build a small error mapping layer with deterministic fallback states.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/api/allianceApi.ts
- Existing diagnostics pattern in other pages

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/api/allianceApi.ts

## Implementation requirements
- Map these error classes:
- invalid civilization id
- civilization not found
- alliance read unavailable
- endpoint unavailable outside development
- unsupported action requested (if surfaced)
- unexpected error
- Show Spanish primary messages:
- No se pudo cargar la lectura diplomatica.
- No hay contexto de civilizacion.
- Las acciones diplomáticas no estan disponibles en esta version.
- Keep technical details collapsed/diagnostics-only.
- Do not silently ignore backend rejections.

## UI/UX requirements
- Clear primary error state.
- Diagnostics should be compact and expandable.
- No primary raw JSON in UI.

## Backend/API requirements
- Add result codes only if needed and tested.
- Keep endpoint read-only.

## Safety constraints
- No mutation flow in error handling.
- No hidden side effects on failed states.

## Acceptance criteria
- Alliance error states are readable and safe.
- Diagnostics remain accessible but not intrusive.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend
- dotnet build --no-restore
- dotnet test --no-build if backend error contract changes

# TASK-27H Alliance Pacts And Future Actions Disabled Placeholders

---
id: TASK-27H
title: Add disabled future diplomatic action placeholders for v1
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: medium
---

## Purpose
Show roadmap-only diplomatic actions in a disabled form so users see future direction without enabling execution.

## Current problem
Alliance naturally suggests operational actions that are not in scope in v1, so we need explicit disabled presentation.

## Context from current implementation
Read-only cockpits already communicate disabled features; Alliance should match this trust model and avoid hidden action affordances.

## Goal
Add a section for read-only future actions with clear non-executable messaging.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- docs/dev/cockpit-copy-guidelines.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts

## Implementation requirements
- Add "Acciones diplomaticas futuras" section.
- Add disabled placeholder entries:
- Crear alianza
- Solicitar entrada
- Invitar civilizacion
- Proponer pacto defensivo
- Proponer pacto comercial
- Gestionar roles
- Each entry should include:
- No disponible en esta version
- Solo lectura en esta cabina
- Esta accion queda visible como referencia futura, pero no se puede ejecutar
- Ensure no click handlers for mutation.
- Ensure placeholders are visually secondary.

## UI/UX requirements
- Spanish-first and clear disabled state language.
- No primary button styling for disabled placeholders.

## Backend/API requirements
- None expected in this task.
- Keep all action entries client-side when possible.

## Safety constraints
- No create/join/update/remove alliance or pact actions.
- No invitation workflow.
- No role management logic.

## Acceptance criteria
- Future actions appear in cockpit but are unmistakably disabled.
- No mutation event paths are added.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If backend later adds action payloads, keep compatibility by keeping v1 disabled rendering behind configuration.

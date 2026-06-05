# TASK-27B Alliance Taxonomy And Display Labels

---
id: TASK-27B
title: Define Spanish taxonomy and labels for Alliance read-only diplomacy
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Create centralized, Spanish-first presentation labels for diplomacy status, contact readiness, and disabled future actions.

## Current problem
Alliance UI cannot expose raw enum or DTO tokens. It needs vocabulary that clearly communicates read-only diplomacy state without implying executable alliance features.

## Context from current implementation
Cockpit status helpers and copy conventions already exist. Alliance should reuse these patterns and keep technical names out of primary UI.

## Goal
Add a dedicated presentation helper (or reuse an existing one) containing all read-only alliance labels and fallback copy.

## Files to inspect first
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/utils/
- src/VoidEmpires.Frontend/src/pages/ModuleCabinPage.tsx
- docs/dev/cockpit-copy-guidelines.md

## Expected files to modify
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- docs/dev/alliance-cockpit-checklist.md

## Implementation requirements
- Add labels for:
- Estado diplomatico
- Sin alianza activa
- Alianza futura
- Pacto defensivo futuro
- Pacto comercial futuro
- Pacto de no agresion futuro
- Invitacion no disponible
- Solicitud no disponible
- Diplomacia solo lectura
- Contacto conocido
- Contacto sin confirmar
- Lectura diplomatica pendiente de clasificar
- Ensure technical enum names are hidden in primary UI and only surfaced in diagnostics.
- Keep raw persisted values unchanged.

## UI/UX requirements
- Spanish-first copy.
- No wording that suggests actions are currently executable.
- Labels should support both summary and card-level states.

## Backend/API requirements
- No backend changes required in this task.
- Consume existing backend values through mapping layer if available.

## Safety constraints
- No gameplay behavior changes.
- No actions become executable through the label layer.

## Acceptance criteria
- Alliance presentation labels are centralized and reusable.
- Unknown states show Spanish fallback text.
- No raw enum/camelCase text appears in primary UI.
- npm run build --prefix src/VoidEmpires.Frontend passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If taxonomy grows beyond this task, keep labels in one module to avoid drift.

# TASK-27F Alliance Dashboard Diplomatic Summary

---
id: TASK-27F
title: Build Alliance dashboard summary cards
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Create a clear top-level Alliance dashboard that communicates diplomatic posture and readiness at a glance.

## Current problem
Alliance should not present only placeholder text; users need a meaningful read model overview and a clear non-executable status statement.

## Context from current implementation
Cockpit pages use CockpitHero and compact status cards for quick comprehension. Alliance should mirror this rhythm without enabling actions.

## Goal
Add summary data and explicit read-only disclaimers in Spanish.

## Files to inspect first
- src/VoidEmpires.Frontend/src/components/CockpitHero.tsx
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/styles.css

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements
- Render hero/summary values including:
- Estado diplomatico
- Civilizacion propia
- Sin alianza activa
- Contactos conocidos
- Pactos futuros
- Foco recomendado
- Explicit statement:
- Esta cabina no crea alianzas ni pactos
- Add fallback copy when data is limited.
- Avoid raw id/guid exposure in summary.

## UI/UX requirements
- Spanish-first compact cards.
- Clear read-only indication.
- No design redesign, preserve shell visual language.

## Backend/API requirements
- Consume read model values from task 27C/27D.

## Safety constraints
- No mutation UI controls.
- No hidden actions.

## Acceptance criteria
- /alliance dashboard has visible diplomatic summary.
- User understands read-only nature immediately.
- Frontend build passes.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- If read model lacks some counts, keep placeholders explicit and deterministic.

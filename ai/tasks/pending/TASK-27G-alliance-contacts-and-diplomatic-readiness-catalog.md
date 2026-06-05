# TASK-27G Alliance Contacts And Diplomatic Readiness Catalog

---
id: TASK-27G
title: Show contact and readiness catalog in Alliance cockpit
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 27A-27P - Alliance cockpit read-only diplomacy foundation v1"
priority: high
---

## Purpose
Render a read-only body section for known contacts, potential contacts, and future pact placeholders.

## Current problem
Without this section, Alliance remains too abstract and does not show any social context beyond status labels.

## Context from current implementation
Other read-only cockpits use grouped sections and compact cards; Alliance should follow the same UX rhythm with no interactive actions.

## Goal
Add grouped cards for contact and readiness data with safe placeholders where data is limited.

## Files to inspect first
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/pages/EspionagePage.tsx
- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx

## Expected files to modify
- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/styles.css

## Implementation requirements
- Render grouped sections:
- Contactos conocidos
- Contactos potenciales
- Pactos futuros
- Lectura diplomatica limitada
- Each row/card should include status text, confidence/readiness and next-cockpit hint when relevant.
- If no other civilizations are available, show deterministic demo-friendly placeholders:
- No hay otra civilizacion diplomatica activa en esta version
- Los pactos quedan preparados para una fase futura
- Never invent real participants or active agreements.

## UI/UX requirements
- Spanish-first, compact, and read-only.
- No raw IDs in visible UI.
- No interactive invitation cards.

## Backend/API requirements
- No backend mutation required.
- Include placeholder entries only if endpoint supplies them.

## Safety constraints
- No invitations or role actions.
- No message threads.
- No diplomacy execution.

## Acceptance criteria
- Contacts and readiness section is visible and deterministic.
- Placeholders are clear when no data is present.
- No fake executable interactions.

## Validation
- npm run build --prefix src/VoidEmpires.Frontend

## Notes / residual risks
- Keep future placeholders visually distinct from active status.

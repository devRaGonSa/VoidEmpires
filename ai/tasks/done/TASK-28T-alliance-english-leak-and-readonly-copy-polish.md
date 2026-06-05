# TASK-28T

---
id: TASK-28T
title: Alliance English Leak and Read-Only Copy Polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Replace visible English and tune Alliance read-only status copy to Spanish-first wording.

## Context
Alliance cockpit is read-only by design. Visible English text currently contradicts local-language UX and risks implying execution availability.

## Implementation steps

1. Replace all visible English status/action copy with Spanish.
2. Verify Alliance cockpit messaging reinforces read-only intent.
3. Ensure no new actions are introduced.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/api/allianceTypes.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/utils/allianceViewModel.ts

## Acceptance criteria

- No visible English sentences in Alliance primary UI.
- Primary UI clearly expresses read-only mode.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.



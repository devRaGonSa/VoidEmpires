# TASK-33K

---
id: TASK-33K
title: Resource summary consistency
status: pending
type: frontend
team: gameplay
supporting_teams: []
roadmap_item: "Block 33A-33P - Playable Loop Integration & Session Navigation v1"
priority: medium
---

## Goal
Make resource display consistent across Planet, Construction, Research, and Shipyard.

## Context
Resource values must remain backend-sourced, but labels and formatting should be consistent across the playable loop pages.

## Implementation steps

1. Audit resource formatting and labels in Planet, Construction, Research, and Shipyard.
2. Use or add common formatting helpers where feasible.
3. Ensure user-facing labels are Spanish:
   - Metal
   - Cristal
   - Gas
   - Energía
4. Do not change backend values or calculations.
5. Do not fake resource increases or optimistic-update resources.
6. Keep raw resource keys in diagnostics only if they remain visible.
7. Update the copy regression guard only if needed to preserve intentional Spanish wording.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/resourceDisplay.ts
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- Optional: scripts/check-frontend-copy-regressions.ps1

## Acceptance criteria

- Resource display is more consistent across the four pages.
- Gameplay values are unchanged.
- Backend remains source of truth.
- Copy guard remains green.

## Constraints

- Do not fake resource data.
- Do not optimistic-update snapshots.
- Do not broaden visual redesign.
- Keep raw backend keys out of primary UI.

## Validation

Before completing the task run:

- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git diff --stat`.
2. Run `git diff --name-only` and compare against expected files.
3. Stage only intended files.
4. Commit with a clear TASK-33K message.
5. Push the branch if the repository workflow expects it.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- If this exceeds the budget, split the resource helper and page migrations into follow-up tasks.

# TASK-28V

---
id: TASK-28V
title: Market Resource Fallback Labels Polish
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Remove repeated raw `"Recurso no clasificado"` labels from Market primary UI.

## Context
Repeated fallback language appears frequently in primary Market views and creates confusion in user-facing summary/production signals.

## Implementation steps

1. Improve resource label fallback mapping in presentation/model.
2. Prefer known resource names from existing mapped data.
3. Use softer fallback alternatives when source type is unresolved.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/MarketPage.tsx
- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/utils/marketPresentation.ts
- src/VoidEmpires.Frontend/src/utils/marketViewModel.ts

## Acceptance criteria

- Primary Market UI does not repeatedly show `"Recurso no clasificado"`.
- Resource and flow copy is natural Spanish.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

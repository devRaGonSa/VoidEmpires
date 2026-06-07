# TASK-29G

---
id: TASK-29G-construction-validation-errors-spanish-copy
title: Map validation failures to Spanish-safe user messages
status: done
type: platform
team: platform
supporting_teams: [frontend]
roadmap_item: block-29a-29p-construction-real-persisted-enqueue-ux-v1
priority: medium
---

## Goal
Turn backend validation failures into understandable Spanish messages while exposing raw payloads in diagnostics.

## Context
No backend masking; no failures should disappear.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx
- scripts/check-frontend-copy-regressions.ps1

## Expected files to modify

- ai/tasks/pending/TASK-29G-construction-validation-errors-spanish-copy.md
- src/VoidEmpires.Frontend/src/pages/ConstructionPage.tsx

Implementation note:

- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts` is updated instead of `ConstructionPage.tsx` because the construction error-copy mapping lives in the shared presentation helper used by the route.

## Implementation steps

1. Map and display known failures:
   - insufficient resources
   - unavailable construction action
   - invalid planet
   - invalid civilization
   - not owned
   - existing/open order constraint
   - unexpected backend error
2. Keep raw backend details in collapsed diagnostics.
3. Update copy guards if new English messages appear in UI.

## Acceptance criteria

- All known failures have Spanish primary copy.
- Diagnostics still show raw details.
- Copy regression check maintained.

## Validation

- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1

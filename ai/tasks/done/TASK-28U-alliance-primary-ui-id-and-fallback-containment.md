# TASK-28U

---
id: TASK-28U
title: Alliance Primary UI ID and Fallback Containment
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: ""
priority: high
---

## Goal
Remove raw IDs/technical suffixes from Alliance primary UI and normalize fallback phrasing.

## Context
Current Alliance rendering can expose backend IDs and technical labels in card subtitles. These should be restricted to diagnostics or replaced with non-technical copy.

## Implementation steps

1. Identify and remove pipe-separated raw IDs in visible alliance/contact labels.
2. Replace fallback text with clean Spanish for pending/incomplete diplomacy reads.
3. Keep technical identifiers in collapsed details only.
4. Re-run frontend build.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/utils/allianceViewModel.ts

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/AlliancePage.tsx
- src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts
- src/VoidEmpires.Frontend/src/utils/allianceViewModel.ts

## Acceptance criteria

- No raw IDs or pipe-delimited technical values in primary Alliance UI.
- Readability of diplomacy/contact fallbacks is improved.
- `npm run build --prefix src/VoidEmpires.Frontend` passes.

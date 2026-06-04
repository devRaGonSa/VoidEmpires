# TASK-22F

---
id: TASK-22F
title: Phase 22F - Espionage dashboard intelligence summary
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Create the top-level `Espionaje` dashboard summary for intelligence coverage, known targets, partial contacts, signals, and recommended focus.

## Purpose

The first viewport should immediately communicate what the civilization knows, what remains uncertain, and where the player should investigate next.

## Current problem

The Espionage cockpit needs an immediate summary of the current intelligence picture. It should not open directly into raw target cards without high-level framing.

## Context

The repository already uses shared cockpit patterns such as `CockpitHero`, status badges, summary tiles, and concise read-only boundary copy. Espionage should reuse those patterns rather than inventing a parallel page structure.

## Files to read first

- `src/VoidEmpires.Frontend/src/components/CockpitHero.tsx`
- Espionage page and view-model helpers
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/cockpit-copy-guidelines.md`

## Component discovery

Inspect how accepted cockpits build their hero, status badges, top summary cards, and read-only boundary text. Match those conventions and keep Espionage visually aligned with the shared shell.

## Implementation requirements

1. Reuse `CockpitHero` or the current shared hero pattern for the page header.
2. Summarize at least:
   - selected civilization
   - known systems count
   - observed targets count
   - partial or uncertain targets count if supported
   - passive signal count if supported
   - current cockpit boundary
3. Include short guidance such as:
   - what is confirmed
   - what is observed indirectly
   - what remains incomplete
4. Add a compact readiness or coverage overview that explains whether the current intelligence picture is broad, partial, or thin.
5. Keep any raw diagnostics or exact backend reasons out of the hero area.

## UI/UX requirements

- Spanish-first and gameplay-facing
- The hero must feel intentional and informative, not like a generic dev card
- Summary content must be scannable on desktop and mobile
- Read-only boundary copy must be visible but not alarmist

## Backend/API requirements

- No backend change expected
- Reuse the read model provided by earlier tasks

## Expected files to modify

- Espionage page component
- supporting view-model or presentation helper files
- shared hero usage only if a small extension is needed
- styles only if the summary layout needs cockpit-specific support

## Safety constraints

- No offensive actions
- No live mission controls
- No fake precision where data is partial

## Acceptance criteria

- The Espionage first viewport explains the module clearly.
- The player can understand the overall intelligence picture without opening diagnostics.
- The boundary between read-only analysis and active operations is explicit.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- If the backend data is sparse, the hero should still present truthful readiness language instead of inflated numbers or empty jargon.
- Avoid overloading the hero; deeper detail belongs in later sections.

## Commit and push

1. Run `git status`.
2. Check changed files.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep the hero and overview incremental.
- Move elaborate target visualization into later tasks.

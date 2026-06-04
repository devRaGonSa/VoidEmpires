# TASK-22H

---
id: TASK-22H
title: Phase 22H - Espionage intelligence levels and partial data cues
status: pending
type: platform
team: platform
supporting_teams:
  - frontend
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Make intelligence levels and partial-information cues explicit across the Espionage cockpit.

## Purpose

Players need to understand why some targets show detailed facts while others only show fragments, signals, or uncertainty markers.

## Current problem

Strategic visibility concepts already exist, but Espionage needs to teach them clearly. Without a legend and consistent cues, partial targets may look broken or misleadingly complete.

## Context

The product decision for this block is read-only intelligence foundation v1. That means uncertainty is a feature of the cockpit, not an implementation flaw to hide.

## Files to read first

- Espionage page component
- Espionage presentation and view-model helpers
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `src/VoidEmpires.Frontend/src/styles.css`

## Component discovery

Inspect shared badge patterns, legend panels, secondary-note styles, and current strategic-map visibility presentation. Reuse the current visual hierarchy where possible.

## Implementation requirements

1. Add a clear section or legend for intelligence levels such as:
   - `Propio / control directo`
   - `Visible / observado`
   - `Contacto parcial`
   - `Senal orbital`
   - `Sin confirmar`
2. Add compact UI cues:
   - badges
   - short descriptions
   - optional confidence text or meter only if it can stay truthful
3. When data is partial:
   - hide unsupported fields
   - mark missing knowledge clearly
   - never invent ownership, type, or fleet detail that the read model does not provide
4. Add copy such as:
   - `Los datos incompletos no desbloquean acciones ofensivas.`
   - `La lectura depende de la visibilidad estrategica actual.`
5. Keep raw reasons inside diagnostics only.

## UI/UX requirements

- Spanish-first
- Clear and readable, not overexplained
- No fake precision
- Unknown fields should look intentionally unavailable, not broken

## Backend/API requirements

- No backend change expected
- Reuse existing confidence or visibility signals if present; otherwise keep cues purely presentational

## Expected files to modify

- Espionage page component
- Espionage presentation helpers
- styles for badges, legend, or partial-data treatment

## Safety constraints

- No new gameplay
- No active mission execution
- No invented intelligence detail

## Acceptance criteria

- Intelligence levels are understandable without opening diagnostics.
- Partial targets do not look fully known.
- The cockpit teaches uncertainty as part of the experience.
- Frontend build passes.

## Validation

```powershell
npm run build --prefix src/VoidEmpires.Frontend
```

## Notes / residual risks

- A confidence meter can become noisy if the backend has only coarse categories; prefer text if that is the more honest representation.
- Future fog-of-war systems may refine this, but current cues should remain compatible.

## Commit and push

1. Run `git status`.
2. Verify only intended frontend files changed.
3. Commit clearly.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Keep legend and cue work localized.

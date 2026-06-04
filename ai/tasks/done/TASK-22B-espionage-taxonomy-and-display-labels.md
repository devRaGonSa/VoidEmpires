# TASK-22B

---
id: TASK-22B
title: Phase 22B - Espionage taxonomy and display labels
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 22A-22P - Espionage cockpit read-only intelligence foundation v1"
priority: high
---

## Goal

Create centralized Spanish player-facing labels for intelligence levels, target visibility, observation states, confidence, and disabled future espionage actions.

## Purpose

Espionage needs a gameplay-facing vocabulary that fits the accepted cockpit language baseline and keeps raw technical values out of the primary UI.

## Current problem

The repository already exposes technical visibility and readiness concepts, but Espionage would read poorly if it surfaced raw enum names, capability keys, or strategic-map implementation labels in heroes, cards, badges, or handoff summaries.

## Context

Cross-cockpit polish already established:

- Spanish-first primary copy
- diagnostics collapsed or secondary
- shared status helpers
- gameplay language over endpoint language

Espionage should extend that pattern with its own consistent intelligence lexicon.

## Files to read first

- `docs/dev/cockpit-copy-guidelines.md`
- `docs/dev/cross-cockpit-language-audit.md`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/pages/StrategicMapPage.tsx`
- `src/VoidEmpires.Domain/`

## Component discovery

Look for the current frontend presentation helpers used by `Research`, `Shipyard`, `Defenses`, `Ground Army`, and `Galaxy`. Reuse the same helper and fallback patterns instead of adding one-off labels directly inside JSX.

## Implementation requirements

1. Introduce frontend presentation helpers for Espionage-oriented labels, for example:
   - `getIntelligenceLevelLabel(...)`
   - `getTargetVisibilityLabel(...)`
   - `getObservationStatusLabel(...)`
   - `getEspionageActionLabel(...)`
   - `getIntelConfidenceLabel(...)`
   - `formatIntelCoverage(...)`
2. Use Spanish-first labels such as:
   - `Inteligencia confirmada`
   - `Observacion directa`
   - `Contacto parcial`
   - `Senal orbital detectada`
   - `Lectura incompleta`
   - `Sin confirmar`
   - `Objetivo conocido`
   - `Objetivo observado`
   - `Objetivo fuera de alcance`
   - `Mision no disponible`
   - `Reconocimiento futuro`
3. Keep fallback copy player-facing, for example `Lectura pendiente de clasificar`, rather than exposing raw enum or camelCase values.
4. Preserve raw values only for collapsed diagnostics.
5. Do not rename persisted enums or backend contract values.
6. Align wording with accepted copy guidance and avoid implying active mission gameplay.

## UI/UX requirements

- Spanish-first primary copy
- Short, scannable labels suitable for badges, legends, cards, and summaries
- Gameplay-oriented language rather than implementation terminology
- Future-action labels must read as disabled roadmap placeholders, not as clickable systems

## Backend/API requirements

- Prefer frontend-only presentation helpers
- Reuse backend display metadata only if it already exists and is stable
- Do not add backend mission logic in this task

## Expected files to modify

- Espionage presentation helper file under `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/utils/cockpitStatus.ts` only if shared label primitives need extension
- Espionage page or view-model files only if required to consume the new helper

## Safety constraints

- No gameplay rule changes
- No mutation endpoints
- No espionage execution
- No active mission availability
- No backend enum renames

## Acceptance criteria

- Espionage labels are centralized instead of duplicated inline.
- Known seeded or strategic values render as player-facing Spanish labels.
- Raw enum names and capability keys stay out of the primary cockpit UI.
- Future mission labels clearly communicate that actions are unavailable in this version.
- Frontend build passes.

## Validation

Run as applicable:

```powershell
npm run build --prefix src/VoidEmpires.Frontend
dotnet build --no-restore
dotnet test --no-build
```

Backend validation is only required if backend files are touched.

## Notes / residual risks

- Exact lore flavor can evolve later, but terminology should stay stable enough that later view models and docs do not need another full rename pass.
- Fallback labels should be rare in seeded demo paths; if they dominate the screen, later tasks should refine mapping coverage.

## Commit and push

1. Run `git status`.
2. Verify only intended frontend presentation files changed.
3. Commit with a clear message.
4. Push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer a small helper-focused change.
- Split follow-up cleanup into later tasks if shared status language requires broader refactors.

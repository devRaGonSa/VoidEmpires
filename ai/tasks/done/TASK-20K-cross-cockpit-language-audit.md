# TASK-20K-cross-cockpit-language-audit

---
id: TASK-20K-cross-cockpit-language-audit
title: Cross-cockpit language audit
status: done
type: platform
team: platform
supporting_teams:
  - frontend
  - docs
roadmap_item: "Block 20K-21Z - Cross-cockpit UX consolidation and gameplay language polish"
priority: high
---

## Goal
Audit the visible Spanish copy across all accepted cockpits and identify technical or debug wording that should be converted into player-facing gameplay language.

## Purpose
Create a shared map of where the UI still reads like a development probe so later polish tasks can improve copy systematically without losing safety information that developers still need.

## Current Problem
The app now has the full accepted cockpit set, but many surfaces still read like endpoint probes rather than a coherent command interface. Terms such as `Superficie de endpoints de desarrollo`, `Prototipo frontend de VoidEmpires`, `DTOs crudos`, `Payloads tecnicos`, `endpoint`, `affordance`, `readiness`, `pendiente de clasificar`, and repeated `No disponible en esta build` messaging still appear too close to primary gameplay content.

## Context
- The project remains a development cockpit, so technical limitations and backend context still matter.
- The primary visible layer should now feel closer to a game command surface than a diagnostic dashboard.
- Technical details should remain available, but preferably inside collapsed diagnostics or secondary panels.
- This task is discovery-first and should guide the rest of the polish block.

## Files to Inspect First
- `src/VoidEmpires.Frontend/src/pages/`
- `src/VoidEmpires.Frontend/src/utils/`
- `src/VoidEmpires.Frontend/src/styles.css`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `ai/current-state.md`

## Implementation Requirements
1. Audit primary UI copy across:
   - Galaxy
   - Planet
   - Construction
   - Research
   - Shipyard
   - Fleets
   - Defenses
   - Ground Army
2. Identify wording that should remain technical but move to diagnostics or secondary panels.
3. Identify wording that should become player-facing gameplay copy.
4. Classify findings into:
   - primary gameplay copy
   - secondary limitation copy
   - collapsed diagnostics
   - developer-only docs
5. Add a short documentation note in `docs/dev/frontend-foundation-smoke-checklist.md` or a dedicated UX note listing the copy rules and examples.
6. Do not attempt a broad rewrite in this task unless a replacement is extremely safe and obvious.

## UI/UX Requirements
- Spanish-first.
- Primary UI should prioritize player meaning and action clarity.
- Development safety warnings may remain, but they must not dominate the visual hierarchy.

## Backend/API Requirements
- None.

## Safety Constraints
- No gameplay changes.
- No backend mutations.
- No route or auth changes.

## Expected Files to Modify
- `docs/dev/frontend-foundation-smoke-checklist.md` or a dedicated UX language note under `docs/dev/`
- optionally a very small number of frontend files only if an obviously safe wording fix is bundled with the audit

## Acceptance Criteria
- A clear cross-cockpit language audit exists in the repo.
- The audit distinguishes what should stay technical versus what should become gameplay language.
- Later tasks can use the audit to guide replacements.
- Validation passes for any touched files.

## Validation
- `npm run build --prefix src/VoidEmpires.Frontend` if frontend files are touched
- `dotnet build --no-restore` and `dotnet test --no-build` only if docs or tests introduce a repo workflow reason to run them

## Notes / Residual Risks
- Useful safety information must not be deleted, only relocated to a more appropriate layer.
- Some dev-facing copy may intentionally remain technical in clearly secondary panels.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Keep the task primarily documentation and audit work.

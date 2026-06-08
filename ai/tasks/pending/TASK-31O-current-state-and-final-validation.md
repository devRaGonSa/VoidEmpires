# TASK-31O

---
id: TASK-31O
title: Current state and final validation
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Update `ai/current-state.md` to reflect the final orbital or military preparation posture and run the full final validation bundle for the block.

## Context
After the feature tasks land, the repository’s current-state summary must capture the accepted Shipyard state, the Defenses status, Fleets and Planet read-only boundaries, the new QA preparation helper, and the continued exclusions around combat, movement, and missions. The block also needs one final consolidated validation pass with recorded test count and any notable warnings.

## Implementation steps

1. Update `ai/current-state.md` with the final accepted scope for Shipyard, Defenses, Fleets, Planet, and the orbital production QA preparation path.
2. Record that visual QA remains user-driven.
3. Run the full validation bundle:
   - `dotnet build --no-restore`
   - `dotnet test --no-build`
   - `npm run build --prefix src/VoidEmpires.Frontend`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
   - `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`
4. Record the final test count and any warnings that are acceptable and still relevant.

## Files to read first

- ai/current-state.md
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/frontend-foundation-smoke-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md
- docs/dev/fleets-cockpit-checklist.md
- docs/dev/planet-cockpit-checklist.md

## Expected files to modify

- ai/current-state.md

## Acceptance criteria

- `ai/current-state.md` reflects the final accepted scope accurately.
- The final validation bundle is green.
- The final test count and relevant warnings are recorded for closure.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not overstate unsupported gameplay

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-route-lazy-imports.ps1`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with a clear message.
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

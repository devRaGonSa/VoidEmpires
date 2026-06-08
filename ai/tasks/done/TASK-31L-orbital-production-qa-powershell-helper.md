# TASK-31L

---
id: TASK-31L
title: Orbital production QA PowerShell helper
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 31A-31P - Orbital Production & Military Preparation Gameplay v1"
priority: medium
---

## Goal
Add a PowerShell helper that prepares Development-only orbital or military production QA state through the documented backend endpoint.

## Context
The repository already uses explicit Development QA preparation helpers for Construction and Research. Orbital production needs the same ergonomics: a clear mutation warning, scoped defaults, concise summary output, parser-check coverage, and no attempt to replace the existing preparation helpers for other systems.

## Implementation steps

1. Add `scripts/dev-qa-prepare-orbital-production-ui-state.ps1` with default `BaseUrl`, `CivilizationId`, and `PlanetId` matching the seeded QA baseline.
2. Make the script call the Development-only orbital production QA preparation endpoint, print a concise result summary, and fail cleanly if the backend is unavailable.
3. Extend `scripts/check-dev-qa-scripts.ps1` so the new helper is parser-checked along with the existing QA scripts.
4. Update the relevant docs with the exact command and intended repeated-QA usage.

## Files to read first

- scripts/dev-qa-prepare-construction-ui-state.ps1
- scripts/dev-qa-prepare-research-ui-state.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md

## Expected files to modify

- scripts/dev-qa-prepare-orbital-production-ui-state.ps1
- scripts/check-dev-qa-scripts.ps1
- docs/dev/persisted-gameplay-flow-checklist.md
- docs/dev/shipyard-cockpit-checklist.md
- docs/dev/defenses-cockpit-checklist.md

## Acceptance criteria

- The new helper script exists and parses.
- The script warns clearly that it mutates the Development database for manual orbital or military production QA.
- The helper uses safe defaults and fails cleanly when the backend is unreachable.
- `scripts/check-dev-qa-scripts.ps1` covers the new script.
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`, `dotnet build --no-restore`, and `dotnet test --no-build` pass.

## Constraints

- Follow the architecture and conventions of the current repository
- Do not modify unrelated files
- Keep the change minimal
- Prefer small commits
- Do not replace the construction or research preparation helpers

## Validation

Before completing the task ensure:

- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- `dotnet build --no-restore`
- `dotnet test --no-build`
- no new warnings or obvious regressions are introduced

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

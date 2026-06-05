# TASK-26U Frontend Build Validation Script Doc Update

---
id: TASK-26U
title: Update frontend validation docs for lazy-loading checks
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 26M-26Z - Frontend bundle splitting and cockpit lazy loading"
priority: medium
---

## Purpose
Update developer validation documentation so the frontend build and any new lazy-loading guard checks are clearly included in the standard workflow.

## Current problem
If a new route-lazy-import check is introduced, the repo documentation must reflect the real commands developers should run to validate frontend route-loading changes.

## Context from current implementation
The repository already uses a repeatable validation sequence across .NET build, .NET tests, frontend build, and QA script checks. This task keeps that sequence accurate after the lazy-loading block.

## Goal
Ensure the frontend smoke checklist documents the actual validation commands, including any new route-import guard if one exists.

## Implementation steps
1. Inspect the current frontend smoke checklist and existing validation scripts.
2. Add lazy-loading validation guidance without inventing commands that do not exist.
3. If the route-import guard script exists, document how to run it and what success looks like.
4. Keep the existing .NET, frontend build, and PowerShell script checks visible.
5. Re-run the documented commands to ensure the docs match reality.

## Files to inspect first
- docs/dev/frontend-foundation-smoke-checklist.md
- scripts/check-dev-qa-scripts.ps1
- scripts/check-frontend-route-lazy-imports.ps1
- ai/current-state.md

## Expected files to modify
- docs/dev/frontend-foundation-smoke-checklist.md

## Implementation requirements
- Update `docs/dev/frontend-foundation-smoke-checklist.md`.
- If `scripts/check-frontend-route-lazy-imports.ps1` exists:
- document the command
- document the expected success output or success condition
- Keep existing commands documented:
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`
- `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-dev-qa-scripts.ps1`
- Do not add non-existent validation commands.

## Frontend requirements
- None beyond accurate validation guidance.

## Backend/API requirements
- None.

## Safety constraints
- Docs must match actual repository scripts and commands.
- Do not add speculative workflow steps.

## Acceptance criteria
- Validation docs match the actual repository commands.
- Build and tests remain green.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / residual risks
- If the lazy-import guard is documented but not integrated into a larger script, make that limitation explicit.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines.

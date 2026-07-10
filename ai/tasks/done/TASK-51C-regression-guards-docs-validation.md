# TASK-51C-regression-guards-docs-validation

---
id: TASK-51C
title: Regression guards docs and validation
status: done
type: qa
team: qa
supporting_teams: [frontend, platform]
roadmap_item: "Block 51 - Live queues and visible resource capacity"
priority: high
---

## Goal
Add focused regression protection, update state documentation, and run the requested validation gates.

## Context
Block 51 fixes visual regressions from manual QA without browser/manual retest by Codex.

## Implementation steps

1. Add backend or static guard coverage for numeric enum active queues and defense unit mapping.
2. Extend copy guards to prevent unit defense level-copy regressions.
3. Update ai/current-state.md.
4. Move Block 51 task files to done after validation.
5. Commit and push the branch.

## Files to read first

- scripts/check-frontend-copy-regressions.ps1
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs
- ai/current-state.md

## Expected files to modify

- scripts/check-frontend-copy-regressions.ps1
- tests/VoidEmpires.Tests/DevPlanetUiStateEndpointTests.cs
- tests/VoidEmpires.Tests/DevDefenseUiStateEndpointTests.cs
- ai/current-state.md
- ai/tasks/done/TASK-51*.md

## Acceptance criteria

- Validation commands requested by Block 51 pass.
- ai/tasks/pending contains only .gitkeep.
- Final output includes root causes, files changed, tests/guards, test count, commit SHA, and pushed branch.
- No browser/manual QA is claimed.

## Constraints

- Do not commit generated build artifacts.
- Do not add combat, movement, market, alliance, or espionage behavior.

## Validation

- dotnet build --no-restore
- dotnet test --no-build
- npm run build --prefix src/VoidEmpires.Frontend
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-dev-qa-scripts.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-route-lazy-imports.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-frontend-copy-regressions.ps1
- powershell -NoProfile -ExecutionPolicy Bypass -File ./scripts/check-repo-secret-scan.ps1

## Commit and push

1. Stage intended files.
2. Commit with a clear Block 51 message.
3. Push `codex/block-51-live-queues-defense-quantities-storage-bars`.

## Change Budget

- Keep guard/doc/test changes focused on Block 51 regressions.

# TASK-47A

---
id: TASK-47A
title: Visual review contract for Block 47 module shell fixes
status: done
type: product
team: frontend
supporting_teams: [backend]
roadmap_item: "Block 47A-47K - Module Shell and Buildability Fixes v1"
priority: high
---

## Goal
Document the Block 47 visual review decisions without behavior changes.

## Context
Block 46 cleaned the core OGame-like modules, but follow-up visual review found repeated shell copy, buildability blockers, alignment issues, and incomplete defense production presentation.

## Implementation steps

1. Record that the repeated "Centro imperial" hero/container must be removed from module pages.
2. Record that construction must be playable on a newly registered home planet.
3. Record that shipyard quantity input/action must align cleanly.
4. Record that unit-based defenses must behave like shipyard production with quantity input and build action.
5. Record that "No se encontro la capacidad de edificios del planeta" is a functional defect, not acceptable final copy.

## Files to read first

- ai/current-state.md
- src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx
- src/VoidEmpires.Frontend/src/pages/ShipyardPage.tsx
- src/VoidEmpires.Frontend/src/pages/DefensesPage.tsx
- src/VoidEmpires.Infrastructure/Services/InitialPlayerWorldBootstrapService.cs

## Expected files to modify

- ai/current-state.md
- ai/tasks/pending/TASK-47A-visual-review-contract.md

## Acceptance criteria

- The Block 47 decisions are recorded.
- No behavior change is made in this task.

## Constraints

- Do not modify gameplay, UI, or tests in this documentation-only task.
- Keep Spanish-first product direction explicit.

## Validation

- dotnet build --no-restore
- dotnet test --no-build



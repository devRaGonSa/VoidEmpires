# TASK-13V

---
id: TASK-13V
title: Phase 13V - Planet construction queue readable panel
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Show the planet construction queue clearly and in Spanish so the player can understand current build progress.

## Context
The repository already includes construction queue foundations. Once the Planet cockpit read model is available, the queue should be visible as a readable panel with useful timing and status cues rather than a raw list of orders.

## Implementation steps

1. Review the Planet cockpit queue data and current time or status fields exposed by the backend.
2. Render queue entries with building name, target level, status, timing, and cost or reservation data when available.
3. Add a Spanish empty state for no queued construction.
4. Keep raw order ids secondary and preserve read-only behavior in this task.

## Files to read first

- Planet cockpit route and queue-related components from earlier Planet tasks
- `src/VoidEmpires.Frontend/src/api/` Planet type files
- `src/VoidEmpires.Application/Buildings/`
- `src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs`
- `tests/VoidEmpires.Tests/ConstructionOrderCompletionServiceTests.cs`

## Expected files to modify

- Planet cockpit queue components
- Planet frontend view-model helpers for queue formatting
- `src/VoidEmpires.Frontend/src/styles.css`, if needed

## Acceptance criteria

- Queue entries are readable in Spanish.
- Building name, target level, status, and timing are shown when available.
- Cost or reservation data appears when available.
- The empty state reads `No hay construcciones en cola.`
- Raw order ids remain secondary or collapsed.

## Constraints

- Do not add queue mutations in this task unless already implemented by an earlier task in the block.
- Keep time formatting readable and Spanish-first.
- Avoid raw JSON-first output.
- Preserve graceful handling of missing timing or cost fields.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`

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

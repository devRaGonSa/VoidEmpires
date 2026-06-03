# TASK-13Y

---
id: TASK-13Y
title: Phase 13Y - Planet complete due construction flow or safe placeholder
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 13K-14B"
priority: medium
---

## Goal
Expose controlled construction completion on Planet only if the current backend support is safe; otherwise provide a clear disabled placeholder.

## Context
The repository already includes construction completion foundations. The Planet cockpit should only surface a completion action when it can use an existing or safely added development-only endpoint. Otherwise it should communicate the limitation clearly instead of pretending the action exists.

## Implementation steps

1. Inspect existing construction completion services and any current dev endpoints for due queue processing.
2. If safe support exists, add a confirmation-based `complete due constructions` flow with a visible ready-count when available.
3. Refresh Planet cockpit data after completion success.
4. If safe support does not exist, add a disabled placeholder with clear Spanish messaging.

## Files to read first

- `src/VoidEmpires.Application/Buildings/IConstructionOrderCompletionService.cs`
- `src/VoidEmpires.Infrastructure/Buildings/ConstructionOrderCompletionService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/ConstructionOrderCompletionServiceTests.cs`
- Planet cockpit action components from earlier Planet tasks

## Expected files to modify

- relevant construction completion endpoint files under `src/VoidEmpires.Web`, if needed
- focused tests for any new endpoint
- Planet frontend action components and API client code

## Acceptance criteria

- A controlled completion action is exposed only when safe backend support exists.
- The action requires explicit confirmation.
- Ready or due count is shown when available.
- The Planet cockpit refreshes after success.
- If support is unavailable, the UI shows a disabled placeholder labeled `No disponible en esta build`.

## Constraints

- Do not add background-worker toggles.
- Do not mutate production data automatically.
- Keep the action development-safe.
- Preserve readable Spanish labels and clear limitations.

## Validation

Before completing the task ensure:

- `dotnet build --no-restore`
- `dotnet test --no-build`
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

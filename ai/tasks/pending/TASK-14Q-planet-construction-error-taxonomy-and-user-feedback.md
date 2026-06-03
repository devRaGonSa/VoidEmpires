# TASK-14Q

---
id: TASK-14Q
title: Phase 14Q - Planet construction error taxonomy and user feedback
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Block 14C-14T"
priority: medium
---

## Goal
Normalize user-facing construction errors and backend validation feedback.

## Context
Construction actions can fail for several reasons, but the player should see a consistent Spanish message that explains what to do next. Technical details should remain available in diagnostics without dominating the primary UI.

## Implementation steps

1. Review the current construction error handling and backend result mapping.
2. Map common backend or dev errors to Spanish UI messages.
3. Keep the original technical error in diagnostics only.
4. Preserve backend validation as the authoritative source.

## Files to read first

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/api/planetTypes.ts`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- `tests/VoidEmpires.Tests/DevConstructionQueueEndpointTests.cs`

## Expected files to modify

- `src/VoidEmpires.Frontend/src/pages/PlanetPage.tsx`
- `src/VoidEmpires.Frontend/src/utils/planetPresentation.ts`
- error mapping helpers, if needed

## Acceptance criteria

- Common construction errors are shown in Spanish.
- Raw English backend errors do not dominate primary messages.
- Diagnostics preserve the technical details.
- The UI tells the player or developer what to do next.

## Constraints

- Do not hide validation failures.
- Keep HTTP and backend validation authoritative.
- Avoid exposing raw exception messages as the main UI.

## Validation

Before completing the task ensure:

- `npm run build --prefix src/VoidEmpires.Frontend`
- dotnet tests only if backend result contracts change

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

# TASK-43M-research-page-action-catalog

---
id: TASK-43M
title: Research page action catalog
status: pending
type: fullstack
team: frontend
supporting_teams: [platform]
roadmap_item: "Block 43 - OGame-like Core Game UI Rework v1"
priority: high
---

## Goal
Rework the Research page into a technology catalog.

## Context
The Research page should focus on active research, available resources, technology catalog, and current technology levels. It should not be a navigation or context page.

## Implementation steps

1. Review research API state, page view model, and existing research action behavior.
2. Ensure active/current research queue is visible.
3. Show available resources, technology catalog, and current technology levels.
4. Show available and blocked technologies.
5. Remove generic CTA cards, unrelated navigation, and context strips.
6. Add backend tests only if DTO/readiness data must change.

## Files to read first

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/api/researchTypes.ts
- src/VoidEmpires.Frontend/src/utils/researchPresentation.ts
- src/VoidEmpires.Infrastructure/Research/ResearchQueueService.cs
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs

## Expected files to modify

- src/VoidEmpires.Frontend/src/pages/ResearchPage.tsx
- src/VoidEmpires.Frontend/src/components/ResearchCatalogCard.tsx
- src/VoidEmpires.Frontend/src/styles.css
- src/VoidEmpires.Application/Research/EnqueueResearchOrderResult.cs
- tests/VoidEmpires.Tests/DevResearchUiStateEndpointTests.cs

## Acceptance criteria

- Research page is a technology action catalog.
- Available and blocked technologies are visible.
- No generic CTA cards, unrelated navigation, or context strips remain.

## Constraints

- Do not change research queue semantics unless needed for display readiness.
- Do not add unrelated gameplay systems.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

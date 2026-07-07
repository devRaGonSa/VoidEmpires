# TASK-42A-registration-product-contract-audit

---
id: TASK-42A
title: Registration product contract audit
status: done
type: docs
team: platform
supporting_teams: [frontend, backend]
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Audit current Identity, account, player, civilization, planet ownership, and onboarding behavior, then document the product contract for real account registration and initial world bootstrap.

## Context
VoidEmpires is an online multiplayer web game. The product entry must be account registration, not a local game or local playable session. This task is documentation and audit only; do not change behavior except docs.

## Implementation steps

1. Read the architecture index, component discovery guide, current state, readiness docs, and the onboarding/frontend entry flow.
2. Identify existing ASP.NET Core Identity setup, registration/email behavior, PlayerProfile/Civilization/PlanetOwnership models, and starting civilization/bootstrap services.
3. Identify current `/onboarding` and "new local game" product behavior that must be replaced by registration.
4. Document the intended final flow: register account, create PlayerProfile, create Civilization, assign or generate home planet, create ownership, initialize resources and production, navigate to game.
5. Record deferred items clearly without claiming manual or browser QA.

## Files to read first

- AGENTS.md
- ai/architecture-index.md
- ai/orchestrator/component-discovery.md
- ai/current-state.md
- docs/dev/product-readiness-report.md
- docs/dev/no-visible-development-report.md
- docs/dev/sql-server-runbook.md
- src/VoidEmpires.Web/Program.cs
- src/VoidEmpires.Infrastructure/Identity/IdentityAccountService.cs
- src/VoidEmpires.Infrastructure/Players/StartingCivilizationService.cs
- src/VoidEmpires.Frontend/src/pages/OnboardingPage.tsx

## Expected files to modify

- docs/dev/user-account-auth-readiness.md
- docs/dev/product-readiness-report.md
- docs/dev/local-session-vs-auth-boundary.md

## Acceptance criteria

- Existing Identity setup is documented.
- Existing player/civilization/ownership/bootstrap components are documented.
- Current onboarding/local-session behavior is documented as a replacement target.
- Final registration and initial world bootstrap contract is documented.
- No runtime behavior changes are made.

## Constraints

- Do not implement registration in this task.
- Do not introduce new product copy in the UI.
- Do not claim browser/manual QA.
- Keep the diff documentation-only and under the task budget.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

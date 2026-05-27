# TASK-025

---
id: TASK-025
title: Add Brevo transactional email sender
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Implement the Infrastructure email sender using Brevo while keeping tests network-free.

## Context
Application contracts define transactional email boundaries. Brevo is the provider for user creation and confirmation emails. Future implementation tasks must inspect `devRaGonSa/CreateCvApp` for any existing Brevo or email implementation patterns, configuration conventions, interfaces, service naming, options classes, templates, or testing approaches before coding, and reuse compatible conventions where appropriate.

## Implementation steps

1. Inspect `devRaGonSa/CreateCvApp` for Brevo or email patterns before coding.
2. Modify `src/VoidEmpires.Infrastructure`.
3. Modify `src/VoidEmpires.Web` as needed for DI registration.
4. Modify tests as needed.
5. Implement the provider-agnostic email sender contract in Infrastructure.
6. Use a clean options class such as `BrevoEmailOptions`.
7. Bind options from configuration.
8. Provide a disabled or no-op sender mode when Brevo is disabled or not configured.
9. Prefer a minimal and testable approach such as `HttpClient` if using the Brevo API.
10. Do not add unnecessary packages.
11. Do not send real emails in tests.
12. Do not commit secrets.
13. Do not add registration endpoints yet.
14. Do not add gameplay systems.

## Files to read first

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`
- `ai/reports/identity-email-foundation.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- The Infrastructure email sender implementation exists and is wired through DI.
- Brevo can be disabled without throwing on startup.
- Disabled mode returns a deterministic skipped or failure result.
- Tests do not call Brevo network endpoints.
- No secrets are exposed in logs or exceptions.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Tests do not perform network calls.
- No secrets are committed.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(email): add Brevo transactional email sender`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

# TASK-022

---
id: TASK-022
title: Add identity and email application contracts
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Introduce application-level contracts for user and account operations plus transactional email without implementing persistence or Brevo yet.

## Context
The Application layer should define use-case-facing contracts. Infrastructure will later implement ASP.NET Core Identity persistence and Brevo email delivery.

## Implementation steps

1. Modify `src/VoidEmpires.Application`.
2. Add provider-agnostic contracts for user registration, email confirmation, and transactional email sending.
3. Add simple request and result models for those use cases.
4. Keep Brevo-specific types out of the Application layer.
5. Keep the models deterministic and testable.
6. Do not modify Infrastructure unless strictly required for compilation.
7. Do not add ASP.NET Core Identity packages unless already present and required.
8. Do not add Brevo packages.
9. Do not add migrations.
10. Do not add controllers or endpoints yet.
11. Do not add gameplay systems.

## Files to read first

- `src/VoidEmpires.Application/*`
- `ai/reports/identity-email-foundation.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `tests/VoidEmpires.Tests/*` if needed

## Acceptance criteria

- Application contains provider-agnostic identity and transactional email contracts.
- Result models represent success or failure deterministically.
- No Brevo-specific type leaks into Application.
- No ASP.NET Core Identity entity leaks into the contracts unless unavoidable.
- Tests cover model creation and any deterministic result helpers.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Application has no Brevo dependency.
- All tests pass.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(application): add identity and email contracts`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

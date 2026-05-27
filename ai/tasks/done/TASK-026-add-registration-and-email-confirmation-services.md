# TASK-026

---
id: TASK-026
title: Add registration and email confirmation services
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 2A - Identity and Brevo email foundation"
priority: high
---

## Goal
Implement application-facing registration and email confirmation services using ASP.NET Core Identity and the transactional email sender.

## Context
VoidEmpires needs a backend foundation for registering users and sending email confirmation links through the email abstraction. Public endpoints can be added later. This task should implement services and tests without requiring real PostgreSQL or Brevo.

## Implementation steps

1. Modify `src/VoidEmpires.Application` if contracts need refinement.
2. Modify `src/VoidEmpires.Infrastructure` for service implementations.
3. Modify `src/VoidEmpires.Web` only for DI registration if needed.
4. Modify tests.
5. Implement `IUserRegistrationService` using ASP.NET Core Identity `UserManager`.
6. Implement `IEmailConfirmationService` using the Identity token confirmation flow.
7. Ensure registration accepts email and password through `RegisterUserRequest`.
8. Ensure registration creates a user with email as username or an equivalent deterministic convention.
9. Generate an email confirmation token.
10. Create a confirmation link using configured base URL or application URL options.
11. Send the confirmation email through `ITransactionalEmailSender`.
12. Return deterministic result models.
13. Do not add a public registration UI.
14. Do not add complex controllers unless minimal API endpoints are explicitly required for testing.
15. Do not add gameplay entities.
16. Do not call real Brevo.
17. Do not connect to real PostgreSQL.

## Files to read first

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`
- `ai/reports/identity-email-foundation.md`
- `ai/task-template.md`

## Expected files to modify

- `src/VoidEmpires.Application/*`
- `src/VoidEmpires.Infrastructure/*`
- `src/VoidEmpires.Web/*`
- `tests/VoidEmpires.Tests/*`

## Acceptance criteria

- Registration and email confirmation services are implemented.
- Confirmation emails are sent through the transactional email sender abstraction.
- Tests use fakes or suitable test infrastructure and do not require real PostgreSQL or Brevo.
- The implementation remains deterministic and testable.

## Validation

Before completing the task ensure:

- `dotnet restore` succeeds.
- `dotnet build --no-restore` succeeds.
- `dotnet test --no-build` succeeds.
- Registration service tests pass.
- Email sender is invoked in registration tests.
- No real email is sent.
- No real database is required.
- Commit and push the changes.

## Commit and push

At the end:

1. Run `git status`.
2. Stage the intended files.
3. Commit with message: `feat(identity): add registration and email confirmation services`
4. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

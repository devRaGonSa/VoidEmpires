# TASK-42D-identity-registration-service

---
id: TASK-42D
title: Identity registration service
status: done
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Implement an account registration service using ASP.NET Core Identity.

## Context
Account creation must use Identity `UserManager` so passwords are hashed by Identity only. This task handles account creation and duplicate account handling, not world bootstrap endpoint wiring.

## Implementation steps

1. Review current Identity service and DI registration.
2. Add an application interface for account registration if one does not already exist.
3. Implement registration in Infrastructure using `UserManager`.
4. Reject duplicate email/account safely and return structured errors.
5. Ensure password and confirm password are not logged, persisted outside Identity, or returned.
6. Add service tests for success, duplicate email, invalid password, and safe errors.

## Files to read first

- ai/orchestrator/di-analysis.md
- src/VoidEmpires.Application/IdentityEmailContracts.cs
- src/VoidEmpires.Infrastructure/Identity/IdentityAccountService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
- tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Identity/IAccountRegistrationService.cs
- src/VoidEmpires.Infrastructure/Identity/IdentityAccountRegistrationService.cs
- src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs
- tests/VoidEmpires.Tests/IdentityAccountRegistrationServiceTests.cs

## Acceptance criteria

- Registration uses `UserManager` and Identity password hashing.
- Duplicate email/account is rejected with safe structured errors.
- Passwords are not logged or returned.
- Normal tests remain provider-independent and do not require SQL Server.

## Constraints

- Do not implement login/logout endpoints in this task.
- Do not require email verification or email delivery.
- Do not store auth tokens in localStorage or backend custom tables.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

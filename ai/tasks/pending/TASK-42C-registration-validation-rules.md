# TASK-42C-registration-validation-rules

---
id: TASK-42C
title: Registration validation rules
status: pending
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Implement authoritative backend validation rules for registration input.

## Context
Frontend validation may improve user experience later, but backend validation is the source of truth. Validation should normalize player-facing names and return safe structured errors.

## Implementation steps

1. Review existing validation patterns in application services and tests.
2. Add or extend registration validation so email is required and valid.
3. Require password and confirmPassword, enforce the configured minimum safe policy, and require the values to match.
4. Require commander/display name and civilization name, normalize whitespace, enforce length limits, and reject empty normalized values.
5. Allow optional home planet name, normalize whitespace, and enforce a length limit when supplied.
6. Add tests for valid input, missing fields, invalid email, weak password, mismatched confirmation, normalization, and length limits.

## Files to read first

- src/VoidEmpires.Application/Identity/AccountRegistrationRequest.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationResult.cs
- tests/VoidEmpires.Tests/AccountRegistrationContractTests.cs
- tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Identity/AccountRegistrationValidator.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationError.cs
- tests/VoidEmpires.Tests/AccountRegistrationValidationTests.cs

## Acceptance criteria

- Backend validation covers all requested fields.
- Normalization is deterministic and tested.
- Validation errors are structured for later Spanish-friendly UI mapping.
- Password values are never included in error messages.

## Constraints

- Do not implement the registration endpoint in this task.
- Do not weaken ASP.NET Core Identity password hashing or policy behavior.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

# TASK-42B-account-registration-domain-contract

---
id: TASK-42B
title: Account registration domain contract
status: done
type: backend
team: platform
supporting_teams: []
roadmap_item: "Block 42 - User Registration, Account Session & Initial Player World Bootstrap v1"
priority: high
---

## Goal
Define backend request and response contracts for account registration and initial player world bootstrap.

## Context
The backend must expose safe application-level contracts before endpoint and UI work. Contracts must not expose passwords, raw Identity internals, or implementation-specific secrets.

## Implementation steps

1. Review existing application contract style and Identity account contracts.
2. Add request DTO fields: `email`, `password`, `confirmPassword`, `commanderName` or `displayName`, `civilizationName`, and optional `homePlanetName`.
3. Add response DTO fields: `succeeded`, safe account/user id if appropriate, `playerProfileId`, `civilizationId`, `homePlanetId`, `homePlanetName`, `nextRoute`, and validation errors.
4. Add structured error DTOs that can later map to Spanish frontend copy.
5. Add focused tests for serialization shape and password exclusion where practical.

## Files to read first

- ai/orchestrator/component-discovery.md
- src/VoidEmpires.Application/IdentityEmailContracts.cs
- src/VoidEmpires.Application/Players/CreateStartingCivilizationRequest.cs
- src/VoidEmpires.Application/Players/CreateStartingCivilizationResult.cs
- tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs

## Expected files to modify

- src/VoidEmpires.Application/Identity/AccountRegistrationRequest.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationResult.cs
- src/VoidEmpires.Application/Identity/AccountRegistrationError.cs
- tests/VoidEmpires.Tests/AccountRegistrationContractTests.cs

## Acceptance criteria

- Registration request and response contracts exist in the Application layer.
- Password and confirm password exist only on request types.
- Response contains safe world bootstrap identifiers and navigation data.
- Validation errors are structured and do not leak Identity internals.

## Constraints

- Do not implement persistence or endpoints in this task.
- Do not store or log passwords.
- Follow existing DTO naming and namespace conventions.

## Validation

- `dotnet build --no-restore`
- `dotnet test --no-build`

## Commit and push

At the end, run `git status`, stage only intended files, commit with a clear message, and push the branch.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Split the work into additional tasks if limits are exceeded.

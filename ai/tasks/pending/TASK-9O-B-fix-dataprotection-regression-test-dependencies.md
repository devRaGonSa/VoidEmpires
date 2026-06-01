# TASK-9O-B

---
id: TASK-9O-B
title: Phase 9O-B - Fix DataProtection regression test dependencies
status: pending
type: bugfix
team: backend
supporting_teams:
  - architecture
  - qa
roadmap_item: "Phase 9O-B - Fix DataProtection regression test dependencies"
priority: high
---

## Goal

Fix the failing regression test introduced in Phase 9O without weakening the DataProtection coverage.

The task should preserve the production Data Protection fix and keep regression coverage proving that `DataProtectorTokenProvider<VoidEmpiresUser>` can be constructed when the service provider is validated.

## Context

Phase 9O was implemented on `main` as commit `c38d1a0`. It added Data Protection registration and a regression test:

- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`
- `IdentityRegistrationSupportsValidatedServiceProviderConstruction`

Current local validation:

- `dotnet build --no-restore` passes
- `dotnet test --no-build` fails with `1` failed test
- Total: `528` tests, Passed: `527`, Failed: `1`

Failing test:

- `VoidEmpires.Tests.PersistenceRegistrationTests.IdentityRegistrationSupportsValidatedServiceProviderConstruction`

Observed error:

- `Unable to resolve service for type 'VoidEmpires.Application.Email.ITransactionalEmailSender' while attempting to activate 'VoidEmpires.Infrastructure.Identity.IdentityAccountService'`

Root cause:

- The new validated service-provider regression test registers:
  - `AddLogging()`
  - `AddVoidEmpiresPersistence(...)`
  - `AddVoidEmpiresIdentity()`
- It then builds the provider with `ValidateOnBuild = true` and `ValidateScopes = true`
- This also validates `IdentityAccountService`
- The isolated persistence and identity test setup does not register a fake or stub `ITransactionalEmailSender`
- Full application startup normally registers email services elsewhere, but this isolated test does not

The corrective task should keep the regression focused on DI and startup validation, without connecting to PostgreSQL, without applying migrations, and without touching frontend behavior.

## Implementation steps

1. Inspect:
   - `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`
   - `src/VoidEmpires.Application/Email/ITransactionalEmailSender.cs` or the repository location of the contract
   - `src/VoidEmpires.Infrastructure/Identity/IdentityAccountService.cs`
   - existing email test fakes or helpers, if any
2. Register a deterministic fake or stub `ITransactionalEmailSender` in the failing test setup, or reuse an existing test fake if the repository already has one.
3. Keep the test focused on DI and startup validation:
   - it should still build the provider with `ValidateOnBuild = true`
   - it should still resolve `DataProtectorTokenProvider<VoidEmpiresUser>`
   - it should still avoid any real external email provider
4. Run:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

5. Move the task file from `ai/tasks/pending` to `ai/tasks/done` only after validation passes.
6. Do not modify unrelated untracked files, especially `xuniverse_planet_generator_variety.py`.

## Files to read first

- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`
- `src/VoidEmpires.Infrastructure/Identity/IdentityAccountService.cs`
- `tests/VoidEmpires.Tests/IdentityEmailContractTests.cs`
- `tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresEmailServiceCollectionExtensions.cs`
- `AGENTS.md`

## Expected files to modify

Expected:

- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`

May also modify:

- `tests/VoidEmpires.Tests/IdentityEmailContractTests.cs`
- `tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs`
- existing shared test helper files if a reusable email fake already exists

## Acceptance criteria

- The production Data Protection fix remains in place.
- The regression test for validated DI construction remains in place.
- The test still verifies `DataProtectorTokenProvider<VoidEmpiresUser>` can be resolved.
- `ValidateOnBuild = true` remains part of the regression strategy unless a clearly justified stronger approach is adopted.
- The failing test no longer depends on unrelated real email registrations.
- No real external email provider is used in tests.
- `dotnet build --no-restore` passes.
- `dotnet test --no-build` passes, expected `528/528` unless the test count legitimately changes.

## Constraints

- Do not commit secrets.
- Do not hardcode local IPs, passwords, or real connection strings.
- Do not apply EF migrations.
- Do not weaken Identity or DataProtection registration.
- Do not remove the regression test.
- Do not change frontend behavior.
- Do not connect to PostgreSQL.
- Keep the change minimal and reviewable.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

Expected:

- build passes
- all tests pass
- no new warnings introduced

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only the intended test and helper files changed.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single small commit.
- If additional unrelated DI issues appear, create a follow-up task instead of broadening this fix.

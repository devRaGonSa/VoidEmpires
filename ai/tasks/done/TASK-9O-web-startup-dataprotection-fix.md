# TASK-9O

---
id: TASK-9O
title: Phase 9O - Web startup DataProtection registration fix
status: done
type: bugfix
team: backend
supporting_teams:
  - architecture
  - qa
roadmap_item: "Phase 9O - Web startup DataProtection registration fix"
priority: high
---

## Goal

Fix `VoidEmpires.Web` startup when persistence and auth are configured by registering ASP.NET Core Data Protection consistently with the existing Identity token provider setup.

The change should be the smallest architecture-consistent fix that allows `DataProtectorTokenProvider<VoidEmpiresUser>` to be constructed during host startup.

## Context

Current consolidated state:

- `main` includes PR `#64`: `feat(frontend): align UI foundation with Figma`
- Backend baseline before this issue: `dotnet build --no-restore` OK and `dotnet test --no-build` `527/527` passing
- Frontend baseline: `npm install` and `npm run build` OK
- PostgreSQL is assumed to be running correctly for manual local startup scenarios

Observed runtime failure:

- Running `dotnet run --project src/VoidEmpires.Web` with `ConnectionStrings__DefaultConnection` configured fails during `builder.Build()`
- The reported DI error is:
  - `System.AggregateException: Some services are not able to be constructed`
  - `Unable to resolve service for type 'Microsoft.AspNetCore.DataProtection.IDataProtectionProvider' while attempting to activate 'Microsoft.AspNetCore.Identity.DataProtectorTokenProvider<VoidEmpiresUser>'`

Relevant code locations:

- `src/VoidEmpires.Web/Program.cs` registers `AddVoidEmpiresIdentity()` only when `DefaultConnection` is configured
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs` contains `AddVoidEmpiresIdentity()`
- `AddVoidEmpiresIdentity()` uses `.AddTokenProvider<DataProtectorTokenProvider<VoidEmpiresUser>>(TokenOptions.DefaultProvider)`
- No Data Protection provider appears to be registered before the Identity token provider is validated

The fix must preserve existing behavior when persistence is not configured, preserve Development-only endpoint behavior, and preserve the current Identity registration semantics.

## Implementation steps

1. Inspect:
   - `src/VoidEmpires.Web/Program.cs`
   - `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
   - existing startup, persistence, identity, and `WebApplicationFactory`-style tests
2. Implement the minimal fix:
   - register ASP.NET Core Data Protection when Identity and `DataProtectorTokenProvider` are used
   - use `Microsoft.AspNetCore.DataProtection` if needed
   - place the registration in the smallest architecture-consistent location
3. Add or adjust automated tests:
   - cover host or service-provider construction with a configured `DefaultConnection`
   - ensure the failure `Unable to resolve IDataProtectionProvider` cannot regress
   - keep tests deterministic and avoid requiring a real PostgreSQL connection
4. Do not apply migrations.
5. Do not connect to the real NAS or PostgreSQL database.
6. Do not change frontend code.
7. Do not add CORS in this task.
8. Do not add gameplay features.
9. Do not introduce auth flows beyond fixing startup dependency registration.
10. Update `ai/current-state.md` only if repository convention expects task or state updates for this kind of backend readiness fix. If updated, keep it concise and factual.

## Files to read first

- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`
- `tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs`
- `tests/VoidEmpires.Tests/IdentityEmailContractTests.cs`
- `AGENTS.md`

## Expected files to modify

Expected:

- `src/VoidEmpires.Web/Program.cs`
- `src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs`
- `tests/VoidEmpires.Tests/PersistenceRegistrationTests.cs`

May also modify:

- `tests/VoidEmpires.Tests/IdentityAccountServiceTests.cs`
- `tests/VoidEmpires.Tests/IdentityEmailContractTests.cs`
- `ai/current-state.md`

## Acceptance criteria

- `VoidEmpires.Web` can build its host successfully when `ConnectionStrings__DefaultConnection` is configured and Identity services are registered.
- `DataProtectorTokenProvider<VoidEmpiresUser>` no longer fails due to missing `IDataProtectionProvider`.
- Existing behavior is preserved when persistence is not configured.
- Development-only endpoint behavior is preserved.
- Identity registration semantics remain unchanged beyond the dependency registration fix.
- Automated tests cover the startup or DI construction path that previously failed.
- Tests remain deterministic and do not require a real PostgreSQL connection.
- No frontend code is changed.
- No migrations are applied.

## Constraints

- Do not commit secrets.
- Do not hardcode local IPs, passwords, or connection strings.
- Do not apply EF migrations automatically.
- Do not change production gameplay behavior.
- Do not change frontend behavior.
- Do not introduce WebSockets, Three.js/WebGL, auth productization, or gameplay mutations.
- Keep architecture boundaries intact: Web startup orchestration in Web, reusable service registrations in Infrastructure where appropriate, tests in `tests/VoidEmpires.Tests`.
- Keep the change incremental and reviewable.

## Validation

Run from repository root:

```powershell
dotnet build --no-restore
dotnet test --no-build
```

Expected:

- build passes
- all existing tests pass
- new or updated tests pass
- no new warnings introduced

## Manual validation note

After the task is implemented and merged locally, rerun:

```powershell
$env:ASPNETCORE_ENVIRONMENT="Development"
$env:ConnectionStrings__DefaultConnection="Host=192.168.178.28;Port=5432;Database=appdb;Username=postgreuser;Password=P5wPduHq2Metallica2"
dotnet run --project src/VoidEmpires.Web
```

Then verify:

- [http://localhost:5142/health](http://localhost:5142/health)

## Commit and push

At the end:

1. Run `git status`.
2. Run `git diff --name-only`.
3. Verify only the intended backend startup, service-registration, test, and optional state files changed.
4. Commit with a clear message.
5. Push the branch to the remote.

## Change Budget

- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Prefer a single commit for this task.
- If the fix expands beyond startup registration and deterministic tests, split follow-up work into a new task.

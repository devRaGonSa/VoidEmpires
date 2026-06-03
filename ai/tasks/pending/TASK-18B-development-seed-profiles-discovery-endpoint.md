# TASK-18B-development-seed-profiles-discovery-endpoint

---
id: TASK-18B-development-seed-profiles-discovery-endpoint
title: Development seed profiles discovery endpoint
status: pending
type: platform
team: platform
supporting_teams:
  - backend
  - docs
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Expose a Development-only endpoint that lists available seed profiles and their intended use.

## Purpose
Make seed profile discovery explicit so users do not have to guess profile names from docs or source code.

## Current Problem
As seed profiles grow, QA becomes harder if users must infer supported profile names manually. The repository already has a dev seed apply flow, but discoverability is still weak unless the available profiles and their usage are exposed directly.

## Context
- A dev seed apply endpoint already exists.
- The new block introduces the concept of multiple named profiles.
- A lightweight read-only list endpoint or richer metadata response would improve both PowerShell use and doc accuracy.

## Files to Inspect First
- `src/VoidEmpires.Application/Development/`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `tests/VoidEmpires.Tests/DevelopmentSeedServiceTests.cs`
- `docs/dev/development-seed-profiles.md`

## Implementation Requirements
1. Add a Development-only endpoint if it does not already exist:
   - `GET /api/dev/seeds/profiles`
2. Return concise profile metadata, including:
   - profile name;
   - description;
   - intended cockpits;
   - `destructive: false`;
   - `deterministic: true`;
   - key ids if useful;
   - recommended QA URLs where appropriate.
3. If a list endpoint already exists, extend it rather than duplicating it.
4. Add endpoint tests for:
   - unavailable or blocked outside Development if that matches current patterns;
   - success in Development;
   - inclusion of at least `minimal-validation` and `cockpit-validation`.
5. Document the endpoint in the seed profile docs.
6. Keep the response concise and readable.

## UI/UX Requirements
- PowerShell and raw JSON output should be easy to scan.
- The response should help humans choose a profile quickly.

## Backend/API Requirements
- Development-only.
- Read-only and non-mutating.
- Do not expose secrets, connection strings, or internal environment details.

## Safety Constraints
- The endpoint must not mutate the database.
- No production exposure.
- Do not overload the endpoint with full seed internals that make the response noisy or brittle.

## Expected Files to Modify
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Application/Development/` or Infrastructure seed metadata files
- `tests/VoidEmpires.Tests/` endpoint tests
- `docs/dev/development-seed-profiles.md`

## Acceptance Criteria
- Users can discover seed profiles through a Development-only endpoint.
- Tests pass.
- Docs mention the endpoint and how to use it.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- A future frontend utility could consume this endpoint, but that is outside this task.
- Keep the endpoint contract stable and minimal.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Prefer a simple metadata endpoint over a more ambitious admin surface.

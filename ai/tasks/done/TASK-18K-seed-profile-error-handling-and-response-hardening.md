# TASK-18K-seed-profile-error-handling-and-response-hardening

---
id: TASK-18K-seed-profile-error-handling-and-response-hardening
title: Seed profile error handling and response hardening
status: done
type: platform
team: platform
supporting_teams:
  - backend
roadmap_item: "Block 18G-18R - Cockpit validation seed idempotency runtime hardening"
priority: medium
---

## Goal
Prevent known seed application failures from surfacing as opaque HTTP 500 responses where a structured Development-friendly error is possible.

## Purpose
Make local QA failures faster to diagnose without hiding unexpected exceptions.

## Current Problem
The current seed apply endpoint lets runtime exceptions bubble into a raw 500. That makes known idempotency mistakes harder to diagnose and pushes developers toward reading logs instead of receiving a structured failure result from the Development endpoint.

## Context
- The current runtime failure surfaced as an unhandled PostgreSQL unique constraint violation.
- The endpoint already returns structured success and validation-failure responses for other cases.

## Files to Inspect First
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- `src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs`
- Seed apply endpoint tests

## Implementation Requirements
1. Decide whether known seed application failures should be translated into a structured failure response consistent with existing endpoint patterns.
2. If endpoint behavior changes, return a developer-friendly structured payload for known idempotency or validation failures.
3. Do not swallow unexpected exceptions silently.
4. Keep diagnostics useful enough that local QA can understand what failed and why.
5. Add or update tests only if endpoint behavior changes.

## UI/UX Requirements
- PowerShell callers should see a meaningful structured response rather than only an opaque 500 when the failure is known and classifiable.

## Backend/API Requirements
- Keep behavior Development-only.
- Preserve existing success payload conventions where possible.
- Avoid broad exception-catching that hides real bugs.

## Safety Constraints
- Do not mask unexpected failures as success.
- Do not weaken validation.
- Keep logs and responses developer-oriented without exposing secrets.

## Expected Files to Modify
- `src/VoidEmpires.Web/DevEndpointMappings.cs`
- Seed-related application or infrastructure files only if needed to classify failures
- Endpoint tests if behavior changes

## Acceptance Criteria
- Known seed-apply failures no longer default to opaque responses when they can be classified safely.
- Unexpected failures remain visible for debugging.
- Tests pass if endpoint behavior changes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`

## Notes / Residual Risks
- The best outcome is still to prevent the collision in the seed itself; response hardening is secondary defense.
- Keep the error contract small and developer-friendly.

## Change Budget
- Prefer modifying fewer than 5 files.
- Prefer changes under 200 lines of code.
- Avoid a large custom exception framework.

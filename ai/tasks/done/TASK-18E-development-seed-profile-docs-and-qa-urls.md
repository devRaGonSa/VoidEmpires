# TASK-18E-development-seed-profile-docs-and-qa-urls

---
id: TASK-18E-development-seed-profile-docs-and-qa-urls
title: Development seed profile docs and QA URLs
status: pending
type: platform
team: platform
supporting_teams:
  - docs
  - backend
roadmap_item: "Block 17U-18F - Development simulation data profiles and cockpit QA seeds"
priority: medium
---

## Goal
Document all supported development seed profiles, deterministic ids, and QA URLs so manual validation is reproducible without tribal knowledge or SQL.

## Purpose
Give users copy-pasteable commands and URLs for cockpit QA, and make the seed profile system operationally useful.

## Current Problem
Seed usage becomes fragile when profile names, commands, and QA URLs are scattered across multiple cockpit docs or hidden in source code. The user explicitly needs a standard path that does not rely on manual SQL.

## Context
- Some cockpit docs already include seeded URLs.
- The new block introduces profile naming, richer QA profiles, and possibly a discovery endpoint.
- The repo already has multiple cockpit checklists that should cross-link rather than drift apart.

## Files to Inspect First
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- `docs/dev/planet-cockpit-checklist.md`
- `docs/dev/construction-cockpit-checklist.md`
- `docs/dev/research-cockpit-checklist.md`
- `docs/dev/shipyard-cockpit-checklist.md`
- `docs/dev/fleet-controlled-mutation-checklist.md`

## Implementation Requirements
1. Create or update `docs/dev/development-seed-profiles.md`.
2. Include:
   - PowerShell examples to apply seed profiles;
   - discovery endpoint example if implemented;
   - profile names and intended use;
   - deterministic ids;
   - QA URLs for:
     - `/galaxy`
     - `/planet`
     - `/construction`
     - `/research`
     - `/shipyard`
     - `/fleets`
     - `/ground-army`
     - `/defenses`
   - expected high-level state per profile.
3. Update individual cockpit checklists to reference `development-seed-profiles.md`.
4. Add explicit guidance:
   - do not use manual SQL for standard QA;
   - reapply the documented profile if the local dev DB state becomes confusing;
   - seed profiles are idempotent and Development-only.
5. Keep the docs concise and practical rather than overly encyclopedic.

## UI/UX Requirements
- Docs must be copy-pasteable and easy to scan.
- URLs and commands should be ready for direct use in local QA.

## Backend/API Requirements
- No backend change is required unless documentation must reference a new discovery endpoint from another task.

## Safety Constraints
- Do not include secrets.
- Do not include connection strings.
- Do not document unsupported behavior as already implemented.

## Expected Files to Modify
- `docs/dev/development-seed-profiles.md`
- `docs/dev/frontend-foundation-smoke-checklist.md`
- Relevant cockpit checklist docs that should cross-link to the seed profile guide

## Acceptance Criteria
- Users can reproduce QA state from docs alone.
- Docs cross-link the relevant cockpit checklists.
- Validation passes.

## Validation
- `dotnet build --no-restore`
- `dotnet test --no-build`
- `npm run build --prefix src/VoidEmpires.Frontend`

## Notes / Residual Risks
- Keep the seed profile guide updated as future profiles are added.
- Prefer one central seed guide plus cockpit-specific cross-links over repeated duplicated instructions.

## Change Budget
- Prefer modifying fewer than 5 files when possible.
- Prefer changes under 200 lines of code when possible.
- Keep the documentation practical and operational.

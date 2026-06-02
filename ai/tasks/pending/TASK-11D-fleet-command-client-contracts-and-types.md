# TASK-11D

---
id: TASK-11D
title: Phase 11D - Fleet command client contracts and types
status: pending
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 11D"
priority: medium
---

## Goal
Add frontend client-side API contracts and types for fleet command execution endpoints so future UI execution work can consume typed responses safely.

## Scope
Inspect the frontend API files, action manifest types, fleet UI state types, and existing client conventions. Keep backend contracts unchanged.

## Work
- Add or refine TypeScript types for estimate, create, cancel, complete-due, split, and merge endpoints.
- Represent success, payload, errors, and conflict or not-found cases consistently.
- Add typed request builders or API functions if the project already has a matching pattern.

## Acceptance
- Frontend has typed contracts for the fleet command endpoints.
- No visible execution controls or backend changes are introduced.
- If frontend is touched, the frontend build is part of validation.

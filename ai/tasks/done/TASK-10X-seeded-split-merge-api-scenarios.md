# TASK-10X

---
id: TASK-10X
title: Phase 10X - Seeded split and merge API scenarios
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10X"
priority: medium
---

## Goal
Add seeded API scenario coverage proving split and merge work correctly against the minimal-validation seed dataset.

## Scope
Use the minimal-validation seed dataset and the seeded WebApplicationFactory patterns from Block 10Q-10T. Update docs only if the scenario flow needs to be recorded.

## Work
- Exercise split, merge, and a small set of rejection scenarios against seeded data.
- Assert state changes, conservation of total quantity, and the expected API outcomes.
- Keep the tests deterministic and use existing seeded constants where available.

## Acceptance
- Seeded split and merge scenarios pass against the minimal-validation seed.
- Rejection scenarios do not mutate state.
- No frontend changes, PostgreSQL access, EF migrations, or manual visual validation.

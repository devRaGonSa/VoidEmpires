# TASK-10R

---
id: TASK-10R
title: Phase 10R - Seeded create transfer API scenario
status: done
type: platform
team: platform
supporting_teams: []
roadmap_item: "Phase 10R"
priority: medium
---

## Goal
Add seeded API scenario coverage proving create transfer mutates the seeded fleet state exactly as intended.

## Scope
Use the minimal-validation seed dataset and the current fleet API surface to verify create-transfer behavior before and after the mutation.

## Work
- Apply the seed and capture initial `ui-state`.
- Create a transfer for a seeded available group and valid destination.
- Capture follow-up `ui-state` and repeat the create call to confirm conflict or rejection.

## Acceptance
- Create returns the current success status and response contract.
- Exactly one active transfer exists for the selected group.
- The group becomes reserved or in-transfer, and resource balance decreases by the expected or returned cost.
- Repeated create does not duplicate the transfer.
- No frontend commands, real-time movement, combat, or EF migrations.

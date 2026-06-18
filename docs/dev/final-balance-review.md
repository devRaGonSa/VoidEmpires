# Final Balance Review

Status date: 2026-06-18

This note reviews the current catalog and starting-data balance assumptions before final DB/catalog consolidation.

It is a documentation-only review. It does not change balance, seed values, queue rules, combat, fleet movement, market execution, or alliance behavior.

## Scope

Reviewed dimensions:

- starting resources and production rates
- building costs, footprint, and upgrade assumptions
- research costs, target-level scaling, and duration assumptions
- ship costs, crew thresholds, and production duration assumptions
- defense readiness and current defense scarcity
- Development seed assumptions that shape visible cockpit comparisons

## Current Snapshot

### Starting stockpile baseline

The minimal Development baseline for the owned `Aurelia` colony is:

| Resource | Minimum visible stockpile |
|---|---:|
| Credits | 125 |
| Metal | 160 |
| Crystal | 100 |
| Gas | 50 |

Current baseline production profile:

| Resource | Rate per hour |
|---|---:|
| Credits | 18 |
| Metal | 14 |
| Crystal | 6 |
| Gas | 3 |

Review:

- The opening stockpile is intentionally tight. It supports one first action path, but not broad parallel progression.
- Metal is the early anchor resource by both stockpile and production rate, which matches building-heavy onboarding.
- Gas is clearly the gating resource for higher-tier building, research, and orbital production choices.
- These values are safe for Development comparison flows, but they are not yet justified as a final live-economy baseline.

### Building costs and upgrade posture

Current building costs show a clear early-to-late ladder:

- Cheap openers: `SolarPlant`, `MetalMine`, `CrystalMine`, `GasExtractor`
- Mid-tier unlocks: `ResearchLab`, `Shipyard`, `DefenseGrid`, `HabitationDistrict`
- Higher readiness gates: `MilitaryAcademy`, `Barracks`, `CrewAcademy`, `FleetCommandCenter`, `LogisticsHub`
- Capital anchor: `CommandCenter`

Review:

- The spread is coherent for a first colony: economy buildings are cheap enough to compare, while orbital and military readiness stays gated.
- `CommandCenter` at `M500 X250` is materially above the rest and reads like a strategic anchor rather than an early upgrade target.
- `Shipyard` at `M250 X100 G50` is reachable only after some economy setup, which matches the current single-colony pacing.
- Current upgrade balance is not actually finalized because the live rule is still `base cost * target level`, documented in the construction/readiness notes rather than seed metadata.
- Footprint values vary meaningfully, but the current review cannot yet prove they create a satisfying long-term colony layout because capacity pressure is still Development-shaped.

### Research costs and duration posture

Current research rules:

- target level is always `current + 1`
- cost is `base cost * target level`
- base duration is `10 minutes * target level`
- `EnergySystems` currently shortens research duration

Review:

- `PlanetaryEngineering` is intentionally the cheapest opening research and works as the visible first-queue candidate.
- `ResourceExtraction` and `EnergySystems` create a credible early fork between economy throughput and timing efficiency.
- `ConstructionAutomation` is priced like a meaningful medium-term infrastructure upgrade, which fits its broad multiplier role.
- `Propulsion`, `ShipWeapons`, `Shielding`, and `Espionage` are more expensive than current fully productized systems justify; that is acceptable only because several of their downstream effects remain readiness-only placeholders.
- There is no accepted final prerequisite tree, max-level rule, or research-path pacing yet, so the current table should be treated as provisional balance scaffolding rather than a final tech tree.

### Ship costs, crew gates, and duration posture

Current orbital production rules:

- build time is `3 minutes * quantity`
- one open orbital production order per planet
- crew capacity is checked against local buildings and population
- ship requirements are still readiness gates, not full movement or combat balance

Review:

- `ScoutCraft` is correctly positioned as the cheapest hull and the first visible orbital-production option.
- `CargoCraft` is a moderate step up, which fits logistics utility without making it the universal first pick.
- `EscortCraft` jumps sharply in both resource and crew requirements; that keeps combat-adjacent hulls out of the early baseline.
- `ColonyCraft` is intentionally expensive and should remain clearly aspirational until colonization execution is accepted product scope.
- Uniform `3 min/unit` timing is safe for Development queue testing, but it is too flat to count as final ship pacing.
- Crew thresholds look directionally useful as readiness gates, but they still need a final relationship to population growth, crew buildings, and orbital scale.

### Defense readiness posture

Current defense state is intentionally narrow:

- only `DefenseGrid` is modeled
- defense cost is `M150 X75`
- defenses remain construction-backed and read-only in their specialized cockpit

Review:

- One defense row is enough for readiness QA, but not enough for a real defense balance pass.
- `DefenseGrid` sits at a sensible mid-tier cost relative to `Shipyard` and `ResearchLab`.
- Because no active mitigation, interception, bombardment, or invasion systems exist yet, current defense cost can only be reviewed as a construction/readiness price, not as combat value.

## Starting-Data Assumptions

Current Development seeds deliberately shape visible comparisons:

- `minimal-validation` preserves a tight stockpile and an empty queue baseline.
- `cockpit-validation` raises stockpiles and adds one visible `DefenseGrid`, one `Barracks`, one completed `EnergySystems` research result, and one completed `ScoutCraft` production result.
- `shipyard-validation`, `research-validation`, `fleet-validation`, and `planet-full-validation` each bias the same `Aurelia` scenario for a targeted cockpit.

Review:

- These profiles are good QA fixtures because they produce deterministic available-versus-blocked comparisons.
- They are not a safe source of final starting balance for live progression because each profile is optimized for route coverage, not for holistic empire pacing.
- Any future final balance pass must separate `Development QA shape` from `production starting state`.

## Honest Placeholders And Unknowns

The following should not be silently treated as final balance:

- construction upgrade costs scaling directly by target level
- construction duration scaling documented in read-model behavior rather than catalog metadata
- research duration and cost scaling without a final prerequisite graph
- single-value `3 min/unit` ship timing across all hulls
- current crew-capacity thresholds without a final population and maintenance economy
- `DefenseGrid` as the only defense type
- `Energy`, `Deuterium`, and `Population` as visible resource concepts without final persisted economy roles
- higher-cost research such as `ShipWeapons`, `Shielding`, and `Espionage` while their downstream systems remain non-final

## Recommended Safe Follow-Ups

1. Freeze current numbers as `Development baseline only` unless a later task explicitly promotes them into final seeded metadata.
2. Add a dedicated final catalog table or document pass for prerequisite graphs, max-level policies, and duration curves before calling research or ship balance final.
3. Review population, crew capacity, and orbital production together rather than balancing ship costs in isolation.
4. Revisit defense pricing only after there is more than one accepted defense row or a real combat-value model.
5. Keep QA seed profiles additive and deterministic, but do not let them become the implicit live-start economy.

## Validation

- This review used the current catalog readiness docs, Development seed profile docs, and the final DB prep note.
- No balance mutation, migration, runtime behavior change, browser QA, or SQL Server action was performed.

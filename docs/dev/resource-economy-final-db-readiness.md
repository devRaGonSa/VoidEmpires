# Resource Economy Final DB Readiness

This note prepares the current resource economy for a later final database/model consolidation pass. It documents accepted resource values, production behavior, spend boundaries, frontend terminology, and final DB dependencies without changing balance.

It does not add migrations, seed rows, generated images, combat, fleet movement, market transactions, alliance mutations, or production-auth behavior.

## Current Authority

- `ResourceType` defines the persisted spendable resources: `Credits`, `Metal`, `Crystal`, and `Gas`.
- `PlanetResourceStockpile` stores planet-local balances for those four resources.
- `PlanetProductionProfile` stores planet-local hourly production rates for those four resources.
- `ResourceProductionService` applies elapsed-time production to a matching planet stockpile.
- `PlanetEconomyTickService` applies production only for an owned planet and multiplies rates by accepted `ResourceExtraction` research level.
- `ResourceSpendService`, construction, research, and asset production services deduct resources from backend-owned stockpiles.
- Frontend resource helpers reserve Spanish labels for `Energia`, `Deuterio`, and `Poblacion`, but those are not current persisted stockpile resources.

## Current Resource Rows

| Key | Spanish label | Persisted stockpile | Production profile | Spendable today | Placeholder key | Current role |
|---|---|---|---|---|---|---|
| `Credits` | Creditos | Yes | Yes | Yes | `resource.credits` | General economy balance used by cost DTOs and stockpile display. |
| `Metal` | Metal | Yes | Yes | Yes | `resource.metal` | Primary construction and production material. |
| `Crystal` | Cristal | Yes | Yes | Yes | `resource.crystal` | Research and advanced construction material. |
| `Gas` | Gas | Yes | Yes | Yes | `resource.gas` | Higher-tier construction, research, and orbital production material. |
| `Energy` | Energia | No | No | No | `resource.energy` | Frontend terminology and research theme only; not a stockpile currency today. |
| `Deuterium` | Deuterio | No | No | No | `resource.deuterium` | Frontend/future market terminology only; no persisted economy behavior today. |
| `Population` | Poblacion | Separate population profile | No | No | `resource.population` | Colony capacity context, not a stockpile currency. |

## Current Balance Values

The Development seed baseline currently ensures the owned planet has at least:

| Resource | Minimum visible stockpile |
|---|---:|
| Credits | 125 |
| Metal | 160 |
| Crystal | 100 |
| Gas | 50 |

The minimal production profile for the owned planet is:

| Resource | Rate per hour |
|---|---:|
| Credits | 18 |
| Metal | 14 |
| Crystal | 6 |
| Gas | 3 |

`cockpit-validation` raises QA stockpile minimums for repeated cockpit checks, but it remains Development-only setup and must not be treated as final live economy balance.

## Current Production And Spend Rules

- Production cannot apply to a negative elapsed duration.
- Production profile and stockpile must belong to the same planet.
- Production applies only through explicit backend service calls; page load must not silently mutate resources.
- Production is multiplied by `ResourceExtraction` research: `1 + level * 0.05`.
- Spending rejects negative costs, missing stockpiles, unknown resource types, and insufficient balances.
- Construction, research, and asset production deduct their visible costs immediately when enqueue succeeds.
- Market pages currently show advisory signals only; no buy, sell, auction, trade-route, or player-to-player transaction mutates resources.

## Final DB Metadata Required

Each final resource metadata row should include:

1. Stable key matching a backend resource key or explicit non-stockpile signal key.
2. Spanish display label and short Spanish description.
3. Resource class: stockpile currency, production signal, capacity signal, fuel, or future market commodity.
4. Whether it is persisted in `PlanetResourceStockpile`.
5. Whether it is produced by `PlanetProductionProfile`.
6. Whether it can be spent by cost DTOs.
7. Placeholder asset key from the table above.
8. Nullable final icon/asset id for the generated asset phase.
9. Decimal precision and rounding/display policy.
10. Minimum/maximum/capacity policy, if accepted.
11. Production source metadata, including buildings, research, or planet traits once accepted.
12. Market/trade eligibility only after transaction behavior is accepted.
13. Sort order and display grouping for cockpit resource strips.
14. Version or revision marker for future balance changes.

## Current Gaps Before Final DB

- There is no resource metadata table; labels and ordering are frontend helpers.
- `Energy`, `Deuterium`, and `Population` are visible terminology but not current stockpile resources.
- Storage caps, overflow, upkeep, fuel, and market pricing are not final.
- Production rates are seed/profile data, not catalog metadata.
- Final DB validation should prove every cost-bearing resource maps to a known backend key and that non-stockpile labels cannot be spent accidentally.

## Seed Phase Requirements

- Add final resource metadata only in a dedicated final DB/model consolidation task.
- Keep metadata separate from planet stockpile balances, production profiles, population profiles, orders, and market state.
- Preserve backend-owned balances and deductions as authoritative gameplay state.
- Do not use metadata to fabricate stockpile quantities, production, market prices, trade offers, or resource readiness.
- Keep production auth, market transactions, fleet movement fuel, combat costs, and alliance treasury behavior outside this readiness note.

## Validation

- Static guard for this documentation task: `powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1`.
- No browser, screenshot, DB migration, final asset generation, balance change, or integration validation was performed for this note.

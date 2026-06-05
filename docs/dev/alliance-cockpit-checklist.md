# Alliance Cockpit Checklist

Use this checklist for the safe `Alliance v1` read-only cockpit foundation and for later tasks that wire frontend copy, route loading, or development-only read aggregation on top of the existing diplomacy foundations.

## Safe v1 scope summary

Accepted safe scope for `Alliance v1`:

- own diplomatic identity read from the requesting civilization
- current alliance status as read-only metadata only
- known diplomatic contacts when they already exist in current persisted data
- pact, invitation, membership-management, and future diplomacy actions shown only as disabled placeholders
- no create, join, leave, accept, reject, invite, kick, role-change, trade, war, espionage, fleet, visibility, or combat mutation

Required status framing for the cockpit:

- `None`: the civilization has no current alliance membership rows
- `ReadOnly`: the civilization has at least one current or historical alliance readiness row that may be displayed as metadata
- `Future`: pact, invitation, role, treasury, permissions, and diplomacy execution surfaces that remain intentionally disabled

## Current backend reality

Current alliance-adjacent backend foundations already exist and are safe to read:

- Player identity lives in [src/VoidEmpires.Domain/Players/PlayerProfile.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Players/PlayerProfile.cs) and civilization identity lives in [src/VoidEmpires.Domain/Players/Civilization.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Players/Civilization.cs).
- Alliance metadata lives in [src/VoidEmpires.Domain/Diplomacy/Alliance.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Diplomacy/Alliance.cs).
- Membership metadata lives in [src/VoidEmpires.Domain/Diplomacy/AllianceMembership.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Diplomacy/AllianceMembership.cs).
- Pact metadata lives in [src/VoidEmpires.Domain/Diplomacy/AlliancePact.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Diplomacy/AlliancePact.cs).
- Diplomatic contact metadata lives in [src/VoidEmpires.Domain/Diplomacy/DiplomaticContact.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Domain/Diplomacy/DiplomaticContact.cs).

Important current constraint:

- these models are metadata foundations only
- they do not carry alliance permissions, invitations, treasury, messaging, shared sensors, shared visibility, shared fleets, diplomacy execution, or authoritative gameplay rules

## Component discovery

Most relevant components for this audit:

- Entrypoint and dev gating: [src/VoidEmpires.Web/Program.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/Program.cs)
- Dev-only alliance/contact endpoints: [src/VoidEmpires.Web/DevAllianceReadinessEndpoints.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/DevAllianceReadinessEndpoints.cs), [src/VoidEmpires.Web/DevAlliancePactReadinessEndpoints.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/DevAlliancePactReadinessEndpoints.cs), [src/VoidEmpires.Web/DevDiplomaticContactEndpoints.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Web/DevDiplomaticContactEndpoints.cs)
- Query contracts: [src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Application/StrategicMap/GetAllianceReadinessResult.cs), [src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Application/StrategicMap/GetAlliancePactReadinessResult.cs), [src/VoidEmpires.Application/StrategicMap/GetDiplomaticContactsResult.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Application/StrategicMap/GetDiplomaticContactsResult.cs)
- Query implementations: [src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/AllianceReadinessQueryService.cs), [src/VoidEmpires.Infrastructure/StrategicMap/AlliancePactReadinessQueryService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/AlliancePactReadinessQueryService.cs), [src/VoidEmpires.Infrastructure/StrategicMap/DiplomaticContactQueryService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/DiplomaticContactQueryService.cs)
- Strategic-map reuse boundary: [src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs)
- DI wiring: [src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/VoidEmpiresPersistenceServiceCollectionExtensions.cs)
- Seed baseline owner: [src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs)
- Relevant tests: [tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs), [tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs), [tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs), [tests/VoidEmpires.Tests/AllianceReadinessSmokeTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AllianceReadinessSmokeTests.cs)

## Dependency map

Current read chain:

- `Program.cs` -> `MapDevAllianceReadinessEndpoints` / `MapDevAlliancePactReadinessEndpoints` / `MapDevDiplomaticContactEndpoints`
- dev endpoints -> `IAllianceReadinessQueryService` / `IAlliancePactReadinessQueryService` / `IDiplomaticContactQueryService`
- DI registration -> `VoidEmpiresPersistenceServiceCollectionExtensions`
- query services -> `VoidEmpiresDbContext`
- strategic-map reuse -> `StrategicMapService` reads alliance readiness, pact readiness, and diplomatic contacts as metadata-only notes

This means a safe Alliance cockpit should extend existing read services rather than invent a new gameplay subsystem.

## What can be reused safely

### Own diplomatic identity

Safe source of truth:

- `Civilization.Id`
- `Civilization.Name`
- `Civilization.Archetype`
- `Civilization.Status`
- `Civilization.HomePlanetId`

Assessment:

- `Civilization` is the correct primary identity for `civilizationId`-scoped Alliance reads.
- `PlayerProfile` can support diagnostics or ownership lineage, but it should not be treated as the primary Alliance cockpit identity model because the cockpit and existing dev read surfaces are civilization-scoped, not user-scoped.

Recommended frontend copy source:

- use civilization-facing labels such as faction name, archetype, and homeworld reference
- keep raw `playerProfileId` out of primary UI unless it is explicitly diagnostic

### Alliance membership read-state

Safe source of truth:

- `GET /api/dev/strategic-map/alliances/readiness?civilizationId={id}`

Returned metadata:

- alliance id, name, tag, status, created date
- requesting-civilization membership id, membership status, role, joined date

Assessment:

- this is safe for `current alliance status` as read-only metadata
- the service is deterministic, civilization-scoped, and tested as read-only
- it includes historical membership statuses, so frontend copy must not equate every row with an active alliance

UI guidance:

- treat `AllianceMembershipStatus.Active` as the only current-membership signal
- treat departed or archived rows as history or diagnostics
- if no active membership exists, display `Sin alianza activa` or equivalent `None` state

### Diplomatic contacts

Safe source of truth:

- `GET /api/dev/strategic-map/diplomatic-contacts?civilizationId={id}`

Returned metadata:

- contacted civilization id
- contact status
- discovered date
- source string

Assessment:

- this is safe for a `known contacts` catalog
- contact rows are requester-scoped and read-only
- contact rows do not imply alliance eligibility, pact eligibility, visibility sharing, or friendly permissions

UI guidance:

- show them as contact or awareness rows only
- keep raw `source` strings secondary because current values are technical or dev-authored

### Pact readiness

Safe source of truth:

- `GET /api/dev/strategic-map/alliances/pacts/readiness?civilizationId={id}`

Returned metadata:

- pact id
- source and target alliance summaries
- pact type
- pact status
- created date

Assessment:

- safe for a `future diplomacy` or `pacts overview` panel
- not safe to interpret as executable treaties or permission changes
- only alliances with active requester membership are included, which is conservative and appropriate for v1

UI guidance:

- present pact rows as passive metadata
- if shown in v1, default to disabled future-facing framing rather than command surfaces

## What must stay out of Alliance v1

- alliance creation
- alliance join or leave
- invitations
- application review
- role management
- treasury or contributions
- member list management beyond current-requester metadata
- alliance chat or messaging
- allied planet, fleet, transfer, sensor, detection, or interception exposure
- shared visibility
- war, peace, sanctions, embargoes, or espionage agreements
- trade execution
- any mutation endpoint or any write flow

The current strategic-map notes already reinforce this boundary in [src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/StrategicMap/StrategicMapService.cs).

## Seed and backend gap assessment

Current seed reality:

- [src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Infrastructure/Development/DevelopmentSeedService.cs) does not currently seed alliance, pact, or diplomatic-contact rows for the standard cockpit profiles.

What already exists:

- minimal persisted domain models
- read-only query services
- dev-only read endpoints
- endpoint and query tests
- strategic-map metadata integration

Current gap:

- there is no Alliance-specific seeded QA baseline yet
- there is no single cockpit-shaped Alliance aggregate endpoint yet

Recommendation for later tasks:

- prefer first reusing the existing three development-only read endpoints from the frontend
- if the frontend needs one consolidated payload for loading simplicity, add a tiny development-only Alliance read-model endpoint that composes the existing query services without introducing new persistence or mutation behavior
- add deterministic seed data only when the frontend route needs a canonical repeatable QA story

## Validation and test evidence

Current evidence for the safe read-only boundary:

- [tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AllianceReadinessQueryServiceTests.cs) verifies requester scoping, deterministic ordering, invalid-id handling, and no mutation
- [tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AlliancePactReadinessQueryServiceTests.cs) verifies active-membership scoping, deterministic ordering, invalid-id handling, and no mutation
- [tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/DiplomaticContactQueryServiceTests.cs) verifies requester scoping, deterministic ordering, invalid-id handling, and no mutation
- [tests/VoidEmpires.Tests/DevAllianceReadinessEndpointTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/DevAllianceReadinessEndpointTests.cs), [tests/VoidEmpires.Tests/DevAlliancePactReadinessEndpointTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/DevAlliancePactReadinessEndpointTests.cs), and [tests/VoidEmpires.Tests/DevDiplomaticContactEndpointTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/DevDiplomaticContactEndpointTests.cs) verify development gating and request validation
- [tests/VoidEmpires.Tests/AllianceReadinessSmokeTests.cs](/D:/Proyectos/VoidEmpires/tests/VoidEmpires.Tests/AllianceReadinessSmokeTests.cs) verifies alliance metadata does not leak allied systems or change visibility

Integration note:

- No integration tests configured.

## Recommended Alliance v1 frontend contract posture

Use these read surfaces only:

- `GET /api/dev/strategic-map/alliances/readiness`
- `GET /api/dev/strategic-map/diplomatic-contacts`
- optionally `GET /api/dev/strategic-map/alliances/pacts/readiness`

Centralized frontend taxonomy owner:

- [src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/alliancePresentation.ts) should remain the single owner for Spanish-first alliance, contact, pact, and disabled-future labels so raw enum or DTO tokens stay out of primary UI.
- [src/VoidEmpires.Frontend/src/api/allianceTypes.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/api/allianceTypes.ts), [src/VoidEmpires.Frontend/src/api/allianceApi.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/api/allianceApi.ts), and [src/VoidEmpires.Frontend/src/utils/allianceViewModel.ts](/D:/Proyectos/VoidEmpires/src/VoidEmpires.Frontend/src/utils/allianceViewModel.ts) now define the typed DTO, fetch, and normalization path for the Alliance cockpit frontend.

Keep the first frontend slice conservative:

- summary card for requesting civilization identity
- current alliance status card
- known diplomatic contacts list
- disabled future actions for invitations, pacts, treasury, permissions, and messaging
- one collapsed diagnostics section for raw statuses, ids, and technical notes

## Non-goals

- No production Alliance API.
- No alliance gameplay authority.
- No visibility or permission expansion from alliance metadata.
- No frontend assumption that all readiness rows are active memberships.
- No seeded Alliance QA profile in this task.

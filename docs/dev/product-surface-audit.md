# Product Surface Audit

This audit covers the normal frontend routes and shared UI surface for Block 41 product-facing cleanup. It is documentation-only: no runtime behavior, gameplay, backend, SQL Server, auth, market, alliance, fleet movement, combat, or asset behavior is changed here.

## Scope Reviewed

- App shell and route map: `src/VoidEmpires.Frontend/src/App.tsx`
- Normal UI routes: `/`, `/galaxy`, `/onboarding`, `/planet`, `/construction`, `/research`, `/shipyard`, `/fleets`, `/defenses`, `/ground-army`, `/market`, `/espionage`, `/alliance`, `/ranking`
- Shared components: shell, sidebar, context strips, loading/error panels, hero, confirmation modal, diagnostics panel, Development tools panel, map cards, fleet panels, placeholder assets, resource bar
- Presentation and routing helpers under `src/VoidEmpires.Frontend/src/utils/`
- API boundary files under `src/VoidEmpires.Frontend/src/api/` for user-visible endpoint/profile leakage
- Existing copy guard: `scripts/check-frontend-copy-regressions.ps1`

## Global Findings

| Surface | Visible issue | Current examples | Target remediation |
|---|---|---|---|
| App shell status and header | Development environment is first-viewport product copy. | `Modo Development`, `Navegacion local sin login de produccion`, `Bucle jugable Development`, `mutaciones Development confirmadas`. | Rename for normal users or hide behind operator mode. Keep implementation scope in docs/diagnostics only. |
| Endpoint notice | Backend URL/profile are visible in the shell. | `Endpoints Development`, `URL base del backend`, `Perfil esperado del backend`, `VoidEmpires.Web http profile`, default `http://localhost:5142`. | Remove from normal shell; collapse or gate behind operator mode. |
| Input placeholders | Raw GUID-shaped IDs are visible in route forms. | `00000000-0000-0000-0000-000000000000`, `40000000-0000-0000-0000-000000000000`. | Replace with product labels, seeded suggestions, or local-session recovery copy. If raw IDs remain, keep them in diagnostics/operator controls. |
| Technical diagnostics | Diagnostics panels expose raw details and JSON on normal routes. | `Diagnostico secundario`, `Soporte tecnico`, `Payload crudo`, `pre className="json-preview"`, raw payload props. | Keep collapsed by default and make operator-only where possible. Use player-facing summaries in primary route content. |
| Backend/provider language | Normal route copy references backend reads, backend acceptance, route validation, database, seed, and Development persistence. | `backend no devolvio`, `lectura backend`, `base de datos de Development`, `seed`, `dev endpoints`. | Replace in primary UI with state/result language: `estado confirmado`, `lectura disponible`, `contexto local`. Preserve exact technical language only in diagnostics/docs. |
| QA/tooling language | QA and operator wording is visible on player routes. | `Development QA`, `QA local`, `validacion del backend`, `herramientas Development`, `Soporte operador`. | Gate behind operator mode or move below collapsed technical areas. |
| Raw identifiers | Compact GUIDs and labels such as tactical/transfer IDs appear in cards. | `ID tactico`, `ID de traslado`, compact civilization/planet/system IDs. | Hide from primary cards; show names, locations, and status first. Keep IDs only in diagnostics. |
| Placeholder/future copy | Some placeholders are product-safe, but several labels still reveal implementation scaffolding. | `Cabina preparada`, `Perfil derivado provisional`, `Operacion futura`, future placeholders. | Rename to explicit product states: `Pendiente de activar`, `No disponible en esta version`, `Lectura futura`. Avoid the word `placeholder` in normal UI. |
| English fallback copy | One map empty state and aria label remain English. | `No relevant systems were returned for this civilization.`, `Read-only two-dimensional strategic map`. | Translate to Spanish-first copy. |
| Mutation terminology | Technical mutation vocabulary remains visible in several places. | `mutacion`, `mutaciones`, `contratos de mutacion`. | Use `orden`, `accion protegida`, `confirmacion`, or `cambio confirmado`; reserve `mutacion` for docs/operator diagnostics. |

## Route Coverage

| Route | Current product role | Offending visible categories | Target remediation |
|---|---|---|---|
| `/onboarding` | Local playable start. | Development/local-session framing, possible backend failure detail, local memory explanation. | Keep as entry flow but rename to product start language; state that it creates a local play context without mentioning production login. |
| `/` and `/galaxy` | Strategic map and system/planet read surface. | QA/local development note, raw GUID input placeholder, backend projection/error copy, technical diagnostics, raw system/planet payload disclosures, visual seed, English map empty state. | Make map-first product copy primary; collapse/gate diagnostics and payloads; translate English fallback; remove raw IDs from normal controls. |
| `/planet` | Main colony hub and queue/resource context. | Development QA materialization controls, backend refresh/materialization copy, diagnostics/raw payloads, technical IDs, global complete-due wording. | Hide materialization controls from product UI or gate behind operator mode; keep colony state and handoffs in player terms. |
| `/construction` | Focused construction route using Planet behavior. | Inherits Planet implementation and risks: Development/backend/materialization wording, raw ID inputs, diagnostics. | Keep construction action confirmation but remove implementation framing from route copy. |
| `/research` | Research catalog and queue. | `Mutaciones Development confirmadas`, backend acceptance/read copy, raw ID placeholders, diagnostics, `base de datos de Development` in modal. | Rename to protected research order flow; modal should say it starts a real research order in the current local world, not Development DB. |
| `/shipyard` | Orbital production catalog and queue. | Development mutation note, backend stock/queue copy, raw ID placeholders, QA materialization guidance, diagnostics/raw payload, English backend fallback. | Use orbital production language; move due-materialization and technical refresh details behind operator diagnostics. |
| `/fleets` | Fleet inspection and transfer readiness. | Development transfer panels, tactical/transfer IDs, raw planet IDs, protected mutation controls, diagnostics. | Keep movement readiness visible; gate Development transfer create/cancel/complete-due controls and IDs behind operator mode. |
| `/defenses` | Defense readiness. | Raw ID placeholders, backend/global construction close limitation, diagnostics. | Keep readiness and limitation copy; remove backend/global-route implementation detail from primary panels. |
| `/ground-army` | Ground readiness. | Raw ID controls, readiness/future limitation copy, diagnostics. | Keep preparation wording; collapse technical details and avoid future-placeholder language. |
| `/market` | Economy read surface. | Possible raw ID inputs, backend/current-read language, diagnostics, deferred transaction limitations. | Use `mercado no ejecuta operaciones en esta version`; keep economy signals product-facing. |
| `/espionage` | Intelligence read surface. | Raw `civilizationId/systemId/planetId` labels, technical error preview, Development helper link, diagnostics. | Rename controls to context selectors; hide exact IDs and error payloads in diagnostics. |
| `/alliance` | Diplomacy read surface. | Future placeholder/action copy, raw ID placeholder, Development helper link, diagnostics, final authorization/dependency wording. | Use diplomacy availability language; avoid `placeholder`; keep deferred mutation boundaries as product scope notes. |
| `/ranking` | Power-index read surface. | Development helper link, raw ID placeholder, validation scenario/comparison wording, no-public-ranking caveats. | Rename validation references to local comparison/reference; keep no-rewards/no-public-leaderboard boundary. |

## Shared Component Notes

- `DevEndpointNotice` should not render in normal product mode.
- `DevelopmentToolsPanel` should be closed, secondary, and operator-gated when product mode exists.
- `DevDiagnosticsPanel` already formats GUID-like values, but the panel still exposes raw JSON; keep it outside primary flow.
- `ActionManifestPanel` is contract metadata and should be operator-only, not normal UI.
- `FleetSummaryPanel` and `FleetSelectedGroupPanel` surface tactical, transfer, and planet IDs; those should be diagnostics-only.
- `PlayableSessionBanner` should keep the boundary that local memory is not authentication, but avoid `ids Development` in normal copy.
- `PlaceholderAsset` is acceptable as an implementation component, but visible labels must not say placeholder/mock/sample.

## Spanish-First Product Terminology Rules

- Primary UI uses Spanish-first player language: `civilizacion`, `planeta`, `colonia`, `galaxia`, `orden`, `cola`, `reservas`, `produccion`, `investigacion`, `preparacion`, `lectura`, `contexto`.
- Avoid implementation words in primary UI: `Development`, `QA`, `backend`, `endpoint`, `payload`, `raw`, `provider`, `seed`, `localhost`, `database`, `base de datos`, `DTO`, `mock`, `dummy`, `placeholder`, `prototype`, `prototipo`, `validacion del backend`, `mutacion`.
- Allowed secondary wording: `Diagnostico secundario`, `Detalle tecnico`, `Soporte operador`, `Fuera del flujo principal`. These should be collapsed or gated.
- Use `solo lectura`, `no disponible en esta version`, and `fuera del alcance de esta cabina` for deferred features.
- Use `confirmacion requerida`, `orden protegida`, and `estado confirmado` for guarded actions; avoid implying automatic completion.
- Do not show raw IDs, URLs, endpoint paths, or JSON payloads in first-viewport or primary action cards.
- Route copy must not overclaim production login, production auth, final database readiness, public rankings, market transactions, alliance mutations, combat, invasion, or fleet movement productization.

## Follow-Up Priorities

1. Remove or gate shell-level Development/backend notices before route-specific copy changes.
2. Replace raw ID placeholders and local-session wording with product context language.
3. Hide materialization, diagnostics, manifests, and transfer controls behind operator mode.
4. Translate remaining English fallback copy.
5. Normalize route hero/card language to the shared terminology rules above.

# No Visible Development Report

Date: 2026-07-07

This report records the automated product-surface status for normal VoidEmpires UI. It distinguishes internal technical references that remain in source/docs from copy allowed in the default product experience.

This is not a browser or manual visual QA report. No screenshot inspection, production login, SQL Server acceptance, or gameplay integration acceptance is claimed here.

## Automated Guard Status

Command run:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File .\scripts\check-frontend-copy-regressions.ps1
```

Result: passed.

The guard blocks visible product-surface use of development/test/prototype/local endpoint language such as `Development`, `Dev`, `QA`, `Test`, `Prueba`, `Prototipo`, `Solo desarrollo`, `cockpit-validation`, `minimal-validation`, `endpoint`, and `localhost` unless the line is explicitly allowlisted as internal or operator-only.

## Normal UI Status

According to the automated guard, normal product UI no longer exposes Development wording, endpoint URLs, localhost details, backend profile details, raw endpoint details, or Development-only mutation copy outside the accepted internal boundaries.

The default product-mode contract remains:

- Product mode is the default for normal route visits.
- Operator/internal mode must be explicitly revealed through the documented local mechanisms.
- Operator mode is a visibility layer only; it is not authorization and does not weaken confirmations.

## Allowed Internal References

The remaining Development references are allowed only in these categories:

- Documentation under `docs/dev/`, including audits, readiness reports, operator-mode notes, and product-mode visibility contracts.
- Automation and validation scripts under `scripts/`, especially copy guards, QA/dev script guards, route lazy-load guards, and local validation helpers.
- Backend/API or health-oriented implementation identifiers and routes that are not normal UI copy, including dev API route names, health/check documentation, and local operation notes.
- Frontend configuration values such as the local default API base URL in `src/VoidEmpires.Frontend/src/config.ts`; this is implementation configuration, not rendered normal UI copy.
- Operator-only frontend components: `ActionManifestPanel`, `DevDiagnosticsPanel`, `DevEndpointNotice`, and `DevelopmentToolsPanel`.
- Operator-gated route code guarded by `isOperatorMode`, `operatorMode`, `?operator=1`, or the local `voidempires.operatorMode` flag.
- Code identifiers, type names, imports, CSS classes, ARIA/data attributes, and action-scope enum values such as `developmentOnly`, `developmentNote`, `dev-meta`, `dev-diagnostics`, and `actionScope="development"`.
- Explicitly allowlisted operator-only visible fragments for local materialization and confirmation tools.
- Legacy `CockpitHero` `developmentNote` fragments that the guard treats as non-rendered where applicable.

## Boundaries

This report does not approve new normal-UI Development copy. Future Development, QA, endpoint, localhost, backend profile, raw payload, or prototype wording must stay in docs, scripts, implementation identifiers, or explicitly gated operator surfaces. If a new exception is required, it should be added narrowly to `scripts/check-frontend-copy-regressions.ps1` with a matching reason.

This report also does not claim final production authentication, public ranking, market transactions, alliance mutations, combat, invasion, interception, SQL Server deployment, or final fleet movement productization.

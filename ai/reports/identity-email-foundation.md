# Identity And Brevo Email Foundation

## Objective

Define the initial boundaries for VoidEmpires user identity and transactional email before implementation code is added.

This document is a planning guide for Phase 2A tasks. It does not introduce application code, migrations, `DbContext` changes, gameplay systems, or provider secrets.

## User And Account Direction

VoidEmpires should treat player identity as a platform concern that can later be related to game-owned empires and administrative roles without coupling gameplay rules to authentication details.

Initial account direction:

- Use ASP.NET Core Identity for user persistence and token workflows unless a later task replaces that decision.
- Keep the user model small at first: stable user identifier, email, normalized email, password credential support, confirmed email state, and standard Identity security metadata.
- Require unique email addresses.
- Use confirmed email as the baseline for account activation-sensitive flows.
- Keep gameplay concepts such as empires, sectors, fleets, resources, and progression outside the identity persistence model.
- Expose application-layer contracts for registration and email confirmation instead of letting web endpoints depend directly on infrastructure details.

## Transactional Email Boundary

Transactional email should be represented by a provider-agnostic application contract. Identity and registration services should request messages through that contract, while Infrastructure owns provider-specific delivery.

The boundary should support:

- email confirmation messages for newly created users
- future password reset and security notification messages
- deterministic test fakes that never contact external services
- clear result reporting for accepted, skipped, invalid, or failed sends

Application-layer contracts must not expose Brevo SDK types, HTTP payloads, API keys, SMTP settings, or provider-specific error models.

## Brevo Provider Direction

Brevo is the selected transactional email provider for user creation and email confirmation flows.

Infrastructure should own the Brevo implementation and DI wiring. Prefer a small, testable adapter around the Brevo API or SMTP path selected by the implementation task. If the HTTP API is used, prefer `HttpClient` registration through existing .NET conventions so tests can replace the handler.

Before coding any Brevo email integration, implementation tasks must inspect `devRaGonSa/CreateCvApp` for existing Brevo or email patterns, configuration conventions, interfaces, service naming, options classes, templates, and testing approaches. Reuse compatible conventions where they fit VoidEmpires, but do not copy secrets, private endpoints, project-specific content, or unrelated application behavior.

The provider should be safe when disabled or incomplete in local and CI environments. Startup should not require a real Brevo key.

## Email Flows

Initial registration and confirmation flow:

1. Web endpoint receives a registration request.
2. Application service validates the request shape and delegates account creation through identity-facing abstractions.
3. Infrastructure creates the Identity user with email as the deterministic username or an equivalent convention.
4. Identity generates an email confirmation token.
5. Application service builds a confirmation message request without provider-specific types.
6. Infrastructure sends the message through the transactional email sender.
7. Confirmation endpoint accepts the user identifier and token, then delegates confirmation to the application service.

The first implementation should keep templates plain and minimal. Future tasks can add richer templates after account creation and confirmation are working.

## Configuration Direction

Configuration must be safe-by-default and secret-free in source control.

Expected conventions:

- Store only placeholder or disabled Brevo settings in committed `appsettings` files.
- Supply real API keys, SMTP secrets, sender identities, and environment-specific URLs through environment variables, user secrets, deployment secrets, or private infrastructure configuration.
- Prefer a dedicated options class such as `BrevoEmailOptions` in Infrastructure.
- Include an explicit enabled/disabled setting so CI and local validation can run without external email access.
- Never commit real Brevo API keys, SMTP credentials, private hostnames, recipient lists, or production sender data.

## Testing Strategy

Tests should prove the boundaries without sending real email.

Recommended coverage:

- Application contract tests for registration and confirmation behavior using fakes.
- Infrastructure tests for Brevo request construction using a fake HTTP handler or equivalent test double.
- Configuration tests proving missing or disabled Brevo settings do not break startup.
- Web endpoint smoke tests that exercise registration and confirmation through fakes or test services.

CI must not call Brevo or require the real PostgreSQL database. If integration tests are later added for identity persistence, they should use repository-approved test infrastructure and remain separate from ordinary validation.

## Non-Goals

- No gameplay user profile model.
- No migrations in this design task.
- No direct Brevo calls from Application or Web.
- No production email templates.
- No committed secrets or private provider data.

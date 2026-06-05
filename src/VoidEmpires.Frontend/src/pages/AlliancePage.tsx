import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchAllianceUiState } from "../api/allianceApi";
import { CockpitHero } from "../components/CockpitHero";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { getAllianceReadOnlyStatement, getAllianceStaticLabels } from "../utils/alliancePresentation";
import {
  getAlliancePrimaryAction,
  groupAllianceContacts,
  mapAllianceUiStateToViewModel,
  selectRecommendedDiplomacyFocus,
  type AllianceUiState,
} from "../utils/allianceViewModel";
import {
  buildDevelopmentHelperUrl,
  buildEspionageUrl,
  buildGalaxyUrl,
  buildMarketUrl,
} from "../utils/routeUrls";

interface AllianceErrorPresentation {
  primaryMessage: string;
  followUp: string | null;
  technicalDetail: string | null;
}

const allianceLabels = getAllianceStaticLabels();

function formatAllianceRequestFailure(message: string | null | undefined): AllianceErrorPresentation {
  const detail = message?.trim() ?? null;

  switch (detail) {
    case "Civilization id is required.":
      return {
        primaryMessage: "La cabina de alianzas necesita un id de civilizacion valido.",
        followUp: "Introduce un contexto valido antes de abrir Alianzas.",
        technicalDetail: detail,
      };
    case "Civilization was not found.":
      return {
        primaryMessage: "No se encontro la civilizacion solicitada para esta lectura diplomatica.",
        followUp: "Revisa el contexto actual o vuelve a entrar desde Galaxia, Mercado o Espionaje.",
        technicalDetail: detail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "La ruta de Alianzas no esta disponible fuera del entorno de desarrollo.",
        followUp: "Verifica que el backend local siga exponiendo la cabina de lectura.",
        technicalDetail: detail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "La persistencia de desarrollo no esta disponible ahora mismo.",
        followUp: "Comprueba la configuracion local antes de reintentar.",
        technicalDetail: detail,
      };
    default:
      return {
        primaryMessage: "La lectura diplomatica no pudo cargarse con el contexto actual.",
        followUp: "La cabina mantiene una lectura segura y no inventa estados cuando el backend no responde.",
        technicalDetail: detail,
      };
  }
}

export function AlliancePage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<AllianceUiState | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const groupedContacts = useMemo(() => groupAllianceContacts(uiState?.contacts ?? []), [uiState?.contacts]);
  const recommendedFocus = useMemo(
    () => (uiState ? selectRecommendedDiplomacyFocus(uiState) : allianceLabels.noActiveAlliance),
    [uiState],
  );
  const primaryAction = useMemo(
    () => (uiState ? getAlliancePrimaryAction(uiState) : allianceLabels.futureAlliance),
    [uiState],
  );
  const pactFutureCount = uiState?.futurePacts.length ?? 0;
  const contactKnownCount = uiState?.contacts.length ?? 0;
  const readOnlyStatement = getAllianceReadOnlyStatement();

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setErrorFollowUp(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setErrorFollowUp(null);
      setTechnicalErrorDetail(null);

      try {
        const response = await fetchAllianceUiState(queryCivilizationId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatAllianceRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        setUiState(mapAllianceUiStateToViewModel(response.uiState));
      } catch (requestError) {
        const failure = formatAllianceRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationIdInput.trim();
    if (!trimmedCivilizationId) {
      const failure = formatAllianceRequestFailure("Civilization id is required.");
      setUiState(null);
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Alianzas v1"
        title="Alianzas"
        description="Cabina diplomatica de solo lectura para identidad, estado actual, contactos conocidos y pactos futuros todavia bloqueados."
        developmentNote={`${readOnlyStatement} Esta superficie no envia invitaciones y no comparte visibilidad. Solo organiza la lectura diplomatica disponible.`}
        badges={(
          <>
            <UiBadge tone="resource">{uiState?.status?.stateLabel ?? "Estado diplomatico"}</UiBadge>
            <UiBadge>{uiState?.identity?.civilizationName ?? "Civilizacion propia"}</UiBadge>
            <UiBadge tone="warn">{recommendedFocus}</UiBadge>
          </>
        )}
      />

      <UiCard className="panel alliance-summary-panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Resumen diplomatico</p>
            <h3>Lectura rapida de alianzas</h3>
            <p>{readOnlyStatement}</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
        </div>
        <div className="alliance-summary-grid">
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Estado diplomatico</p>
            <strong>{uiState?.status?.stateLabel ?? allianceLabels.noActiveAlliance}</strong>
            <span>{uiState?.status?.headline ?? allianceLabels.unclassifiedRead}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Civilizacion propia</p>
            <strong>{uiState?.identity?.civilizationName ?? "Sin contexto cargado"}</strong>
            <span>{uiState?.identity?.archetypeLabel ?? "Lectura diplomatica pendiente de clasificar"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Sin alianza activa</p>
            <strong>{uiState?.status?.hasActiveAlliance ? "No" : "Si"}</strong>
            <span>{uiState?.status?.supportText ?? allianceLabels.futureAlliance}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Contactos conocidos</p>
            <strong>{contactKnownCount}</strong>
            <span>{contactKnownCount > 0 ? allianceLabels.knownContact : "Sin contactos conocidos"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Pactos futuros</p>
            <strong>{pactFutureCount}</strong>
            <span>{pactFutureCount > 0 ? allianceLabels.futureAlliance : "Sin pactos futuros visibles"}</span>
          </section>
          <section className="subpanel figma-subpanel alliance-summary-card">
            <p className="eyebrow">Foco recomendado</p>
            <strong>{recommendedFocus}</strong>
            <span>{primaryAction}</span>
          </section>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto diplomatico</p>
              <h3>Cargar lectura de alianzas</h3>
            </div>
            <UiBadge>{cockpitStatusLabels.developmentOnly}</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input
                type="text"
                value={civilizationIdInput}
                onChange={(event) => setCivilizationIdInput(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir alianzas"}
            </button>
          </form>
          {error ? (
            <div className="subpanel figma-subpanel figma-mini-card-warn">
              <p className="error-text">{error}</p>
              {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
            </div>
          ) : null}
          {!queryCivilizationId ? (
            <p className="figma-panel-note">
              Introduce un `civilizationId` valido para convertir esta ruta en una cabina diplomatica real.
            </p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Estado visible</p>
              <h3>Resumen diplomatico</h3>
            </div>
            <UiBadge>{uiState?.status?.stateLabel ?? allianceLabels.noActiveAlliance}</UiBadge>
          </div>
          {uiState?.identity && uiState?.status ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState.identity.civilizationName}</strong></div>
              <div className="figma-data-row"><span>Foco recomendado</span><strong>{recommendedFocus}</strong></div>
              <div className="figma-data-row"><span>Estado</span><strong>{uiState.status.headline}</strong></div>
              <div className="figma-data-row"><span>Postura</span><strong>{uiState.status.supportText}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">
              Cuando exista un contexto valido, la cabina mostrara identidad, estado diplomatico y el siguiente foco de lectura.
            </p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite de la cabina</p>
              <h3>Diplomacia solo lectura</h3>
            </div>
            <UiBadge tone="warn">Sin ejecucion</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Lee identidad, membresia, contactos conocidos y pactos futuros.</li>
            <li>No crea ni modifica alianzas, invitaciones o solicitudes.</li>
            <li>No comparte visibilidad, sensores, flotas, comercio ni permisos.</li>
            <li>{readOnlyStatement}</li>
          </ul>
        </UiCard>
      </div>

      {!queryCivilizationId && !isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Sin contexto</p>
              <h3>Alianzas necesita una civilizacion antes de mostrar una lectura util.</h3>
            </div>
            <UiBadge tone="warn">Contexto requerido</UiBadge>
          </div>
          <p className="figma-panel-note">
            Usa el formulario superior o entra desde Galaxia, Mercado o Espionaje para conservar `civilizationId`.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildDevelopmentHelperUrl()}>
              Abrir contexto de desarrollo
            </Link>
          </div>
        </UiCard>
      ) : null}

      {isLoading ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Cargando</p>
              <h3>Sincronizando lectura diplomatica</h3>
            </div>
            <UiBadge>Cargando...</UiBadge>
          </div>
          <p>Consultando identidad, estado de alianza, contactos conocidos y placeholders diplomáticos.</p>
        </UiCard>
      ) : null}

      {uiState?.identity && uiState?.status ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Identidad diplomatica</p>
                <h3>{uiState.identity.civilizationName}</h3>
                <p>La cabina toma como fuente principal a la civilizacion actual y deja el perfil del jugador como soporte tecnico.</p>
              </div>
              <UiBadge tone={uiState.status.hasActiveAlliance ? "good" : "warn"}>
                {uiState.status.stateLabel}
              </UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Arquetipo</span><strong>{uiState.identity.archetypeLabel}</strong></div>
                  <div className="figma-data-row"><span>Estado civilizatorio</span><strong>{uiState.identity.statusLabel}</strong></div>
                  <div className="figma-data-row"><span>Mundo base</span><strong>{uiState.identity.homePlanetLabel}</strong></div>
                  <div className="figma-data-row"><span>Perfil visible</span><strong>{uiState.identity.playerDisplayName}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Alianzas activas</span><strong>{uiState.status.activeAllianceCount}</strong></div>
                  <div className="figma-data-row"><span>Historial de alianzas</span><strong>{uiState.status.historicalAllianceCount}</strong></div>
                  <div className="figma-data-row"><span>Contactos conocidos</span><strong>{uiState.status.knownContactCount}</strong></div>
                  <div className="figma-data-row"><span>Pactos activos</span><strong>{uiState.status.activePactCount}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado de alianza</p>
                <h3>{uiState.status.headline}</h3>
                <p>Si existe una alianza activa, la cabina solo muestra metadata. Si no existe, prepara el terreno para placeholders futuros sin insinuar acciones ejecutables.</p>
              </div>
              <UiBadge tone={uiState.status.hasActiveAlliance ? "good" : "warn"}>{uiState.status.supportText}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Estado actual</span><strong>{uiState.status.stateLabel}</strong></div>
                  <div className="figma-data-row"><span>Accion principal</span><strong>{primaryAction}</strong></div>
                  <div className="figma-data-row"><span>Foco recomendado</span><strong>{recommendedFocus}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                {uiState.status.primaryAlliance ? (
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Nombre</span><strong>{uiState.status.primaryAlliance.name}</strong></div>
                    <div className="figma-data-row"><span>Sigla</span><strong>{uiState.status.primaryAlliance.tag}</strong></div>
                    <div className="figma-data-row"><span>Estado visible</span><strong>{uiState.status.primaryAlliance.statusLabel}</strong></div>
                    <div className="figma-data-row"><span>Creada</span><strong>{uiState.status.primaryAlliance.createdAtLabel}</strong></div>
                    {uiState.status.membership ? (
                      <>
                        <div className="figma-data-row"><span>Rol visible</span><strong>{uiState.status.membership.roleLabel}</strong></div>
                        <div className="figma-data-row"><span>Membresia</span><strong>{uiState.status.membership.statusLabel}</strong></div>
                      </>
                    ) : null}
                  </div>
                ) : (
                  <p className="figma-panel-note">
                    {allianceLabels.noActiveAlliance}. La cabina mantiene el estado honesto y deriva cualquier futura accion a placeholders desactivados.
                  </p>
                )}
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Contactos diplomaticos</p>
                <h3>Catalogo de contactos</h3>
                <p>Los contactos se agrupan por certeza de lectura para no mezclar una relacion conocida con un dato todavia sin confirmar.</p>
              </div>
              <UiBadge>{uiState.contacts.length} contactos</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Contactos conocidos</p>
                    <h4>{groupedContacts.confirmed.length} lecturas</h4>
                  </div>
                  <UiBadge tone="good">{allianceLabels.knownContact}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {groupedContacts.confirmed.length > 0 ? groupedContacts.confirmed.map((contact) => (
                    <li key={contact.contactedCivilizationId}>
                      <strong>{contact.contactLabel}</strong>: {contact.sourceLabel} | {contact.discoveredAtLabel}
                    </li>
                  )) : (
                    <li>Sin contactos conocidos en esta lectura.</li>
                  )}
                </ul>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Lecturas por confirmar</p>
                    <h4>{groupedContacts.unconfirmed.length} lecturas</h4>
                  </div>
                  <UiBadge tone="warn">{allianceLabels.unconfirmedContact}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {groupedContacts.unconfirmed.length > 0 ? groupedContacts.unconfirmed.map((contact) => (
                    <li key={contact.contactedCivilizationId}>
                      <strong>{contact.contactLabel}</strong>: {contact.confidenceLabel}
                    </li>
                  )) : (
                    <li>La lectura actual no contiene contactos sin confirmar.</li>
                  )}
                </ul>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Pactos y acciones futuras</p>
                <h3>Hoja de ruta diplomatica</h3>
                <p>Los pactos y acciones siguen visibles como referencia futura, pero todos los controles permanecen desactivados en esta fase.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.safePlaceholder}</UiBadge>
            </div>
            <div className="market-future-actions-grid">
              {uiState.futurePacts.map((pact) => (
                <section key={pact.pactTypeKey} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Pacto futuro</p>
                      <h4>{pact.pactLabel}</h4>
                    </div>
                    <UiBadge tone="warn">{pact.stateLabel}</UiBadge>
                  </div>
                  <div className="market-future-action-state" aria-hidden="true">
                    {pact.reasonLabel}
                  </div>
                </section>
              ))}
              {uiState.futureActions.map((action) => (
                <section key={action.actionKey} className="subpanel figma-subpanel market-future-action-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Accion futura</p>
                      <h4>{action.label}</h4>
                    </div>
                    <UiBadge tone="warn">{action.stateLabel}</UiBadge>
                  </div>
                  <div className="market-future-action-state" aria-hidden="true">
                    {action.reasonLabel}
                  </div>
                </section>
              ))}
            </div>
          </UiCard>
        </>
      ) : null}

      {(technicalErrorDetail || uiState?.diagnostics.technical.length || uiState?.diagnostics.limitations.length) ? (
        <details className="technical-disclosure">
          <summary>
            <div>
              <p className="eyebrow">Diagnostico secundario</p>
              <strong>Errores, limites y notas tecnicas</strong>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </summary>
          <div className="technical-disclosure-body">
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Soporte de alianzas</p>
                  <h3>Lectura tecnica</h3>
                </div>
                <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
              </div>
              {technicalErrorDetail ? (
                <ul className="stack-list compact-list">
                  <li>{technicalErrorDetail}</li>
                </ul>
              ) : null}
              {uiState?.diagnostics.limitations.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Limitaciones</p>
                      <h4>Fase actual</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.limitations.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
              {uiState?.diagnostics.technical.length ? (
                <>
                  <div className="figma-section-header module-boundary-spacer">
                    <div>
                      <p className="eyebrow">Trazas tecnicas</p>
                      <h4>Solo soporte</h4>
                    </div>
                  </div>
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.technical.map((item) => (
                      <li key={item}>{item}</li>
                    ))}
                  </ul>
                </>
              ) : null}
            </UiCard>
          </div>
        </details>
      ) : null}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>Cabinas relacionadas</h3>
            <p>Alianzas conserva el contexto de civilizacion y deriva la exploracion estrategica, economica e informativa hacia las cabinas ya implementadas.</p>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildGalaxyUrl(queryCivilizationId)}>
            Volver a Galaxia
          </Link>
          <Link className="selection-chip" to={buildMarketUrl(queryCivilizationId, uiState?.identity?.homePlanetId ?? null)}>
            Abrir Mercado
          </Link>
          <Link className="selection-chip" to={buildEspionageUrl(queryCivilizationId, undefined, uiState?.identity?.homePlanetId ?? null)}>
            Abrir Espionaje
          </Link>
          <span className="selection-chip" aria-disabled="true">
            Abrir Ranking
          </span>
        </div>
      </UiCard>
    </section>
  );
}

import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { fetchDefensesUiState } from "../api/defenseApi";
import { CockpitHero } from "../components/CockpitHero";
import { PageContextStrip } from "../components/PageContextStrip";
import { PlayableSessionBanner } from "../components/PlayableSessionBanner";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatResourceType } from "../utils/domainPresentation";
import { formatDefenseRequestFailure } from "../utils/defensePresentation";
import {
  getDefensePrimaryAction,
  groupDefenseOptionsByCategory,
  mapDefensesUiStateToViewModel,
  selectRecommendedDefenseAction,
  type DefensesViewModel,
} from "../utils/defenseViewModel";
import {
  buildConstructionUrl,
  buildDefensesUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildShipyardUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";
import { cockpitNavigationLabels, cockpitStatusLabels } from "../utils/cockpitStatus";
import { usePlayableRouteContext } from "../utils/usePlayableRouteContext";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

function getProtectionPosture(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel) {
    return "Sin contexto defensivo";
  }

  if (!viewModel.isOwnedByRequestingCivilization) {
    return "Observacion externa";
  }

  if (viewModel.protectionSummary.structureCount > 0) {
    return "Proteccion desplegada";
  }

  if (viewModel.protectionSummary.availableOptionCount > 0) {
    return "Fortificacion lista";
  }

  if (viewModel.protectionSummary.queueItemCount > 0) {
    return "Refuerzo en cola";
  }

  return "Proteccion inicial";
}

function getRecommendedNextStep(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel) {
    return "Cargar contexto defensivo";
  }

  if (!viewModel.isOwnedByRequestingCivilization) {
    return "Volver a una colonia propia";
  }

  if (viewModel.protectionSummary.queueItemCount > 0) {
    return "Revisar cola defensiva";
  }

  if (viewModel.protectionSummary.availableOptionCount > 0) {
    return "Abrir Construccion";
  }

  if (viewModel.options.length > 0) {
    return "Resolver bloqueo visible";
  }

  return "Continuar en Construccion";
}

function getResourcePressureSummary(viewModel: DefensesViewModel["defenses"]) {
  if (!viewModel || viewModel.stockpile.length === 0) {
    return "Sin reservas visibles";
  }

  const ordered = [...viewModel.stockpile]
    .sort((left, right) => right.quantity - left.quantity)
    .slice(0, 3)
    .map((entry) => `${formatResourceType(entry.resourceType)} ${entry.quantity}`);

  return ordered.join(" | ");
}

export function DefensesPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<DefensesViewModel | null>(null);
  const [localSessionCleared, setLocalSessionCleared] = useState(false);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const defenses = uiState?.defenses ?? null;
  const playableRouteContext = usePlayableRouteContext(queryCivilizationId);
  const playableSession = localSessionCleared ? null : playableRouteContext.playableSession;
  const routeSession = uiState?.civilizationId && defenses
    ? {
      civilizationId: uiState.civilizationId,
      planetId: defenses.planetId,
      civilizationName: defenses.ownerCivilizationName ?? undefined,
      planetName: defenses.planetName,
      createdAt: "route-context",
      updatedAt: "route-context",
    }
    : null;
  const bannerSession = routeSession ?? playableSession;
  const playableSessionUrl = playableSession
    ? buildDefensesUrl(playableSession.civilizationId, playableSession.planetId)
    : null;
  const hasSafeDefenseEnqueue = false;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const optionGroups = useMemo(() => groupDefenseOptionsByCategory(defenses?.options ?? []), [defenses?.options]);
  const recommendedAction = useMemo(() => selectRecommendedDefenseAction(defenses?.options ?? []), [defenses?.options]);
  const protectionPosture = useMemo(() => getProtectionPosture(defenses), [defenses]);
  const recommendedNextStep = useMemo(() => getRecommendedNextStep(defenses), [defenses]);
  const resourcePressureSummary = useMemo(() => getResourcePressureSummary(defenses), [defenses]);
  const availableOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey === "Available"), [defenses?.options]);
  const blockedOptions = useMemo(() => (defenses?.options ?? []).filter((option) => option.statusKey !== "Available"), [defenses?.options]);

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

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
        const response = await fetchDefensesUiState(queryCivilizationId, queryPlanetId);
        if (!response.succeeded || !response.uiState) {
          const failure = formatDefenseRequestFailure(response.errors[0] ?? null);
          setUiState(null);
          setError(failure.primaryMessage);
          setErrorFollowUp(failure.followUp);
          setTechnicalErrorDetail(failure.technicalDetail);
          return;
        }

        const nextState = mapDefensesUiStateToViewModel(response.uiState);
        setUiState(nextState);

        if (nextState.selectedPlanetId && nextState.selectedPlanetId !== queryPlanetId) {
          const nextParams = new URLSearchParams(searchParams);
          nextParams.set("civilizationId", queryCivilizationId);
          nextParams.set("planetId", nextState.selectedPlanetId);
          setSearchParams(nextParams, { replace: true });
        }
      } catch (requestError) {
        const failure = formatDefenseRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId, searchParams, setSearchParams]);

  function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    const trimmedCivilizationId = civilizationIdInput.trim();

    if (!trimmedCivilizationId) {
      const failure = formatDefenseRequestFailure("Civilization id is required.");
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);

    const trimmedPlanetId = planetIdInput.trim();
    if (trimmedPlanetId) {
      nextParams.set("planetId", trimmedPlanetId);
    }

    setSearchParams(nextParams);
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Defensas v1"
        title="Defensas"
        description="Lectura / readiness de proteccion planetaria y estructuras locales."
        developmentNote="Sin combate ni intercepcion: esta cabina mantiene la preparacion visible."
        badges={
          <>
            <UiBadge>Lectura / readiness</UiBadge>
            <UiBadge>Preparacion visible</UiBadge>
            <UiBadge tone="warn">Sin combate ni intercepcion</UiBadge>
          </>
        }
      />

      <PlayableSessionBanner
        session={bannerSession}
        onClear={() => setLocalSessionCleared(true)}
      />

      {defenses ? (
        <PageContextStrip
          eyebrow="Cabina defensiva"
          title={defenses.planetName}
          purpose="Readiness defensiva, estructuras locales y handoff seguro hacia Construccion."
          statusLabel={protectionPosture}
          statusTone={defenses.isOwnedByRequestingCivilization ? "good" : "warn"}
          contextItems={[
            { label: "Sistema", value: defenses.solarSystemName },
            { label: "Control", value: defenses.isOwnedByRequestingCivilization ? "Colonia propia" : defenses.ownerCivilizationName ?? "Sin control local" },
            { label: "Siguiente paso", value: recommendedNextStep },
            {
              label: "Cola defensiva",
              value: defenses.queue.length > 0 ? `${defenses.queue.length} ordenes visibles` : "Sin cola",
              detail: `${defenses.protectionSummary.dueQueueItemCount} vencidas`,
            },
          ]}
          resourceItems={defenses.stockpile.slice(0, 4).map((resource) => ({
            label: formatResourceType(resource.resourceType),
            value: String(resource.quantity),
          }))}
          primaryAction={
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildConstructionUrl(activeCivilizationId, defenses.planetId)}>
                Construccion
              </Link>
              <Link className="selection-chip" to={buildPlanetUrl(activeCivilizationId, defenses.planetId)}>
                Planeta
              </Link>
              <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, null, defenses.planetId)}>
                Galaxia
              </Link>
            </div>
          }
        />
      ) : null}

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Entrada de cabina</p>
              <h3>Cargar contexto defensivo</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
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
            <label className="field">
              <span>Id de planeta opcional</span>
              <input
                type="text"
                value={planetIdInput}
                onChange={(event) => setPlanetIdInput(event.target.value)}
                placeholder="40000000-0000-0000-0000-000000000000"
                spellCheck={false}
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Cargando..." : "Abrir defensas"}
            </button>
          </form>
          {error ? (
            <div className="subpanel figma-subpanel figma-mini-card-warn">
              <p className="error-text">{error}</p>
              {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
            </div>
          ) : null}
          {isLoading ? <p className="figma-panel-note">Cargando estructuras, opciones y limites defensivos...</p> : null}
          {!queryCivilizationId && !isLoading ? (
            <p className="figma-panel-note">Introduce un `civilizationId` valido para abrir la cabina de defensas.</p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite actual</p>
              <h3>Que pertenece aqui</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.preparation}</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Preparacion de proteccion planetaria y lectura de estructuras defensivas.</li>
            <li>Contexto de recursos, cola y capacidad para futuras fortificaciones.</li>
            <li>Explicacion clara de limites y handoff hacia Construccion, Astillero y Flotas.</li>
          </ul>
          <div className="figma-section-header module-boundary-spacer">
            <div>
              <p className="eyebrow">Fuera de alcance</p>
              <h4>Sin combate activo</h4>
            </div>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Esta build no ejecuta combate, intercepcion, dano, bombardeo ni invasion.</li>
            <li>La infraestructura general sigue perteneciendo a Construccion.</li>
            <li>La movilidad y el stock orbital siguen perteneciendo a Flotas y Astillero.</li>
          </ul>
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El identificador de civilizacion no parece valido para esta cabina.</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
        </UiCard>
      ) : null}

      {!queryCivilizationId && playableSession && playableSessionUrl ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Inicio local disponible</p>
              <h3>Continuar con {playableSession.planetName ?? "la ultima colonia"}</h3>
            </div>
            <UiBadge tone="good">Memoria local</UiBadge>
          </div>
          <p className="figma-panel-note">
            Este enlace abre Defensas con ids locales de navegacion. La cabina sigue siendo solo lectura.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={playableSessionUrl}>
              Abrir Defensas
            </Link>
          </div>
        </UiCard>
      ) : null}

      {defenses ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Dashboard defensivo</p>
                <h3>Postura actual y siguiente paso</h3>
                <p>La primera lectura resume proteccion real, presion visible y la siguiente accion segura para esta colonia.</p>
              </div>
              <UiBadge tone={recommendedAction?.statusKey === "Available" ? "good" : "neutral"}>
                {getDefensePrimaryAction(recommendedAction)}
              </UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Postura</span><strong>{protectionPosture}</strong></div>
                  <div className="figma-data-row"><span>Estructuras</span><strong>{defenses.protectionSummary.structureCount}</strong></div>
                  <div className="figma-data-row"><span>Nivel total</span><strong>{defenses.protectionSummary.totalDefenseLevel}</strong></div>
                  <div className="figma-data-row"><span>Siguiente paso</span><strong>{recommendedNextStep}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Presion de recursos</span><strong>{resourcePressureSummary}</strong></div>
                  <div className="figma-data-row"><span>Cola defensiva</span><strong>{defenses.protectionSummary.queueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Vencidas</span><strong>{defenses.protectionSummary.dueQueueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Opciones disponibles</span><strong>{defenses.protectionSummary.availableOptionCount}</strong></div>
                  <div className="figma-data-row"><span>Opciones bloqueadas</span><strong>{defenses.protectionSummary.blockedOptionCount}</strong></div>
                </div>
              </section>
            </div>
            <p>
              {recommendedAction
                ? `${recommendedAction.structureLabel}: ${recommendedAction.reasonLabel}.`
                : "Todavia no hay una accion defensiva recomendada para este contexto."}
            </p>
            <p className="figma-panel-note">
              {defenses.actionAvailability.completeDue.supported
                ? "La lectura detecta obras vencidas, pero esta cabina no las cierra porque la accion global sigue fuera de alcance."
                : "La proteccion actual sigue anclada a readiness, recursos y cola; no implica eficacia de combate ni mitigacion real."}
            </p>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Reservas locales</p>
                <h3>Scope real de affordability</h3>
                <p>Las preparaciones defensivas se comparan contra las reservas visibles del planeta activo, no contra una economia global inventada.</p>
              </div>
              <UiBadge tone="resource">{defenses.stockpile.length} recursos</UiBadge>
            </div>
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Scope</span><strong>{`Reservas de ${defenses.planetName}`}</strong></div>
              <div className="figma-data-row"><span>Lectura</span><strong>{resourcePressureSummary}</strong></div>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estructuras y preparacion</p>
                <h3>Lo que ya protege y lo que falta preparar</h3>
                <p>Las cartas separan defensa desplegada, opciones viables y bloqueos visibles sin fingir que toda accion se ejecuta desde aqui.</p>
              </div>
              <UiBadge tone="resource">{defenses.options.length} opciones</UiBadge>
            </div>
            {defenses.structures.length > 0 ? (
              <>
                <div className="figma-section-header module-boundary-spacer">
                  <div>
                    <p className="eyebrow">Proteccion actual</p>
                    <h4>Estructuras desplegadas</h4>
                  </div>
                </div>
                <div className="readiness-grid">
                  {defenses.structures.map((structure) => (
                    <article key={`${structure.buildingType}-${structure.level}`} className="subpanel figma-subpanel">
                      <div className="figma-section-header">
                        <div>
                          <p className="eyebrow">{structure.categoryLabel}</p>
                          <h4>{structure.label}</h4>
                        </div>
                        <UiBadge tone="good">Nivel {structure.level}</UiBadge>
                      </div>
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>Estado</span><strong>Ya desplegada</strong></div>
                        <div className="figma-data-row"><span>Huella</span><strong>{structure.footprint}</strong></div>
                        <div className="figma-data-row"><span>Lectura primaria</span><strong>Proteccion estructural</strong></div>
                      </div>
                    </article>
                  ))}
                </div>
              </>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Inventario pendiente</p>
                    <h4>Sin estructuras defensivas visibles</h4>
                  </div>
                  <UiBadge tone="warn">Sin despliegue</UiBadge>
                </div>
                <p className="figma-panel-note">Todavia no hay estructuras defensivas visibles en esta colonia. La cabina conserva el contexto y mostrara preparaciones o bloqueos reales cuando existan.</p>
              </section>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Preparaciones disponibles</p>
                <h4>Opciones viables en esta lectura</h4>
              </div>
              <UiBadge tone={availableOptions.length > 0 ? "good" : "neutral"}>{availableOptions.length}</UiBadge>
            </div>
            {availableOptions.length > 0 ? (
              <div className="readiness-grid">
                {availableOptions.map((option) => (
                  <article key={`${option.buildingType}-${option.targetLevel}`} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{option.categoryLabel}</p>
                        <h4>{option.structureLabel}</h4>
                      </div>
                      <UiBadge tone="good">{option.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Accion</span><strong>{option.actionLabel}</strong></div>
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {option.targetLevel}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
                      <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
                    </div>
                    <p>{option.reasonLabel}</p>
                    <p className="figma-panel-note">Reservas de {defenses.planetName}. La preparacion visible se confirma desde Construccion antes de enviar cualquier orden.</p>
                    <div className="selection-chip-row">
                      <Link className="planet-action-button-secondary planet-action-handoff" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
                        Abrir Construccion
                      </Link>
                      <span className="planet-action-handoff-message">
                        Sin enqueue defensivo
                      </span>
                    </div>
                  </article>
                ))}
              </div>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Catalogo readiness</p>
                    <h4>Sin preparaciones disponibles</h4>
                  </div>
                  <UiBadge tone="neutral">Handoff pendiente</UiBadge>
                </div>
                <p className="figma-panel-note">No hay una preparacion defensiva habilitada en este momento. Revisa la cola, los recursos o el control del planeta desde Construccion.</p>
              </section>
            )}
            <div className="figma-section-header module-boundary-spacer">
              <div>
                <p className="eyebrow">Bloqueos visibles</p>
                <h4>Preparaciones que aun no pueden avanzar</h4>
              </div>
              <UiBadge tone={blockedOptions.length > 0 ? "warn" : "neutral"}>{blockedOptions.length}</UiBadge>
            </div>
            {blockedOptions.length > 0 ? (
              <div className="readiness-grid">
                {blockedOptions.map((option) => (
                  <article key={`${option.buildingType}-${option.targetLevel}`} className="subpanel figma-subpanel figma-mini-card-warn">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{option.categoryLabel}</p>
                        <h4>{option.structureLabel}</h4>
                      </div>
                      <UiBadge tone="warn">{option.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Accion</span><strong>{option.actionLabel}</strong></div>
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {option.targetLevel}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{option.estimatedCostLabel}</strong></div>
                      <div className="figma-data-row"><span>Duracion</span><strong>{option.estimatedDurationLabel}</strong></div>
                    </div>
                    <p>{option.reasonLabel}</p>
                    {option.affordabilityLabel ? <p className="figma-panel-note">{option.affordabilityLabel}</p> : null}
                    {option.requirementLabel ? <p className="figma-panel-note">{option.requirementLabel}</p> : null}
                    <div className="selection-chip-row">
                      <span className="planet-action-handoff-message">
                        {option.statusKey === "Unsupported" ? cockpitStatusLabels.readOnly : "Bloqueada en esta lectura"}
                      </span>
                    </div>
                  </article>
                ))}
              </div>
            ) : (
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Bloqueos</p>
                    <h4>Sin bloqueos defensivos visibles</h4>
                  </div>
                  <UiBadge tone="neutral">Sin bloqueos</UiBadge>
                </div>
                <p className="figma-panel-note">No hay preparaciones bloqueadas visibles para esta lectura. No se fabrica una lista defensiva adicional.</p>
              </section>
            )}
            {optionGroups.length === 0 ? (
              <p className="figma-panel-note">El catalogo defensivo aun no tiene mas opciones para esta colonia. La cabina conserva el contexto y explica el limite con honestidad.</p>
            ) : null}
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Ejecucion segura</p>
                <h3>Como se trata cada accion visible</h3>
                <p>Defensas no duplica el flujo de confirmacion de Construccion en esta build. La cabina clasifica cada opcion y aplica el tratamiento seguro correspondiente.</p>
              </div>
              <UiBadge tone="warn">Sin accion local</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{cockpitStatusLabels.available}</p>
                    <h4>Handoff a Construccion</h4>
                  </div>
                  <UiBadge tone="good">{availableOptions.length}</UiBadge>
                </div>
                <p>Las preparaciones viables se revisan aqui, pero la confirmacion y el enqueue siguen perteneciendo a Construccion.</p>
                {!hasSafeDefenseEnqueue ? <p className="figma-panel-note">La auditoria actual no encontro ningun `POST /api/dev/defenses/...` seguro. Defensas consume solo contratos de readiness derivados de Construccion.</p> : null}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">{cockpitStatusLabels.blocked}</p>
                    <h4>Sin accion inmediata</h4>
                  </div>
                  <UiBadge tone="warn">{blockedOptions.length}</UiBadge>
                </div>
                <p>Las opciones bloqueadas permanecen visibles con motivo explicito y sin senales enganosas de confirmacion.</p>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cola defensiva</p>
                <h3>Ordenes visibles y cierre conservador</h3>
                <p>Esta lectura usa la cola de construccion filtrada para defensas y mantiene el cierre vencido como affordance secundaria.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={defenses.queue.length > 0 ? "warn" : "neutral"}>
                  {defenses.queue.length > 0 ? `${defenses.queue.length} visibles` : "Sin cola"}
                </UiBadge>
                <UiBadge tone={defenses.actionAvailability.completeDue.supported ? "warn" : "neutral"}>
                  {defenses.actionAvailability.completeDue.supported ? "Cierre no habilitado aqui" : "Sin cierre seguro"}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Estado general</p>
                    <h4>Lectura de cola</h4>
                  </div>
                  <UiBadge tone="neutral">{cockpitStatusLabels.readOnly}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Origen</span><strong>{defenses.planetName}</strong></div>
                  <div className="figma-data-row"><span>Scope</span><strong>Construccion filtrada para defensas</strong></div>
                  <div className="figma-data-row"><span>Vencidas</span><strong>{defenses.protectionSummary.dueQueueItemCount}</strong></div>
                  <div className="figma-data-row"><span>Cierre visible</span><strong>{defenses.actionAvailability.completeDue.supported ? "Detectado pero no habilitado" : "No soportado"}</strong></div>
                </div>
                <div className="selection-chip-row">
                  <span className="planet-action-handoff-message">
                    Cierre pendiente de ruta acotada
                  </span>
                </div>
                <p className="figma-panel-note">
                  El backend actual conserva un cierre global de construccion. Esta cabina no lo ejecuta hasta que exista una ruta segura y acotada al planeta.
                </p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Interpretacion</p>
                    <h4>Como leer esta seccion</h4>
                  </div>
                  <UiBadge tone="warn">Sin auto-cierre</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  <li>Una orden visible confirma readiness de construccion, no combate ni cierre automatico.</li>
                  <li>Una orden vencida sigue siendo una lectura de backlog, no una autorizacion para completar desde aqui.</li>
                  <li>Los ids tecnicos quedan fuera de la vista principal y se reservan para diagnosticos.</li>
                </ul>
              </section>
            </div>
            {defenses.queue.length === 0 ? (
              <p className="figma-panel-note">No hay ordenes defensivas en cola.</p>
            ) : (
              <div className="readiness-grid">
                {defenses.queue.map((item) => (
                  <article key={item.orderId} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{item.actionLabel}</p>
                        <h4>{item.structureLabel}</h4>
                      </div>
                      <UiBadge tone={item.isDue ? "warn" : "neutral"}>{item.statusLabel}</UiBadge>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Planeta</span><strong>{defenses.planetName}</strong></div>
                      <div className="figma-data-row"><span>Objetivo</span><strong>Nivel {item.targetLevel}</strong></div>
                      <div className="figma-data-row"><span>Inicio</span><strong>{formatDateTime(item.startsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Fin</span><strong>{formatDateTime(item.endsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Coste</span><strong>{item.estimatedCostLabel}</strong></div>
                    </div>
                    <p>{item.isDue ? "La orden ya vencio en la lectura actual y permanece pendiente de una via segura de cierre." : "La orden sigue visible en la ventana temporal actual."}</p>
                  </article>
                ))}
              </div>
            )}
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Navegacion</p>
                <h3>{cockpitNavigationLabels.relatedCabins}</h3>
                <p>Defensas resume proteccion y readiness, pero cada sistema vecino conserva su propio alcance y su propia accion segura.</p>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Infraestructura</p>
                    <h4>Continuar en Construccion</h4>
                  </div>
                  <UiBadge>{cockpitNavigationLabels.relatedCabin}</UiBadge>
                </div>
                <p>Usa Construccion cuando la siguiente defensa siga siendo una obra planetaria o una mejora de infraestructura.</p>
                <div className="selection-chip-row">
                  <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
                    {cockpitNavigationLabels.openConstruction}
                  </Link>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Activos orbitales</p>
                    <h4>Astillero</h4>
                  </div>
                  <UiBadge>{cockpitNavigationLabels.relatedCabin}</UiBadge>
                </div>
                <p>Usa Astillero cuando el siguiente paso implique plataformas, stock orbital o produccion que no pertenece a Defensas.</p>
                <div className="selection-chip-row">
                  <Link className="selection-chip" to={buildShipyardUrl(activeCivilizationId, selectedPlanetId)}>
                    {cockpitNavigationLabels.openShipyard}
                  </Link>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Mando y movimiento</p>
                    <h4>Flotas</h4>
                  </div>
                  <UiBadge>{cockpitNavigationLabels.relatedCabin}</UiBadge>
                </div>
                <p>Usa Flotas para escuadras, traslados y movimiento orbital. Defensas no ordena grupos ni abre combate.</p>
                <div className="selection-chip-row">
                  <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
                    {cockpitNavigationLabels.openFleets}
                  </Link>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Contexto general</p>
                    <h4>Planeta y Galaxia</h4>
                  </div>
                  <UiBadge>{cockpitNavigationLabels.relatedCabin}</UiBadge>
                </div>
                <p>Usa Planeta para la vision integral de la colonia y Galaxia para la lectura estrategica del teatro local.</p>
                <div className="selection-chip-row">
                  <Link className="selection-chip" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
                    {cockpitNavigationLabels.returnToPlanet}
                  </Link>
                  <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
                    {cockpitNavigationLabels.returnToGalaxy}
                  </Link>
                </div>
              </section>
            </div>
          </UiCard>

          {defenses.diagnostics.playerFacing.length > 0 || defenses.diagnostics.limitations.length > 0 || technicalErrorDetail ? (
            <details className="technical-disclosure">
              <summary>
                <div>
                  <p className="eyebrow">Diagnostico secundario</p>
                  <strong>Lectura tecnica</strong>
                </div>
                <UiBadge tone="warn">Contraido por defecto</UiBadge>
              </summary>
              <div className="technical-disclosure-body">
              <UiCard className="panel fleet-technical-panel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Diagnosticos</p>
                    <h3>Notas y limitaciones</h3>
                  </div>
                  <UiBadge tone="warn">{cockpitStatusLabels.developmentOnly}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {defenses.diagnostics.playerFacing.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {defenses.diagnostics.limitations.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {technicalErrorDetail ? <li>{technicalErrorDetail}</li> : null}
                </ul>
              </UiCard>
              </div>
            </details>
          ) : null}
        </>
      ) : (
        !isLoading && queryCivilizationId && !error ? (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado vacio</p>
                <h3>Sistema defensivo pendiente</h3>
              </div>
              <UiBadge tone="warn">Preparacion limitada</UiBadge>
            </div>
            <p className="figma-panel-note">No hay un contexto defensivo util para este planeta. La cabina mantiene acceso, contexto y explicacion de limites.</p>
          </UiCard>
        ) : null
      )}

      <UiCard className="panel">
        <div className="figma-section-header">
          <div>
            <p className="eyebrow">Navegacion</p>
            <h3>{cockpitNavigationLabels.relatedCabins}</h3>
          </div>
          <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
        </div>
        <div className="selection-chip-row">
          <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.returnToPlanet}
          </Link>
          <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openConstruction}
          </Link>
          <Link className="selection-chip" to={buildShipyardUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openShipyard}
          </Link>
          <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openFleets}
          </Link>
          <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, undefined, selectedPlanetId)}>
            {cockpitNavigationLabels.returnToGalaxy}
          </Link>
        </div>
      </UiCard>
    </section>
  );
}

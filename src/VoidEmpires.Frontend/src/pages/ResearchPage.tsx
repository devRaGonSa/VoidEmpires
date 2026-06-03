import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { enqueueResearchOrder, fetchResearchUiState } from "../api/researchApi";
import type { ResearchTechnology, ResearchUiState } from "../utils/researchPresentation";
import {
  formatResearchCommandFailure,
  formatResearchRequestFailure,
  getResearchPrimaryAction,
  getResearchVisualState,
  groupResearchTechnologiesByCategory,
  mapResearchUiStateToViewModel,
  selectRecommendedResearch,
  summarizeResearchCatalog,
} from "../utils/researchPresentation";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { buildConstructionUrl, buildFleetsUrl, buildGalaxyUrl, buildPlanetUrl, isSuspiciousCabinContext } from "../utils/routeUrls";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

export function ResearchPage() {
  const hasSafeResearchEnqueue = true;
  const [searchParams, setSearchParams] = useSearchParams();
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<ResearchUiState | null>(null);
  const [preparedResearchType, setPreparedResearchType] = useState("");
  const [hasEnqueueAcknowledgement, setHasEnqueueAcknowledgement] = useState(false);
  const [isSubmittingEnqueue, setIsSubmittingEnqueue] = useState(false);
  const [enqueueFeedback, setEnqueueFeedback] = useState<string | null>(null);
  const [enqueueError, setEnqueueError] = useState<string | null>(null);
  const [enqueueOrderDetails, setEnqueueOrderDetails] = useState<{
    orderId: string | null;
    startsAtUtc: string | null;
    endsAtUtc: string | null;
  } | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const recommendedResearch = useMemo(() => selectRecommendedResearch(uiState?.catalog ?? []), [uiState?.catalog]);
  const catalogGroups = useMemo(() => groupResearchTechnologiesByCategory(uiState?.catalog ?? []), [uiState?.catalog]);
  const catalogSummary = useMemo(() => summarizeResearchCatalog(uiState?.catalog ?? []), [uiState?.catalog]);
  const preparedResearch = useMemo(
    () => uiState?.catalog.find((item) => item.researchType === preparedResearchType) ?? null,
    [preparedResearchType, uiState?.catalog],
  );
  const dueQueueCount = useMemo(
    () => uiState?.queue.filter((item) => item.isDue).length ?? 0,
    [uiState?.queue],
  );

  async function reloadResearchState(
    civilizationId: string,
    planetId?: string | null,
    replaceParams = false,
    preserveCurrentStateOnFailure = false,
  ) {
    const response = await fetchResearchUiState(civilizationId, planetId);
    if (!response.succeeded || !response.uiState) {
      const failure = formatResearchCommandFailure(response.errors[0] ?? null);
      if (!preserveCurrentStateOnFailure) {
        setUiState(null);
      }
      setError(failure.primaryMessage);
      setTechnicalErrorDetail(failure.technicalDetail);
      return null;
    }

    const nextState = mapResearchUiStateToViewModel(response.uiState);
    setTechnicalErrorDetail(null);
    setUiState(nextState);

    if (nextState.selectedPlanetId && nextState.selectedPlanetId !== planetId) {
      const nextParams = new URLSearchParams(searchParams);
      nextParams.set("civilizationId", civilizationId);
      nextParams.set("planetId", nextState.selectedPlanetId);
      setSearchParams(nextParams, { replace: replaceParams });
    }

    return nextState;
  }

  useEffect(() => {
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setTechnicalErrorDetail(null);
        return;
      }

      setIsLoading(true);
      setError(null);

      try {
        await reloadResearchState(queryCivilizationId, queryPlanetId, true);
      } catch (requestError) {
        const failure = formatResearchRequestFailure(requestError instanceof Error ? requestError.message : null);
        setUiState(null);
        setError(failure.primaryMessage);
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
      setError("El id de civilizacion es obligatorio.");
      setTechnicalErrorDetail("Civilization id is required.");
      setUiState(null);
      return;
    }

    const nextParams = new URLSearchParams();
    nextParams.set("civilizationId", trimmedCivilizationId);
    if (planetIdInput.trim()) {
      nextParams.set("planetId", planetIdInput.trim());
    }
    setSearchParams(nextParams);
  }

  async function handleResearchSubmit() {
    if (
      !preparedResearch ||
      !preparedResearch.availability.canEnqueue ||
      !preparedResearch.enqueueCommand ||
      !hasSafeResearchEnqueue
    ) {
      return;
    }

    setIsSubmittingEnqueue(true);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setEnqueueOrderDetails(null);
    setTechnicalErrorDetail(null);

    try {
      const { civilizationId, sourcePlanetId, researchType } = preparedResearch.enqueueCommand;
      const result = await enqueueResearchOrder({
        civilizationId,
        sourcePlanetId,
        researchType,
        requestedAtUtc: new Date().toISOString(),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const failure = formatResearchCommandFailure(result.response?.errors[0] ?? null, result.httpStatus);
        setEnqueueError(failure.primaryMessage);
        setTechnicalErrorDetail(failure.technicalDetail);
        return;
      }

      setEnqueueOrderDetails({
        orderId: result.response.orderId,
        startsAtUtc: result.response.startsAtUtc,
        endsAtUtc: result.response.endsAtUtc,
      });
      setEnqueueFeedback("Investigacion enviada a la cola.");
      setPreparedResearchType("");
      setHasEnqueueAcknowledgement(false);

      const refreshed = await reloadResearchState(civilizationId, sourcePlanetId, false, true);
      if (!refreshed) {
        setEnqueueError(
          "La orden se envio, pero la cabina no pudo recargar el estado actualizado. Refresca la vista para confirmar el resultado final.",
        );
        setTechnicalErrorDetail("Research UI state refresh failed after a successful enqueue.");
      }
    } catch (requestError) {
      const failure = formatResearchRequestFailure(requestError instanceof Error ? requestError.message : null);
      setEnqueueError(failure.primaryMessage);
      setTechnicalErrorDetail(failure.technicalDetail);
    } finally {
      setIsSubmittingEnqueue(false);
    }
  }

  function handleResearchPreparation(technology: ResearchTechnology) {
    if (!technology.enqueueCommand) {
      setPreparedResearchType("");
      setHasEnqueueAcknowledgement(false);
      setEnqueueFeedback(null);
      setEnqueueOrderDetails(null);
      setEnqueueError("No se puede preparar esta investigacion en esta build.");
      setTechnicalErrorDetail("Research enqueue command metadata is missing.");
      return;
    }

    setPreparedResearchType(technology.researchType);
    setHasEnqueueAcknowledgement(false);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setEnqueueOrderDetails(null);
    setTechnicalErrorDetail(null);
  }

  function handleResearchCancel() {
    setPreparedResearchType("");
    setHasEnqueueAcknowledgement(false);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setEnqueueOrderDetails(null);
    setTechnicalErrorDetail(null);
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Investigacion v1</UiBadge>
          <h2>Investigacion</h2>
          <p>Cabina de investigacion con carga de contexto, catalogo y diagnostico sin exponer DTOs crudos en la superficie principal.</p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Cabina de lectura</UiBadge>
          <UiBadge tone={hasSafeResearchEnqueue ? "good" : "warn"}>
            {hasSafeResearchEnqueue ? "Mutacion dev protegida" : "Sin mutacion segura"}
          </UiBadge>
          <UiBadge tone="warn">Contexto conservado entre saltos</UiBadge>
        </div>
      </UiCard>

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Enlace cientifico</p>
              <h3>Cargar contexto de investigacion</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Id de civilizacion</span>
              <input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="00000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <label className="field">
              <span>Id de planeta opcional</span>
              <input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="40000000-0000-0000-0000-000000000000" spellCheck={false} />
            </label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Abrir cabina"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? <p className="figma-panel-note">Introduce un `civilizationId` valido o entra desde Galaxia para fijar el contexto automaticamente.</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Resumen</p>
              <h3>Estado de investigacion</h3>
            </div>
            <UiBadge>{uiState ? `${uiState.catalog.length} tecnologias` : "Sin datos"}</UiBadge>
          </div>
          {uiState ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Planeta seleccionado</span><strong>{uiState.selectedPlanetName ?? "Sin planeta"}</strong></div>
              <div className="figma-data-row"><span>Disponibles</span><strong>{catalogSummary.availableCount}</strong></div>
              <div className="figma-data-row"><span>Bloqueadas</span><strong>{catalogSummary.blockedCount}</strong></div>
              <div className="figma-data-row"><span>Cola</span><strong>{uiState.queue.length}</strong></div>
              <div className="figma-data-row"><span>En espera de cierre</span><strong>{dueQueueCount}</strong></div>
              <div className="figma-data-row"><span>Proyectos</span><strong>{catalogSummary.completedCount}</strong></div>
              <div className="figma-data-row"><span>Recomendacion</span><strong>{recommendedResearch ? recommendedResearch.label : "No hay investigaciones disponibles ahora."}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara catalogo, cola y diagnostico cuando exista un contexto valido.</p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limites de cabina</p>
              <h3>Que entra aqui</h3>
            </div>
            <UiBadge tone="good">Lectura segura</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>La cabina prioriza lectura de catalogo, cola y proyectos completados.</li>
            <li>La navegacion conserva `civilizationId` y `planetId` siempre que existen.</li>
            <li>Las altas a cola requieren confirmacion explicita y solo usan una ruta dev segura.</li>
            <li>El diagnostico tecnico se mantiene aparte para no ensuciar la vista principal.</li>
          </ul>
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El id de civilizacion no parece valido para esta cabina.</h3>
            </div>
            <UiBadge tone="warn">Revisar contexto</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Abrir Planeta</Link>
          </div>
        </UiCard>
      ) : null}

      {isLoading ? <UiCard className="panel"><p>Cargando investigacion, catalogo y diagnostico del contexto seleccionado.</p></UiCard> : null}

      {!isLoading && uiState && uiState.catalog.length === 0 ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Catalogo vacio</p>
              <h3>No hay tecnologias disponibles en esta build.</h3>
            </div>
            <UiBadge tone="warn">Sin catalogo</UiBadge>
          </div>
          <p className="figma-panel-note">La cabina esta preparada, pero el backend no devolvio tecnologias de investigacion para este contexto.</p>
        </UiCard>
      ) : null}

      {uiState ? (
        <>
          <UiCard className="panel research-queue-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cola y progreso</p>
                <h3>Elementos activos y completados</h3>
              </div>
              <UiBadge tone="warn">Contexto conservado</UiBadge>
            </div>
            <div className="figma-two-column">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Cola</p><h4>Ordenes activas</h4></div>
                  <UiBadge>{uiState.queue.length}</UiBadge>
                </div>
                {uiState.queue.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.queue.map((item) => (
                      <li key={item.orderId}>
                        {item.label} nivel {item.targetLevel} | {item.isDue ? "Lista para cierre" : item.statusLabel} | cierre {formatDateTime(item.endsAtUtc)}
                      </li>
                    ))}
                  </ul>
                ) : <p className="figma-panel-note">No hay ordenes activas en la cola.</p>}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Completadas</p><h4>Proyectos cerrados</h4></div>
                  <UiBadge>{uiState.projects.length}</UiBadge>
                </div>
                {uiState.projects.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.projects.map((item) => (
                      <li key={`${item.researchType}`}>{item.label} nivel {item.currentLevel}</li>
                    ))}
                  </ul>
                ) : <p className="figma-panel-note">No hay proyectos completados para mostrar.</p>}
              </section>
            </div>
          </UiCard>

          <UiCard className="panel research-catalog-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Catalogo</p>
                <h3>Tecnologias por categoria</h3>
              </div>
              <UiBadge tone="good">Vista normalizada</UiBadge>
            </div>
            <div className="planet-building-groups research-catalog-groups">
              {catalogGroups.map((group) => (
                <section key={group.key} className="subpanel figma-subpanel">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">{group.label}</p>
                      <h4>{group.label}</h4>
                    </div>
                    <UiBadge>{group.technologies.length}</UiBadge>
                  </div>
                  <div className="planet-building-grid research-tech-grid">
                    {group.technologies.map((technology) => {
                      const visualState = getResearchVisualState(technology);
                      const canPrepare = hasSafeResearchEnqueue && visualState === "ready" && Boolean(technology.enqueueCommand);
                      const cardClassName = `subpanel figma-subpanel research-tech-card research-tech-card-${visualState}`;
                      const buttonClassName = visualState === "blocked"
                        ? "planet-action-button-blocked"
                        : visualState === "ready"
                          ? "research-action-button-ready"
                          : "planet-action-button-secondary";
                      const blockedReasonLabel = technology.availability.reasonKey === "InsufficientResources"
                        ? `Recursos insuficientes en ${uiState.selectedPlanetName ?? "el planeta seleccionado"}.`
                        : technology.availability.reasonLabel;

                      return (
                      <article
                        key={`${technology.researchType}`}
                        className={cardClassName}
                      >
                        <div className="figma-section-header">
                          <div>
                            <p className="eyebrow">{technology.bonusLabel}</p>
                            <h4>{technology.label}</h4>
                          </div>
                          <UiBadge tone={visualState === "ready" ? "good" : visualState === "blocked" ? "warn" : "resource"}>{technology.availability.label}</UiBadge>
                        </div>
                        <div className="figma-data-list">
                          <div className="figma-data-row"><span>Nivel</span><strong>{`${technology.currentLevel} -> ${technology.nextLevel}`}</strong></div>
                          <div className="figma-data-row"><span>Coste</span><strong>{technology.estimatedCostLabel}</strong></div>
                          <div className="figma-data-row"><span>Duracion</span><strong>{technology.estimatedDurationLabel}</strong></div>
                          <div className="figma-data-row"><span>Accion</span><strong>{getResearchPrimaryAction(technology)}</strong></div>
                        </div>
                        <div className="research-requirements-block">
                          <p className="research-card-caption">Requisitos visibles</p>
                          <div className="selection-chip-row research-requirements-row">
                            {technology.requirements.map((requirement) => (
                              <span key={`${technology.researchType}-${requirement.key}`} className="selection-chip">
                                {requirement.label}
                              </span>
                            ))}
                          </div>
                        </div>
                        <div className="transfer-confirmation-actions">
                          <button
                            type="button"
                            className={buttonClassName}
                            onClick={() => handleResearchPreparation(technology)}
                            disabled={!canPrepare}
                          >
                            {preparedResearchType === technology.researchType
                              ? "Revision preparada"
                              : technology.primaryActionLabel}
                          </button>
                        </div>
                        {visualState !== "ready" ? (
                          <p className="figma-panel-note">
                            {technology.availability.canCompleteDue
                              ? "El cierre manual de investigaciones vencidas sigue fuera de esta cabina."
                              : `No se puede iniciar: ${blockedReasonLabel}`}
                          </p>
                        ) : !hasSafeResearchEnqueue ? (
                          <p className="figma-panel-note">
                            Esta build no expone una ruta segura para iniciar investigacion desde la cabina.
                          </p>
                        ) : !technology.enqueueCommand ? (
                          <p className="figma-panel-note">
                            No se puede preparar esta investigacion en esta build.
                          </p>
                        ) : (
                          <p className="figma-panel-note">
                            La investigacion no se enviara hasta que confirmes la orden en el paso final.
                          </p>
                        )}
                      </article>
                      );
                    })}
                  </div>
                </section>
              ))}
            </div>
          </UiCard>

          {preparedResearch && preparedResearch.availability.canEnqueue ? (
            <UiCard className="panel transfer-confirmation-panel research-confirmation-panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Paso final</p>
                  <h3>Confirmar inicio de investigacion</h3>
                  <p>Esta accion enviara una unica orden segura de investigacion para el planeta seleccionado.</p>
                </div>
                <UiBadge tone="warn">Confirmacion obligatoria</UiBadge>
              </div>
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState.civilizationId}</strong></div>
                <div className="figma-data-row"><span>Planeta</span><strong>{uiState.selectedPlanetName ?? "Sin planeta"}</strong></div>
                <div className="figma-data-row"><span>Tecnologia</span><strong>{preparedResearch.label}</strong></div>
                <div className="figma-data-row"><span>Categoria</span><strong>{preparedResearch.categoryLabel}</strong></div>
                <div className="figma-data-row"><span>Nivel objetivo</span><strong>{preparedResearch.nextLevel}</strong></div>
                <div className="figma-data-row"><span>Coste</span><strong>{preparedResearch.estimatedCostLabel}</strong></div>
                <div className="figma-data-row"><span>Duracion</span><strong>{preparedResearch.estimatedDurationLabel}</strong></div>
                <div className="figma-data-row"><span>Requisitos</span><strong>{preparedResearch.availability.reasonKey === "Ready" ? "Listos para envio" : preparedResearch.availability.reasonLabel}</strong></div>
              </div>
              <p className="figma-panel-note">
                {preparedResearch.availability.reasonKey === "Ready"
                  ? "La orden esta lista para enviarse cuando confirmes."
                  : `La cabina validara tambien: ${preparedResearch.availability.reasonLabel}.`}
              </p>
              <label className="confirmation-checkbox">
                <input
                  type="checkbox"
                  checked={hasEnqueueAcknowledgement}
                  onChange={(event) => setHasEnqueueAcknowledgement(event.target.checked)}
                />
                <span>Confirmo que quiero iniciar esta investigacion</span>
              </label>
              <div className="transfer-confirmation-actions">
                <button
                  type="button"
                  className="planet-action-button-secondary"
                  onClick={handleResearchCancel}
                  disabled={isSubmittingEnqueue}
                >
                  Cancelar
                </button>
                <button
                  type="button"
                  onClick={() => void handleResearchSubmit()}
                  disabled={isSubmittingEnqueue || !hasEnqueueAcknowledgement}
                >
                  {isSubmittingEnqueue ? "Confirmando..." : "Confirmar"}
                </button>
              </div>
            </UiCard>
          ) : null}

          {enqueueFeedback ? <p>{enqueueFeedback}</p> : null}
          {enqueueError ? <p className="error-text">{enqueueError}</p> : null}
          {enqueueOrderDetails ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Orden creada</p>
                  <h3>Resultado confirmado por la API</h3>
                </div>
                <UiBadge tone="good">Sin optimismo local</UiBadge>
              </div>
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Orden</span><strong>{enqueueOrderDetails.orderId ?? "No devuelto"}</strong></div>
                <div className="figma-data-row"><span>Inicio</span><strong>{enqueueOrderDetails.startsAtUtc ? formatDateTime(enqueueOrderDetails.startsAtUtc) : "No devuelto"}</strong></div>
                <div className="figma-data-row"><span>Cierre</span><strong>{enqueueOrderDetails.endsAtUtc ? formatDateTime(enqueueOrderDetails.endsAtUtc) : "No devuelto"}</strong></div>
              </div>
            </UiCard>
          ) : null}

          <UiCard className="panel research-action-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Cierre vencido</p>
                <h3>Completar investigaciones vencidas</h3>
                <p>La ruta disponible en esta build procesa vencimientos de forma global y no queda limitada a esta cabina.</p>
              </div>
              <UiBadge tone="warn">{dueQueueCount} vencidas</UiBadge>
            </div>
            <div className="transfer-confirmation-actions">
              <button type="button" className="planet-action-button-blocked" disabled>
                Completar vencidas no disponible
              </button>
            </div>
            <p className="figma-panel-note">
              Esta accion no esta disponible desde Investigacion en esta build porque el cierre seguro todavia no esta acotado al contexto visible.
            </p>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Navegacion</p>
                <h3>Siguientes cabinas</h3>
              </div>
              <UiBadge tone="warn">Contexto conservado</UiBadge>
            </div>
            <div className="selection-chip-row">
              <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Volver a Planeta</Link>
              <Link className="selection-chip" to={buildConstructionUrl(activeCivilizationId, selectedPlanetId)}>Abrir Construccion</Link>
              {selectedPlanetId ? <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>Abrir Flotas</Link> : null}
              <Link className="selection-chip" to={buildGalaxyUrl(activeCivilizationId, null, selectedPlanetId)}>Volver a Galaxia</Link>
            </div>
          </UiCard>

          <details className="technical-disclosure">
            <summary>
              <div>
                <p className="eyebrow">Diagnostico secundario</p>
                <strong>Ids, limites y lectura tecnica</strong>
              </div>
              <UiBadge tone="warn">Contraido por defecto</UiBadge>
            </summary>
            <div className="technical-disclosure-body">
              <UiCard className="panel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Metadatos</p><h3>Soporte tecnico</h3></div>
                  <UiBadge>Diagnostico</UiBadge>
                </div>
                <div className="figma-data-list">
                  {uiState.diagnostics.lines.map((line) => <div key={line} className="figma-data-row"><span>Linea</span><strong>{line}</strong></div>)}
                </div>
                {uiState.diagnostics.limitations.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {uiState.diagnostics.limitations.map((item) => <li key={item}>{item}</li>)}
                  </ul>
                ) : null}
                {technicalErrorDetail ? (
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Detalle tecnico</span><strong>{technicalErrorDetail}</strong></div>
                  </div>
                ) : null}
              </UiCard>
            </div>
          </details>
        </>
      ) : null}
    </section>
  );
}

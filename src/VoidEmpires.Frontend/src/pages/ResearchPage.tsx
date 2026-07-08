import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { enqueueResearchOrder, fetchResearchUiState } from "../api/researchApi";
import type { EnqueueResearchOrderFailureResponse, ResearchApiErrorCode } from "../api/researchTypes";
import { CockpitHero } from "../components/CockpitHero";
import { DevDiagnosticsPanel } from "../components/DevDiagnosticsPanel";
import { GameModal } from "../components/GameModal";
import { PlayableSessionBanner } from "../components/PlayableSessionBanner";
import { ResearchCatalogCard } from "../components/ResearchCatalogCard";
import type { ResearchTechnology, ResearchUiState } from "../utils/researchPresentation";
import {
  formatResearchCommandFailure,
  formatResearchRequestFailure,
  getResearchVisualState,
  groupResearchTechnologiesByCategory,
  mapResearchUiStateToViewModel,
  summarizeResearchCatalog,
} from "../utils/researchPresentation";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { isOperatorMode } from "../utils/playableSession";
import { buildPlanetUrl, buildResearchUrl, isSuspiciousCabinContext } from "../utils/routeUrls";
import { usePlayableRouteContext } from "../utils/usePlayableRouteContext";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

function buildResearchEnqueueTechnicalDetail(
  response: EnqueueResearchOrderFailureResponse | null,
  httpStatus: number,
) {
  if (!response) {
    return `Research enqueue request failed with status ${httpStatus}.`;
  }

  return JSON.stringify({
    httpStatus,
    failureKind: response.failureKind,
    isOpenOrderNoOp: response.isOpenOrderNoOp,
    errors: response.errors,
    errorEntries: response.errorEntries,
  }, null, 2);
}

function matchesResearchEnqueuePattern(value: string, patterns: readonly string[]) {
  const normalizedValue = value.trim().toLowerCase();
  return patterns.some((pattern) => normalizedValue.includes(pattern));
}

function getBlockedResearchReasonLabel(reasonKey: string, canCompleteDue: boolean) {
  if (canCompleteDue) {
    return "investigacion en curso";
  }

  switch (reasonKey) {
    case "InsufficientResources":
      return "faltan recursos";
    case "RequirementPending":
      return "requisito pendiente";
    case "OpenQueueSlot":
      return "investigacion en curso";
    case "NotAvailableInThisBuild":
      return "no disponible en esta lectura";
    case "SourcePlanetMissing":
      return "fuera de alcance";
    default:
      return "fuera de alcance";
  }
}

function getBlockedResearchReasonDetail(
  technology: ResearchTechnology,
  selectedPlanetName?: string | null,
) {
  if (technology.availability.canCompleteDue) {
    return "El cierre manual de investigaciones vencidas sigue fuera de esta vista.";
  }

  switch (technology.availability.reasonKey) {
    case "InsufficientResources":
      return `Faltan recursos en ${selectedPlanetName ?? "el planeta seleccionado"} para iniciar esta investigacion.`;
    case "RequirementPending":
      return "Todavia falta un requisito previo para habilitar esta investigacion.";
    case "OpenQueueSlot":
      return "Ya hay una investigacion en curso para esta civilizacion y no se puede abrir otra orden.";
    case "NotAvailableInThisBuild":
      return "Esta investigacion no esta disponible en la lectura actual de la vista.";
    case "SourcePlanetMissing":
      return "El contexto visible ya no permite enviar esta investigacion desde este planeta.";
    default:
      return `No se puede iniciar: ${technology.availability.reasonLabel}`;
  }
}

function formatResearchEnqueueValidationError(
  response: EnqueueResearchOrderFailureResponse | null,
  httpStatus: number,
  selectedPlanetName?: string | null,
) {
  const fallback = formatResearchCommandFailure(
    response?.errors[0] ?? null,
    httpStatus,
    selectedPlanetName,
  );
  const technicalDetail = buildResearchEnqueueTechnicalDetail(response, httpStatus);
  const scopedPlanetLabel = selectedPlanetName?.trim() || "el planeta seleccionado";

  if (!response) {
    return {
      primaryMessage: fallback.primaryMessage,
      technicalDetail,
    };
  }

  const rawMessages = response.errorEntries.map((entry) => entry.message);
  const hasPattern = (...patterns: string[]) => rawMessages.some((message) => matchesResearchEnqueuePattern(message, patterns));
  const hasCode = (...codes: ResearchApiErrorCode[]) => response.errorEntries.some((entry) => codes.includes(entry.code));

  if (hasCode("OpenResearchOrderExists") || hasPattern("open research order", "open order")) {
    return {
      primaryMessage: "Ya existe una investigacion en curso para esta civilizacion. Espera a que termine antes de enviar otra orden.",
      technicalDetail,
    };
  }

  if (hasCode("InsufficientResources") || hasPattern("insufficient resources")) {
    return {
      primaryMessage: `No hay recursos suficientes en ${scopedPlanetLabel} para iniciar esta investigacion.`,
      technicalDetail,
    };
  }

  if (hasCode("MissingCivilizationId") || hasPattern("civilization was not found", "civilization id is required")) {
    return {
      primaryMessage: "La civilizacion indicada no es valida para esta vista. Recarga el contexto desde Galaxia antes de reintentar.",
      technicalDetail,
    };
  }

  if (
    hasCode("MissingSourcePlanetId", "SourcePlanetNotOwned", "SourcePlanetStockpileMissing") ||
    hasPattern("source planet id is required", "planet is not owned", "planet resource stockpile was not found", "planet was not found")
  ) {
    return {
      primaryMessage: "El planeta de origen ya no es valido para esta orden. Actualiza la vista y vuelve a seleccionar el contexto.",
      technicalDetail,
    };
  }

  if (hasCode("MissingResearchType") || hasPattern("research type is required", "research type was not found", "invalid research type")) {
    return {
      primaryMessage: "La tecnologia solicitada no es valida o ya no esta disponible en esta version. Actualiza la vista antes de reintentar.",
      technicalDetail,
    };
  }

  if (hasPattern("prerequisite", "requirement", "missing prerequisite")) {
    return {
      primaryMessage: "Falta un requisito previo para iniciar esta investigacion. Revisa el catalogo antes de volver a enviarla.",
      technicalDetail,
    };
  }

  if (hasPattern("already researched", "level unavailable", "max level", "target level")) {
    return {
      primaryMessage: "Esta investigacion ya esta completada o no tiene mas niveles disponibles para esta civilizacion.",
      technicalDetail,
    };
  }

  if (hasPattern("not available", "unavailable research")) {
    return {
      primaryMessage: "La investigacion solicitada no esta disponible para este contexto.",
      technicalDetail,
    };
  }

  if (response.failureKind === "validation" || response.failureKind === "conflict") {
    return {
      primaryMessage: "No se pudo completar la orden de investigacion. Revisa el contexto y vuelve a intentarlo.",
      technicalDetail,
    };
  }

  return {
    primaryMessage: fallback.primaryMessage,
    technicalDetail,
  };
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
  const [localSessionCleared, setLocalSessionCleared] = useState(false);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const operatorMode = isOperatorMode(searchParams);
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const playableRouteContext = usePlayableRouteContext(queryCivilizationId);
  const playableSession = localSessionCleared ? null : playableRouteContext.playableSession;
  const routeSession = uiState?.civilizationId && selectedPlanetId
    ? {
      civilizationId: uiState.civilizationId,
      planetId: selectedPlanetId,
      planetName: uiState.selectedPlanetName ?? undefined,
      createdAt: "route-context",
      updatedAt: "route-context",
    }
    : null;
  const bannerSession = routeSession ?? playableSession;
  const playableSessionUrl = playableSession
    ? buildResearchUrl(playableSession.civilizationId, playableSession.planetId)
    : null;
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
        setError(`No se pudo cargar Investigacion. ${failure.primaryMessage}`);
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
      const selectedPlanetName = uiState?.selectedPlanetName ?? null;
      const result = await enqueueResearchOrder({
        civilizationId,
        sourcePlanetId,
        researchType,
        requestedAtUtc: new Date().toISOString(),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const failure = formatResearchEnqueueValidationError(
          result.response && !result.response.succeeded ? result.response : null,
          result.httpStatus,
          selectedPlanetName,
        );
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
          "La investigacion fue aceptada; la cola visible se actualizara con la siguiente lectura disponible.",
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
      setEnqueueError("No se puede preparar esta investigacion en esta version.");
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
      <CockpitHero
        versionLabel="Investigacion v1"
        title="Laboratorio"
        description="Tecnologias disponibles, cola de investigacion y coste visible antes de confirmar una orden cientifica."
        developmentNote="Progreso cientifico con confirmacion obligatoria antes de iniciar una investigacion."
        badges={
          <>
            <UiBadge tone="good">Tecnologias disponibles</UiBadge>
            <UiBadge>Cola de investigacion</UiBadge>
            <UiBadge tone="warn">Confirmacion obligatoria</UiBadge>
          </>
        }
      />

      <PlayableSessionBanner
        session={bannerSession}
        onClear={() => setLocalSessionCleared(true)}
      />

      <div className="strategic-cockpit-top">
        <UiCard className="panel strategic-loader-panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Laboratorio</p>
              <h3>Cargar contexto cientifico</h3>
            </div>
            <UiBadge>Uso local</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Civilizacion</span>
              <input type="text" value={civilizationIdInput} onChange={(event) => setCivilizationIdInput(event.target.value)} placeholder="Usa la seleccion disponible o un enlace de Galaxia" spellCheck={false} />
            </label>
            <label className="field">
              <span>Planeta de laboratorio</span>
              <input type="text" value={planetIdInput} onChange={(event) => setPlanetIdInput(event.target.value)} placeholder="Opcional si vienes desde una colonia" spellCheck={false} />
            </label>
            <button type="submit" disabled={isLoading}>{isLoading ? "Cargando..." : "Abrir vista"}</button>
          </form>
          {error ? <p className="error-text">{error}</p> : null}
          {!queryCivilizationId ? <p className="figma-panel-note">Entra desde Galaxia o usa el inicio local disponible para reconstruir la URL con contexto.</p> : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Laboratorio</p>
              <h3>Que entra aqui</h3>
            </div>
            <UiBadge tone="good">Lectura segura</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>La vista prioriza tecnologias disponibles, cola de investigacion y proyectos completados.</li>
            <li>La navegacion conserva el contexto cientifico al cambiar de vista.</li>
            <li>Las altas a cola requieren confirmacion explicita.</li>
            <li>Los detalles de soporte quedan fuera de la vista principal.</li>
          </ul>
        </UiCard>
      </div>

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El id de civilizacion no parece valido para esta vista.</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Abrir Planeta</Link>
          </div>
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
            Este enlace recupera la ultima colonia local guardada; cada vista volvera a comprobar el estado de juego antes de mostrar acciones.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={playableSessionUrl}>
              Abrir Investigacion
            </Link>
          </div>
        </UiCard>
      ) : null}

      {isLoading ? <UiCard className="panel"><p>Cargando laboratorio, tecnologias disponibles y cola de investigacion.</p></UiCard> : null}

      {!isLoading && uiState && uiState.catalog.length === 0 ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Laboratorio</p>
              <h3>Tecnologia pendiente de seleccion.</h3>
            </div>
            <UiBadge tone="warn">Sin catalogo</UiBadge>
          </div>
          <p className="figma-panel-note">La vista esta preparada, pero este contexto aun no tiene tecnologias de investigacion visibles; no se muestran opciones falsas.</p>
        </UiCard>
      ) : null}

      {uiState ? (
        <>
          <UiCard className="panel research-queue-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Progreso cientifico</p>
                <h3>Cola de investigacion</h3>
              </div>
              <UiBadge tone="warn">{cockpitStatusLabels.contextPreserved}</UiBadge>
            </div>
            <div className="figma-two-column">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Cola de investigacion</p><h4>Ordenes e historial visible</h4></div>
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
                ) : <p className="figma-panel-note">No hay ordenes abiertas ni historial de cola para mostrar.</p>}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div><p className="eyebrow">Progreso cientifico</p><h4>Tecnologias completadas</h4></div>
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
                <p className="eyebrow">Laboratorio</p>
                <h3>Tecnologias disponibles y bloqueadas</h3>
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
                      const blockedReasonLabel = getBlockedResearchReasonLabel(
                        technology.availability.reasonKey,
                        technology.availability.canCompleteDue,
                      );
                      const blockedReasonDetail = getBlockedResearchReasonDetail(
                        technology,
                        uiState.selectedPlanetName,
                      );

                      return (
                        <ResearchCatalogCard
                          key={`${technology.researchType}`}
                          technology={technology}
                          isPrepared={preparedResearchType === technology.researchType}
                          canPrepare={canPrepare}
                          blockedReasonLabel={blockedReasonLabel}
                          blockedReasonDetail={blockedReasonDetail}
                          onPrepare={handleResearchPreparation}
                        />
                      );
                    })}
                  </div>
                </section>
              ))}
            </div>
          </UiCard>

          {enqueueFeedback ? <p>{enqueueFeedback}</p> : null}
          {enqueueError ? <p className="error-text">{enqueueError}</p> : null}
          {enqueueOrderDetails ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Orden creada</p>
                  <h3>Investigacion aceptada</h3>
                </div>
                <UiBadge tone="good">Lectura actualizada</UiBadge>
              </div>
              <div className="figma-data-list">
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
                <p>El cierre seguro de investigaciones vencidas todavia no esta disponible desde esta vista.</p>
              </div>
              <UiBadge tone="warn">{dueQueueCount} vencidas</UiBadge>
            </div>
            <div className="transfer-confirmation-actions">
              <button type="button" className="planet-action-button-blocked" disabled>
                No disponible en esta version
              </button>
            </div>
            <p className="figma-panel-note">
              Esta accion no esta disponible desde Investigacion en esta version porque el cierre seguro todavia no esta acotado al contexto visible.
            </p>
          </UiCard>

          {operatorMode ? (
            <>
              <DevDiagnosticsPanel
                title="Diagnostico de investigacion"
                summaryItems={[
                  { label: "Civilizacion", value: activeCivilizationId },
                  { label: "Planeta", value: selectedPlanetId },
                  { label: "Tecnologias", value: uiState.catalog.length },
                  { label: "Cola visible", value: uiState.queue.length },
                  { label: "Ordenes vencidas", value: dueQueueCount },
                  { label: "Proyectos completados", value: uiState.projects.length },
                ]}
                notes={[
                  ...uiState.diagnostics.limitations,
                  ...(technicalErrorDetail ? [technicalErrorDetail] : []),
                ]}
                rawPayload={{
                  diagnostics: uiState.diagnostics,
                  enqueueOrderDetails,
                }}
              />

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
                      <UiBadge>{cockpitStatusLabels.diagnostics}</UiBadge>
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
                      technicalErrorDetail.includes("\n") ? (
                        <div className="figma-data-list">
                          <div className="figma-data-row">
                            <span>Detalle tecnico</span>
                            <pre style={{ margin: 0, whiteSpace: "pre-wrap" }}>{technicalErrorDetail}</pre>
                          </div>
                        </div>
                      ) : (
                        <div className="figma-data-list">
                          <div className="figma-data-row"><span>Detalle tecnico</span><strong>{technicalErrorDetail}</strong></div>
                        </div>
                      )
                    ) : null}
                    {enqueueOrderDetails?.orderId ? (
                      <div className="figma-data-list">
                        <div className="figma-data-row"><span>Orden confirmada</span><strong>{enqueueOrderDetails.orderId}</strong></div>
                      </div>
                    ) : null}
                  </UiCard>
                </div>
              </details>
            </>
          ) : null}
        </>
      ) : null}

      {preparedResearch && preparedResearch.availability.canEnqueue ? (
        <GameModal
          actionScope="gameplay"
          canClose={!isSubmittingEnqueue}
          closeLabel="Cerrar"
          description="Revisa tecnologia, coste y duracion antes de enviar la investigacion a la cola del planeta."
          isBusy={isSubmittingEnqueue}
          isOpen
          onClose={handleResearchCancel}
          primaryAction={{
            label: "Iniciar investigacion",
            onClick: () => void handleResearchSubmit(),
            disabled: !hasEnqueueAcknowledgement,
          }}
          secondaryAction={{
            label: "Cancelar",
            onClick: handleResearchCancel,
          }}
          title="Iniciar investigacion"
        >
          <div className="figma-data-list">
            <div className="figma-data-row"><span>Planeta</span><strong>{uiState?.selectedPlanetName ?? "Sin planeta"}</strong></div>
            <div className="figma-data-row"><span>Tecnologia</span><strong>{preparedResearch.label}</strong></div>
            <div className="figma-data-row"><span>Categoria</span><strong>{preparedResearch.categoryLabel}</strong></div>
            <div className="figma-data-row"><span>Nivel objetivo</span><strong>{preparedResearch.nextLevel}</strong></div>
            <div className="figma-data-row"><span>Coste</span><strong>{preparedResearch.estimatedCostLabel}</strong></div>
            <div className="figma-data-row"><span>Duracion</span><strong>{preparedResearch.estimatedDurationLabel}</strong></div>
            <div className="figma-data-row"><span>Requisitos</span><strong>{preparedResearch.availability.reasonKey === "Ready" ? "Listos para envio" : preparedResearch.availability.reasonLabel}</strong></div>
          </div>
          <p className="figma-panel-note">
            {preparedResearch.availability.reasonKey === "Ready"
              ? "La investigacion esta lista para entrar en cola cuando confirmes."
              : `Requisito pendiente: ${preparedResearch.availability.reasonLabel}.`}
          </p>
          <ul className="stack-list compact-list">
            <li>El coste indicado se descuenta cuando la orden queda aceptada.</li>
            <li>La cola mostrara el progreso en la siguiente lectura disponible.</li>
            <li>Esta confirmacion no completa la investigacion de forma instantanea.</li>
          </ul>
          <label className="confirmation-checkbox">
            <input
              type="checkbox"
              checked={hasEnqueueAcknowledgement}
              onChange={(event) => setHasEnqueueAcknowledgement(event.target.checked)}
            />
            <span>Confirmo que quiero iniciar esta investigacion</span>
          </label>
        </GameModal>
      ) : null}
    </section>
  );
}

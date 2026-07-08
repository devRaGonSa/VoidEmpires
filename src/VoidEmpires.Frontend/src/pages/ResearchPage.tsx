import { useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { enqueueResearchOrder, fetchResearchUiState } from "../api/researchApi";
import type { EnqueueResearchOrderFailureResponse, ResearchApiErrorCode } from "../api/researchTypes";
import { CockpitHero } from "../components/CockpitHero";
import { DevDiagnosticsPanel } from "../components/DevDiagnosticsPanel";
import { GameModal } from "../components/GameModal";
import { ResearchCatalogCard } from "../components/ResearchCatalogCard";
import type { ResearchTechnology, ResearchUiState } from "../utils/researchPresentation";
import {
  formatResearchCommandFailure,
  formatResearchRequestFailure,
  getResearchVisualState,
  mapResearchUiStateToViewModel,
} from "../utils/researchPresentation";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { isOperatorMode } from "../utils/playableSession";
import { buildPlanetUrl, buildResearchUrl, isSuspiciousCabinContext } from "../utils/routeUrls";

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

function getResearchProductOrder(technology: ResearchTechnology) {
  const key = `${technology.researchType}`.toLowerCase();
  const category = `${technology.categoryKey}`.toLowerCase();

  if (key.includes("energy") || key.includes("resource") || category.includes("energy") || category.includes("econom")) {
    return 10;
  }

  if (key.includes("constructionautomation") || category.includes("administr")) {
    return 20;
  }

  if (key.includes("propulsion") || category.includes("log")) {
    return 30;
  }

  if (key.includes("weapon") || key.includes("shield") || category.includes("militar") || category.includes("defense")) {
    return 40;
  }

  if (key.includes("espionage") || category.includes("explor")) {
    return 50;
  }

  if (key.includes("planetary") || category.includes("colon")) {
    return 60;
  }

  return 70;
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
  const operatorMode = isOperatorMode(searchParams);
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const catalogItems = useMemo(
    () => [...(uiState?.catalog ?? [])].sort((left, right) => getResearchProductOrder(left) - getResearchProductOrder(right)),
    [uiState?.catalog],
  );
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
      const nextRoute = buildResearchUrl(civilizationId, nextState.selectedPlanetId);
      const nextParams = new URLSearchParams(nextRoute.split("?")[1] ?? "");
      setSearchParams(nextParams, { replace: replaceParams });
    }

    return nextState;
  }

  useEffect(() => {
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
        title="Laboratorio"
        description="Catalogo tecnologico compacto y cola activa cuando hay una investigacion en curso."
        badges={
          <>
            <UiBadge tone="good">Tecnologias disponibles</UiBadge>
            <UiBadge>Cola de investigacion</UiBadge>
            <UiBadge tone="warn">Confirmacion obligatoria</UiBadge>
          </>
        }
      />

      {error ? <p className="error-text">{error}</p> : null}

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>La civilizacion activa no parece valida para esta vista.</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el id del planeta como civilizacion.</p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={buildPlanetUrl(activeCivilizationId, selectedPlanetId)}>Abrir Planeta</Link>
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
          {uiState.queue.length > 0 ? (
          <UiCard className="panel research-queue-panel">
            <div className="figma-section-header">
              <div>
                <h3>Investigacion activa</h3>
              </div>
              <UiBadge tone="warn">{uiState.queue.length} en curso</UiBadge>
            </div>
            <ul className="stack-list compact-list">
              {uiState.queue.map((item) => (
                <li key={item.orderId}>
                  {item.label} nivel {item.targetLevel} | {item.isDue ? "Lista para cierre" : item.statusLabel} | cierre {formatDateTime(item.endsAtUtc)}
                </li>
              ))}
            </ul>
          </UiCard>
          ) : null}

          <UiCard className="panel research-catalog-panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Laboratorio</p>
                <h3>Tecnologias disponibles y bloqueadas</h3>
              </div>
              <UiBadge tone="good">{catalogItems.length} tecnologias</UiBadge>
            </div>
            <div className="planet-building-grid research-tech-grid">
                    {catalogItems.map((technology) => {
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
              : `Requisito no cumplido: ${preparedResearch.availability.reasonLabel}.`}
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

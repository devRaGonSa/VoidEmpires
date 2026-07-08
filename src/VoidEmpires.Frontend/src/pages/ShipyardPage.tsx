import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import { enqueueShipyardProduction, fetchShipyardUiState } from "../api/shipyardApi";
import type { ShipyardApiErrorCode } from "../api/shipyardTypes";
import { CockpitHero } from "../components/CockpitHero";
import { DevDiagnosticsPanel } from "../components/DevDiagnosticsPanel";
import { GameModal } from "../components/GameModal";
import { PlaceholderAsset } from "../components/PlaceholderAsset";
import { ShipyardCatalogCard } from "../components/ShipyardCatalogCard";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import {
  groupAssetOptionsByCategory,
  mapShipyardUiStateToViewModel,
  selectRecommendedAssetProduction,
  type ShipyardAssetOption,
  type ShipyardViewModel,
} from "../utils/shipyardViewModel";
import { isSuspiciousCabinContext } from "../utils/routeUrls";
import { cockpitStatusLabels } from "../utils/cockpitStatus";
import { isOperatorMode } from "../utils/playableSession";
import { formatResourceDelta } from "../utils/resourceDisplay";

function formatDateTime(value: string) {
  const parsed = Date.parse(value);
  return Number.isNaN(parsed)
    ? "No disponible"
    : new Intl.DateTimeFormat("es-ES", { dateStyle: "short", timeStyle: "short" }).format(parsed);
}

function formatCountLabel(count: number, singular: string, plural: string) {
  return `${count} ${count === 1 ? singular : plural}`;
}

function getCatalogBucket(asset: ShipyardAssetOption) {
  if (asset.statusKey === "Available") {
    return "available";
  }

  if (asset.reasonKey === "CatalogUnavailable") {
    return "unsupported";
  }

  return "blocked";
}

function formatRequirementLabel(asset: ShipyardAssetOption) {
  return `Astillero ${asset.requirements.buildingLevel}+ | tripulacion ${asset.requirements.operatorCapacity}`;
}

interface ShipyardReviewSelection {
  asset: ShipyardAssetOption;
  bucket: "available" | "blocked" | "unsupported";
}

interface ShipyardErrorPresentation {
  primaryMessage: string;
  followUp: string | null;
  technicalDetail: string | null;
}

interface ShipyardFailureContext {
  code?: ShipyardApiErrorCode | null;
  detail: string | null;
  httpStatus?: number | null;
  bodyParseFailed?: boolean;
}

interface ShipyardRefreshAudit {
  queueBefore: number;
  queueAfter: number;
  visibleOrderId: string | null;
  visibleOrderLabel: string | null;
  visibleOrderWindow: string | null;
  resourceDelta: string[];
  stockDigest: string;
}

function buildShipyardStockDigest(shipyard: ShipyardViewModel["shipyard"]) {
  if (!shipyard || shipyard.orbitalStock.length === 0) {
    return "Sin reservas orbitales registradas";
  }

  const totalUnits = shipyard.orbitalStock.reduce((sum, entry) => sum + entry.quantity, 0);
  const leadStock = [...shipyard.orbitalStock]
    .sort((left, right) => right.quantity - left.quantity)
    .slice(0, 2)
    .map((entry) => `${entry.label} ${entry.quantityLabel}`);

  return `${formatCountLabel(totalUnits, "unidad local", "unidades locales")} en ${formatCountLabel(shipyard.orbitalStock.length, "tipo", "tipos")}${leadStock.length > 0 ? ` · ${leadStock.join(" · ")}` : ""}`;
}

function buildShipyardRefreshAudit(
  beforeShipyard: ShipyardViewModel["shipyard"],
  afterShipyard: ShipyardViewModel["shipyard"],
  orderId: string | null,
) {
  const beforeStockpile = beforeShipyard?.stockpile ?? [];
  const afterStockpile = afterShipyard?.stockpile ?? [];
  const resourceTypes = Array.from(new Set([
    ...beforeStockpile.map((item) => item.resourceType),
    ...afterStockpile.map((item) => item.resourceType),
  ]));

  const resourceDelta = resourceTypes
    .map((resourceType) => {
      const before = beforeStockpile.find((item) => item.resourceType === resourceType)?.quantity ?? 0;
      const after = afterStockpile.find((item) => item.resourceType === resourceType)?.quantity ?? 0;
      const delta = after - before;

      if (delta === 0) {
        return null;
      }

      return formatResourceDelta({ resourceType, quantity: delta });
    })
    .filter((item): item is string => item !== null);

  const visibleOrder = orderId
    ? afterShipyard?.queue.find((item) => item.orderId === orderId) ?? null
    : null;

  return {
    queueBefore: beforeShipyard?.queue.length ?? 0,
    queueAfter: afterShipyard?.queue.length ?? 0,
    visibleOrderId: visibleOrder?.orderId ?? null,
    visibleOrderLabel: visibleOrder ? `${visibleOrder.label} ${visibleOrder.quantityLabel}` : null,
    visibleOrderWindow: visibleOrder
      ? `${formatDateTime(visibleOrder.startsAtUtc)} -> ${formatDateTime(visibleOrder.endsAtUtc)}`
      : null,
    resourceDelta,
    stockDigest: buildShipyardStockDigest(afterShipyard),
  } satisfies ShipyardRefreshAudit;
}

function formatShipyardCommandFailure(failureContext: ShipyardFailureContext, planetName?: string | null) {
  const contextPlanetName = planetName ? `Usa el contexto de ${planetName}.` : "Revisa el contexto de planeta cargado.";
  const detail = failureContext.detail;

  switch (failureContext.code ?? detail) {
    case "MissingCivilizationId":
    case "Civilization id is required.":
      return {
        primaryMessage: "La civilizacion es obligatoria para enviar produccion.",
        followUp: "Carga una civilizacion valida antes de abrir esta vista.",
        technicalDetail: detail,
      };
    case "MissingPlanetId":
    case "Planet id is required.":
      return {
        primaryMessage: "El planeta es obligatorio para enviar produccion.",
        followUp: contextPlanetName,
        technicalDetail: detail,
      };
    case "Planet was not found.":
      return {
        primaryMessage: "El planeta solicitado no existe en la lectura actual.",
        followUp: contextPlanetName,
        technicalDetail: detail,
      };
    case "PlanetNotOwned":
    case "Planet is not owned by the requesting civilization.":
      return {
        primaryMessage: "El planeta seleccionado no pertenece a la civilizacion cargada.",
        followUp: contextPlanetName,
        technicalDetail: detail,
      };
    case "MissingAssetType":
    case "InvalidAssetType":
    case "Asset type is required.":
    case "Space asset type is invalid.":
      return {
        primaryMessage: "El activo orbital seleccionado no es valido para esta vista.",
        followUp: "Vuelve a abrir la revision desde una carta orbital visible.",
        technicalDetail: detail,
      };
    case "InsufficientResources":
    case "Insufficient resources.":
      return {
        primaryMessage: `No hay recursos suficientes${planetName ? ` en ${planetName}` : ""} para enviar esta produccion.`,
        followUp: "Revisa recursos.",
        technicalDetail: detail,
      };
    case "OpenProductionOrderExists":
    case "Planet already has an open asset production order.":
      return {
        primaryMessage: "Ya existe una orden orbital abierta en este planeta.",
        followUp: "Espera a que la cola se libere antes de enviar otra produccion.",
        technicalDetail: detail,
      };
    case "MissingRequiredBuilding":
    case "Required building is missing or below required level.":
      return {
        primaryMessage: "La infraestructura orbital requerida todavia no esta lista.",
        followUp: "Construye o mejora Astillero.",
        technicalDetail: detail,
      };
    case "PopulationProfileMissing":
    case "Planet population profile was not found.":
      return {
        primaryMessage: "Falta el perfil de tripulacion necesario para producir este activo.",
        followUp: "Completa la preparacion local antes de intentar producir.",
        technicalDetail: detail,
      };
    case "InsufficientOperatorCapacity":
    case "Insufficient local operator capacity.":
      return {
        primaryMessage: "La capacidad local de tripulacion no alcanza para este activo.",
        followUp: "Mejora la capacidad orbital antes de reintentar.",
        technicalDetail: detail,
      };
    case "PlanetStockpileMissing":
    case "Planet resource stockpile was not found.":
      return {
        primaryMessage: "El planeta no expone reservas locales utilizables para esta accion.",
        followUp: "Esta accion no esta disponible para este contexto.",
        technicalDetail: detail,
      };
    case "MissingQuantity":
    case "Quantity must be positive.":
      return {
        primaryMessage: "La cantidad solicitada no es valida para esta produccion.",
        followUp: "Vuelve a abrir la revision desde una carta orbital visible.",
        technicalDetail: detail,
      };
    case "RequestedAtUtcNotUtc":
    case "Requested date must be UTC.":
      return {
        primaryMessage: "La vista no pudo preparar una fecha valida para esta orden.",
        followUp: "Reintenta la accion.",
        technicalDetail: detail,
      };
    case "Shipyard UI state refresh failed after a successful enqueue.":
      return {
        primaryMessage: "La orden se envio, pero la vista no pudo recargar el estado actualizado.",
        followUp: "Abre de nuevo el astillero para confirmar cola, stock y bloqueos.",
        technicalDetail: detail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "No se pudo completar la accion de astillero.",
        followUp: "Esta accion no esta disponible en este entorno.",
        technicalDetail: detail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "No se pudo cargar el estado del astillero ahora mismo.",
        followUp: "Reintenta en unos momentos antes de enviar otra orden.",
        technicalDetail: detail,
      };
    default:
      if (failureContext.bodyParseFailed) {
        return {
          primaryMessage: "No se pudo interpretar la respuesta del astillero.",
          followUp: "Reintenta la accion y consulta el panel de operador si persiste.",
          technicalDetail: detail ?? `Request failed with status ${failureContext.httpStatus ?? "desconocido"}.`,
        };
      }

      return {
        primaryMessage: "La produccion orbital no pudo enviarse.",
        followUp: "Consulta diagnosticos si el problema persiste.",
        technicalDetail: detail ?? (failureContext.httpStatus ? `Request failed with status ${failureContext.httpStatus}.` : null),
      };
  }
}

export function ShipyardPage() {
  const [searchParams, setSearchParams] = useSearchParams();
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [errorFollowUp, setErrorFollowUp] = useState<string | null>(null);
  const [technicalErrorDetail, setTechnicalErrorDetail] = useState<string | null>(null);
  const [uiState, setUiState] = useState<ShipyardViewModel | null>(null);
  const [reviewSelection, setReviewSelection] = useState<ShipyardReviewSelection | null>(null);
  const [hasEnqueueAcknowledgement, setHasEnqueueAcknowledgement] = useState(false);
  const [isSubmittingEnqueue, setIsSubmittingEnqueue] = useState(false);
  const [enqueueFeedback, setEnqueueFeedback] = useState<string | null>(null);
  const [enqueueError, setEnqueueError] = useState<string | null>(null);
  const [enqueueErrorFollowUp, setEnqueueErrorFollowUp] = useState<string | null>(null);
  const [enqueueOrderDetails, setEnqueueOrderDetails] = useState<{
    orderId: string | null;
    startsAtUtc: string | null;
    endsAtUtc: string | null;
  } | null>(null);
  const [enqueueRefreshAudit, setEnqueueRefreshAudit] = useState<ShipyardRefreshAudit | null>(null);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const operatorMode = isOperatorMode(searchParams);
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const shipyard = uiState?.shipyard ?? null;
  const hasSafeShipyardEnqueue = shipyard?.actionAvailability.enqueue.supported ?? false;
  const categoryGroups = useMemo(() => groupAssetOptionsByCategory(shipyard?.catalog ?? []), [shipyard?.catalog]);
  const recommendedAsset = useMemo(() => selectRecommendedAssetProduction(shipyard?.catalog ?? []), [shipyard?.catalog]);
  const catalogBuckets = useMemo(() => {
    const assets = shipyard?.catalog ?? [];
    return {
      available: assets.filter((asset) => getCatalogBucket(asset) === "available"),
      blocked: assets.filter((asset) => getCatalogBucket(asset) === "blocked"),
      unsupported: assets.filter((asset) => getCatalogBucket(asset) === "unsupported"),
    };
  }, [shipyard?.catalog]);
  const dueQueueCount = useMemo(() => shipyard?.queue.filter((item) => item.isDue).length ?? 0, [shipyard?.queue]);
  const readinessNotes = useMemo(() => {
    if (!shipyard) {
      return [];
    }

    const notes = new Set<string>();
    if (!shipyard.isOwnedByRequestingCivilization) {
      notes.add("El planeta no esta bajo control de la civilizacion cargada.");
    }

    if (!shipyard.buildingReadiness.hasPopulationProfile) {
      notes.add("Falta perfil de tripulacion para sostener produccion orbital.");
    }

    if (!shipyard.actionAvailability.enqueue.supported) {
      notes.add(shipyard.actionAvailability.enqueue.reasonLabel);
    }

    if (shipyard.actionAvailability.completeDue.supported) {
      notes.add("Hay produccion vencida pendiente de completar.");
    }

    return [...notes];
  }, [shipyard]);
  async function reloadShipyardState(
    civilizationId: string,
    planetId?: string | null,
    replaceParams = false,
    preserveCurrentStateOnFailure = false,
  ) {
    const response = await fetchShipyardUiState(civilizationId, planetId);
    if (!response.succeeded || !response.uiState) {
      const failure = formatShipyardCommandFailure({ detail: response.errors[0] ?? null }, shipyard?.planetName ?? null);
      if (!preserveCurrentStateOnFailure) {
        setUiState(null);
      }
      setError(failure.primaryMessage);
      setErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
      return null;
    }

    const nextState = mapShipyardUiStateToViewModel(response.uiState);
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
    async function load() {
      if (!queryCivilizationId) {
        setUiState(null);
        setError(null);
        setErrorFollowUp(null);
        setTechnicalErrorDetail(null);
        setEnqueueRefreshAudit(null);
        return;
      }

      setIsLoading(true);
      setError(null);
      setErrorFollowUp(null);

      try {
        await reloadShipyardState(queryCivilizationId, queryPlanetId, true);
      } catch (requestError) {
        const failure = formatShipyardCommandFailure({ detail: requestError instanceof Error ? requestError.message : null }, shipyard?.planetName ?? null);
        setUiState(null);
        setError(`No se pudo cargar Astillero. ${failure.primaryMessage}`);
        setErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
      } finally {
        setIsLoading(false);
      }
    }

    void load();
  }, [queryCivilizationId, queryPlanetId, searchParams, setSearchParams]);

  function handleReviewAsset(asset: ShipyardAssetOption) {
    setHasEnqueueAcknowledgement(false);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setEnqueueErrorFollowUp(null);
    setEnqueueOrderDetails(null);
    setEnqueueRefreshAudit(null);
    setTechnicalErrorDetail(null);
    setReviewSelection({
      asset,
      bucket: getCatalogBucket(asset),
    });
  }

  function handleCancelReview() {
    setReviewSelection(null);
    setHasEnqueueAcknowledgement(false);
    setEnqueueError(null);
    setEnqueueErrorFollowUp(null);
    setEnqueueOrderDetails(null);
    setEnqueueRefreshAudit(null);
  }

  async function handleConfirmProduction() {
    if (
      !reviewSelection ||
      reviewSelection.bucket !== "available" ||
      !shipyard ||
      !hasSafeShipyardEnqueue ||
      isSubmittingEnqueue
    ) {
      return;
    }

    setIsSubmittingEnqueue(true);
    setEnqueueFeedback(null);
    setEnqueueError(null);
    setEnqueueErrorFollowUp(null);
    setEnqueueOrderDetails(null);
    setEnqueueRefreshAudit(null);
    setTechnicalErrorDetail(null);

    try {
      const beforeShipyard = uiState?.shipyard ?? null;
      const result = await enqueueShipyardProduction({
        civilizationId: reviewSelection.asset.enqueueCommand?.civilizationId ?? activeCivilizationId,
        planetId: reviewSelection.asset.enqueueCommand?.planetId ?? shipyard.planetId,
        route: reviewSelection.asset.enqueueCommand?.route,
        target: reviewSelection.asset.enqueueCommand?.target,
        assetType: reviewSelection.asset.enqueueCommand?.assetType ?? reviewSelection.asset.assetType,
        quantity: reviewSelection.asset.enqueueCommand?.quantity ?? 1,
        requestedAtUtc: new Date().toISOString(),
      });

      if (result.httpStatus !== 201 || !result.response?.succeeded) {
        const primaryError = result.response?.succeeded === false ? result.response.errorEntries[0] : null;
        const failure = formatShipyardCommandFailure({
          code: primaryError?.code ?? null,
          detail: primaryError?.message ?? result.response?.errors[0] ?? null,
          httpStatus: result.httpStatus,
          bodyParseFailed: result.bodyParseFailed,
        }, shipyard.planetName);
        setEnqueueError(failure.primaryMessage);
        setEnqueueErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(primaryError?.rawMessage ?? failure.technicalDetail);
        return;
      }

      setEnqueueOrderDetails({
        orderId: result.response.orderId,
        startsAtUtc: result.response.startsAtUtc,
        endsAtUtc: result.response.endsAtUtc,
      });
      setReviewSelection(null);
      setHasEnqueueAcknowledgement(false);

      const refreshed = await reloadShipyardState(activeCivilizationId, shipyard.planetId, false, true);
      if (!refreshed) {
        const failure = formatShipyardCommandFailure({ detail: "Shipyard UI state refresh failed after a successful enqueue." }, shipyard.planetName);
        setEnqueueError(failure.primaryMessage);
        setEnqueueErrorFollowUp(failure.followUp);
        setTechnicalErrorDetail(failure.technicalDetail);
        return;
      }

      const audit = buildShipyardRefreshAudit(beforeShipyard, refreshed.shipyard, result.response.orderId);
      setEnqueueRefreshAudit(audit);

      if (!audit.visibleOrderId) {
        setEnqueueFeedback("La orden fue aceptada; la cola visible se actualizara con la siguiente lectura disponible.");
        setTechnicalErrorDetail(
          result.response.orderId
            ? `Order accepted with id ${result.response.orderId}, but the refreshed queue does not expose it yet.`
            : "Order accepted, but refreshed queue visibility is still pending.",
        );
        return;
      }

      setEnqueueFeedback("Produccion enviada a la cola con lectura actualizada.");
    } catch (requestError) {
      const failure = formatShipyardCommandFailure({ detail: requestError instanceof Error ? requestError.message : null }, shipyard.planetName);
      setEnqueueError(failure.primaryMessage);
      setEnqueueErrorFollowUp(failure.followUp);
      setTechnicalErrorDetail(failure.technicalDetail);
    } finally {
      setIsSubmittingEnqueue(false);
    }
  }

  return (
    <section className="page-grid">
      <CockpitHero
        versionLabel="Astillero v1"
        title="Astillero orbital"
        description="Naves disponibles, cola de produccion y stock orbital con confirmacion antes de gastar recursos. El stock orbital no equivale automaticamente a una escuadra visible en Flotas."
        developmentNote="Produccion orbital con confirmacion obligatoria; preparar escuadras queda fuera de esta vista."
        badges={
          <>
            <UiBadge tone="good">Naves disponibles</UiBadge>
            <UiBadge>Cola de produccion</UiBadge>
            <UiBadge tone="warn">Sin misiones de flota</UiBadge>
          </>
        }
      />

      {error ? (
        <UiCard className="panel">
          <p className="error-text">{error}</p>
          {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
        </UiCard>
      ) : null}

      {isSuspiciousContext ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Contexto sospechoso</p>
              <h3>El contexto de civilizacion no parece valido para esta vista.</h3>
            </div>
            <UiBadge tone="warn">{cockpitStatusLabels.reviewContext}</UiBadge>
          </div>
          <p className="figma-panel-note">Revisa que no hayas usado el contexto de planeta como civilizacion.</p>
        </UiCard>
      ) : null}

      {shipyard ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Catalogo de produccion</p>
                <h3>Opciones orbitales y disponibilidad</h3>
                <p>El catalogo separa lo producible de los bloqueos visibles sin simular acciones de cola todavia.</p>
              </div>
                  <UiBadge tone="warn">{cockpitStatusLabels.readOnly}</UiBadge>
            </div>
            {shipyard.catalog.length === 0 ? (
              <p className="figma-panel-note">El catalogo orbital aun no tiene opciones utiles para este contexto. Astillero mantiene el acceso y deja visible la frontera del modulo.</p>
            ) : (
              <>
                {categoryGroups.map((group) => (
                  <section key={group.key} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Categoria orbital</p>
                        <h4>{group.label}</h4>
                        <p>{formatCountLabel(group.assets.length, "opcion visible", "opciones visibles")}</p>
                      </div>
                      <UiBadge>{group.assets.length} activos</UiBadge>
                    </div>
                    <div className="readiness-grid shipyard-catalog-grid">
                      {group.assets.map((asset) => {
                        const isRecommended = recommendedAsset?.assetType === asset.assetType && asset.statusKey === "Available";
                        const bucket = getCatalogBucket(asset);

                        return (
                          <ShipyardCatalogCard
                            key={asset.assetType}
                            asset={asset}
                            bucket={bucket}
                            isRecommended={isRecommended}
                            onReview={handleReviewAsset}
                          />
                        );
                      })}
                    </div>
                  </section>
                ))}

              </>
            )}
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Produccion orbital</p>
                <h3>Produccion orbital</h3>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                  {shipyard.queue.length > 0 ? `${shipyard.queue.length} visibles` : "0 visibles"}
                </UiBadge>
                <UiBadge tone={dueQueueCount > 0 ? "warn" : "neutral"}>
                  {dueQueueCount > 0 ? `${dueQueueCount} vencidas` : "Sin vencidas"}
                </UiBadge>
              </div>
            </div>
            {shipyard.queue.length === 0 ? (
              <p className="figma-panel-note">Sin produccion orbital</p>
            ) : (
              <div className="readiness-grid">
                {shipyard.queue.map((item) => (
                  <section key={item.orderId} className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Orden {item.sequence}</p>
                        <h4>{item.label}</h4>
                      </div>
                      <div className="figma-badge-row">
                        <UiBadge tone={item.isDue ? "warn" : "neutral"}>{item.statusLabel}</UiBadge>
                        {item.isDue ? <UiBadge tone="warn">Lista para cierre futuro</UiBadge> : null}
                      </div>
                    </div>
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>Salida</span><strong>{item.quantityLabel}</strong></div>
                      <div className="figma-data-row"><span>Inicio visible</span><strong>{formatDateTime(item.startsAtUtc)}</strong></div>
                      <div className="figma-data-row"><span>Fin visible</span><strong>{formatDateTime(item.endsAtUtc)}</strong></div>
                    </div>
                    <p>{item.isDue ? "La orden ya vencio en la lectura actual. La vista la mantiene visible sin completarla automaticamente." : "La orden sigue en progreso o pendiente dentro de la ventana temporal visible."}</p>
                  </section>
                ))}
              </div>
            )}
          </UiCard>

          {operatorMode && (shipyard.diagnostics.playerFacing.length > 0 || technicalErrorDetail || enqueueOrderDetails) ? (
            <DevDiagnosticsPanel
              title="Diagnostico de astillero"
              summaryItems={[
                { label: "Civilizacion", value: activeCivilizationId },
                { label: "Planeta", value: shipyard.planetId },
                { label: "Cola visible", value: shipyard.queue.length },
                { label: "Ordenes vencidas", value: dueQueueCount },
                { label: "Opciones disponibles", value: catalogBuckets.available.length },
                { label: "Stock orbital", value: shipyard.orbitalStock.length },
              ]}
              notes={[
                ...shipyard.diagnostics.playerFacing,
                ...readinessNotes,
                ...(technicalErrorDetail ? [technicalErrorDetail] : []),
              ]}
              rawPayload={{
                diagnostics: shipyard.diagnostics,
                enqueueOrderDetails,
                enqueueRefreshAudit,
              }}
            />
          ) : null}

          {operatorMode && (shipyard.diagnostics.playerFacing.length > 0 || technicalErrorDetail || enqueueOrderDetails) ? (
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
                    <h3>Notas del servicio</h3>
                  </div>
                  <UiBadge tone="warn">{cockpitStatusLabels.developmentOnly}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  {shipyard.diagnostics.playerFacing.map((line) => (
                    <li key={line}>{line}</li>
                  ))}
                  {technicalErrorDetail ? <li>{technicalErrorDetail}</li> : null}
                </ul>
                {enqueueOrderDetails ? (
                  <div className="figma-data-list">
                    <div className="figma-data-row"><span>Orden</span><strong>{enqueueOrderDetails.orderId ?? "No devuelto"}</strong></div>
                    <div className="figma-data-row"><span>Inicio</span><strong>{enqueueOrderDetails.startsAtUtc ? formatDateTime(enqueueOrderDetails.startsAtUtc) : "No devuelto"}</strong></div>
                    <div className="figma-data-row"><span>Cierre</span><strong>{enqueueOrderDetails.endsAtUtc ? formatDateTime(enqueueOrderDetails.endsAtUtc) : "No devuelto"}</strong></div>
                  </div>
                ) : null}
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
                <h3>Astillero pendiente de seleccion</h3>
              </div>
              <UiBadge tone="warn">Pendiente</UiBadge>
            </div>
            <p className="figma-panel-note">No hay un contexto de astillero util para este planeta. La vista sigue visible para mantener el acceso y el contexto de navegacion.</p>
          </UiCard>
        ) : null
      )}

      {enqueueFeedback ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Confirmacion registrada</p>
              <h3>Produccion enviada</h3>
              <p>La vista recargo cola, catalogo y reservas con el estado confirmado por la lectura actual.</p>
            </div>
            <UiBadge tone="good">Actualizada</UiBadge>
          </div>
          <p>{enqueueFeedback}</p>
          {enqueueRefreshAudit ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Cola visible</span><strong>{`${enqueueRefreshAudit.queueBefore} -> ${enqueueRefreshAudit.queueAfter}`}</strong></div>
              <div className="figma-data-row"><span>Orden refrescada</span><strong>{enqueueRefreshAudit.visibleOrderLabel ?? "Pendiente de aparecer en la lectura actual"}</strong></div>
              <div className="figma-data-row"><span>Ventana visible</span><strong>{enqueueRefreshAudit.visibleOrderWindow ?? "Pendiente de nueva lectura"}</strong></div>
              <div className="figma-data-row"><span>Stock orbital visible</span><strong>{enqueueRefreshAudit.stockDigest}</strong></div>
              <div className="figma-data-row"><span>Delta de recursos</span><strong>{enqueueRefreshAudit.resourceDelta.length > 0 ? enqueueRefreshAudit.resourceDelta.join(" · ") : "Sin cambios visibles"}</strong></div>
            </div>
          ) : null}
        </UiCard>
      ) : null}
      {enqueueError ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Validacion de produccion</p>
              <h3>La accion no pudo completarse</h3>
            </div>
            <UiBadge tone="warn">Sin cambios locales</UiBadge>
          </div>
          <p className="error-text">{enqueueError}</p>
          {enqueueErrorFollowUp ? <p>{enqueueErrorFollowUp}</p> : null}
        </UiCard>
      ) : null}

      {reviewSelection && shipyard ? (
        <GameModal
          actionScope="gameplay"
          canClose={!isSubmittingEnqueue}
          closeLabel="Cerrar"
          description="Revisa nave, coste y duracion antes de enviar la produccion orbital a la cola."
          isBusy={isSubmittingEnqueue}
          isOpen
          onClose={handleCancelReview}
          primaryAction={{
            label: reviewSelection.bucket === "available" ? "Enviar produccion" : "No disponible",
            onClick: () => void handleConfirmProduction(),
            disabled: reviewSelection.bucket !== "available" || !hasEnqueueAcknowledgement,
          }}
          secondaryAction={{
            label: "Cancelar",
            onClick: handleCancelReview,
            disabled: isSubmittingEnqueue,
          }}
          title="Enviar produccion orbital"
        >
          <UiBadge tone={reviewSelection.bucket === "available" ? "good" : "warn"}>
            {reviewSelection.bucket === "available" ? "Lista para cola" : "No enviable"}
          </UiBadge>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <PlaceholderAsset
                kind="ship"
                label={reviewSelection.asset.label}
                typeLabel={reviewSelection.asset.roleLabel}
                detail={reviewSelection.asset.description}
                imageKey={reviewSelection.asset.imageKey}
              />
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
                <div className="figma-data-row"><span>Nave</span><strong>{reviewSelection.asset.label}</strong></div>
                <div className="figma-data-row"><span>Rol</span><strong>{reviewSelection.asset.roleLabel}</strong></div>
                <div className="figma-data-row"><span>Clase</span><strong>{reviewSelection.asset.categoryLabel}</strong></div>
                <div className="figma-data-row"><span>Stock orbital actual</span><strong>{reviewSelection.asset.quantityLabel}</strong></div>
                <div className="figma-data-row"><span>Duracion</span><strong>{reviewSelection.asset.estimatedDurationLabel}</strong></div>
              </div>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Coste</span><strong>{reviewSelection.asset.estimatedCostLabel}</strong></div>
                <div className="figma-data-row"><span>Estado</span><strong>{reviewSelection.asset.statusLabel}</strong></div>
                <div className="figma-data-row"><span>Requisitos</span><strong>{formatRequirementLabel(reviewSelection.asset)}</strong></div>
                <div className="figma-data-row"><span>Motivo visible</span><strong>{reviewSelection.asset.reasonLabel}</strong></div>
              </div>
            </section>
          </div>
          <p>
            {reviewSelection.bucket === "available"
              ? "La produccion entrara en la cola orbital cuando confirmes. Si la orden queda aceptada, el coste visible se descuenta al momento."
              : "Esta opcion no puede enviarse todavia. Revisa requisitos y estado antes de volver a intentarlo."}
          </p>
          {reviewSelection.bucket === "available" ? (
            <label className="confirmation-checkbox">
              <input
                type="checkbox"
                checked={hasEnqueueAcknowledgement}
                onChange={(event) => setHasEnqueueAcknowledgement(event.target.checked)}
              />
              <span>Confirmo que quiero enviar esta produccion orbital a la cola</span>
            </label>
          ) : null}
        </GameModal>
      ) : null}
    </section>
  );
}

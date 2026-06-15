import { FormEvent, useEffect, useMemo, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import { enqueueShipyardProduction, fetchShipyardUiState } from "../api/shipyardApi";
import type { ShipyardApiErrorCode } from "../api/shipyardTypes";
import { CockpitHero } from "../components/CockpitHero";
import { DevDiagnosticsPanel } from "../components/DevDiagnosticsPanel";
import { GameModal } from "../components/GameModal";
import { PlayableSessionBanner } from "../components/PlayableSessionBanner";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatPlanetPrimaryLabel, formatPlanetSecondaryLabel } from "../utils/domainPresentation";
import {
  getShipyardPrimaryAction,
  groupAssetOptionsByCategory,
  mapShipyardUiStateToViewModel,
  selectRecommendedAssetProduction,
  type ShipyardAssetOption,
  type ShipyardViewModel,
} from "../utils/shipyardViewModel";
import {
  buildConstructionUrl,
  buildFleetsUrl,
  buildGalaxyUrl,
  buildPlanetUrl,
  buildResearchUrl,
  buildShipyardUrl,
  isSuspiciousCabinContext,
} from "../utils/routeUrls";
import { cockpitNavigationLabels, cockpitStatusLabels } from "../utils/cockpitStatus";
import { formatResourceAmountList, formatResourceDelta, formatResourceLabel } from "../utils/resourceDisplay";
import { usePlayableRouteContext } from "../utils/usePlayableRouteContext";

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
        followUp: "Carga una civilizacion valida antes de abrir esta cabina.",
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
        primaryMessage: "El planeta solicitado no existe en esta build.",
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
        primaryMessage: "El activo orbital seleccionado no es valido para esta cabina.",
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
        followUp: "Esta accion no esta disponible en esta build.",
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
        primaryMessage: "La cabina no pudo preparar una fecha valida para esta orden.",
        followUp: "Reintenta la accion.",
        technicalDetail: detail,
      };
    case "Shipyard UI state refresh failed after a successful enqueue.":
      return {
        primaryMessage: "La orden se envio, pero la cabina no pudo recargar el estado actualizado.",
        followUp: "Abre de nuevo el astillero para confirmar cola, stock y bloqueos.",
        technicalDetail: detail,
      };
    case "Request failed with status 404.":
      return {
        primaryMessage: "Esta accion no esta disponible en esta build.",
        followUp: "El endpoint de desarrollo no esta expuesto en este entorno.",
        technicalDetail: detail,
      };
    case "Request failed with status 503.":
      return {
        primaryMessage: "La persistencia de desarrollo no esta disponible ahora mismo.",
        followUp: "Comprueba la configuracion local antes de reintentar.",
        technicalDetail: detail,
      };
    default:
      if (failureContext.bodyParseFailed) {
        return {
          primaryMessage: "La cabina recibio una respuesta no interpretable del backend.",
          followUp: "Revisa los diagnosticos secundarios antes de reintentar.",
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
  const [civilizationIdInput, setCivilizationIdInput] = useState(searchParams.get("civilizationId") ?? "");
  const [planetIdInput, setPlanetIdInput] = useState(searchParams.get("planetId") ?? "");
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
  const [localSessionCleared, setLocalSessionCleared] = useState(false);

  const queryCivilizationId = searchParams.get("civilizationId") ?? "";
  const queryPlanetId = searchParams.get("planetId");
  const selectedPlanetId = uiState?.selectedPlanetId ?? queryPlanetId ?? null;
  const activeCivilizationId = uiState?.civilizationId ?? queryCivilizationId;
  const isSuspiciousContext = isSuspiciousCabinContext(queryCivilizationId, queryPlanetId);
  const playableRouteContext = usePlayableRouteContext(queryCivilizationId);
  const playableSession = localSessionCleared ? null : playableRouteContext.playableSession;
  const shipyard = uiState?.shipyard ?? null;
  const routeSession = uiState?.civilizationId && shipyard
    ? {
      civilizationId: uiState.civilizationId,
      planetId: shipyard.planetId,
      civilizationName: shipyard.ownerCivilizationName ?? undefined,
      planetName: shipyard.planetName,
      createdAt: "route-context",
      updatedAt: "route-context",
    }
    : null;
  const bannerSession = routeSession ?? playableSession;
  const playableSessionUrl = playableSession
    ? buildShipyardUrl(playableSession.civilizationId, playableSession.planetId)
    : null;
  const hasSafeShipyardEnqueue = shipyard?.actionAvailability.enqueue.supported ?? false;
  const hasSafeShipyardCompleteDue = false;
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
  const stockDigest = useMemo(() => {
    if (!shipyard || shipyard.orbitalStock.length === 0) {
      return "Sin reservas orbitales registradas";
    }

    const totalUnits = shipyard.orbitalStock.reduce((sum, entry) => sum + entry.quantity, 0);
    const leadStock = [...shipyard.orbitalStock]
      .sort((left, right) => right.quantity - left.quantity)
      .slice(0, 2)
      .map((entry) => `${entry.label} ${entry.quantityLabel}`);

    return `${formatCountLabel(totalUnits, "unidad local", "unidades locales")} en ${formatCountLabel(shipyard.orbitalStock.length, "tipo", "tipos")}${leadStock.length > 0 ? ` · ${leadStock.join(" · ")}` : ""}`;
  }, [shipyard]);
  const resourceDigest = useMemo(() => {
    if (!shipyard || shipyard.stockpile.length === 0) {
      return "El backend todavia no expone reservas locales utiles.";
    }

    return formatResourceAmountList(
      shipyard.stockpile
        .slice()
        .sort((left, right) => right.quantity - left.quantity)
        .slice(0, 3),
      { separator: " · " },
    );
  }, [shipyard]);
  const readinessTone = shipyard?.actionAvailability.enqueue.supported
    ? "good"
    : shipyard?.actionAvailability.completeDue.supported
      ? "warn"
      : "neutral";
  const recommendedActionSummary = !recommendedAsset
    ? "El catalogo orbital aun no ofrece una siguiente produccion clara."
    : recommendedAsset.statusKey === "Available"
      ? `${recommendedAsset.label} puede entrar en cola con ${recommendedAsset.estimatedCostLabel} y ${recommendedAsset.estimatedDurationLabel}.`
      : `${recommendedAsset.label} sigue bloqueada: ${recommendedAsset.reasonLabel}.`;

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
    setCivilizationIdInput(queryCivilizationId);
    setPlanetIdInput(queryPlanetId ?? "");

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
        setError(`No se pudo contactar con el backend del astillero. ${failure.primaryMessage}`);
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
      setError("El id de civilizacion es obligatorio.");
      setErrorFollowUp("Carga una civilizacion valida antes de abrir esta cabina.");
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
        setEnqueueFeedback("La orden fue aceptada por el backend; la cola visible se actualizara con la siguiente lectura disponible.");
        setTechnicalErrorDetail(
          result.response.orderId
            ? `Order accepted with id ${result.response.orderId}, but the refreshed queue does not expose it yet.`
            : "Order accepted by backend, but refreshed queue visibility is still pending.",
        );
        return;
      }

      setEnqueueFeedback("Produccion enviada a la cola con lectura backend actualizada.");
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
        title="Astillero"
        description="La cabina prioriza produccion orbital, capacidad local, cola y stock antes que el detalle tecnico."
        developmentNote="La ruta sigue siendo de desarrollo para QA local: la produccion visible permanece segura y el mando de flotas sigue fuera de esta cabina."
        badges={
          <>
            <UiBadge tone="good">Produccion local</UiBadge>
            <UiBadge>Cola y stock</UiBadge>
            <UiBadge tone="warn">Sin movimiento de flota</UiBadge>
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
              <p className="eyebrow">Entrada de cabina</p>
              <h3>Cargar contexto del astillero</h3>
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
              {isLoading ? "Cargando..." : "Abrir astillero"}
            </button>
          </form>
          {error ? (
            <div className="subpanel figma-subpanel figma-mini-card-warn">
              <p className="error-text">{error}</p>
              {errorFollowUp ? <p className="figma-panel-note">{errorFollowUp}</p> : null}
            </div>
          ) : null}
          {isLoading ? <p className="figma-panel-note">Cargando catalogo, cola, stock y capacidad orbital...</p> : null}
          {!queryCivilizationId && !isLoading ? (
            <p className="figma-panel-note">Introduce un `civilizationId` valido, entra desde Galaxia o usa el inicio local disponible para abrir Astillero con contexto.</p>
          ) : null}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Resumen de planeta</p>
              <h3>Contexto cargado</h3>
            </div>
            <UiBadge>{selectedPlanetId ? formatPlanetSecondaryLabel(selectedPlanetId) ?? formatPlanetPrimaryLabel(selectedPlanetId) : "Sin planeta"}</UiBadge>
          </div>
          {shipyard ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
              <div className="figma-data-row"><span>Sistema</span><strong>{shipyard.solarSystemName}</strong></div>
              <div className="figma-data-row"><span>Control</span><strong>{shipyard.isOwnedByRequestingCivilization ? "Propio" : "Sin control local"}</strong></div>
              <div className="figma-data-row"><span>Accion principal</span><strong>{getShipyardPrimaryAction(recommendedAsset)}</strong></div>
            </div>
          ) : (
            <p className="figma-panel-note">La cabina mostrara el planeta seleccionado cuando el servicio del astillero devuelva un contexto valido; no se inventa stock ni cola sin backend.</p>
          )}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Limite del modulo</p>
              <h3>Que hace esta cabina</h3>
            </div>
            <UiBadge tone="warn">Frontera visible</UiBadge>
          </div>
          <ul className="stack-list strategic-rules-list">
            <li>Astillero prepara o produce activos orbitales.</li>
            <li>Flotas mueve escuadras orbitales ya existentes.</li>
            <li>La cola, el stock y los bloqueos deben mostrarse de forma honesta aunque falten acciones ejecutables.</li>
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
            Este enlace solo reconstruye la URL con ids guardados por `/onboarding`; el backend seguira validando el estado real.
          </p>
          <div className="selection-chip-row">
            <Link className="selection-chip selection-chip-active" to={playableSessionUrl}>
              Abrir Astillero
            </Link>
          </div>
        </UiCard>
      ) : null}

      {shipyard ? (
        <>
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Overview orbital</p>
                <h3>Resumen estrategico del astillero</h3>
                <p>Lectura rapida de capacidad, cola, stock y accion recomendada antes de revisar el catalogo.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={readinessTone}>
                  {shipyard.actionAvailability.enqueue.supported ? "Produccion habilitable" : "Produccion bloqueada"}
                </UiBadge>
                <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                  {shipyard.queue.length > 0 ? `${shipyard.queue.length} en cola` : "Cola vacia"}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Mando actual</p>
                    <h4>Contexto seleccionado</h4>
                  </div>
                  <UiBadge>{formatPlanetSecondaryLabel(shipyard.planetId) ?? "Planeta activo"}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Civilizacion</span><strong>{uiState?.civilizationId ?? activeCivilizationId}</strong></div>
                  <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
                  <div className="figma-data-row"><span>Sistema</span><strong>{shipyard.solarSystemName}</strong></div>
                  <div className="figma-data-row"><span>Control</span><strong>{shipyard.isOwnedByRequestingCivilization ? "Propio" : shipyard.ownerCivilizationName ?? "Sin control local"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Capacidad de produccion</p>
                    <h4>Readiness orbital</h4>
                  </div>
                  <UiBadge tone={readinessTone}>{shipyard.actionAvailability.enqueue.reasonLabel}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Nivel astillero</span><strong>{shipyard.buildingReadiness.shipyardLevel}</strong></div>
                  <div className="figma-data-row"><span>Mando de flota</span><strong>{shipyard.buildingReadiness.fleetCommandCenterLevel}</strong></div>
                  <div className="figma-data-row"><span>Centro logistico</span><strong>{shipyard.buildingReadiness.logisticsHubLevel}</strong></div>
                  <div className="figma-data-row"><span>Catalogo visible</span><strong>{formatCountLabel(shipyard.catalog.length, "activo", "activos")}</strong></div>
                </div>
                {readinessNotes.length > 0 ? (
                  <ul className="stack-list compact-list">
                    {readinessNotes.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                ) : (
                  <p className="figma-panel-note">No hay bloqueos visibles en la lectura actual.</p>
                )}
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas y stock</p>
                    <h4>Estado local</h4>
                  </div>
                  <UiBadge tone="resource">{formatCountLabel(shipyard.stockpile.length, "recurso", "recursos")}</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Recursos clave</span><strong>{resourceDigest}</strong></div>
                  <div className="figma-data-row"><span>Stock orbital</span><strong>{stockDigest}</strong></div>
                  <div className="figma-data-row"><span>Cola activa</span><strong>{shipyard.queue.length > 0 ? formatCountLabel(shipyard.queue.length, "orden abierta", "ordenes abiertas") : "Sin ordenes abiertas"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Siguiente paso</p>
                    <h4>Accion recomendada</h4>
                  </div>
                  <UiBadge tone={recommendedAsset?.statusKey === "Available" ? "good" : "warn"}>
                    {getShipyardPrimaryAction(recommendedAsset)}
                  </UiBadge>
                </div>
                <p>{recommendedActionSummary}</p>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Enqueue</span><strong>{shipyard.actionAvailability.enqueue.reasonLabel}</strong></div>
                  <div className="figma-data-row"><span>Completar vencidas</span><strong>{shipyard.actionAvailability.completeDue.reasonLabel}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado de shell</p>
                <h3>Cockpit listo para ampliar</h3>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={recommendedAsset?.statusKey === "Available" ? "good" : "neutral"}>
                  {recommendedAsset?.statusLabel ?? "Sin recomendacion"}
                </UiBadge>
                <UiBadge tone={shipyard.actionAvailability.completeDue.supported ? "warn" : "neutral"}>
                  {shipyard.actionAvailability.completeDue.label}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Capacidad local</p>
                    <h4>Infraestructura visible</h4>
                  </div>
                  <UiBadge>{shipyard.catalog.length} activos</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Astillero</span><strong>Nivel {shipyard.buildingReadiness.shipyardLevel}</strong></div>
                  <div className="figma-data-row"><span>Mando de flota</span><strong>Nivel {shipyard.buildingReadiness.fleetCommandCenterLevel}</strong></div>
                  <div className="figma-data-row"><span>Centro logistico</span><strong>Nivel {shipyard.buildingReadiness.logisticsHubLevel}</strong></div>
                  <div className="figma-data-row"><span>Tripulacion</span><strong>{shipyard.buildingReadiness.hasPopulationProfile ? "Perfil disponible" : "Sin perfil"}</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Reservas y cola</p>
                    <h4>Lectura actual</h4>
                  </div>
                  <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                    {shipyard.queue.length > 0 ? `${shipyard.queue.length} ordenes` : "Sin cola"}
                  </UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Reservas visibles</span><strong>{shipyard.stockpile.length}</strong></div>
                  <div className="figma-data-row"><span>Stock orbital</span><strong>{shipyard.orbitalStock.length}</strong></div>
                  <div className="figma-data-row"><span>Ordenes vencidas</span><strong>{shipyard.actionAvailability.completeDue.supported ? "Si" : "No"}</strong></div>
                  <div className="figma-data-row"><span>Enqueue</span><strong>{shipyard.actionAvailability.enqueue.reasonLabel}</strong></div>
                </div>
              </section>
            </div>
          </UiCard>

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
              <p className="figma-panel-note">El backend no devolvio catalogo orbital util todavia. La cabina mantiene el shell y deja visible la frontera del modulo.</p>
            ) : (
              <>
                <div className="readiness-grid">
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                    <p className="eyebrow">{cockpitStatusLabels.available}</p>
                        <h4>Listas para entrar en cola</h4>
                      </div>
                      <UiBadge tone="good">{catalogBuckets.available.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.available.length > 0
                        ? "Estas opciones tienen una lectura de readiness util en el backend actual."
                        : "No hay opciones claramente producibles en la lectura actual."}
                    </p>
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">{cockpitStatusLabels.blocked}</p>
                        <h4>Requieren resolver contexto</h4>
                      </div>
                      <UiBadge tone="warn">{catalogBuckets.blocked.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.blocked.length > 0
                        ? "Cada carta mantiene visible el motivo del bloqueo para evitar lecturas engañosas."
                        : "No hay bloqueos catalogados aparte de limitaciones del backend."}
                    </p>
                  </section>
                  <section className="subpanel figma-subpanel">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">No soportadas</p>
                        <h4>Lectura parcial</h4>
                      </div>
                      <UiBadge tone="neutral">{catalogBuckets.unsupported.length}</UiBadge>
                    </div>
                    <p className="figma-panel-note">
                      {catalogBuckets.unsupported.length > 0
                        ? "El servicio aun no describe bien estas opciones y la UI las muestra como limite real."
                        : "No hay opciones marcadas como no soportadas."}
                    </p>
                  </section>
                </div>

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
                    <div className="readiness-grid">
                      {group.assets.map((asset) => {
                        const isRecommended = recommendedAsset?.assetType === asset.assetType && asset.statusKey === "Available";
                        const bucket = getCatalogBucket(asset);
                        const badgeTone = bucket === "available" ? "good" : bucket === "blocked" ? "warn" : "neutral";

                        return (
                          <article key={asset.assetType} className="subpanel figma-subpanel">
                            <div className="figma-section-header">
                              <div>
                                <p className="eyebrow">{asset.roleLabel}</p>
                                <h4>{asset.label}</h4>
                              </div>
                              <div className="figma-badge-row">
                                {isRecommended ? <UiBadge tone="good">Recomendada</UiBadge> : null}
                                <UiBadge tone={badgeTone}>{asset.statusLabel}</UiBadge>
                              </div>
                            </div>
                            <div className="figma-data-list">
                              <div className="figma-data-row"><span>Coste</span><strong>{asset.estimatedCostLabel}</strong></div>
                              <div className="figma-data-row"><span>Duracion</span><strong>{asset.estimatedDurationLabel}</strong></div>
                              <div className="figma-data-row"><span>Stock local</span><strong>{asset.quantityLabel}</strong></div>
                              <div className="figma-data-row"><span>Requisitos</span><strong>{formatRequirementLabel(asset)}</strong></div>
                            </div>
                            <p>{bucket === "available" ? "Lista para preparar produccion cuando la accion dedicada quede habilitada." : asset.reasonLabel}</p>
                            <div className="selection-chip-row">
                              <button
                                type="button"
                                className="planet-action-button-secondary"
                                onClick={() => handleReviewAsset(asset)}
                              >
                                {bucket === "available" ? "Revisar orden" : bucket === "blocked" ? "Revisar bloqueo" : "Revisar limite"}
                              </button>
                            </div>
                          </article>
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
                <p className="eyebrow">Cola orbital</p>
                <h3>Produccion visible y readiness temporal</h3>
                <p>La cola muestra progreso o historial visible desde backend sin inventar cierres locales.</p>
              </div>
              <div className="figma-badge-row">
                <UiBadge tone={shipyard.queue.length > 0 ? "warn" : "neutral"}>
                  {shipyard.queue.length > 0 ? `${shipyard.queue.length} visibles` : "Sin cola visible"}
                </UiBadge>
                <UiBadge tone={dueQueueCount > 0 ? "warn" : "neutral"}>
                  {dueQueueCount > 0 ? `${dueQueueCount} vencidas` : "Sin vencidas"}
                </UiBadge>
              </div>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Resumen de cola</p>
                    <h4>Estado general</h4>
                  </div>
                  <UiBadge tone={shipyard.actionAvailability.completeDue.supported ? "warn" : "neutral"}>
                    {shipyard.actionAvailability.completeDue.reasonLabel}
                  </UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Ordenes visibles</span><strong>{formatCountLabel(shipyard.queue.length, "orden", "ordenes")}</strong></div>
                  <div className="figma-data-row"><span>Produccion vencida</span><strong>{dueQueueCount > 0 ? `${dueQueueCount} detectadas` : "No detectada"}</strong></div>
                  <div className="figma-data-row"><span>Cierre seguro</span><strong>{hasSafeShipyardCompleteDue ? "Disponible con confirmacion" : "Usar Planeta para QA"}</strong></div>
                </div>
                <div className="selection-chip-row">
                  <button type="button" className="planet-action-button-blocked" disabled>
                    No disponible en esta version
                  </button>
                </div>
                <p className="figma-panel-note">
                  Esta cabina no ejecuta materializacion directa. Usa la accion Development QA de Planeta para materializar ordenes vencidas y volver a leer stock, cola y readiness desde backend.
                </p>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Lectura temporal</p>
                    <h4>Como interpretar la cola</h4>
                  </div>
                  <UiBadge tone="neutral">{cockpitStatusLabels.readOnly}</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  <li>Una orden vencida significa que el backend la detecta; esta cabina espera una lectura posterior para mostrar stock o historial actualizado.</li>
                  <li>Las fechas muestran la ventana visible de produccion para que un futuro refresh refleje cambios reales.</li>
                  <li>La cola vacia sigue siendo un estado util y no se rellena con placeholders.</li>
                </ul>
              </section>
            </div>
            {shipyard.queue.length === 0 ? (
              <p className="figma-panel-note">No hay ordenes orbitales abiertas ni historial visible en este planeta. Tras materializar desde Planeta, este panel debe seguir mostrando solo estado backend.</p>
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
                    <p>{item.isDue ? "La orden ya vencio en la lectura actual. La cabina la mantiene visible sin completarla automaticamente." : "La orden sigue en progreso o pendiente dentro de la ventana temporal visible."}</p>
                  </section>
                ))}
              </div>
            )}
          </UiCard>

          {shipyard.stockpile.length > 0 ? (
            <UiCard className="panel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Reservas locales</p>
                  <h3>Recursos disponibles</h3>
                </div>
                <UiBadge tone="resource">{shipyard.stockpile.length} balances</UiBadge>
              </div>
              <div className="readiness-grid">
                {shipyard.stockpile.map((entry) => (
                  <section key={entry.resourceType} className="subpanel figma-subpanel">
                    <div className="figma-data-list">
                      <div className="figma-data-row"><span>{formatResourceLabel(entry.resourceType)}</span><strong>{entry.quantity}</strong></div>
                    </div>
                  </section>
                ))}
              </div>
            </UiCard>
          ) : null}

          {shipyard.diagnostics.playerFacing.length > 0 || technicalErrorDetail || enqueueOrderDetails ? (
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

          {shipyard.diagnostics.playerFacing.length > 0 || technicalErrorDetail || enqueueOrderDetails ? (
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

          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Handoff a Flotas</p>
                <h3>Como se relacionan stock, cola y grupos orbitales</h3>
                <p>Astillero produce y conserva stock local. Flotas inspecciona los grupos orbitales ya visibles y sus movimientos.</p>
              </div>
              <UiBadge tone="warn">Sin accion de flota</UiBadge>
            </div>
            <div className="readiness-grid">
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">En esta cabina</p>
                    <h4>Produccion y gasto inmediato</h4>
                  </div>
                  <UiBadge tone="resource">{shipyard.orbitalStock.length} stocks</UiBadge>
                </div>
                <div className="figma-data-list">
                  <div className="figma-data-row"><span>Cola orbital</span><strong>{shipyard.queue.length > 0 ? formatCountLabel(shipyard.queue.length, "orden activa", "ordenes activas") : "Sin ordenes activas"}</strong></div>
                  <div className="figma-data-row"><span>Stock local</span><strong>{shipyard.orbitalStock.length > 0 ? formatCountLabel(shipyard.orbitalStock.length, "tipo en reserva", "tipos en reserva") : "Sin stock orbital visible"}</strong></div>
                  <div className="figma-data-row"><span>Lectura de flota</span><strong>Fuera de este modulo</strong></div>
                </div>
              </section>
              <section className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">En Flotas</p>
                    <h4>Grupos orbitales y movimiento</h4>
                  </div>
                  <UiBadge>Cabina vecina</UiBadge>
                </div>
                <ul className="stack-list compact-list">
                  <li>El stock orbital no equivale automaticamente a una escuadra visible en Flotas.</li>
                  <li>Las ordenes en cola siguen en produccion hasta que el backend las complete y las convierta en stock util.</li>
                  <li>La asignacion a flota se mantiene fuera de esta cabina en esta version.</li>
                </ul>
              </section>
            </div>
            <div className="selection-chip-row">
              <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
                Abrir Flotas
              </Link>
              <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
                Ver grupos orbitales
              </Link>
            </div>
          </UiCard>
        </>
      ) : (
        !isLoading && queryCivilizationId && !error ? (
          <UiCard className="panel">
            <div className="figma-section-header">
              <div>
                <p className="eyebrow">Estado vacio</p>
                <h3>Sin datos de astillero</h3>
              </div>
              <UiBadge tone="warn">Vacio</UiBadge>
            </div>
            <p className="figma-panel-note">El backend no devolvio un contexto de astillero util para este planeta. La shell sigue visible para mantener el acceso y el contexto de navegacion.</p>
          </UiCard>
        ) : null
      )}

      {enqueueFeedback ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Confirmacion registrada</p>
              <h3>Produccion enviada</h3>
              <p>La cabina recargo cola, catalogo y reservas con el estado confirmado por el backend.</p>
            </div>
            <UiBadge tone="good">Actualizada</UiBadge>
          </div>
          <p>{enqueueFeedback}</p>
          {enqueueRefreshAudit ? (
            <div className="figma-data-list">
              <div className="figma-data-row"><span>Cola visible</span><strong>{`${enqueueRefreshAudit.queueBefore} -> ${enqueueRefreshAudit.queueAfter}`}</strong></div>
              <div className="figma-data-row"><span>Orden refrescada</span><strong>{enqueueRefreshAudit.visibleOrderLabel ?? "Pendiente de aparecer en la lectura actual"}</strong></div>
              <div className="figma-data-row"><span>Ventana visible</span><strong>{enqueueRefreshAudit.visibleOrderWindow ?? "Pendiente de lectura backend"}</strong></div>
              <div className="figma-data-row"><span>Stock orbital visible</span><strong>{enqueueRefreshAudit.stockDigest}</strong></div>
              <div className="figma-data-row"><span>Delta de recursos</span><strong>{enqueueRefreshAudit.resourceDelta.length > 0 ? enqueueRefreshAudit.resourceDelta.join(" · ") : "Sin cambios visibles"}</strong></div>
            </div>
          ) : null}
          <div className="selection-chip-row">
            <Link className="selection-chip" to={buildFleetsUrl(activeCivilizationId, selectedPlanetId)}>
              Ver readiness en Flotas
            </Link>
          </div>
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
          canClose={!isSubmittingEnqueue}
          closeLabel="Cerrar"
          description="La cabina revisa el impacto visible antes de habilitar cualquier accion real, incluido el gasto inmediato de recursos cuando el envio se acepta."
          isBusy={isSubmittingEnqueue}
          isOpen
          onClose={handleCancelReview}
          primaryAction={{
            label: reviewSelection.bucket === "available" ? "Confirmar" : "No disponible en esta version",
            onClick: () => void handleConfirmProduction(),
            disabled: reviewSelection.bucket !== "available" || !hasEnqueueAcknowledgement,
          }}
          secondaryAction={{
            label: "Cancelar",
            onClick: handleCancelReview,
            disabled: isSubmittingEnqueue,
          }}
          title="Confirmacion de produccion orbital"
        >
          <UiBadge tone={reviewSelection.bucket === "available" ? "good" : "warn"}>
            {reviewSelection.bucket === "available" ? "Lista para futura cola" : "No enviable"}
          </UiBadge>
          <div className="readiness-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Planeta</span><strong>{shipyard.planetName}</strong></div>
                <div className="figma-data-row"><span>Activo</span><strong>{reviewSelection.asset.label}</strong></div>
                <div className="figma-data-row"><span>Salida</span><strong>{reviewSelection.asset.quantityLabel}</strong></div>
                <div className="figma-data-row"><span>Duracion</span><strong>{reviewSelection.asset.estimatedDurationLabel}</strong></div>
              </div>
            </section>
            <section className="subpanel figma-subpanel">
              <div className="figma-data-list">
                <div className="figma-data-row"><span>Coste</span><strong>{reviewSelection.asset.estimatedCostLabel}</strong></div>
                <div className="figma-data-row"><span>Readiness</span><strong>{reviewSelection.asset.statusLabel}</strong></div>
                <div className="figma-data-row"><span>Requisitos</span><strong>{formatRequirementLabel(reviewSelection.asset)}</strong></div>
                <div className="figma-data-row"><span>Motivo visible</span><strong>{reviewSelection.asset.reasonLabel}</strong></div>
              </div>
            </section>
          </div>
          <p>
            {reviewSelection.bucket === "available"
              ? "La opcion ya puede enviarse por la via protegida de desarrollo si confirmas la revision. Si el backend la acepta, el coste visible se descuenta al momento."
              : "Esta revision permanece en modo diagnostico. La cabina no intentara enviar nada mientras el estado siga bloqueado o no soportado."}
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
          <Link className="selection-chip" to={buildResearchUrl(activeCivilizationId, selectedPlanetId)}>
            {cockpitNavigationLabels.openResearch}
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

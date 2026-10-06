import { useEffect, useMemo, useState } from "react";
import { useSearchParams } from "react-router-dom";
import type { EstimateOrbitalTravelResponse, FleetCommandApiResult } from "../api/fleetCommandTypes";
import type { FleetGroupSummary, FleetOrbitalStock, FleetUiState } from "../api/fleetTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { ActiveFleetMovements } from "../components/ActiveFleetMovements";
import { CockpitHero } from "../components/CockpitHero";
import { FleetMovementComposer } from "../components/FleetMovementComposer";
import { FleetSummary } from "../components/FleetSummary";
import { GameModal } from "../components/GameModal";
import { LocalOrbitalStockPanel } from "../components/LocalOrbitalStockPanel";
import { StationedFleetList } from "../components/StationedFleetList";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { formatResourceType, formatSpaceAssetType } from "../utils/domainPresentation";
import { usePlayableRouteContext } from "../utils/usePlayableRouteContext";

interface FleetCreationSelection { stock: FleetOrbitalStock; quantity: number; }
interface EstimateSnapshot { groupId: string; originPlanetId: string; destinationPlanetId: string; estimatedArrivalAtUtc: string; }

function positiveInteger(value: number) {
  return Number.isFinite(value) && value > 0 ? Math.floor(value) : 1;
}

function durationMilliseconds(value?: string | null) {
  const match = /^(?:(\d+)\.)?(\d{2}):(\d{2}):(\d{2})$/.exec(value ?? "");
  return match ? ((Number(match[1] ?? 0) * 86400) + (Number(match[2]) * 3600) + (Number(match[3]) * 60) + Number(match[4])) * 1000 : 0;
}

function formatDuration(value?: string | null) {
  const minutes = Math.ceil(durationMilliseconds(value) / 60000);
  if (!minutes) return "No disponible";
  return minutes < 60 ? `${minutes} min` : `${Math.floor(minutes / 60)} h ${minutes % 60} min`;
}

export function FleetsPage() {
  const [searchParams] = useSearchParams();
  const queryCivilizationId = searchParams.get("civilizationId");
  const queryPlanetId = searchParams.get("planetId");
  const routeContext = usePlayableRouteContext(queryCivilizationId);
  const civilizationId = queryCivilizationId ?? routeContext.resolvedWorldEntry?.civilizationId ?? "";
  const requestedPlanetId = queryPlanetId ?? routeContext.resolvedWorldEntry?.planetId ?? "";
  const [uiState, setUiState] = useState<FleetUiState | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [feedback, setFeedback] = useState<string | null>(null);
  const [quantityByAssetType, setQuantityByAssetType] = useState<Record<string, number>>({});
  const [creationSelection, setCreationSelection] = useState<FleetCreationSelection | null>(null);
  const [isCreating, setIsCreating] = useState(false);
  const [selectedGroupId, setSelectedGroupId] = useState("");
  const [destinationPlanetId, setDestinationPlanetId] = useState("");
  const [estimateResult, setEstimateResult] = useState<FleetCommandApiResult<EstimateOrbitalTravelResponse> | null>(null);
  const [estimateSnapshot, setEstimateSnapshot] = useState<EstimateSnapshot | null>(null);
  const [isEstimating, setIsEstimating] = useState(false);
  const [isTransferReviewOpen, setIsTransferReviewOpen] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [isCancelling, setIsCancelling] = useState(false);
  const [arrivalRefreshError, setArrivalRefreshError] = useState<string | null>(null);

  async function reloadFleetState() {
    const response = await voidEmpiresApi.getFleetUiState(civilizationId, requestedPlanetId || undefined);
    if (!response.succeeded || !response.uiState) throw new Error(response.errors[0] ?? "No se pudo actualizar Flotas.");
    setUiState(response.uiState);
    return response.uiState;
  }

  useEffect(() => {
    async function load() {
      if (!civilizationId) return;
      setIsLoading(true);
      setError(null);
      try { await reloadFleetState(); }
      catch (requestError) { setUiState(null); setError(requestError instanceof Error ? requestError.message : "No se pudo cargar Flotas."); }
      finally { setIsLoading(false); }
    }
    void load();
  }, [civilizationId, requestedPlanetId]);

  const selectedPlanetId = uiState?.selectedPlanetId ?? requestedPlanetId;
  const planets = uiState?.planets ?? [];
  const planetName = (planetId: string) => planets.find((planet) => planet.planetId === planetId)?.planetName ?? "Planeta desconocido";
  const stationedGroups = useMemo(() => uiState?.groups.filter((group) => group.currentPlanetId === selectedPlanetId && group.status === "Stationed" && !group.activeTransfer) ?? [], [selectedPlanetId, uiState]);
  const activeGroups = useMemo(() => uiState?.groups.filter((group) => Boolean(group.activeTransfer)) ?? [], [uiState]);
  const selectedGroup = stationedGroups.find((group) => group.id === selectedGroupId) ?? null;
  const destinations = planets.filter((planet) => planet.planetId !== selectedGroup?.currentPlanetId);
  const estimate = estimateResult?.httpStatus === 200 && estimateResult.response?.succeeded ? estimateResult.response : null;
  const estimateIsCurrent = Boolean(estimate && estimateSnapshot && selectedGroup && estimateSnapshot.groupId === selectedGroup.id && estimateSnapshot.originPlanetId === selectedGroup.currentPlanetId && estimateSnapshot.destinationPlanetId === destinationPlanetId);
  const canSendEstimate = Boolean(estimateIsCurrent && estimate?.canAfford && (estimate.fuelReadiness?.isFuelReady ?? true));
  const originResources = uiState?.resourceContexts?.find((context) => context.planetId === selectedGroup?.currentPlanetId)?.balances ?? [];
  const summary = { total: uiState?.groups.length ?? 0, stationed: uiState?.groups.filter((group) => group.status === "Stationed" && !group.activeTransfer).length ?? 0, transit: activeGroups.length, ships: uiState?.groups.reduce((total, group) => total + group.quantity, 0) ?? 0 };
  const estimatedCosts = estimate?.resourceCosts.map((cost) => `${formatResourceType(cost.resourceType)} ${cost.quantity}`).join(" | ") || "Sin coste";
  const resourceAvailability = originResources.map((resource) => `${formatResourceType(resource.resourceType)} ${resource.quantity}`).join(" | ") || "Sin reservas visibles";
  const arrivalLabel = estimateSnapshot ? new Date(estimateSnapshot.estimatedArrivalAtUtc).toLocaleString("es-ES") : "No disponible";

  function clearEstimate() { setEstimateResult(null); setEstimateSnapshot(null); setIsTransferReviewOpen(false); }
  function selectGroup(group: FleetGroupSummary) { setSelectedGroupId(group.id); setDestinationPlanetId(""); clearEstimate(); }
  function changeQuantity(assetType: string, quantity: number) { setQuantityByAssetType((current) => ({ ...current, [assetType]: positiveInteger(quantity) })); }
  function reviewCreation(stock: FleetOrbitalStock, quantity: number) { setCreationSelection({ stock, quantity: Math.min(stock.quantity, positiveInteger(quantity)) }); }

  async function confirmFleetCreation() {
    if (!creationSelection || !selectedPlanetId || isCreating) return;
    setIsCreating(true); setError(null);
    try {
      const result = await voidEmpiresApi.createOrbitalGroup({ civilizationId, planetId: selectedPlanetId, assetType: creationSelection.stock.assetType, quantity: creationSelection.quantity });
      if (result.httpStatus !== 201 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo crear la flota."); return; }
      setCreationSelection(null); setFeedback("Flota creada y estacionada."); await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo crear la flota."); }
    finally { setIsCreating(false); }
  }

  async function calculateRoute() {
    if (!selectedGroup || !destinationPlanetId || isEstimating) return;
    setIsEstimating(true); setError(null);
    try {
      const result = await voidEmpiresApi.estimateOrbitalTravel({ civilizationId, orbitalGroupId: selectedGroup.id, destinationPlanetId });
      setEstimateResult(result);
      if (result.httpStatus === 200 && result.response?.succeeded) setEstimateSnapshot({ groupId: selectedGroup.id, originPlanetId: selectedGroup.currentPlanetId, destinationPlanetId, estimatedArrivalAtUtc: new Date(Date.now() + durationMilliseconds(result.response.estimatedDuration)).toISOString() });
      else { setEstimateSnapshot(null); setError(result.response?.errors[0] ?? "No se pudo calcular la ruta."); }
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo calcular la ruta."); }
    finally { setIsEstimating(false); }
  }

  async function confirmTransfer() {
    if (!selectedGroup || !estimateSnapshot || !canSendEstimate || isSending) return;
    setIsSending(true); setError(null);
    try {
      const result = await voidEmpiresApi.createOrbitalTransfer({ civilizationId, orbitalGroupId: selectedGroup.id, destinationPlanetId: estimateSnapshot.destinationPlanetId, requestedAtUtc: new Date().toISOString() });
      if (result.httpStatus !== 201 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo enviar la flota."); return; }
      setSelectedGroupId(""); clearEstimate(); setFeedback("Flota enviada."); await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo enviar la flota."); }
    finally { setIsSending(false); }
  }

  async function cancelTransfer(group: FleetGroupSummary) {
    if (!group.activeTransfer || !group.commands?.canCancelTransfer || isCancelling) return;
    setIsCancelling(true); setError(null);
    try {
      const result = await voidEmpiresApi.cancelOrbitalTransfer({ civilizationId, orbitalTransferId: group.activeTransfer.id });
      if (result.httpStatus !== 200 || !result.response?.succeeded) { setError(result.response?.errors[0] ?? "No se pudo cancelar el movimiento."); return; }
      setFeedback("Movimiento cancelado; la flota vuelve a estar disponible."); await reloadFleetState();
    } catch (requestError) { setError(requestError instanceof Error ? requestError.message : "No se pudo cancelar el movimiento."); }
    finally { setIsCancelling(false); }
  }

  async function refreshArrivals() { setArrivalRefreshError(null); try { await reloadFleetState(); } catch { setArrivalRefreshError("No se pudo actualizar la llegada. Reintentar."); } }

  return <section className="page-grid">
    <CockpitHero title="Flotas" description="Crea flotas desde el stock orbital y gestiona movimientos entre planetas." badges={<><UiBadge>{selectedPlanetId ? planetName(selectedPlanetId) : "Sin planeta"}</UiBadge><UiBadge>Grupos orbitales</UiBadge></>} />
    {error ? <UiCard className="panel"><p className="error-text">{error}</p></UiCard> : null}{feedback ? <p>{feedback}</p> : null}
    {uiState ? <><FleetSummary {...summary} /><LocalOrbitalStockPanel stock={uiState.localStock ?? []} quantities={quantityByAssetType} onQuantityChange={changeQuantity} onCreate={reviewCreation} /><div className="readiness-grid"><StationedFleetList groups={stationedGroups} planetName={selectedPlanetId ? planetName(selectedPlanetId) : "Planeta"} selectedGroupId={selectedGroupId} onSelect={selectGroup} /><FleetMovementComposer group={selectedGroup} destinations={destinations} destinationPlanetId={destinationPlanetId} originName={selectedGroup ? planetName(selectedGroup.currentPlanetId) : ""} isEstimating={isEstimating} estimateReady={estimateIsCurrent} canReview={canSendEstimate} duration={formatDuration(estimate?.estimatedDuration)} cost={estimatedCosts} arrival={arrivalLabel} onDestinationChange={(planetId) => { setDestinationPlanetId(planetId); clearEstimate(); }} onCalculate={() => void calculateRoute()} onReview={() => setIsTransferReviewOpen(true)} /></div><ActiveFleetMovements groups={activeGroups} isCancelling={isCancelling} refreshError={arrivalRefreshError} planetName={planetName} onCancel={(group) => void cancelTransfer(group)} onExpire={() => void refreshArrivals()} /></> : null}
    {creationSelection && selectedPlanetId ? <GameModal actionScope="gameplay" canClose={!isCreating} closeLabel="Cerrar" description="Confirma la conversion de stock orbital en una flota estacionada." isBusy={isCreating} isOpen onClose={() => setCreationSelection(null)} primaryAction={{ label: "Crear flota", onClick: () => void confirmFleetCreation() }} secondaryAction={{ label: "Cancelar", onClick: () => setCreationSelection(null), disabled: isCreating }} title="Crear flota"><div className="figma-data-list"><div className="figma-data-row"><span>Planeta</span><strong>{planetName(selectedPlanetId)}</strong></div><div className="figma-data-row"><span>Nave</span><strong>{formatSpaceAssetType(creationSelection.stock.assetType)}</strong></div><div className="figma-data-row"><span>Stock actual</span><strong>{creationSelection.stock.quantity}</strong></div><div className="figma-data-row"><span>Unidades</span><strong>{creationSelection.quantity}</strong></div><div className="figma-data-row"><span>Stock restante</span><strong>{creationSelection.stock.quantity - creationSelection.quantity}</strong></div></div></GameModal> : null}
    {isTransferReviewOpen && selectedGroup && estimate && estimateSnapshot ? <GameModal actionScope="gameplay" canClose={!isSending} closeLabel="Cerrar" description="Revisa la ruta calculada antes de reservar y enviar la flota." isBusy={isSending} isOpen onClose={() => setIsTransferReviewOpen(false)} primaryAction={{ label: "Enviar flota", onClick: () => void confirmTransfer(), disabled: !canSendEstimate }} secondaryAction={{ label: "Cancelar", onClick: () => setIsTransferReviewOpen(false), disabled: isSending }} title="Confirmar movimiento"><div className="figma-data-list"><div className="figma-data-row"><span>Flota</span><strong>{formatSpaceAssetType(selectedGroup.assetType)} | {selectedGroup.quantity} naves</strong></div><div className="figma-data-row"><span>Origen</span><strong>{planetName(selectedGroup.currentPlanetId)}</strong></div><div className="figma-data-row"><span>Destino</span><strong>{planetName(estimateSnapshot.destinationPlanetId)}</strong></div><div className="figma-data-row"><span>Duracion</span><strong>{formatDuration(estimate.estimatedDuration)}</strong></div><div className="figma-data-row"><span>Llegada</span><strong>{arrivalLabel}</strong></div><div className="figma-data-row"><span>Coste</span><strong>{estimatedCosts}</strong></div><div className="figma-data-row"><span>Recursos actuales</span><strong>{resourceAvailability}</strong></div></div></GameModal> : null}
    {!uiState && !isLoading && civilizationId && !error ? <p className="figma-panel-note">No hay estado de flotas disponible.</p> : null}
  </section>;
}

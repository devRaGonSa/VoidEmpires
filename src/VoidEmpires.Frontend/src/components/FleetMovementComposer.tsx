import type { FleetGroupSummary, FleetPlanetOption } from "../api/fleetTypes";
import { formatSpaceAssetType } from "../utils/domainPresentation";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface FleetMovementComposerProps {
  group: FleetGroupSummary | null;
  destinations: FleetPlanetOption[];
  destinationPlanetId: string;
  originName: string;
  isEstimating: boolean;
  estimateReady: boolean;
  canReview: boolean;
  duration: string;
  cost: string;
  arrival: string;
  onDestinationChange: (planetId: string) => void;
  onCalculate: () => void;
  onReview: () => void;
}

export function FleetMovementComposer(props: FleetMovementComposerProps) {
  const { group, destinations, destinationPlanetId, originName, isEstimating, estimateReady, canReview, duration, cost, arrival, onDestinationChange, onCalculate, onReview } = props;
  return (
    <UiCard className="panel">
      <div className="figma-section-header"><div><p className="eyebrow">Movimiento</p><h3>Preparar ruta</h3></div><UiBadge>{group ? "Flota seleccionada" : "Selecciona una flota"}</UiBadge></div>
      {group ? <div className="figma-data-list">
        <div className="figma-data-row"><span>Flota</span><strong>{formatSpaceAssetType(group.assetType)} | {group.quantity} naves</strong></div>
        <div className="figma-data-row"><span>Origen</span><strong>{originName}</strong></div>
        <label className="field"><span>Destino</span><select value={destinationPlanetId} onChange={(event) => onDestinationChange(event.target.value)}><option value="">Seleccionar destino</option>{destinations.map((planet) => <option key={planet.planetId} value={planet.planetId}>{planet.planetName}</option>)}</select></label>
        <button type="button" disabled={!destinationPlanetId || isEstimating} onClick={onCalculate}>{isEstimating ? "Calculando..." : "Calcular ruta"}</button>
        {!destinationPlanetId ? <p className="figma-panel-note">Selecciona un destino distinto del origen.</p> : null}
        {estimateReady ? <><div className="figma-data-row"><span>Duracion</span><strong>{duration}</strong></div><div className="figma-data-row"><span>Coste</span><strong>{cost}</strong></div><div className="figma-data-row"><span>Llegada estimada</span><strong>{arrival}</strong></div><button type="button" disabled={!canReview} onClick={onReview}>Revisar envio</button>{!canReview ? <p className="figma-panel-note">La ruta no puede enviarse con los recursos o disponibilidad actuales.</p> : null}</> : null}
      </div> : <p className="figma-panel-note">Elige una flota estacionada para preparar su movimiento.</p>}
    </UiCard>
  );
}

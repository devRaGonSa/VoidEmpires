import type { FleetGroupSummary } from "../api/fleetTypes";
import { formatOrbitalGroupStatus, formatSpaceAssetType } from "../utils/domainPresentation";
import { LiveQueueCountdown } from "./LiveQueueCountdown";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface ActiveFleetMovementsProps {
  groups: FleetGroupSummary[];
  isCancelling: boolean;
  refreshError: string | null;
  planetName: (planetId: string) => string;
  onCancel: (group: FleetGroupSummary) => void;
  onExpire: () => void;
}

export function ActiveFleetMovements({ groups, isCancelling, refreshError, planetName, onCancel, onExpire }: ActiveFleetMovementsProps) {
  if (!groups.length) return null;
  return (
    <UiCard className="panel">
      <div className="figma-section-header"><div><p className="eyebrow">Movimientos activos</p><h3>Flotas en transito</h3></div><UiBadge tone="warn">{groups.length}</UiBadge></div>
      <div className="readiness-grid">{groups.map((group) => group.activeTransfer ? <article key={group.activeTransfer.id} className="subpanel figma-subpanel"><div className="figma-section-header"><div><p className="eyebrow">{formatSpaceAssetType(group.assetType)}</p><h4>{planetName(group.currentPlanetId)} a {planetName(group.activeTransfer.destinationPlanetId)}</h4></div><UiBadge>{formatOrbitalGroupStatus(group.status)}</UiBadge></div><div className="figma-data-list"><div className="figma-data-row"><span>Naves</span><strong>{group.quantity}</strong></div><div className="figma-data-row"><span>Estado</span><strong>{formatOrbitalGroupStatus(group.status)}</strong></div><div className="figma-data-row"><span>Tiempo hasta llegada</span><strong><LiveQueueCountdown endsAtUtc={group.activeTransfer.arrivalAtUtc} expireKey={group.activeTransfer.id} onExpire={onExpire} /></strong></div></div>{group.commands?.canCancelTransfer ? <button type="button" disabled={isCancelling} onClick={() => onCancel(group)}>Cancelar movimiento</button> : <p className="figma-panel-note">Este movimiento ya no admite cancelacion.</p>}</article> : null)}</div>
      {refreshError ? <div className="transfer-confirmation-actions"><p className="error-text">{refreshError}</p><button type="button" onClick={onExpire}>Reintentar</button></div> : null}
    </UiCard>
  );
}

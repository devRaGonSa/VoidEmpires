import type { FleetGroupSummary } from "../api/fleetTypes";
import {
  formatCompactGuid,
  formatOrbitalGroupStatus,
  formatPlanetReference,
  formatSpaceAssetType,
} from "../utils/domainPresentation";
import { UiBadge } from "./ui/UiBadge";

interface FleetSummaryPanelProps {
  group: FleetGroupSummary;
  isSelected: boolean;
  onSelect: (groupId: string) => void;
}

export function FleetSummaryPanel({
  group,
  isSelected,
  onSelect,
}: FleetSummaryPanelProps) {
  const readinessLabel = group.hasActiveTransfer
    ? "Transferencia activa"
    : group.commands?.canCreateTransfer
      ? "Lista para ordenar"
      : "Solo inspeccion";

  return (
    <article className={`subpanel figma-subpanel fleet-summary-card${isSelected ? " fleet-summary-card-selected" : ""}`}>
      <div className="figma-section-header">
        <div className="fleet-identity-block">
          <p className="eyebrow">Escuadra orbital</p>
          <h4>{formatSpaceAssetType(group.assetType)}</h4>
          <p>Orbita en {formatPlanetReference(group.currentPlanetId)}. Lista breve para saltar entre escuadras y abrir la cabina principal.</p>
          <p className="dev-meta">ID tactico {formatCompactGuid(group.id)}</p>
        </div>
        <UiBadge tone={group.hasActiveTransfer ? "warn" : group.commands?.canCreateTransfer ? "good" : "neutral"}>
          {formatOrbitalGroupStatus(group.status)}
        </UiBadge>
      </div>

      <div className="figma-data-list">
        <div className="figma-data-row">
          <span>Cantidad</span>
          <strong>{group.quantity}</strong>
        </div>
        <div className="figma-data-row">
          <span>Estado tactico</span>
          <strong>{readinessLabel}</strong>
        </div>
      </div>

      <div className="figma-badge-row">
        <UiBadge tone={group.routeFuelReadiness?.canRequestTravelEstimate ? "good" : "warn"}>
          {group.routeFuelReadiness?.canRequestTravelEstimate ? "Ruta estimable" : "Ruta bloqueada"}
        </UiBadge>
        {group.activeTransfer ? (
          <UiBadge tone="warn">{formatPlanetReference(group.activeTransfer.destinationPlanetId)}</UiBadge>
        ) : null}
      </div>

      <button type="button" className="fleet-summary-select-button" onClick={() => onSelect(group.id)}>
        {isSelected ? "Cabina enfocada" : "Abrir cabina"}
      </button>
    </article>
  );
}

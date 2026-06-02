import type { FleetGroupSummary } from "../api/fleetTypes";
import type { FleetSquadListPresentationItem } from "../utils/fleetCommandPresentation";
import { UiBadge } from "./ui/UiBadge";

interface FleetSummaryPanelProps {
  group: FleetGroupSummary;
  presentation: FleetSquadListPresentationItem;
  isSelected: boolean;
  hasDueTransfer: boolean;
  onSelect: (groupId: string) => void;
}

export function FleetSummaryPanel({
  group,
  presentation,
  isSelected,
  hasDueTransfer,
  onSelect,
}: FleetSummaryPanelProps) {
  return (
    <article className={`subpanel figma-subpanel fleet-summary-card${isSelected ? " fleet-summary-card-selected" : ""}`}>
      <div className="fleet-summary-card-head">
        <div className="fleet-summary-card-title">
          <p className="eyebrow">Escuadra orbital</p>
          <h4>{presentation.title}</h4>
          <p className="fleet-summary-card-location">Orbita en {presentation.locationLabel}</p>
        </div>
        <div className="fleet-summary-card-status">
          <UiBadge tone={presentation.statusTone}>{presentation.statusLabel}</UiBadge>
          <strong>{presentation.quantityLabel}</strong>
        </div>
      </div>

      <div className="fleet-summary-card-strip">
        <span className="fleet-summary-card-route-label">
          {group.activeTransfer ? "Destino activo" : "Ruta"}
        </span>
        <strong>{presentation.destinationLabel}</strong>
      </div>

      <div className="fleet-summary-card-meta">
        <div className="figma-badge-row">
          <UiBadge tone={hasDueTransfer ? "warn" : presentation.readinessTone}>
            {hasDueTransfer ? "Llegada vencida" : presentation.readinessLabel}
          </UiBadge>
          {isSelected ? <UiBadge tone="good">En foco</UiBadge> : null}
        </div>
        <p className="dev-meta">ID tactico {presentation.technicalIdLabel}</p>
      </div>

      <button type="button" className="fleet-summary-select-button" onClick={() => onSelect(group.id)}>
        {isSelected ? "Cabina enfocada" : "Abrir cabina"}
      </button>
    </article>
  );
}

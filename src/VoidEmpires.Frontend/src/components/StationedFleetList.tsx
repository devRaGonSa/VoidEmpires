import type { FleetGroupSummary } from "../api/fleetTypes";
import { formatOrbitalGroupStatus, formatSpaceAssetType } from "../utils/domainPresentation";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface StationedFleetListProps {
  groups: FleetGroupSummary[];
  planetName: string;
  selectedGroupId: string;
  onSelect: (group: FleetGroupSummary) => void;
}

export function StationedFleetList({ groups, planetName, selectedGroupId, onSelect }: StationedFleetListProps) {
  return (
    <UiCard className="panel">
      <div className="figma-section-header"><div><p className="eyebrow">Flotas estacionadas</p><h3>{planetName}</h3></div><UiBadge>{groups.length}</UiBadge></div>
      {groups.length ? <ul className="stack-list">{groups.map((group) => <li key={group.id}><div><strong>{formatSpaceAssetType(group.assetType)}</strong><p>{group.quantity} naves | {formatOrbitalGroupStatus(group.status)}</p></div><button type="button" onClick={() => onSelect(group)}>{selectedGroupId === group.id ? "Seleccionada" : "Mover"}</button></li>)}</ul> : <p className="figma-panel-note">No hay flotas estacionadas en este planeta.</p>}
    </UiCard>
  );
}

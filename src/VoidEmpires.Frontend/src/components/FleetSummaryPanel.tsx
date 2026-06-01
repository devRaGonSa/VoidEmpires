import type { FleetGroupSummary } from "../api/fleetTypes";
import { StatusBadge } from "./StatusBadge";

interface FleetSummaryPanelProps {
  group: FleetGroupSummary;
}

export function FleetSummaryPanel({
  group,
}: FleetSummaryPanelProps) {
  return (
    <article className="panel">
      <div className="section-heading">
        <div>
          <h3>{group.assetType}</h3>
          <p>{group.id}</p>
        </div>
        <StatusBadge tone={group.hasActiveTransfer ? "warn" : "good"}>
          {group.status}
        </StatusBadge>
      </div>

      <div className="stat-grid">
        <div className="stat-tile">
          <strong>{group.quantity}</strong>
          <span>Quantity</span>
        </div>
        <div className="stat-tile">
          <strong>{group.currentPlanetId}</strong>
          <span>Current planet</span>
        </div>
        <div className="stat-tile">
          <strong>{group.originPlanetId}</strong>
          <span>Origin planet</span>
        </div>
      </div>

      <div className="inline-note-row">
        {group.commands?.canCreateTransfer && (
          <StatusBadge tone="good">Transfer available</StatusBadge>
        )}
        {group.commands?.canCancelTransfer && (
          <StatusBadge tone="warn">Transfer can cancel</StatusBadge>
        )}
        {group.routeFuelReadiness?.fuelReadinessPolicy && (
          <StatusBadge>
            Fuel policy: {group.routeFuelReadiness.fuelReadinessPolicy}
          </StatusBadge>
        )}
      </div>

      {group.routeFuelReadiness?.notes?.length ? (
        <ul className="stack-list compact-list">
          {group.routeFuelReadiness.notes.map((note) => (
            <li key={note}>{note}</li>
          ))}
        </ul>
      ) : null}

      {group.activeTransfer && (
        <div className="subpanel">
          <h4>Active transfer</h4>
          <p>
            {group.activeTransfer.status} to {group.activeTransfer.destinationPlanetId}
          </p>
          <p>
            Distance {group.activeTransfer.abstractDistanceUnits} · arrival{" "}
            {group.activeTransfer.arrivalAtUtc}
          </p>
          {group.activeTransfer.interceptionReadiness?.readinessNote && (
            <p>{group.activeTransfer.interceptionReadiness.readinessNote}</p>
          )}
        </div>
      )}
    </article>
  );
}

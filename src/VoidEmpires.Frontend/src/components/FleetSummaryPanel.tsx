import type { FleetGroupSummary } from "../api/fleetTypes";
import {
  formatCompactGuid,
  formatFuelReadinessPolicy,
  formatOrbitalGroupStatus,
  formatPlanetReference,
  formatSpaceAssetType,
  formatTransferStatus,
} from "../utils/domainPresentation";
import { StatusBadge } from "./StatusBadge";

interface FleetSummaryPanelProps {
  group: FleetGroupSummary;
}

export function FleetSummaryPanel({
  group,
}: FleetSummaryPanelProps) {
  const commandReadiness = [
    { label: "Travel preview", value: group.routeFuelReadiness?.canRequestTravelEstimate ? "Preview ready" : "Preview blocked" },
    { label: "Create transfer", value: group.commands?.canCreateTransfer ? "Ready" : "Blocked" },
    { label: "Split group", value: group.commands?.canSplit ? "Ready" : "Blocked" },
    { label: "Merge groups", value: group.commands?.canMerge ? "Ready" : "Blocked" },
    { label: "Cancel transfer", value: group.commands?.canCancelTransfer ? "Ready" : "Blocked" },
  ];

  return (
    <article className="panel">
      <div className="section-heading">
        <div>
          <h3>{formatSpaceAssetType(group.assetType)}</h3>
          <p>{formatCompactGuid(group.id)}</p>
        </div>
        <StatusBadge tone={group.hasActiveTransfer ? "warn" : "good"}>
          {formatOrbitalGroupStatus(group.status)}
        </StatusBadge>
      </div>

      <div className="stat-grid">
        <div className="stat-tile">
          <strong>{group.quantity}</strong>
          <span>Quantity</span>
        </div>
        <div className="stat-tile">
          <strong>{formatPlanetReference(group.currentPlanetId)}</strong>
          <span>Current planet</span>
        </div>
        <div className="stat-tile">
          <strong>{formatPlanetReference(group.originPlanetId)}</strong>
          <span>Origin planet</span>
        </div>
      </div>

      <div className="stack-list compact-list">
        {commandReadiness.map((item) => (
          <p key={`${group.id}-${item.label}`}>
            <strong>{item.label}:</strong> {item.value}
          </p>
        ))}
      </div>

      <div className="inline-note-row">
        {group.routeFuelReadiness?.fuelReadinessPolicy && (
          <StatusBadge>
            Fuel policy: {formatFuelReadinessPolicy(group.routeFuelReadiness.fuelReadinessPolicy)}
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
            {formatTransferStatus(group.activeTransfer.status)} to {formatPlanetReference(group.activeTransfer.destinationPlanetId)}
          </p>
          <p>
            Distance {group.activeTransfer.abstractDistanceUnits} - arrival{" "}
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

import { FormEvent, useMemo, useState } from "react";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetGroupSummary, FleetUiState } from "../api/fleetTypes";
import type { ReadinessNote } from "../api/strategicMapTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { ActionManifestPanel } from "../components/ActionManifestPanel";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { UiProgressBar } from "../components/ui/UiProgressBar";
import {
  formatBooleanLabel,
  formatCompactGuid,
  formatFuelReadinessPolicy,
  formatOrbitalGroupStatus,
  formatResourceType,
  formatSpaceAssetType,
  formatTransferStatus,
} from "../utils/domainPresentation";

function formatNote(note: ReadinessNote) {
  if (typeof note === "string") {
    return note;
  }

  return note.note ?? "Readiness metadata present.";
}

function getTransferProgress(group: FleetGroupSummary) {
  const activeTransfer = group.activeTransfer;
  if (!activeTransfer) {
    return null;
  }

  const departure = Date.parse(activeTransfer.departureAtUtc);
  const arrival = Date.parse(activeTransfer.arrivalAtUtc);
  const now = Date.now();

  if (Number.isNaN(departure) || Number.isNaN(arrival) || arrival <= departure) {
    return null;
  }

  return Math.max(0, Math.min(100, ((now - departure) / (arrival - departure)) * 100));
}

function getGroupTone(group: FleetGroupSummary): "good" | "warn" | "neutral" {
  if (group.hasActiveTransfer) {
    return "warn";
  }

  if (group.commands?.canCreateTransfer) {
    return "good";
  }

  return "neutral";
}

interface SummaryMetricProps {
  label: string;
  value: number;
}

function SummaryMetric({ label, value }: SummaryMetricProps) {
  return (
    <div className="figma-stat">
      <strong>{value}</strong>
      <span>{label}</span>
    </div>
  );
}

interface FleetDataRowProps {
  label: string;
  value: string;
}

function FleetDataRow({ label, value }: FleetDataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

export function FleetsPage() {
  const [civilizationId, setCivilizationId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [uiState, setUiState] = useState<FleetUiState | null>(null);
  const [fleetManifest, setFleetManifest] = useState<ActionManifestAction[]>([]);
  const [strategicMapManifest, setStrategicMapManifest] = useState<
    ActionManifestAction[]
  >([]);

  const summary = useMemo(() => {
    if (!uiState) {
      return null;
    }

    return {
      groups: uiState.groups.length,
      transfers: uiState.groups.filter((group) => group.activeTransfer).length,
      resourceContexts: uiState.resourceContexts?.length ?? 0,
      actionHints: uiState.actionHints?.length ?? 0,
    };
  }, [uiState]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationId.trim();
    if (!trimmedCivilizationId) {
      setError("Civilization id is required.");
      setUiState(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const [uiStateResponse, fleetManifestResponse, strategicMapManifestResponse] =
        await Promise.all([
          voidEmpiresApi.getFleetUiState(trimmedCivilizationId),
          voidEmpiresApi.getFleetActionManifest(),
          voidEmpiresApi.getStrategicMapActionManifest(),
        ]);

      if (!uiStateResponse.succeeded || !uiStateResponse.uiState) {
        setUiState(null);
        setError(uiStateResponse.errors[0] ?? "Fleet UI state request failed.");
        return;
      }

      setUiState(uiStateResponse.uiState);
      setFleetManifest(fleetManifestResponse.manifest?.actions ?? []);
      setStrategicMapManifest(strategicMapManifestResponse.manifest?.actions ?? []);
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Fleet requests failed.";
      setUiState(null);
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Phase 9M fleet alignment</UiBadge>
          <h2>Fleet UI state and action manifests</h2>
          <p>
            Fleet groups, active transfers, and route/readiness metadata are
            grouped into compact Figma-style cards while mutating actions remain
            visible but unwired.
          </p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Manifest metadata only</UiBadge>
          <UiBadge>Progress bars for active transfers</UiBadge>
          <UiBadge tone="warn">No command execution</UiBadge>
        </div>
      </UiCard>

      <div className="figma-two-column">
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Development endpoint</p>
              <h3>Load fleet inspection state</h3>
            </div>
            <UiBadge>Read-only fleet surface</UiBadge>
          </div>
          <form className="query-form" onSubmit={handleSubmit}>
            <label className="field">
              <span>Civilization id</span>
              <input
                type="text"
                value={civilizationId}
                onChange={(event) => setCivilizationId(event.target.value)}
                placeholder="00000000-0000-0000-0000-000000000000"
                spellCheck={false}
                aria-label="Civilization id"
              />
            </label>
            <button type="submit" disabled={isLoading}>
              {isLoading ? "Loading..." : "Load fleet panels"}
            </button>
          </form>
          {error && <p className="error-text">{error}</p>}
        </UiCard>

        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Current limits</p>
              <h3>Constraints</h3>
            </div>
            <UiBadge tone="warn">No mutations</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Action manifests remain documentation and readiness metadata only.</li>
            <li>Mutating routes are labeled but never executed from this page.</li>
            <li>Route/fuel and interception details remain non-authoritative hints.</li>
            <li>All responses are development tooling, not production gameplay APIs.</li>
          </ul>
        </UiCard>
      </div>

      {summary && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Operational summary</p>
              <h3>Fleet footprint</h3>
            </div>
            <UiBadge>{formatCompactGuid(uiState?.civilizationId)}</UiBadge>
          </div>
          <div className="figma-stat-grid">
            <SummaryMetric label="Groups" value={summary.groups} />
            <SummaryMetric label="Active transfers" value={summary.transfers} />
            <SummaryMetric label="Resource contexts" value={summary.resourceContexts} />
            <SummaryMetric label="Action hints" value={summary.actionHints} />
          </div>
        </UiCard>
      )}

      {uiState && summary && summary.groups === 0 && summary.resourceContexts === 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Empty development state</p>
              <h3>No orbital groups deployed yet</h3>
              <p>
                This civilization currently has no orbital groups, active transfers,
                or local resource contexts in the development dataset.
              </p>
            </div>
            <UiBadge tone="warn">Safe zero-state</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Fleet counters remain at zero until groups are seeded or created elsewhere.</li>
            <li>Action manifests below still document available contracts, but no gameplay mutation can be executed from this screen.</li>
          </ul>
        </UiCard>
      )}

      {uiState?.interceptionNotes?.length ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Readiness</p>
              <h3>Interception notes</h3>
            </div>
            <UiBadge tone="warn">Informational only</UiBadge>
          </div>
          <ul className="stack-list compact-list">
            {uiState.interceptionNotes.map((note, index) => (
              <li key={`interception-${index}`}>{formatNote(note)}</li>
            ))}
          </ul>
        </UiCard>
      ) : null}

      {uiState?.resourceContexts?.length ? (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Local stockpiles</p>
              <h3>Resource contexts</h3>
            </div>
            <UiBadge>{uiState.resourceContexts.length} planets</UiBadge>
          </div>
          <div className="readiness-grid">
            {uiState.resourceContexts.map((context) => (
              <section key={context.planetId} className="subpanel figma-subpanel">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Current planet</p>
                    <h4>{formatCompactGuid(context.planetId)}</h4>
                  </div>
                  <UiBadge tone="resource">
                    {(context.balances ?? []).length} balances
                  </UiBadge>
                </div>
                <div className="figma-data-list">
                  {(context.balances ?? []).map((balance) => (
                    <FleetDataRow
                      key={`${context.planetId}-${balance.resourceType}`}
                      label={formatResourceType(balance.resourceType)}
                      value={String(balance.quantity)}
                    />
                  ))}
                </div>
              </section>
            ))}
          </div>
        </UiCard>
      ) : null}

      {uiState?.groups.length ? (
        <div className="figma-fleet-grid">
          {uiState.groups.map((group) => {
            const transferProgress = getTransferProgress(group);

            return (
              <UiCard key={group.id} className="panel figma-fleet-card">
                <div className="figma-section-header">
                  <div>
                    <p className="eyebrow">Fleet group</p>
                    <h3>{formatSpaceAssetType(group.assetType)}</h3>
                    <p>{formatCompactGuid(group.id)}</p>
                  </div>
                  <UiBadge tone={getGroupTone(group)}>{formatOrbitalGroupStatus(group.status)}</UiBadge>
                </div>

                <div className="figma-stat-grid">
                  <SummaryMetric label="Quantity" value={group.quantity} />
                  <SummaryMetric
                    label="Transfer distance"
                    value={group.activeTransfer?.abstractDistanceUnits ?? 0}
                  />
                </div>

                <div className="figma-data-list">
                  <FleetDataRow label="Current planet" value={formatCompactGuid(group.currentPlanetId)} />
                  <FleetDataRow label="Origin planet" value={formatCompactGuid(group.originPlanetId)} />
                  <FleetDataRow
                    label="Stationed away"
                    value={formatBooleanLabel(group.isStationedAwayFromOrigin)}
                  />
                </div>

                <div className="figma-badge-row">
                  {group.commands?.canCreateTransfer && <UiBadge tone="good">Transfer available</UiBadge>}
                  {group.commands?.canSplit && <UiBadge>Split ready</UiBadge>}
                  {group.commands?.canMerge && <UiBadge>Merge ready</UiBadge>}
                  {group.commands?.canCancelTransfer && (
                    <UiBadge tone="warn">Cancellation available</UiBadge>
                  )}
                  {group.routeFuelReadiness?.fuelReadinessPolicy && (
                    <UiBadge>{formatFuelReadinessPolicy(group.routeFuelReadiness.fuelReadinessPolicy)}</UiBadge>
                  )}
                </div>

                {group.activeTransfer && (
                  <div className="figma-transfer-card">
                    <div className="figma-section-header">
                      <div>
                        <p className="eyebrow">Active transfer</p>
                        <h4>{formatTransferStatus(group.activeTransfer.status)}</h4>
                      </div>
                      <UiBadge tone="warn">{formatCompactGuid(group.activeTransfer.destinationPlanetId)}</UiBadge>
                    </div>
                    {transferProgress !== null && (
                      <UiProgressBar value={transferProgress} tone="neutral" />
                    )}
                    <div className="figma-data-list">
                      <FleetDataRow
                        label="Departure"
                        value={group.activeTransfer.departureAtUtc}
                      />
                      <FleetDataRow
                        label="Arrival"
                        value={group.activeTransfer.arrivalAtUtc}
                      />
                    </div>
                    {group.activeTransfer.interceptionReadiness?.readinessNote && (
                      <p className="figma-panel-note">
                        {group.activeTransfer.interceptionReadiness.readinessNote}
                      </p>
                    )}
                  </div>
                )}

                {group.routeFuelReadiness?.notes?.length ? (
                  <ul className="stack-list compact-list">
                    {group.routeFuelReadiness.notes.map((note) => (
                      <li key={note}>{note}</li>
                    ))}
                  </ul>
                ) : null}
              </UiCard>
            );
          })}
        </div>
      ) : null}

      {fleetManifest.length > 0 && (
        <ActionManifestPanel title="Fleet action manifest" actions={fleetManifest} />
      )}

      {strategicMapManifest.length > 0 && (
        <ActionManifestPanel
          title="Strategic map action manifest"
          actions={strategicMapManifest}
        />
      )}
    </section>
  );
}

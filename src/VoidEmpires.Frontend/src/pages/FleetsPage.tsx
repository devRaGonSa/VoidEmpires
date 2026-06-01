import { FormEvent, useMemo, useState } from "react";
import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetUiState } from "../api/fleetTypes";
import type { ReadinessNote } from "../api/strategicMapTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { ActionManifestPanel } from "../components/ActionManifestPanel";
import { FleetSummaryPanel } from "../components/FleetSummaryPanel";
import { StatusBadge } from "../components/StatusBadge";

function formatNote(note: ReadinessNote) {
  if (typeof note === "string") {
    return note;
  }

  return note.note ?? "Readiness metadata present.";
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
      <article className="panel panel-hero">
        <StatusBadge>Phase 9D read panels</StatusBadge>
        <h2>Fleet UI state and action manifests</h2>
        <p>
          This page inspects read-only fleet state plus machine-readable action
          manifests. Mutating backend actions are labeled but intentionally not
          executable from the frontend prototype.
        </p>
      </article>

      <article className="panel">
        <h3>Load fleet inspection state</h3>
        <form className="query-form" onSubmit={handleSubmit}>
          <label className="field">
            <span>Civilization id</span>
            <input
              type="text"
              value={civilizationId}
              onChange={(event) => setCivilizationId(event.target.value)}
              placeholder="00000000-0000-0000-0000-000000000000"
              spellCheck={false}
            />
          </label>
          <button type="submit" disabled={isLoading}>
            {isLoading ? "Loading..." : "Load fleet panels"}
          </button>
        </form>
        {error && <p className="error-text">{error}</p>}
      </article>

      <article className="panel">
        <h3>Constraints</h3>
        <ul className="stack-list">
          <li>Action manifests are documentation and readiness metadata only.</li>
          <li>Mutating actions stay unwired in this frontend phase.</li>
          <li>Interception and route/fuel details remain read-only metadata.</li>
          <li>Dev endpoints are not production APIs.</li>
        </ul>
      </article>

      {summary && (
        <article className="panel">
          <h3>Fleet summary</h3>
          <div className="stat-grid">
            <div className="stat-tile">
              <strong>{summary.groups}</strong>
              <span>Groups</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.transfers}</strong>
              <span>Active transfers</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.resourceContexts}</strong>
              <span>Resource contexts</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.actionHints}</strong>
              <span>Action hints</span>
            </div>
          </div>
        </article>
      )}

      {uiState?.interceptionNotes?.length ? (
        <article className="panel">
          <h3>Interception readiness notes</h3>
          <ul className="stack-list compact-list">
            {uiState.interceptionNotes.map((note, index) => (
              <li key={`interception-${index}`}>{formatNote(note)}</li>
            ))}
          </ul>
        </article>
      ) : null}

      {uiState?.resourceContexts?.length ? (
        <article className="panel">
          <h3>Resource contexts</h3>
          <div className="readiness-grid">
            {uiState.resourceContexts.map((context) => (
              <section key={context.planetId} className="subpanel">
                <h4>{context.planetId}</h4>
                <ul className="stack-list compact-list">
                  {(context.balances ?? []).map((balance) => (
                    <li key={`${context.planetId}-${balance.resourceType}`}>
                      {balance.resourceType}: {balance.quantity}
                    </li>
                  ))}
                </ul>
              </section>
            ))}
          </div>
        </article>
      ) : null}

      {uiState?.groups.map((group) => (
        <FleetSummaryPanel key={group.id} group={group} />
      ))}

      {fleetManifest.length > 0 && (
        <ActionManifestPanel
          title="Fleet action manifest"
          actions={fleetManifest}
        />
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

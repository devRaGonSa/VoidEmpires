import { FormEvent, useMemo, useState } from "react";
import type {
  ReadinessNote,
  StrategicMapResult,
} from "../api/strategicMapTypes";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { StatusBadge } from "../components/StatusBadge";
import { StrategicMapSystemCard } from "../components/StrategicMapSystemCard";

const readinessSections = [
  { label: "Route and fuel notes", key: "routeFuelNotes" },
  { label: "Sensor notes", key: "sensorNotes" },
  { label: "Detection notes", key: "detectionNotes" },
  { label: "Interception notes", key: "interceptionNotes" },
  { label: "Alliance notes", key: "allianceNotes" },
  { label: "Alliance pact notes", key: "alliancePactNotes" },
] as const;

function formatNote(note: ReadinessNote) {
  if (typeof note === "string") {
    return note;
  }

  return note.note ?? "Readiness metadata present.";
}

export function StrategicMapPage() {
  const [civilizationId, setCivilizationId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [result, setResult] = useState<StrategicMapResult | null>(null);

  const summary = useMemo(() => {
    if (!result) {
      return null;
    }

    return result.systems.reduce(
      (accumulator, system) => {
        accumulator.systems += 1;
        accumulator.planets += system.planets?.length ?? 0;
        accumulator.fleets += system.fleetPresence?.length ?? 0;
        accumulator.transfers += system.transferOverlays?.length ?? 0;
        return accumulator;
      },
      { systems: 0, planets: 0, fleets: 0, transfers: 0 },
    );
  }, [result]);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationId.trim();
    if (!trimmedCivilizationId) {
      setError("Civilization id is required.");
      setResult(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await voidEmpiresApi.getStrategicMap(trimmedCivilizationId);
      if (!response.succeeded || !response.map) {
        setResult(null);
        setError(response.errors[0] ?? "Strategic map request failed.");
        return;
      }

      setResult(response.map);
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Strategic map request failed.";
      setResult(null);
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <section className="page-grid">
      <article className="panel panel-hero">
        <StatusBadge>Phase 9C read slice</StatusBadge>
        <h2>Strategic map development read</h2>
        <p>
          This screen consumes the development-only strategic map endpoint as a
          read-only prototype. Readiness metadata is shown conservatively and is
          not treated as gameplay authorization.
        </p>
      </article>

      <article className="panel">
        <h3>Load civilization map state</h3>
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
            {isLoading ? "Loading..." : "Load strategic map"}
          </button>
        </form>
        {error && <p className="error-text">{error}</p>}
      </article>

      <article className="panel">
        <h3>Constraints</h3>
        <ul className="stack-list">
          <li>Endpoint is development-only and may return `404` or `503`.</li>
          <li>Data is read-only; no gameplay mutation commands are wired.</li>
          <li>Readiness metadata does not grant authorization.</li>
          <li>No production auth or 3D rendering is introduced here.</li>
        </ul>
      </article>

      {summary && (
        <article className="panel">
          <h3>Map summary</h3>
          <div className="stat-grid">
            <div className="stat-tile">
              <strong>{summary.systems}</strong>
              <span>Systems</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.planets}</strong>
              <span>Planets</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.fleets}</strong>
              <span>Fleet markers</span>
            </div>
            <div className="stat-tile">
              <strong>{summary.transfers}</strong>
              <span>Transfer overlays</span>
            </div>
          </div>
        </article>
      )}

      {result && (
        <article className="panel">
          <h3>Readiness metadata</h3>
          <div className="readiness-grid">
            {readinessSections.map((section) => {
              const notes = result[section.key];
              if (!notes?.length) {
                return null;
              }

              return (
                <section key={section.key} className="subpanel">
                  <h4>{section.label}</h4>
                  <ul className="stack-list compact-list">
                    {notes.map((note, index) => (
                      <li key={`${section.key}-${index}`}>{formatNote(note)}</li>
                    ))}
                  </ul>
                </section>
              );
            })}

            <section className="subpanel">
              <h4>Alliance readiness</h4>
              <p>{result.allianceReadiness?.length ?? 0} membership rows exposed.</p>
            </section>

            <section className="subpanel">
              <h4>Alliance pact readiness</h4>
              <p>{result.alliancePacts?.length ?? 0} pact rows exposed.</p>
            </section>
          </div>
        </article>
      )}

      {result?.systems.map((system) => (
        <StrategicMapSystemCard key={system.systemId} system={system} />
      ))}
    </section>
  );
}

import { FormEvent, useMemo, useState } from "react";
import type {
  ReadinessNote,
  StrategicMapResult,
  StrategicMapSystem,
} from "../api/strategicMapTypes";
import type {
  PlanetVisualStateResponse,
  SystemVisualStateResponse,
} from "../api/voidEmpiresApi";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import { StrategicMap2DView } from "../components/StrategicMap2DView";
import { UiBadge } from "../components/ui/UiBadge";
import { UiCard } from "../components/ui/UiCard";
import { UiProgressBar } from "../components/ui/UiProgressBar";
import {
  formatBooleanLabel,
  formatColonizationStatus,
  formatCommandBlockReason,
  formatCompactGuid,
  formatPlanetType,
  formatStarType,
  formatVisibilityLevel,
  formatVisibilityReason,
  isOwnedVisibilityLevel,
  isVisibleVisibilityLevel,
} from "../utils/domainPresentation";

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

function readText(value: unknown, fallback = "Unavailable") {
  return typeof value === "string" && value.length > 0 ? value : fallback;
}

function readCommands(value: unknown) {
  return Array.isArray(value)
    ? value.filter(
        (
          item,
        ): item is {
          actionKey?: string;
          note?: string;
          isAvailable?: boolean;
          blockReason?: string;
        } => typeof item === "object" && item !== null,
      )
    : [];
}

function readRecord(value: unknown) {
  return value as Record<string, unknown>;
}

function formatJson(value: unknown) {
  return JSON.stringify(value, null, 2);
}

function formatCoordinate(value: number | null) {
  return value ?? "?";
}

function readDomainValue(record: Record<string, unknown>, key: string) {
  const value = record[key];
  return typeof value === "string" || typeof value === "number" ? value : null;
}

function getVisibilityTone(level: string): "good" | "warn" | "neutral" {
  if (isOwnedVisibilityLevel(level)) {
    return "good";
  }

  if (isVisibleVisibilityLevel(level)) {
    return "neutral";
  }

  return "warn";
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

interface DataRowProps {
  label: string;
  value: string;
}

function DataRow({ label, value }: DataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

interface IntensityRowProps {
  label: string;
  value: number | null | undefined;
}

function formatPercentage(value: number | null | undefined) {
  if (typeof value !== "number" || Number.isNaN(value)) {
    return "No disponible";
  }

  return `${Math.round(Math.max(0, Math.min(1, value)) * 100)}%`;
}

function IntensityRow({ label, value }: IntensityRowProps) {
  const percent =
    typeof value === "number" && !Number.isNaN(value)
      ? Math.max(0, Math.min(100, value * 100))
      : 0;

  return (
    <div>
      <div className="figma-data-row">
        <span>{label}</span>
        <strong>{formatPercentage(value)}</strong>
      </div>
      <UiProgressBar value={percent} tone="neutral" />
    </div>
  );
}

export function StrategicMapPage() {
  const [civilizationId, setCivilizationId] = useState("");
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [result, setResult] = useState<StrategicMapResult | null>(null);
  const [selectedSystemId, setSelectedSystemId] = useState<string | null>(null);
  const [selectedPlanetId, setSelectedPlanetId] = useState<string | null>(null);
  const [isLoadingSystemVisual, setIsLoadingSystemVisual] = useState(false);
  const [systemVisualError, setSystemVisualError] = useState<string | null>(null);
  const [systemVisualState, setSystemVisualState] =
    useState<SystemVisualStateResponse["visualState"]>(null);
  const [isLoadingPlanetVisual, setIsLoadingPlanetVisual] = useState(false);
  const [planetVisualError, setPlanetVisualError] = useState<string | null>(null);
  const [planetVisualState, setPlanetVisualState] =
    useState<PlanetVisualStateResponse["visualState"]>(null);

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

  const selectedSystem = useMemo(
    () =>
      result?.systems.find((system) => system.systemId === selectedSystemId) ??
      result?.systems[0] ??
      null,
    [result, selectedSystemId],
  );

  const selectedPlanet = useMemo(
    () =>
      selectedSystem?.planets?.find((planet) => planet.planetId === selectedPlanetId) ??
      selectedSystem?.planets?.[0] ??
      null,
    [selectedPlanetId, selectedSystem],
  );

  const systemCommands = useMemo(
    () => (selectedSystem ? readCommands(readRecord(selectedSystem).commands) : []),
    [selectedSystem],
  );

  const planetCommands = useMemo(
    () => (selectedPlanet ? readCommands(readRecord(selectedPlanet).commands) : []),
    [selectedPlanet],
  );

  const selectedSystemRecord = selectedSystem ? readRecord(selectedSystem) : null;
  const selectedPlanetRecord = selectedPlanet ? readRecord(selectedPlanet) : null;

  function selectSystem(system: StrategicMapSystem) {
    setSelectedSystemId(system.systemId);
    setSelectedPlanetId(system.planets?.[0]?.planetId ?? null);
    setSystemVisualState(null);
    setSystemVisualError(null);
    setPlanetVisualState(null);
    setPlanetVisualError(null);
  }

  async function loadSystemVisualState() {
    if (!selectedSystem) {
      return;
    }

    setIsLoadingSystemVisual(true);
    setSystemVisualError(null);
    try {
      const response = await voidEmpiresApi.getSystemVisualState(selectedSystem.systemId);
      setSystemVisualState(response.succeeded ? response.visualState : null);
      setSystemVisualError(
        response.succeeded
          ? null
          : response.errors[0] ?? "System visual-state request failed.",
      );
    } catch (requestError) {
      setSystemVisualState(null);
      setSystemVisualError(
        requestError instanceof Error
          ? requestError.message
          : "System visual-state request failed.",
      );
    } finally {
      setIsLoadingSystemVisual(false);
    }
  }

  async function loadPlanetVisualState() {
    if (!selectedPlanet?.isVisible) {
      setPlanetVisualState(null);
      setPlanetVisualError("Select a visible planet to inspect its visual state.");
      return;
    }

    setIsLoadingPlanetVisual(true);
    setPlanetVisualError(null);
    try {
      const response = await voidEmpiresApi.getPlanetVisualState(selectedPlanet.planetId);
      setPlanetVisualState(response.succeeded ? response.visualState : null);
      setPlanetVisualError(
        response.succeeded
          ? null
          : response.errors[0] ?? "Planet visual-state request failed.",
      );
    } catch (requestError) {
      setPlanetVisualState(null);
      setPlanetVisualError(
        requestError instanceof Error
          ? requestError.message
          : "Planet visual-state request failed.",
      );
    } finally {
      setIsLoadingPlanetVisual(false);
    }
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const trimmedCivilizationId = civilizationId.trim();
    if (!trimmedCivilizationId) {
      setError("Civilization id is required.");
      setResult(null);
      setSelectedSystemId(null);
      setSelectedPlanetId(null);
      setSystemVisualState(null);
      setSystemVisualError(null);
      setPlanetVisualState(null);
      setPlanetVisualError(null);
      return;
    }

    setIsLoading(true);
    setError(null);

    try {
      const response = await voidEmpiresApi.getStrategicMap(trimmedCivilizationId);
      if (!response.succeeded || !response.map) {
        setResult(null);
        setSelectedSystemId(null);
        setSelectedPlanetId(null);
        setSystemVisualState(null);
        setSystemVisualError(null);
        setPlanetVisualState(null);
        setPlanetVisualError(null);
        setError(response.errors[0] ?? "Strategic map request failed.");
        return;
      }

      setResult(response.map);
      setSelectedSystemId(response.map.systems[0]?.systemId ?? null);
      setSelectedPlanetId(response.map.systems[0]?.planets?.[0]?.planetId ?? null);
      setSystemVisualState(null);
      setSystemVisualError(null);
      setPlanetVisualState(null);
      setPlanetVisualError(null);
    } catch (requestError) {
      const message =
        requestError instanceof Error
          ? requestError.message
          : "Strategic map request failed.";
      setResult(null);
      setSelectedSystemId(null);
      setSelectedPlanetId(null);
      setSystemVisualState(null);
      setSystemVisualError(null);
      setPlanetVisualState(null);
      setPlanetVisualError(null);
      setError(message);
    } finally {
      setIsLoading(false);
    }
  }

  return (
    <section className="page-grid">
      <UiCard className="panel panel-hero figma-hero-card">
        <div className="figma-hero-copy">
          <UiBadge tone="resource">Phase 9M map alignment</UiBadge>
          <h2>Strategic map development read</h2>
          <p>
            The map, selection panel, and renderer previews now share the same
            compact Figma-derived panel language while staying strictly
            development-only and read-only.
          </p>
        </div>
        <div className="figma-badge-row">
          <UiBadge>Backend coordinates preserved</UiBadge>
          <UiBadge>Read-only contract surface</UiBadge>
          <UiBadge tone="warn">No gameplay mutations</UiBadge>
        </div>
      </UiCard>

      <div className="figma-two-column">
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Development endpoint</p>
              <h3>Load civilization map state</h3>
            </div>
            <UiBadge>Safe inspection only</UiBadge>
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
              {isLoading ? "Loading..." : "Load strategic map"}
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
            <UiBadge tone="warn">Dev-only</UiBadge>
          </div>
          <ul className="stack-list">
            <li>Endpoint may return `404` or `503` outside dev-ready backend runs.</li>
            <li>Data is read-only and does not grant gameplay authorization.</li>
            <li>Readiness metadata is informative, not command authority.</li>
            <li>No auth, renderer execution, or mutating controls are added here.</li>
          </ul>
        </UiCard>
      </div>

      {summary && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Operational summary</p>
              <h3>Map footprint</h3>
            </div>
            <UiBadge>{formatCompactGuid(result?.civilizationId)}</UiBadge>
          </div>
          <div className="figma-stat-grid">
            <SummaryMetric label="Systems" value={summary.systems} />
            <SummaryMetric label="Planets" value={summary.planets} />
            <SummaryMetric label="Fleet markers" value={summary.fleets} />
            <SummaryMetric label="Transfer overlays" value={summary.transfers} />
          </div>
        </UiCard>
      )}

      {result && (
        <UiCard className="panel">
          <div className="figma-map-layout">
            <div className="figma-map-stage">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Galaxia</p>
                  <h3>Strategic map 2D view</h3>
                  <p>
                    Backend coordinates are projected into a deterministic SVG
                    viewport. The view is visual-only and keeps empty and
                    single-system states safe.
                  </p>
                </div>
                <UiBadge>SVG read model</UiBadge>
              </div>
              <StrategicMap2DView
                systems={result.systems}
                selectedSystemId={selectedSystem?.systemId}
                onSelectSystem={(systemId) => {
                  const system = result.systems.find((item) => item.systemId === systemId);
                  if (system) {
                    selectSystem(system);
                  }
                }}
              />
            </div>

            <aside className="figma-map-sidebar">
              <div className="figma-mini-card">
                <p className="eyebrow">Legend</p>
                <div className="figma-legend-grid">
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-owned" />
                    <strong>Propio</strong>
                    <small>Direct control</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-visible" />
                    <strong>Visible</strong>
                    <small>Observed system</small>
                  </div>
                  <div className="figma-legend-item">
                    <span className="figma-legend-dot figma-legend-unknown" />
                    <strong>Desconocido</strong>
                    <small>Sanitized contact</small>
                  </div>
                </div>
              </div>

              {selectedSystem && (
                <div className="figma-mini-card">
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Active focus</p>
                      <h4>{selectedSystem.systemName ?? "Unknown system"}</h4>
                    </div>
                    <UiBadge tone={getVisibilityTone(selectedSystem.visibilityLevel)}>
                      {formatVisibilityLevel(selectedSystem.visibilityLevel)}
                    </UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <DataRow
                      label="Coordinates"
                      value={`${formatCoordinate(selectedSystem.coordinateX)}, ${formatCoordinate(selectedSystem.coordinateY)}, ${formatCoordinate(selectedSystem.coordinateZ)}`}
                    />
                    <DataRow
                      label="Planets"
                      value={String(selectedSystem.planets?.length ?? 0)}
                    />
                    <DataRow
                      label="Fleet markers"
                      value={String(selectedSystem.fleetPresence?.length ?? 0)}
                    />
                    <DataRow
                      label="Transfers"
                      value={String(selectedSystem.transferOverlays?.length ?? 0)}
                    />
                  </div>
                </div>
              )}
            </aside>
          </div>
        </UiCard>
      )}

      {result && result.systems.length === 0 && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Empty development state</p>
              <h3>No relevant systems found yet</h3>
              <p>
                The current civilization has no owned or discovered systems in this
                development dataset, so Galaxia stays on a safe read-only zero state.
              </p>
            </div>
            <UiBadge tone="warn">0 visible systems</UiBadge>
          </div>
          <ul className="stack-list">
            <li>System, planet, fleet-marker, and transfer counters remain at zero.</li>
            <li>The map preview is intentionally visual-only and does not create data; load seeded backend data to inspect populated systems.</li>
          </ul>
        </UiCard>
      )}

      {result && selectedSystem && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Focused system</p>
              <h3>Selection detail</h3>
              <p>
                System, planet, and command metadata stay backend-driven and
                visually grouped for quick inspection.
              </p>
            </div>
            <UiBadge tone="warn">Non-authoritative readiness</UiBadge>
          </div>

          <div className="selection-chip-row">
            {result.systems.map((system) => (
              <button
                key={system.systemId}
                type="button"
                className={`selection-chip${selectedSystem.systemId === system.systemId ? " selection-chip-active" : ""}`}
                onClick={() => selectSystem(system)}
                aria-pressed={selectedSystem.systemId === system.systemId}
              >
                {system.systemName ?? "Unknown system"}
              </button>
            ))}
          </div>

          <div className="figma-detail-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">System overview</p>
                  <h4>{selectedSystem.systemName ?? "Unknown system"}</h4>
                </div>
                <UiBadge tone={getVisibilityTone(selectedSystem.visibilityLevel)}>
                  {formatVisibilityReason(selectedSystem.visibilityReason)}
                </UiBadge>
              </div>
              <div className="figma-data-list">
                <DataRow
                  label="Coordinates"
                  value={`${formatCoordinate(selectedSystem.coordinateX)}, ${formatCoordinate(selectedSystem.coordinateY)}, ${formatCoordinate(selectedSystem.coordinateZ)}`}
                />
                {readDomainValue(selectedSystemRecord ?? {}, "starType") && (
                  <DataRow
                    label="Star type"
                    value={formatStarType(
                      readDomainValue(selectedSystemRecord ?? {}, "starType"),
                    )}
                  />
                )}
                <DataRow
                  label="Owned by requester"
                  value={Boolean(selectedSystemRecord?.isOwnedByRequestingCivilization) ? "Si" : "No"}
                />
                <DataRow
                  label="Sensor summaries"
                  value={String(
                    (selectedSystem.sensorProfiles?.length ?? 0) +
                      (selectedSystem.detectionCoverage?.length ?? 0),
                  )}
                />
              </div>
              {systemCommands.length > 0 && (
                <div className="figma-command-list">
                  {systemCommands.map((command) => (
                    <div
                      key={command.actionKey}
                      className="figma-command-item"
                    >
                      <div>
                        <strong>{readText(command.actionKey, "unknown.command")}</strong>
                        <p>{readText(command.note, "Read-only metadata")}</p>
                      </div>
                      <UiBadge tone={command.isAvailable ? "good" : "warn"}>
                        {command.isAvailable ? "Disponible" : formatCommandBlockReason(command.blockReason, "Bloqueado")}
                      </UiBadge>
                    </div>
                  ))}
                </div>
              )}
            </section>

            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Planet selector</p>
                  <h4>Planet details</h4>
                </div>
                <UiBadge>{selectedSystem.planets?.length ?? 0} planets</UiBadge>
              </div>

              <div className="selection-chip-row">
                {selectedSystem.planets?.map((planet) => (
                  <button
                    key={planet.planetId}
                    type="button"
                    className={`selection-chip${selectedPlanet?.planetId === planet.planetId ? " selection-chip-active" : ""}`}
                    onClick={() => {
                      setSelectedPlanetId(planet.planetId);
                      setPlanetVisualState(null);
                      setPlanetVisualError(null);
                    }}
                    aria-pressed={selectedPlanet?.planetId === planet.planetId}
                    title={planet.planetId}
                  >
                    {planet.planetName ?? "Unknown planet"} ·{" "}
                    {formatPlanetType(readDomainValue(readRecord(planet), "planetType"), "Tipo pendiente")} ·{" "}
                    {Boolean(readRecord(planet).isOwnedByRequestingCivilization)
                      ? "Propio"
                      : formatVisibilityLevel(planet.visibilityLevel)}{" "}
                    ·{" "}
                    {formatColonizationStatus(
                      readDomainValue(readRecord(planet), "colonizationStatus"),
                      "Colonizacion pendiente",
                    )}
                  </button>
                ))}
              </div>

              {selectedPlanet ? (
                <>
                  <div className="figma-section-header">
                    <div>
                      <p className="eyebrow">Selected planet</p>
                      <h4>{selectedPlanet.planetName ?? "Unknown planet"}</h4>
                    </div>
                    <UiBadge tone={getVisibilityTone(selectedPlanet.visibilityLevel)}>
                      {Boolean(selectedPlanetRecord?.isOwnedByRequestingCivilization)
                        ? "Propio"
                        : formatVisibilityLevel(selectedPlanet.visibilityLevel)}
                    </UiBadge>
                  </div>
                  <div className="figma-data-list">
                    <DataRow
                      label="Estado"
                      value={`${formatVisibilityLevel(selectedPlanet.visibilityLevel)} (${formatVisibilityReason(selectedPlanet.visibilityReason)})`}
                    />
                    <DataRow
                      label="Type"
                      value={formatPlanetType(readDomainValue(selectedPlanetRecord ?? {}, "planetType"))}
                    />
                    <DataRow
                      label="Colonization"
                      value={formatColonizationStatus(
                        readDomainValue(selectedPlanetRecord ?? {}, "colonizationStatus"),
                      )}
                    />
                    <DataRow
                      label="Id breve"
                      value={formatCompactGuid(selectedPlanet.planetId)}
                    />
                  </div>

                  {planetCommands.length > 0 && (
                    <div className="figma-command-list">
                      {planetCommands.map((command) => (
                        <div
                          key={command.actionKey}
                          className="figma-command-item"
                        >
                          <div>
                            <strong>{readText(command.actionKey, "unknown.command")}</strong>
                            <p>{readText(command.note, "Read-only metadata")}</p>
                          </div>
                          <UiBadge tone={command.isAvailable ? "good" : "warn"}>
                            {command.isAvailable ? "Disponible" : formatCommandBlockReason(command.blockReason, "Bloqueado")}
                          </UiBadge>
                        </div>
                      ))}
                    </div>
                  )}
                </>
              ) : (
                <p>No planets are available for the selected system.</p>
              )}
            </section>

            <section className="subpanel figma-subpanel">
              <div className="figma-section-header">
                <div>
                  <p className="eyebrow">Readiness scope</p>
                  <h4>Current metadata exposure</h4>
                </div>
                <UiBadge>Inspection only</UiBadge>
              </div>
              <div className="figma-data-list">
                <DataRow
                  label="Alliance rows"
                  value={String(result.allianceReadiness?.length ?? 0)}
                />
                <DataRow
                  label="Alliance pact rows"
                  value={String(result.alliancePacts?.length ?? 0)}
                />
                <DataRow
                  label="Fleet notes"
                  value={String(result.interceptionNotes?.length ?? 0)}
                />
                <DataRow
                  label="Route/fuel notes"
                  value={String(result.routeFuelNotes?.length ?? 0)}
                />
              </div>
              <p className="figma-panel-note">
                Readiness hints do not bypass server validation and should be
                treated as UI planning signals only.
              </p>
            </section>
          </div>
        </UiCard>
      )}

      {result && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Renderer-facing contracts</p>
              <h3>Visual-state preview</h3>
              <p>
                These development-only reads expose payloads for future
                renderer work. The preview stays compact, readable, and clearly
                non-final.
              </p>
            </div>
            <UiBadge tone="warn">Dev-only payloads</UiBadge>
          </div>

          <div className="figma-detail-grid">
            <section className="subpanel figma-subpanel">
              <div className="figma-preview-actions">
                <div>
                  <p className="eyebrow">System preview</p>
                  <h4>System visual state</h4>
                </div>
                <button
                  type="button"
                  className="selection-chip"
                  onClick={loadSystemVisualState}
                  disabled={!selectedSystem || isLoadingSystemVisual}
                >
                  {isLoadingSystemVisual ? "Loading..." : "Load system visual state"}
                </button>
              </div>
              {systemVisualError && <p className="error-text">{systemVisualError}</p>}
              {systemVisualState && (
                <>
                  <div className="figma-data-list">
                    <DataRow
                      label="Star class"
                      value={readText(systemVisualState.star?.visualClass)}
                    />
                    <DataRow
                      label="Star type"
                      value={formatStarType(systemVisualState.star?.starType)}
                    />
                    <DataRow
                      label="Layout hints"
                      value={String(systemVisualState.layoutHints?.length ?? 0)}
                    />
                    <DataRow
                      label="Transfer overlays"
                      value={String(systemVisualState.transferOverlays?.length ?? 0)}
                    />
                  </div>
                  <details className="json-details">
                    <summary>Raw system payload</summary>
                    <pre className="json-preview">{formatJson(systemVisualState)}</pre>
                  </details>
                </>
              )}
            </section>

            <section className="subpanel figma-subpanel">
              <div className="figma-preview-actions">
                <div>
                  <p className="eyebrow">Planet preview</p>
                  <h4>Planet visual state</h4>
                </div>
                <button
                  type="button"
                  className="selection-chip"
                  onClick={loadPlanetVisualState}
                  disabled={!selectedPlanet || isLoadingPlanetVisual}
                >
                  {isLoadingPlanetVisual ? "Loading..." : "Load planet visual state"}
                </button>
              </div>
              {!selectedPlanet?.isVisible && (
                <p>Only visible planets should be inspected through this preview.</p>
              )}
              {planetVisualError && <p className="error-text">{planetVisualError}</p>}
              {planetVisualState && (
                <>
                  <div className="figma-data-list">
                    <DataRow
                      label="Planet name"
                      value={readText(planetVisualState.planetName)}
                    />
                    <DataRow
                      label="Planet type"
                      value={formatPlanetType(planetVisualState.planetType)}
                    />
                    <DataRow
                      label="Colonization"
                      value={formatColonizationStatus(planetVisualState.colonizationStatus)}
                    />
                    <DataRow
                      label="Visual seed"
                      value={String(planetVisualState.visualSeed ?? "Unavailable")}
                    />
                    <DataRow
                      label="Surface profile"
                      value={readText(planetVisualState.profile?.surfaceProfile)}
                    />
                    <DataRow
                      label="Light distribution"
                      value={readText(planetVisualState.profile?.lightDistributionMode)}
                    />
                    <DataRow
                      label="Platform mode"
                      value={readText(planetVisualState.profile?.platformMode)}
                    />
                    <DataRow
                      label="Atmosphere profile"
                      value={readText(planetVisualState.profile?.atmosphereProfile)}
                    />
                    <DataRow
                      label="Cloud profile"
                      value={readText(planetVisualState.profile?.cloudProfile)}
                    />
                    <DataRow
                      label="Night lights"
                      value={formatBooleanLabel(Boolean(planetVisualState.profile?.supportsNightLights))}
                    />
                    <DataRow
                      label="Surface platforms"
                      value={formatBooleanLabel(
                        Boolean(planetVisualState.profile?.supportsSurfacePlatforms),
                      )}
                    />
                    <DataRow
                      label="Orbital mega hints"
                      value={formatBooleanLabel(
                        Boolean(
                          planetVisualState.profile?.supportsOrbitalMegastructureHints,
                        ),
                      )}
                    />
                    {planetVisualState.profile?.paletteKey && (
                      <DataRow
                        label="Palette profile"
                        value={readText(planetVisualState.profile.paletteKey)}
                      />
                    )}
                  </div>
                  <div className="readiness-grid">
                    <section className="subpanel figma-subpanel">
                      <h4>Intensities</h4>
                      <div className="stack-list compact-list">
                        <IntensityRow
                          label="Colonization intensity"
                          value={planetVisualState.colonizationIntensity}
                        />
                        <IntensityRow
                          label="Urban intensity"
                          value={planetVisualState.urbanIntensity}
                        />
                        <IntensityRow
                          label="Industrial intensity"
                          value={planetVisualState.industrialIntensity}
                        />
                        <IntensityRow
                          label="Terraforming intensity"
                          value={planetVisualState.terraformingIntensity}
                        />
                        <IntensityRow
                          label="Military intensity"
                          value={planetVisualState.militaryIntensity}
                        />
                        <IntensityRow
                          label="Orbital presence intensity"
                          value={planetVisualState.orbitalPresenceIntensity}
                        />
                      </div>
                    </section>
                  </div>
                  <details className="json-details">
                    <summary>Raw planet payload</summary>
                    <pre className="json-preview">{formatJson(planetVisualState)}</pre>
                  </details>
                </>
              )}
            </section>
          </div>
        </UiCard>
      )}

      {result && (
        <UiCard className="panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Readiness annotations</p>
              <h3>Metadata ledger</h3>
              <p>
                Route, sensor, detection, interception, and alliance notes are
                presented as compact side-information only.
              </p>
            </div>
            <UiBadge>{readinessSections.length} note groups</UiBadge>
          </div>

          <div className="readiness-grid">
            {readinessSections.map((section) => {
              const notes = result[section.key];
              if (!notes?.length) {
                return null;
              }

              return (
                <section key={section.key} className="subpanel figma-subpanel">
                  <h4>{section.label}</h4>
                  <ul className="stack-list compact-list">
                    {notes.map((note, index) => (
                      <li key={`${section.key}-${index}`}>{formatNote(note)}</li>
                    ))}
                  </ul>
                </section>
              );
            })}
          </div>
        </UiCard>
      )}
    </section>
  );
}

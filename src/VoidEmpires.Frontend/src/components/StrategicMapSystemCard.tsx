import type { StrategicMapSystem } from "../api/strategicMapTypes";
import { StatusBadge } from "./StatusBadge";

interface StrategicMapSystemCardProps {
  system: StrategicMapSystem;
}

function getVisibilityTone(level: string): "neutral" | "good" | "warn" {
  if (level === "Owned" || level === "Visible") {
    return "good";
  }

  return "warn";
}

export function StrategicMapSystemCard({
  system,
}: StrategicMapSystemCardProps) {
  const planets = system.planets ?? [];
  const visiblePlanets = planets.filter((planet) => planet.isVisible);

  return (
    <article className="panel system-card">
      <div className="system-card-header">
        <div>
          <h3>{system.systemName ?? "Unknown system"}</h3>
          <p>
            {system.coordinateX ?? "?"}, {system.coordinateY ?? "?"},{" "}
            {system.coordinateZ ?? "?"}
          </p>
        </div>
        <StatusBadge tone={getVisibilityTone(system.visibilityLevel)}>
          {system.visibilityLevel}
        </StatusBadge>
      </div>

      <div className="stat-grid">
        <div className="stat-tile">
          <strong>{planets.length}</strong>
          <span>Planets</span>
        </div>
        <div className="stat-tile">
          <strong>{visiblePlanets.length}</strong>
          <span>Visible planets</span>
        </div>
        <div className="stat-tile">
          <strong>{system.fleetPresence?.length ?? 0}</strong>
          <span>Fleet markers</span>
        </div>
        <div className="stat-tile">
          <strong>{system.transferOverlays?.length ?? 0}</strong>
          <span>Transfer overlays</span>
        </div>
      </div>

      <div className="inline-note-row">
        <StatusBadge>Reason: {system.visibilityReason}</StatusBadge>
        {(system.sensorProfiles?.length ?? 0) > 0 && (
          <StatusBadge>Sensor metadata present</StatusBadge>
        )}
        {(system.detectionCoverage?.length ?? 0) > 0 && (
          <StatusBadge>Detection metadata present</StatusBadge>
        )}
      </div>

      <ul className="stack-list compact-list">
        {planets.map((planet) => (
          <li key={planet.planetId}>
            <strong>{planet.planetName ?? "Unknown planet"}</strong> ·{" "}
            {planet.visibilityLevel} · {planet.visibilityReason}
          </li>
        ))}
      </ul>
    </article>
  );
}

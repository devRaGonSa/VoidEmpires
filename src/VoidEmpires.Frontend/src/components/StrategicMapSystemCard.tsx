import type { StrategicMapSystem } from "../api/strategicMapTypes";
import {
  formatVisibilityLevel,
  formatVisibilityReason,
} from "../utils/domainPresentation";
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
          <h3>{system.systemName ?? "Sistema desconocido"}</h3>
          <p>
            {system.coordinateX ?? "?"}, {system.coordinateY ?? "?"},{" "}
            {system.coordinateZ ?? "?"}
          </p>
        </div>
        <StatusBadge tone={getVisibilityTone(system.visibilityLevel)}>
          {formatVisibilityLevel(system.visibilityLevel)}
        </StatusBadge>
      </div>

      <div className="stat-grid">
        <div className="stat-tile">
          <strong>{planets.length}</strong>
          <span>Planetas</span>
        </div>
        <div className="stat-tile">
          <strong>{visiblePlanets.length}</strong>
          <span>Planetas visibles</span>
        </div>
        <div className="stat-tile">
          <strong>{system.fleetPresence?.length ?? 0}</strong>
          <span>Marcadores de flota</span>
        </div>
        <div className="stat-tile">
          <strong>{system.transferOverlays?.length ?? 0}</strong>
          <span>Rutas activas</span>
        </div>
      </div>

      <div className="inline-note-row">
        <StatusBadge>Motivo: {formatVisibilityReason(system.visibilityReason)}</StatusBadge>
        {(system.sensorProfiles?.length ?? 0) > 0 && (
          <StatusBadge>Lecturas de sensores disponibles</StatusBadge>
        )}
        {(system.detectionCoverage?.length ?? 0) > 0 && (
          <StatusBadge>Cobertura de deteccion disponible</StatusBadge>
        )}
      </div>

      <ul className="stack-list compact-list">
        {planets.map((planet) => (
          <li key={planet.planetId}>
            <strong>{planet.planetName ?? "Planeta desconocido"}</strong> ·{" "}
            {formatVisibilityLevel(planet.visibilityLevel)} ·{" "}
            {formatVisibilityReason(planet.visibilityReason)}
          </li>
        ))}
      </ul>
    </article>
  );
}

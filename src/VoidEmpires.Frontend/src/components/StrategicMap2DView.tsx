import type { StrategicMapSystem } from "../api/strategicMapTypes";
import {
  formatVisibilityLevel,
  isOwnedVisibilityLevel,
  isVisibleVisibilityLevel,
} from "../utils/domainPresentation";

interface StrategicMap2DViewProps {
  systems: StrategicMapSystem[];
  selectedSystemId?: string | null;
  onSelectSystem?: (systemId: string) => void;
}

const width = 960;
const height = 540;
const pad = 72;

function tone(level: string) {
  return isOwnedVisibilityLevel(level)
    ? "owned"
    : isVisibleVisibilityLevel(level)
      ? "visible"
      : "unknown";
}

function project(systems: StrategicMapSystem[]) {
  if (systems.length === 1) {
    return [{ system: systems[0], x: width / 2, y: height / 2 }];
  }

  const values = systems.map((system) => ({
    system,
    x: system.coordinateX ?? 0,
    y: system.coordinateY ?? 0,
  }));
  const xs = values.map((item) => item.x);
  const ys = values.map((item) => item.y);
  const minX = Math.min(...xs);
  const maxX = Math.max(...xs);
  const minY = Math.min(...ys);
  const maxY = Math.max(...ys);
  const spanX = maxX - minX || 1;
  const spanY = maxY - minY || 1;

  return values.map((item) => ({
    system: item.system,
    x: pad + ((item.x - minX) / spanX) * (width - pad * 2),
    y: pad + ((item.y - minY) / spanY) * (height - pad * 2),
  }));
}

export function StrategicMap2DView({
  systems,
  selectedSystemId,
  onSelectSystem,
}: StrategicMap2DViewProps) {
  if (systems.length === 0) {
    return (
      <p className="map-empty-state">
        No relevant systems were returned for this civilization.
      </p>
    );
  }

  return (
    <div className="strategic-map-view">
      <svg
        className="strategic-map-canvas"
        viewBox={`0 0 ${width} ${height}`}
        role="img"
        aria-label="Read-only two-dimensional strategic map"
      >
        <defs>
          <pattern id="map-grid" width="64" height="64" patternUnits="userSpaceOnUse">
            <path
              d="M 64 0 L 0 0 0 64"
              fill="none"
              stroke="rgba(132, 163, 214, 0.15)"
              strokeWidth="1"
            />
          </pattern>
          <radialGradient id="map-nebula" cx="50%" cy="18%" r="75%">
            <stop offset="0%" stopColor="rgba(70, 239, 255, 0.12)" />
            <stop offset="100%" stopColor="rgba(5, 8, 20, 0)" />
          </radialGradient>
        </defs>

        <rect width={width} height={height} className="map-backdrop" />
        <rect width={width} height={height} className="map-frame" />
        <rect width={width} height={height} fill="url(#map-nebula)" />

        {project(systems).map(({ system, x, y }) => (
          <g
            key={system.systemId}
            transform={`translate(${x} ${y})`}
            className={`map-node map-node-${tone(system.visibilityLevel)}${selectedSystemId === system.systemId ? " map-node-selected" : ""}`}
            role="button"
            tabIndex={0}
            aria-label={`Select ${system.systemName ?? "unknown system"}`}
            onClick={() => onSelectSystem?.(system.systemId)}
            onKeyDown={(event) => {
              if (event.key === "Enter" || event.key === " ") {
                event.preventDefault();
                onSelectSystem?.(system.systemId);
              }
            }}
          >
            <title>{system.systemName ?? "Unknown system"}</title>
            <circle r="30" className="map-node-halo" />
            {(system.transferOverlays?.length ?? 0) > 0 && (
              <>
                <path
                  d="M -24 -18 L -10 -4"
                  className="map-node-transfer-line"
                />
                <path
                  d="M -24 -4 L -10 10"
                  className="map-node-transfer-line"
                />
              </>
            )}
            <circle r="20" className="map-node-ring" />
            {tone(system.visibilityLevel) === "owned" ? (
              <circle r="7" className="map-node-core" />
            ) : tone(system.visibilityLevel) === "visible" ? (
              <polygon
                points="0,-8 8,0 0,8 -8,0"
                className="map-node-core map-node-core-diamond"
              />
            ) : (
              <rect
                x="-6.5"
                y="-6.5"
                width="13"
                height="13"
                rx="2.5"
                className="map-node-core map-node-core-square"
              />
            )}
            {(system.fleetPresence?.length ?? 0) > 0 && (
              <path
                d="M 18 -19 L 24 -11 L 12 -11 Z"
                className="map-node-indicator"
              />
            )}
            <text className="map-node-title" x="0" y="44">
              {system.systemName ?? "Unknown system"}
            </text>
            <text className="map-node-meta" x="0" y="63">
              {formatVisibilityLevel(system.visibilityLevel)} | P {system.planets?.length ?? 0} | F{" "}
              {system.fleetPresence?.length ?? 0} | T {system.transferOverlays?.length ?? 0}
            </text>
          </g>
        ))}
      </svg>
    </div>
  );
}

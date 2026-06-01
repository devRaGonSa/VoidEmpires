import type { StrategicMapSystem } from "../api/strategicMapTypes";

interface StrategicMap2DViewProps {
  systems: StrategicMapSystem[];
}

const width = 960;
const height = 540;
const pad = 72;

function tone(level: string) {
  return level === "Owned"
    ? "owned"
    : level === "Visible"
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

export function StrategicMap2DView({ systems }: StrategicMap2DViewProps) {
  if (systems.length === 0) {
    return <p className="map-empty-state">No relevant systems were returned for this civilization.</p>;
  }

  return (
    <div className="strategic-map-view">
      <p className="map-legend">Owned, visible, and unknown systems are color-coded. Labels stay read-only.</p>
      <svg className="strategic-map-canvas" viewBox={`0 0 ${width} ${height}`} role="img" aria-label="Read-only two-dimensional strategic map">
        <defs>
          <pattern id="map-grid" width="64" height="64" patternUnits="userSpaceOnUse">
            <path d="M 64 0 L 0 0 0 64" fill="none" stroke="rgba(132, 163, 214, 0.15)" strokeWidth="1" />
          </pattern>
        </defs>
        <rect width={width} height={height} className="map-frame" />
        {project(systems).map(({ system, x, y }) => (
          <g key={system.systemId} transform={`translate(${x} ${y})`} className={`map-node map-node-${tone(system.visibilityLevel)}`}>
            <circle r="20" className="map-node-ring" />
            <circle r="7" className="map-node-core" />
            <text className="map-node-title" x="0" y="38">{system.systemName ?? "Unknown system"}</text>
            <text className="map-node-meta" x="0" y="56">
              {system.visibilityLevel} | P {system.planets?.length ?? 0} | F {system.fleetPresence?.length ?? 0} | T {system.transferOverlays?.length ?? 0}
            </text>
          </g>
        ))}
      </svg>
    </div>
  );
}

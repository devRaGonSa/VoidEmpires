const strategicMapEndpoints = [
  "GET /api/dev/strategic-map",
  "GET /api/dev/strategic-map/action-manifest",
  "GET /api/dev/solar-systems/{systemId}/visual-state",
  "GET /api/dev/planets/{planetId}/visual-state",
];

export function StrategicMapPage() {
  return (
    <section className="page-grid">
      <article className="panel panel-hero">
        <span className="badge">Phase 9B shell</span>
        <h2>Strategic map prototype route</h2>
        <p>
          This route is reserved for the first read-only strategic map slice.
          The page intentionally avoids fetching gameplay state until the
          dedicated task adds DTOs, loading states, and conservative rendering.
        </p>
      </article>

      <article className="panel">
        <h3>Planned integration surface</h3>
        <ul className="stack-list">
          {strategicMapEndpoints.map((endpoint) => (
            <li key={endpoint}>{endpoint}</li>
          ))}
        </ul>
      </article>

      <article className="panel">
        <h3>Constraints</h3>
        <ul className="stack-list">
          <li>Read-only development endpoints only.</li>
          <li>No gameplay mutation buttons in the frontend shell.</li>
          <li>No production authentication assumptions.</li>
          <li>No committed 3D renderer or final UI design in this phase.</li>
        </ul>
      </article>
    </section>
  );
}

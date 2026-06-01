const fleetEndpoints = [
  "GET /api/dev/fleets/ui-state",
  "GET /api/dev/fleets/action-manifest",
  "POST /api/dev/fleets/orbital-travel/estimate",
];

export function FleetsPage() {
  return (
    <section className="page-grid">
      <article className="panel panel-hero">
        <span className="badge">Phase 9B shell</span>
        <h2>Fleet readiness prototype route</h2>
        <p>
          This page is the navigation and layout foundation for later read-only
          fleet state and action manifest inspection. It does not execute
          movement, transfer, or combat actions.
        </p>
      </article>

      <article className="panel">
        <h3>Planned integration surface</h3>
        <ul className="stack-list">
          {fleetEndpoints.map((endpoint) => (
            <li key={endpoint}>{endpoint}</li>
          ))}
        </ul>
      </article>

      <article className="panel">
        <h3>Constraints</h3>
        <ul className="stack-list">
          <li>Action manifests will be rendered as documentation only.</li>
          <li>Mutating backend actions stay unwired in this phase.</li>
          <li>Readiness metadata is not gameplay authorization.</li>
          <li>Production auth and WebSocket assumptions remain out of scope.</li>
        </ul>
      </article>
    </section>
  );
}

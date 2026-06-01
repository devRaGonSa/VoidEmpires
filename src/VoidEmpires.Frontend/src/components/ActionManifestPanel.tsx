import type { ActionManifestAction } from "../api/actionManifestTypes";
import { StatusBadge } from "./StatusBadge";

interface ActionManifestPanelProps {
  title: string;
  actions: ActionManifestAction[];
}

function normalizeNotes(notes: ActionManifestAction["notes"]) {
  if (!notes) {
    return [];
  }

  return Array.isArray(notes) ? notes : [notes];
}

export function ActionManifestPanel({
  title,
  actions,
}: ActionManifestPanelProps) {
  return (
    <article className="panel">
      <div className="section-heading">
        <div>
          <h3>{title}</h3>
          <p>Read-only manifest metadata for current development contracts.</p>
        </div>
        <StatusBadge>{actions.length} actions</StatusBadge>
      </div>

      <div className="manifest-list">
        {actions.map((action) => (
          <section key={action.actionKey} className="subpanel">
            <div className="manifest-header">
              <div>
                <h4>{action.displayName}</h4>
                <p>{action.actionKey}</p>
              </div>
              <StatusBadge tone={action.isReadOnly ? "good" : "warn"}>
                {action.isReadOnly ? "Read-only" : "Mutating"}
              </StatusBadge>
            </div>

            <dl className="meta-list">
              <div>
                <dt>Method</dt>
                <dd>{action.method}</dd>
              </div>
              <div>
                <dt>Route</dt>
                <dd>{action.route}</dd>
              </div>
              <div>
                <dt>Required fields</dt>
                <dd>{action.requiredFields?.join(", ") || "None"}</dd>
              </div>
              <div>
                <dt>Success</dt>
                <dd>{action.successStatus ?? "n/a"}</dd>
              </div>
            </dl>

            {normalizeNotes(action.notes).length > 0 && (
              <ul className="stack-list compact-list">
                {normalizeNotes(action.notes).map((note, index) => (
                  <li key={`${action.actionKey}-${index}`}>{note}</li>
                ))}
              </ul>
            )}
          </section>
        ))}
      </div>
    </article>
  );
}

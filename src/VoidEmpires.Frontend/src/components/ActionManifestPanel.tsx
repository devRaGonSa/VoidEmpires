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

function safeCompactJson(value: unknown) {
  try { return JSON.stringify(value); } catch { return String(value); }
}

function formatHttpMethod(method: string) {
  const normalizedMethod = method.trim().toUpperCase();
  return normalizedMethod || "N/A";
}

function formatRequiredField(field: unknown) {
  if (typeof field === "string") {
    return field;
  }

  if (field && typeof field === "object") {
    const record = field as Record<string, unknown>;
    const name = typeof record.name === "string" ? record.name : null;
    const type = typeof record.type === "string" ? record.type : null;
    const isRequired = record.isRequired !== false;

    if (name || type) {
      const label = [name, type ? `(${type})` : null].filter(Boolean).join(" ");
      return isRequired ? label : `${label} optional`;
    }
  }

  return safeCompactJson(field);
}

function formatRequiredFields(fields: ActionManifestAction["requiredFields"]) {
  return fields?.length ? fields.map(formatRequiredField) : ["None"];
}

export function ActionManifestPanel({
  title,
  actions,
}: ActionManifestPanelProps) {
  const readOnlyActions = actions.filter((action) => action.isReadOnly);
  const mutatingActions = actions.filter((action) => !action.isReadOnly);

  function renderActionGroup(groupTitle: string, groupActions: ActionManifestAction[], tone: "good" | "warn") {
    if (groupActions.length === 0) {
      return null;
    }

    return (
      <section className="manifest-group">
        <div className="manifest-group-title">
          <h4>{groupTitle}</h4>
          <StatusBadge tone={tone}>{groupActions.length}</StatusBadge>
        </div>

        <div className="manifest-list">
          {groupActions.map((action) => (
            <section
              key={action.actionKey}
              className={`subpanel ${action.isReadOnly ? "manifest-card-readonly" : "manifest-card-mutating"}`}
            >
              <div className="manifest-header">
                <div>
                  <h4>{action.displayName}</h4>
                  <p className="manifest-keyline" translate="no">
                    {action.actionKey}
                  </p>
                </div>
                <StatusBadge tone={action.isReadOnly ? "good" : "warn"}>
                  {action.isReadOnly ? "Read-only metadata" : "Mutation metadata"}
                </StatusBadge>
              </div>

              <dl className="meta-list">
                <div>
                  <dt>Method</dt>
                  <dd>
                    <code className="manifest-code" translate="no">
                      {formatHttpMethod(action.method)}
                    </code>
                  </dd>
                </div>
                <div>
                  <dt>Route</dt>
                  <dd>
                    <code className="manifest-code" translate="no">
                      {action.route}
                    </code>
                  </dd>
                </div>
                <div>
                  <dt>Required fields</dt>
                  <dd>
                    <ul className="manifest-required-list">
                      {formatRequiredFields(action.requiredFields).map((field) => (
                        <li key={`${action.actionKey}-${field}`}>{field}</li>
                      ))}
                    </ul>
                  </dd>
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
      </section>
    );
  }

  return (
    <article className="panel">
      <div className="section-heading">
        <div>
          <h3>{title}</h3>
          <p>Read-only manifest metadata for current development contracts.</p>
        </div>
        <StatusBadge>{actions.length} actions</StatusBadge>
      </div>

      {renderActionGroup("Read-only actions", readOnlyActions, "good")}
      {renderActionGroup("Mutation actions", mutatingActions, "warn")}
    </article>
  );
}

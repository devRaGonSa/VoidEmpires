import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetMutationConfirmationModel } from "../utils/fleetCommandPresentation";
import { StatusBadge } from "./StatusBadge";

interface ActionManifestPanelProps {
  title: string;
  actions: ActionManifestAction[];
  mutationConfirmations?: FleetMutationConfirmationModel[];
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
  mutationConfirmations = [],
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
        {tone === "warn" ? (
          <p className="figma-panel-note">
            Development-only mutation contracts are documented here for prototype alignment. This panel never executes them.
          </p>
        ) : null}

        <div className="manifest-list">
          {groupActions.map((action) => {
            const confirmation = mutationConfirmations.find((item) => item.actionKey === action.actionKey);

            return (
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
                  {action.isReadOnly ? "Read-only metadata" : "Prototype-only mutation metadata"}
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

              {!action.isReadOnly && confirmation ? (
                <dl className="meta-list">
                  <div>
                    <dt>Prototype level</dt>
                    <dd>{confirmation.prototypeLevel}</dd>
                  </div>
                  <div>
                    <dt>Mutation summary</dt>
                    <dd>{confirmation.mutationSummary}</dd>
                  </div>
                  <div>
                    <dt>Confirmation</dt>
                    <dd>{confirmation.confirmationText}</dd>
                  </div>
                  <div>
                    <dt>Disabled reason</dt>
                    <dd>{confirmation.disabledReason}</dd>
                  </div>
                </dl>
              ) : null}
            </section>
            );
          })}
        </div>
      </section>
    );
  }

  return (
    <article className="panel">
      <div className="section-heading">
        <div>
          <h3>{title}</h3>
          <p>Contract metadata for current development routes. This panel is intentionally non-executable.</p>
        </div>
        <StatusBadge>{actions.length} actions</StatusBadge>
      </div>

      {renderActionGroup("Read-only actions", readOnlyActions, "good")}
      {renderActionGroup("Prototype mutation actions", mutatingActions, "warn")}
    </article>
  );
}

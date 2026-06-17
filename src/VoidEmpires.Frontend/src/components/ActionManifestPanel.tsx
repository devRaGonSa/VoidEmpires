import type { ActionManifestAction } from "../api/actionManifestTypes";
import type { FleetMutationConfirmationModel } from "../utils/fleetCommandPresentation";
import { getUserFacingActionLabel } from "../utils/fleetCommandPresentation";
import { ActionStateBadge } from "./ActionStateBadge";
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
  return normalizedMethod || "N/D";
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
      return isRequired ? label : `${label} opcional`;
    }
  }

  return safeCompactJson(field);
}

function formatRequiredFields(fields: ActionManifestAction["requiredFields"]) {
  return fields?.length ? fields.map(formatRequiredField) : ["Ninguno"];
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
            Los contratos de mutacion siguen visibles para alinear herramientas Development. Este panel nunca los ejecuta.
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
                  <h4>{getUserFacingActionLabel(action.actionKey, action.displayName)}</h4>
                  <p className="manifest-keyline" translate="no">
                    {action.actionKey}
                  </p>
                </div>
                <ActionStateBadge state={action.isReadOnly ? "readOnly" : "developmentOnly"} />
              </div>

              <dl className="meta-list">
                <div>
                  <dt>Metodo</dt>
                  <dd>
                    <code className="manifest-code" translate="no">
                      {formatHttpMethod(action.method)}
                    </code>
                  </dd>
                </div>
                <div>
                  <dt>Ruta</dt>
                  <dd>
                    <code className="manifest-code" translate="no">
                      {action.route}
                    </code>
                  </dd>
                </div>
                <div>
                  <dt>Campos requeridos</dt>
                  <dd>
                    <ul className="manifest-required-list">
                      {formatRequiredFields(action.requiredFields).map((field) => (
                        <li key={`${action.actionKey}-${field}`}>{field}</li>
                      ))}
                    </ul>
                  </dd>
                </div>
                <div>
                  <dt>Exito</dt>
                  <dd>{action.successStatus ?? "n/d"}</dd>
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
                    <dt>Superficie</dt>
                    <dd>{confirmation.surfaceLabel}</dd>
                  </div>
                  <div>
                    <dt>Disponibilidad</dt>
                    <dd>{confirmation.readinessLabel}</dd>
                  </div>
                  <div>
                    <dt>Nivel de preparacion</dt>
                    <dd>{confirmation.prototypeLevel}</dd>
                  </div>
                  <div>
                    <dt>Resumen de mutacion</dt>
                    <dd>{confirmation.mutationSummary}</dd>
                  </div>
                  <div>
                    <dt>Confirmacion</dt>
                    <dd>{confirmation.confirmationText}</dd>
                  </div>
                  <div>
                    <dt>Motivo del bloqueo</dt>
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
          <p>Metadatos de contrato para las rutas actuales de desarrollo. Este panel es intencionadamente no ejecutable.</p>
        </div>
        <StatusBadge>{actions.length} acciones</StatusBadge>
      </div>

      {renderActionGroup("Acciones de solo lectura", readOnlyActions, "good")}
      {renderActionGroup("Acciones de mutacion Development", mutatingActions, "warn")}
    </article>
  );
}

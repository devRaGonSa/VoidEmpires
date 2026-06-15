import { useMemo } from "react";
import { UiBadge } from "./ui/UiBadge";

export interface DevDiagnosticsSummaryItem {
  label: string;
  value: string | number | null | undefined;
}

interface DevDiagnosticsPanelProps {
  title?: string;
  description?: string;
  summaryItems: DevDiagnosticsSummaryItem[];
  notes?: string[];
  rawPayload?: unknown;
  defaultOpen?: boolean;
}

function formatValue(value: DevDiagnosticsSummaryItem["value"]) {
  if (value === null || value === undefined || value === "") {
    return "No disponible";
  }

  return String(value);
}

export function DevDiagnosticsPanel({
  title = "Diagnostico de desarrollo",
  description = "Ids, estado tecnico y payload de soporte permanecen secundarios.",
  summaryItems,
  notes = [],
  rawPayload,
  defaultOpen = false,
}: DevDiagnosticsPanelProps) {
  const rawJson = useMemo(
    () => rawPayload === undefined ? null : JSON.stringify(rawPayload, null, 2),
    [rawPayload],
  );

  return (
    <details className="technical-disclosure" open={defaultOpen}>
      <summary>
        <div>
          <p className="eyebrow">Diagnostico secundario</p>
          <strong>{title}</strong>
        </div>
        <UiBadge tone="warn">Development</UiBadge>
      </summary>

      <div className="technical-disclosure-body">
        <section className="ui-card panel">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Soporte tecnico</p>
              <h3>{title}</h3>
            </div>
            <UiBadge>Contraido por defecto</UiBadge>
          </div>
          <p className="figma-panel-note">{description}</p>

          {summaryItems.length > 0 ? (
            <div className="figma-data-list">
              {summaryItems.map((item) => (
                <div key={item.label} className="figma-data-row">
                  <span>{item.label}</span>
                  <strong>{formatValue(item.value)}</strong>
                </div>
              ))}
            </div>
          ) : (
            <p className="figma-panel-note">No hay resumen tecnico disponible.</p>
          )}

          {notes.length > 0 ? (
            <ul className="stack-list compact-list">
              {notes.map((note) => (
                <li key={note}>{note}</li>
              ))}
            </ul>
          ) : null}

          {rawJson ? (
            <details className="json-details">
              <summary>Ver payload tecnico</summary>
              <pre className="json-preview">{rawJson}</pre>
            </details>
          ) : null}
        </section>
      </div>
    </details>
  );
}

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

function isGuidLike(value: string) {
  return /^[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}$/i.test(value);
}

function isTechnicalSummaryItem(item: DevDiagnosticsSummaryItem) {
  const label = item.label.toLowerCase();
  const value = formatValue(item.value);

  return (
    label.includes("id") ||
    label.includes("clave") ||
    label.includes("ruta") ||
    label.includes("payload") ||
    label.includes("tecnico") ||
    isGuidLike(value)
  );
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
  const primarySummaryItems = summaryItems.filter((item) => !isTechnicalSummaryItem(item));
  const technicalSummaryItems = summaryItems.filter(isTechnicalSummaryItem);

  return (
    <details className="technical-disclosure dev-diagnostics-panel" open={defaultOpen}>
      <summary>
        <div>
          <p className="eyebrow">Diagnostico secundario</p>
          <strong>{title}</strong>
        </div>
        <UiBadge tone="warn">Development</UiBadge>
      </summary>

      <div className="technical-disclosure-body">
        <section className="ui-card panel dev-diagnostics-card">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Soporte tecnico</p>
              <h3>{title}</h3>
            </div>
            <UiBadge>Contraido por defecto</UiBadge>
          </div>
          <p className="figma-panel-note">{description}</p>

          {primarySummaryItems.length > 0 ? (
            <div className="figma-data-list">
              {primarySummaryItems.map((item) => (
                <div key={item.label} className="figma-data-row">
                  <span>{item.label}</span>
                  <strong>{formatValue(item.value)}</strong>
                </div>
              ))}
            </div>
          ) : (
            <p className="figma-panel-note">No hay resumen visible disponible. Las claves tecnicas quedan abajo.</p>
          )}

          {notes.length > 0 ? (
            <ul className="stack-list compact-list">
              {notes.map((note) => (
                <li key={note}>{note}</li>
              ))}
            </ul>
          ) : null}

          {technicalSummaryItems.length > 0 ? (
            <details className="dev-diagnostics-technical-details">
              <summary>Ver claves tecnicas</summary>
              <div className="figma-data-list dev-diagnostics-technical-list">
                {technicalSummaryItems.map((item) => (
                  <div key={item.label} className="figma-data-row">
                    <span>{item.label}</span>
                    <strong>{formatValue(item.value)}</strong>
                  </div>
                ))}
              </div>
            </details>
          ) : null}

          {rawJson ? (
            <details className="json-details dev-diagnostics-raw-payload">
              <summary>Ver payload tecnico</summary>
              <pre className="json-preview">{rawJson}</pre>
            </details>
          ) : null}
        </section>
      </div>
    </details>
  );
}

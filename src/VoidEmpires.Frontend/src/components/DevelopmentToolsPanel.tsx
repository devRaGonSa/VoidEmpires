import type { ReactNode } from "react";
import { UiBadge, type UiBadgeTone } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

export interface DevelopmentToolsResult {
  label: string;
  summary: string;
  detail?: string;
  tone?: UiBadgeTone;
}

interface DevelopmentToolsPanelProps {
  title?: string;
  description?: string;
  actions?: ReactNode;
  lastResult?: DevelopmentToolsResult | null;
  diagnosticsLink?: ReactNode;
  defaultOpen?: boolean;
}

export function DevelopmentToolsPanel({
  title = "Herramientas de operador",
  description = "Acciones manuales y materializaciones controladas quedan separadas del flujo jugable principal.",
  actions,
  lastResult,
  diagnosticsLink,
  defaultOpen = false,
}: DevelopmentToolsPanelProps) {
  const hasSecondaryContent = actions || lastResult || diagnosticsLink;

  return (
    <details className="technical-disclosure development-tools-panel" open={defaultOpen}>
      <summary>
        <div>
          <p className="eyebrow">Soporte operador</p>
          <strong>{title}</strong>
        </div>
        <UiBadge tone="warn">Pendiente</UiBadge>
      </summary>

      <div className="technical-disclosure-body">
        <UiCard as="section" className="panel development-tools-card">
          <div className="figma-section-header">
            <div>
              <p className="eyebrow">Fuera del flujo principal</p>
              <h3>{title}</h3>
              <p>{description}</p>
            </div>
            <UiBadge tone="warn">Contraido por defecto</UiBadge>
          </div>

          {hasSecondaryContent ? (
            <div className="development-tools-grid">
              {actions ? (
                <section className="development-tools-actions" aria-label="Acciones de operador">
                  <div className="development-tools-section-head">
                    <h4>Acciones manuales</h4>
                    <UiBadge tone="warn">Mutacion controlada</UiBadge>
                  </div>
                  <div className="development-tools-action-list">{actions}</div>
                </section>
              ) : null}

              {lastResult ? (
                <section className="development-tools-result" aria-label="Ultimo resultado de operador">
                  <div className="development-tools-section-head">
                    <h4>Ultimo resultado</h4>
                    <UiBadge tone={lastResult.tone ?? "neutral"}>{lastResult.label}</UiBadge>
                  </div>
                  <p>{lastResult.summary}</p>
                  {lastResult.detail ? <p className="figma-panel-note">{lastResult.detail}</p> : null}
                </section>
              ) : null}

              {diagnosticsLink ? (
                <section className="development-tools-diagnostics" aria-label="Diagnosticos relacionados">
                  <div className="development-tools-section-head">
                    <h4>Diagnostico</h4>
                    <UiBadge>Secundario</UiBadge>
                  </div>
                  <div className="development-tools-link">{diagnosticsLink}</div>
                </section>
              ) : null}
            </div>
          ) : (
            <p className="figma-panel-note">
              Sin acciones manuales conectadas en esta vista. El componente queda listo para migraciones futuras.
            </p>
          )}
        </UiCard>
      </div>
    </details>
  );
}

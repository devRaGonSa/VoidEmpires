import type { ReactNode } from "react";
import { UiBadge, type UiBadgeTone } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

type PageStateKind = "loading" | "empty" | "error";

interface PageStatePanelProps {
  kind: PageStateKind;
  title: string;
  description: string;
  eyebrow?: string;
  badgeLabel?: string;
  badgeTone?: UiBadgeTone;
  detail?: string | null;
  action?: ReactNode;
}

const stateDefaults: Record<PageStateKind, { eyebrow: string; badgeLabel: string; badgeTone: UiBadgeTone }> = {
  loading: { eyebrow: "Cargando", badgeLabel: "Cargando...", badgeTone: "neutral" },
  empty: { eyebrow: "Sin contexto", badgeLabel: "Contexto requerido", badgeTone: "warn" },
  error: { eyebrow: "Lectura no disponible", badgeLabel: "Sin lectura", badgeTone: "warn" },
};

export function PageStatePanel({
  kind,
  title,
  description,
  eyebrow,
  badgeLabel,
  badgeTone,
  detail,
  action,
}: PageStatePanelProps) {
  const defaults = stateDefaults[kind];

  return (
    <UiCard
      as="section"
      className={`panel page-state-panel page-state-panel-${kind}`}
      role={kind === "error" ? "alert" : "status"}
      aria-live={kind === "error" ? "assertive" : "polite"}
      aria-busy={kind === "loading" ? "true" : undefined}
    >
      <div className="figma-section-header">
        <div>
          <p className="eyebrow">{eyebrow ?? defaults.eyebrow}</p>
          <h3>{title}</h3>
        </div>
        <UiBadge tone={badgeTone ?? defaults.badgeTone}>{badgeLabel ?? defaults.badgeLabel}</UiBadge>
      </div>
      <p className={kind === "error" ? "error-text" : "figma-panel-note"}>{description}</p>
      {detail ? <p className="figma-panel-note">{detail}</p> : null}
      {action ? <div className="selection-chip-row">{action}</div> : null}
    </UiCard>
  );
}

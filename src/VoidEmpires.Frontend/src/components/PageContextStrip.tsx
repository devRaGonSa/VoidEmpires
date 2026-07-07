import type { ReactNode } from "react";
import { UiBadge, type UiBadgeTone } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

export interface PageContextItem {
  label: string;
  value: string;
  detail?: string;
}

export interface PageContextResourceItem {
  label: string;
  value: string;
  tone?: UiBadgeTone;
}

interface PageContextStripProps {
  title: string;
  purpose: string;
  statusLabel: string;
  eyebrow?: string;
  statusTone?: UiBadgeTone;
  contextItems?: ReadonlyArray<PageContextItem>;
  resourceItems?: ReadonlyArray<PageContextResourceItem>;
  primaryAction?: ReactNode;
  secondaryAction?: ReactNode;
}

function hasVisibleValue(item: { value: string }) {
  return item.value.trim().length > 0;
}

export function PageContextStrip({
  title,
  purpose,
  statusLabel,
  eyebrow = "Seleccion",
  statusTone = "neutral",
  contextItems = [],
  resourceItems = [],
  primaryAction,
  secondaryAction,
}: PageContextStripProps) {
  const visibleContextItems = contextItems.filter(hasVisibleValue);
  const visibleResourceItems = resourceItems.filter(hasVisibleValue);
  const hasActions = primaryAction || secondaryAction;
  const hasDetails = visibleContextItems.length > 0 || visibleResourceItems.length > 0 || hasActions;

  return (
    <UiCard as="section" className="panel page-context-strip">
      <div className="page-context-strip-main">
        <div className="page-context-strip-copy">
          <p className="eyebrow">{eyebrow}</p>
          <h2>{title}</h2>
          <p>{purpose}</p>
        </div>
        <UiBadge tone={statusTone}>{statusLabel}</UiBadge>
      </div>

      {hasDetails ? (
        <div className="page-context-strip-details">
          {visibleContextItems.length > 0 ? (
            <dl className="page-context-strip-list" aria-label="Seleccion actual">
              {visibleContextItems.map((item) => (
                <div key={`${item.label}:${item.value}`}>
                  <dt>{item.label}</dt>
                  <dd>
                    <strong>{item.value}</strong>
                    {item.detail ? <span>{item.detail}</span> : null}
                  </dd>
                </div>
              ))}
            </dl>
          ) : null}

          {visibleResourceItems.length > 0 ? (
            <div className="page-context-strip-resources" aria-label="Recursos visibles">
              {visibleResourceItems.map((item) => (
                <UiBadge key={`${item.label}:${item.value}`} tone={item.tone ?? "resource"}>
                  {item.label}: {item.value}
                </UiBadge>
              ))}
            </div>
          ) : null}

          {hasActions ? (
            <div className="page-context-strip-actions">
              {primaryAction}
              {secondaryAction}
            </div>
          ) : null}
        </div>
      ) : null}
    </UiCard>
  );
}

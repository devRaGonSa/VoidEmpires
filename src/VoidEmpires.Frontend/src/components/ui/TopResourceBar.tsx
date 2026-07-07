import { UiBadge } from "./UiBadge";

export interface TopBarStatusItem {
  label: string;
  value: string;
}

interface TopStatusBarProps {
  items: TopBarStatusItem[];
}

export function TopStatusBar({ items }: TopStatusBarProps) {
  return (
    <div className="top-status-bar" aria-label="Estado global del imperio">
      <div className="top-status-pill-row">
        {items.map((item) => (
          <section key={item.label} className="top-status-pill">
            <div className="top-status-pill-head">
              <span>{item.label}</span>
              <strong>{item.value}</strong>
            </div>
          </section>
        ))}
      </div>
      <UiBadge tone="resource">Estado imperial</UiBadge>
    </div>
  );
}

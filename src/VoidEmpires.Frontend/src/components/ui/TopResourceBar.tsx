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
    <div className="top-status-bar" aria-label="Estado global del prototipo">
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
      <UiBadge tone="resource">Sin recursos globales simulados</UiBadge>
    </div>
  );
}

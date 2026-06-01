import { UiBadge } from "./UiBadge";
import { UiProgressBar } from "./UiProgressBar";

export interface TopResourceItem {
  label: string;
  value: string;
  progress: number;
  tone?: "metal" | "crystal" | "deuterium" | "power" | "neutral";
}

interface TopResourceBarProps {
  resources: TopResourceItem[];
  userLabel: string;
}

export function TopResourceBar({
  resources,
  userLabel,
}: TopResourceBarProps) {
  return (
    <div className="top-resource-bar">
      <div className="top-resource-pill-row" aria-label="Empire resources">
        {resources.map((resource) => (
          <section key={resource.label} className="top-resource-pill">
            <div className="top-resource-pill-head">
              <span>{resource.label}</span>
              <strong>{resource.value}</strong>
            </div>
            <UiProgressBar value={resource.progress} tone={resource.tone} />
          </section>
        ))}
      </div>
      <UiBadge tone="resource">{userLabel}</UiBadge>
    </div>
  );
}

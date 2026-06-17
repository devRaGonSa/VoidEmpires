import type { ReactNode } from "react";

export type UiBadgeTone = "neutral" | "good" | "warn" | "resource";

interface UiBadgeProps {
  children: ReactNode;
  tone?: UiBadgeTone;
}

export function UiBadge({ children, tone = "neutral" }: UiBadgeProps) {
  const className = [
    "ui-badge",
    tone === "good"
      ? "ui-badge-good"
      : tone === "warn"
        ? "ui-badge-warn"
        : tone === "resource"
          ? "ui-badge-resource"
          : "ui-badge-neutral",
  ].join(" ");

  return <span className={className}>{children}</span>;
}

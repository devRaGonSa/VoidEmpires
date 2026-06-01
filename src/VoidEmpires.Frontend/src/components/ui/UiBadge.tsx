import type { ReactNode } from "react";

interface UiBadgeProps {
  children: ReactNode;
  tone?: "neutral" | "good" | "warn" | "resource";
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

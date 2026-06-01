import type { ReactNode } from "react";

interface StatusBadgeProps {
  children: ReactNode;
  tone?: "neutral" | "good" | "warn";
}

export function StatusBadge({
  children,
  tone = "neutral",
}: StatusBadgeProps) {
  const className =
    tone === "good"
      ? "badge badge-good"
      : tone === "warn"
        ? "badge badge-warn"
        : "badge";

  return <span className={className}>{children}</span>;
}

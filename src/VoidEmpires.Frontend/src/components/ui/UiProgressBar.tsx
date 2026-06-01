interface UiProgressBarProps {
  value: number;
  tone?: "neutral" | "metal" | "crystal" | "deuterium" | "power";
}

export function UiProgressBar({
  value,
  tone = "neutral",
}: UiProgressBarProps) {
  const clampedValue = Math.max(0, Math.min(100, value));
  const className = [
    "ui-progress-bar",
    tone === "metal"
      ? "ui-progress-bar-metal"
      : tone === "crystal"
        ? "ui-progress-bar-crystal"
        : tone === "deuterium"
          ? "ui-progress-bar-deuterium"
          : tone === "power"
            ? "ui-progress-bar-power"
            : "ui-progress-bar-neutral",
  ].join(" ");

  return (
    <div
      className={className}
      role="progressbar"
      aria-valuemin={0}
      aria-valuemax={100}
      aria-valuenow={Math.round(clampedValue)}
    >
      <span style={{ width: `${clampedValue}%` }} />
    </div>
  );
}

import type { ReactNode } from "react";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface CockpitHeroProps {
  title: string;
  description: string;
  versionLabel?: string | null;
  badges?: ReactNode;
  developmentNote?: string;
  developmentNoteLabel?: string;
}

export function CockpitHero({
  title,
  description,
  versionLabel,
}: CockpitHeroProps) {
  return (
    <UiCard className="panel panel-hero figma-hero-card">
      <div className="figma-hero-copy">
        {versionLabel ? <UiBadge tone="resource">{versionLabel}</UiBadge> : null}
        <h2>{title}</h2>
        <p>{description}</p>
      </div>
    </UiCard>
  );
}

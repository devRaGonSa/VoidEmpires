import type { ReactNode } from "react";
import { UiBadge } from "./ui/UiBadge";
import { UiCard } from "./ui/UiCard";

interface CockpitHeroProps {
  title: string;
  description: string;
  versionLabel: string;
  badges: ReactNode;
  developmentNote: string;
}

export function CockpitHero({
  title,
  description,
  versionLabel,
  badges,
  developmentNote,
}: CockpitHeroProps) {
  return (
    <UiCard className="panel panel-hero figma-hero-card">
      <div className="figma-hero-copy">
        <UiBadge tone="resource">{versionLabel}</UiBadge>
        <h2>{title}</h2>
        <p>{description}</p>
      </div>
      <div className="figma-badge-row">{badges}</div>
      <div className="cockpit-hero-dev-note">
        <UiBadge tone="warn">Solo desarrollo</UiBadge>
        <p>{developmentNote}</p>
      </div>
    </UiCard>
  );
}

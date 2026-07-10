import { Link } from "react-router-dom";
import type { ReactNode } from "react";

interface PlanetDataRowProps {
  label: string;
  value: ReactNode;
}

interface ModuleStatusCardProps {
  to: string;
  title: string;
  label: string;
  status: string;
  description: string;
  className?: string;
}

export function PlanetDataRow({ label, value }: PlanetDataRowProps) {
  return (
    <div className="figma-data-row">
      <span>{label}</span>
      <strong>{value}</strong>
    </div>
  );
}

export function ModuleStatusCard({
  to,
  title,
  label,
  status,
  description,
  className,
}: ModuleStatusCardProps) {
  return (
    <Link className={`planet-related-module-card module-status-card${className ? ` ${className}` : ""}`} to={to}>
      <strong>{title}</strong>
      <span>{label}</span>
      <span>{status}</span>
      <small>{description}</small>
    </Link>
  );
}

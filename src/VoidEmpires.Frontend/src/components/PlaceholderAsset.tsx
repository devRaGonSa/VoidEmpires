import { UiBadge } from "./ui/UiBadge";

export type PlaceholderAssetKind =
  | "building"
  | "technology"
  | "ship"
  | "defense"
  | "resource"
  | "civilization";

interface PlaceholderAssetProps {
  kind: PlaceholderAssetKind;
  label: string;
  typeLabel: string;
  detail?: string;
  imageKey?: string | number | null;
  className?: string;
}

function getInitials(label: string) {
  const normalized = label.trim();

  if (!normalized) {
    return "VE";
  }

  const parts = normalized.split(/\s+/).filter(Boolean);
  const initials = parts.slice(0, 2).map((part) => part[0]).join("");

  return initials.toUpperCase();
}

export function PlaceholderAsset({
  kind,
  label,
  typeLabel,
  detail,
  imageKey,
  className,
}: PlaceholderAssetProps) {
  const classNames = [
    "placeholder-asset",
    `placeholder-asset-${kind}`,
    className,
  ].filter(Boolean).join(" ");

  return (
    <div className={classNames} data-placeholder-image-key={imageKey ?? undefined}>
      <div className="placeholder-asset-slot" aria-hidden="true">
        <span>{getInitials(label)}</span>
      </div>
      <div className="placeholder-asset-copy">
        <div className="placeholder-asset-head">
          <p className="eyebrow">{typeLabel}</p>
          <UiBadge>Imagen pendiente</UiBadge>
        </div>
        <strong>{label}</strong>
        {detail ? <p>{detail}</p> : null}
      </div>
    </div>
  );
}

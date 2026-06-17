import type { UiBadgeTone } from "./ui/UiBadge";
import { UiBadge } from "./ui/UiBadge";

export type ActionState =
  | "available"
  | "blocked"
  | "pending"
  | "loading"
  | "confirmed"
  | "failed"
  | "readOnly"
  | "developmentOnly";

const actionStateLabels: Record<ActionState, string> = {
  available: "Disponible",
  blocked: "Bloqueado",
  pending: "Pendiente",
  loading: "Cargando",
  confirmed: "Confirmado",
  failed: "Fallido",
  readOnly: "Solo lectura",
  developmentOnly: "Development-only",
};

const actionStateTones: Record<ActionState, UiBadgeTone> = {
  available: "good",
  blocked: "warn",
  pending: "neutral",
  loading: "neutral",
  confirmed: "good",
  failed: "warn",
  readOnly: "good",
  developmentOnly: "warn",
};

interface ActionStateBadgeProps {
  state: ActionState;
  label?: string;
}

export function ActionStateBadge({
  state,
  label = actionStateLabels[state],
}: ActionStateBadgeProps) {
  return <UiBadge tone={actionStateTones[state]}>{label}</UiBadge>;
}

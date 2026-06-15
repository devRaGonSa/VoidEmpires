import { useMemo } from "react";
import { loadPlayableSession } from "./playableSession";

export function usePlayableRouteContext(civilizationId: string | null | undefined) {
  const hasQueryCivilizationId = Boolean(civilizationId?.trim());

  const playableSession = useMemo(
    () => (hasQueryCivilizationId ? null : loadPlayableSession()),
    [hasQueryCivilizationId],
  );

  return {
    hasQueryCivilizationId,
    playableSession,
  };
}

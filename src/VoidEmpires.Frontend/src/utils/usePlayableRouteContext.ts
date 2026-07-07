import { useMemo } from "react";
import { getCurrentAccountWorldEntry } from "./currentAccountSession";
import {
  createPlayableSessionSnapshot,
  getPlayableSessionInputFromAccountSession,
  loadPlayableSession,
} from "./playableSession";
import { useCurrentAccountSession } from "./useCurrentAccountSession";

export function usePlayableRouteContext(civilizationId: string | null | undefined) {
  const currentAccountSession = useCurrentAccountSession();
  const hasQueryCivilizationId = Boolean(civilizationId?.trim());
  const accountWorldEntry = useMemo(
    () => getCurrentAccountWorldEntry(currentAccountSession.session),
    [currentAccountSession.session],
  );
  const accountPlayableSession = useMemo(
    () => {
      const input = getPlayableSessionInputFromAccountSession(currentAccountSession.session);
      return input ? createPlayableSessionSnapshot(input) : null;
    },
    [currentAccountSession.session],
  );

  const playableSession = useMemo(
    () => (hasQueryCivilizationId ? null : accountPlayableSession ?? loadPlayableSession()),
    [accountPlayableSession, hasQueryCivilizationId],
  );
  const resolvedWorldEntry = accountWorldEntry ?? playableSession;

  return {
    accountWorldEntry,
    currentAccountSession,
    hasQueryCivilizationId,
    playableSession,
    resolvedWorldEntry,
  };
}

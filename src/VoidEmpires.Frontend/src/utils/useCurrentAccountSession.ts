import { useCallback, useEffect, useState } from "react";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import {
  createCurrentAccountSessionError,
  initialCurrentAccountSessionState,
  resolveCurrentAccountSession,
  type CurrentAccountSessionState,
} from "./currentAccountSession";

async function loadCurrentAccountSession(): Promise<CurrentAccountSessionState> {
  try {
    return resolveCurrentAccountSession(await voidEmpiresApi.account.getCurrentUser());
  } catch {
    return createCurrentAccountSessionError("No se pudo comprobar la cuenta actual.");
  }
}

export function useCurrentAccountSession() {
  const [state, setState] = useState<CurrentAccountSessionState>(initialCurrentAccountSessionState);

  const refresh = useCallback(async () => {
    setState((current) => ({ ...current, status: "loading" }));
    const nextState = await loadCurrentAccountSession();
    setState(nextState);
    return nextState;
  }, []);

  useEffect(() => {
    let isCurrent = true;

    setState((current) => ({ ...current, status: "loading" }));
    void loadCurrentAccountSession().then((nextState) => {
      if (isCurrent) {
        setState(nextState);
      }
    });

    return () => {
      isCurrent = false;
    };
  }, []);

  return {
    ...state,
    refresh,
  };
}

import { useCallback, useEffect, useState } from "react";
import { voidEmpiresApi } from "../api/voidEmpiresApi";
import {
  createCurrentAccountSessionError,
  initialCurrentAccountSessionState,
  resolveCurrentAccountSession,
  type CurrentAccountSessionState,
} from "./currentAccountSession";

type CurrentAccountSessionListener = (state: CurrentAccountSessionState) => void;

let sharedCurrentAccountSessionState = initialCurrentAccountSessionState;
let sharedCurrentAccountSessionLoad: Promise<CurrentAccountSessionState> | null = null;
const currentAccountSessionListeners = new Set<CurrentAccountSessionListener>();

async function loadCurrentAccountSession(): Promise<CurrentAccountSessionState> {
  try {
    return resolveCurrentAccountSession(await voidEmpiresApi.account.getCurrentUser());
  } catch {
    return createCurrentAccountSessionError("No se pudo comprobar la cuenta actual.");
  }
}

function publishCurrentAccountSessionState(state: CurrentAccountSessionState) {
  sharedCurrentAccountSessionState = state;
  currentAccountSessionListeners.forEach((listener) => listener(state));
}

async function refreshCurrentAccountSession() {
  if (sharedCurrentAccountSessionLoad) {
    return sharedCurrentAccountSessionLoad;
  }

  publishCurrentAccountSessionState({ ...sharedCurrentAccountSessionState, status: "loading" });
  sharedCurrentAccountSessionLoad = loadCurrentAccountSession()
    .then((nextState) => {
      publishCurrentAccountSessionState(nextState);
      return nextState;
    })
    .finally(() => {
      sharedCurrentAccountSessionLoad = null;
    });

  return sharedCurrentAccountSessionLoad;
}

export function useCurrentAccountSession() {
  const [state, setState] = useState<CurrentAccountSessionState>(sharedCurrentAccountSessionState);

  const refresh = useCallback(async () => {
    return refreshCurrentAccountSession();
  }, []);

  useEffect(() => {
    currentAccountSessionListeners.add(setState);
    setState(sharedCurrentAccountSessionState);
    if (sharedCurrentAccountSessionState.status === "loading") {
      void refreshCurrentAccountSession();
    }

    return () => {
      currentAccountSessionListeners.delete(setState);
    };
  }, []);

  return {
    ...state,
    refresh,
  };
}

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
let sharedCurrentAccountSessionLoadVersion = 0;
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

async function refreshCurrentAccountSession(options?: { force?: boolean }) {
  if (sharedCurrentAccountSessionLoad && !options?.force) {
    return sharedCurrentAccountSessionLoad;
  }

  const loadVersion = sharedCurrentAccountSessionLoadVersion + 1;
  sharedCurrentAccountSessionLoadVersion = loadVersion;
  publishCurrentAccountSessionState({ ...sharedCurrentAccountSessionState, status: "loading" });
  const currentLoad = loadCurrentAccountSession()
    .then((nextState) => {
      if (loadVersion === sharedCurrentAccountSessionLoadVersion) {
        publishCurrentAccountSessionState(nextState);
      }

      return nextState;
    })
    .finally(() => {
      if (sharedCurrentAccountSessionLoad === currentLoad) {
        sharedCurrentAccountSessionLoad = null;
      }
    });

  sharedCurrentAccountSessionLoad = currentLoad;
  return sharedCurrentAccountSessionLoad;
}

export function useCurrentAccountSession() {
  const [state, setState] = useState<CurrentAccountSessionState>(sharedCurrentAccountSessionState);

  const refresh = useCallback(async (options?: { force?: boolean }) => {
    return refreshCurrentAccountSession(options);
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

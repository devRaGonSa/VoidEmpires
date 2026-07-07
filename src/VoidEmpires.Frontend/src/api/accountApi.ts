import { appConfig } from "../config";
import type {
  AccountApiResult,
  AccountLoginRequest,
  AccountLogoutResponse,
  AccountRegistrationRequest,
  AccountRegistrationResponse,
  AccountSessionResponse,
} from "./accountTypes";

function buildUrl(path: string) {
  return new URL(path, appConfig.apiBaseUrl).toString();
}

interface AccountRequestOptions {
  body?: unknown;
  method?: "GET" | "POST";
}

async function requestAccountJson<TResponse>(
  path: string,
  options?: AccountRequestOptions,
): Promise<AccountApiResult<TResponse>> {
  const response = await fetch(buildUrl(path), {
    body: options?.body ? JSON.stringify(options.body) : undefined,
    credentials: "include",
    headers: {
      Accept: "application/json",
      ...(options?.body ? { "Content-Type": "application/json" } : {}),
    },
    method: options?.method ?? "GET",
  });
  const contentType = response.headers.get("content-type") ?? "";
  const hasJsonBody = contentType.includes("application/json");
  let payload: TResponse | null = null;

  if (hasJsonBody) {
    try {
      payload = (await response.json()) as TResponse;
    } catch {
      return { httpStatus: response.status, hasJsonBody: true, bodyParseFailed: true, response: null };
    }
  }

  return { httpStatus: response.status, hasJsonBody, bodyParseFailed: false, response: payload };
}

export const accountApi = {
  register(request: AccountRegistrationRequest) {
    return requestAccountJson<AccountRegistrationResponse>("/api/accounts/register", {
      body: request,
      method: "POST",
    });
  },
  login(request: AccountLoginRequest) {
    return requestAccountJson<AccountSessionResponse>("/api/accounts/login", {
      body: request,
      method: "POST",
    });
  },
  logout() {
    return requestAccountJson<AccountLogoutResponse>("/api/accounts/logout", { method: "POST" });
  },
  getCurrentUser() {
    return requestAccountJson<AccountSessionResponse>("/api/accounts/me");
  },
};

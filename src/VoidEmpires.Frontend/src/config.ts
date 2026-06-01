const defaultApiBaseUrl = "http://localhost:5142";

export const appConfig = {
  apiBaseUrl:
    (import.meta.env.VITE_VOIDEMPIRES_API_BASE_URL as string | undefined)?.trim() ||
    defaultApiBaseUrl,
  backendProfile: "VoidEmpires.Web http profile",
} as const;

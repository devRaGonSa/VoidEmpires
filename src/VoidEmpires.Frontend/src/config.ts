const defaultApiBaseUrl = "http://localhost:5142";

type ViteImportMeta = ImportMeta & {
  readonly env?: {
    readonly VITE_VOIDEMPIRES_API_BASE_URL?: string;
  };
};

const viteEnv = import.meta as ViteImportMeta;

export const appConfig = {
  apiBaseUrl:
    viteEnv.env?.VITE_VOIDEMPIRES_API_BASE_URL?.trim() || defaultApiBaseUrl,
  backendProfile: "VoidEmpires.Web http profile",
} as const;

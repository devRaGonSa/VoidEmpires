export interface ActionManifestResponse {
  succeeded: boolean;
  manifest: {
    actions: ActionManifestAction[];
  } | null;
  errors: string[];
}

export interface ActionManifestAction {
  actionKey: string;
  displayName: string;
  method: string;
  route: string;
  isReadOnly: boolean;
  requiredFields?: string[];
  successStatus?: number | string;
  errorStatuses?: Array<number | string>;
  notes?: string | string[] | null;
}

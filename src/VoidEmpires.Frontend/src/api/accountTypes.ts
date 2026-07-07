export interface AccountApiError {
  code: string;
  message: string;
  field: string | null;
}

export interface AccountResourceSnapshot {
  credits: number;
  metal: number;
  crystal: number;
  gas: number;
}

export interface AccountRegistrationRequest {
  email: string;
  password: string;
  confirmPassword: string;
  displayName: string;
  civilizationName: string;
  homePlanetName?: string | null;
}

export interface AccountRegistrationResponse {
  succeeded: boolean;
  userId: string | null;
  playerProfileId: string | null;
  civilizationId: string | null;
  homePlanetId: string | null;
  homePlanetName: string | null;
  nextRoute: string | null;
  startingResources: AccountResourceSnapshot | null;
  errors: readonly AccountApiError[];
}

export interface AccountLoginRequest {
  email: string;
  password: string;
}

export interface AccountSessionResponse {
  succeeded: boolean;
  userId: string | null;
  playerProfileId: string | null;
  civilizationId: string | null;
  homePlanetId: string | null;
  homePlanetName: string | null;
  nextRoute: string | null;
  errors: readonly AccountApiError[];
}

export interface AccountLogoutResponse {
  succeeded: boolean;
  errors: readonly string[];
}

export interface AccountApiResult<TResponse> {
  httpStatus: number;
  hasJsonBody: boolean;
  bodyParseFailed: boolean;
  response: TResponse | null;
}

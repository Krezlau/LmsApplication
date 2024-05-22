import {UserModel} from "./user-model";

export interface AuthState {
  isAuthenticated: boolean;
  accessToken: string;
  refreshToken: string;
  userData: UserModel | null;
  tenantId: string;
}


import { Injectable, signal } from '@angular/core';
import { AuthState } from '../types/users/auth-state';
import { HttpClient } from '@angular/common/http';
import { LoginResponse } from '../types/users/login-response';
import { UserModel } from '../types/users/user-model';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private initialState: AuthState = {
    isAuthenticated: false,
    accessToken: '',
    refreshToken: '',
    userData: null,
    validUntil: null,
  };
  public authState = signal(this.initialState);

  public authStateLoading = signal(false);

  constructor(private http: HttpClient) {}

  public loadAuthState() {
    this.authStateLoading.set(true);
    const authState = this.checkAuthState();

    // couldnt recover auth state from local storage
    // or auth state is expired
    // so we reset it to default
    if (!authState) {
      this.authState.set(this.initialState);
      this.authStateLoading.set(false);
      return;
    }

    this.authState.set(authState);
    this.authStateLoading.set(false);
  }

  public login(email: string, password: string) {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/login`, { email, password });
  }

  public loadState(accessToken: string, refreshToken: string, userData: UserModel) {
    const validUntil = new Date();
    validUntil.setSeconds(validUntil.getSeconds() + (3600 * 60) - 360);

    const authState: AuthState = {
      isAuthenticated: true,
      accessToken,
      refreshToken,
      userData,
      validUntil,
    };

    localStorage.setItem('authState', JSON.stringify(authState));
    this.authState.set(authState);
  }

  public logOut() {
    this.authState.set(this.initialState);
    localStorage.removeItem('authState');
  }

  private checkAuthState(): AuthState | null {
    const stateInStorage = localStorage.getItem('authState');
    if (!stateInStorage) {
      return null;
    }

    const authState = JSON.parse(stateInStorage) as AuthState;
    if (
      !authState ||
      !authState.validUntil ||
      authState.validUntil < new Date() ||
      !authState.userData
    ) {
      return null;
    }

    return authState;
  }
}

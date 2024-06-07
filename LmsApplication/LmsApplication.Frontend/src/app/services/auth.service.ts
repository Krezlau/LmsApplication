import {Injectable, OnDestroy, signal} from '@angular/core';
import {LoginResponse, OidcSecurityService} from "angular-auth-oidc-client";
import {UserService} from "./user.service";
import {Router} from "@angular/router";
import {AuthState} from "../types/users/auth-state";
import {BaseService} from "./base.service";
import {HttpClient} from "@angular/common/http";

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService implements OnDestroy {
  private initialState: AuthState = {
    isAuthenticated: false,
    accessToken: '',
    refreshToken: '',
    userData: null,
    tenantId: '',
  }

  public authState = signal(this.initialState);

  private configDict = new Map([
    ['tenant1', 'tenant1'],
    ['tenant2', 'tenant2']
  ]);

  constructor(private oidcSecurityService: OidcSecurityService, private userService: UserService, router: Router, http: HttpClient) {
    super(router, http);
  }

  private getConfigId() {
    return this.configDict.get(this.getTenantId());
  }

  public checkAuth() {
    // this isnt the best
    // wait until the route is loaded
    if (this.router.url === '/') {
      setTimeout(() => {
        this.checkAuth();
      }, 500);
    }

    const configId = this.getConfigId();
    console.log(configId)
    if (!configId) return;
    this.sub.add(this.oidcSecurityService.checkAuth(undefined, configId).subscribe((response : LoginResponse) => {
      this.sub.add(this.userService.getMe().subscribe((userData) => {
        this.authState.set({
          isAuthenticated: response.isAuthenticated,
          accessToken: response.accessToken,
          refreshToken: response.idToken,
          tenantId: this.userService.getTenantId(),
          userData: {
            email: userData.email,
            name: userData.name,
            surname: userData.surname,
            photo: userData.photo,
            userId: userData.userId,
            role: userData.role
          }
        });
        console.log(userData);
      }));
      console.log(response.userData);
    }));
  }

  public authorize() {
    console.log(this.getConfigId());
    this.oidcSecurityService.authorize(this.getConfigId());
  }

  public logoff() {
    this.oidcSecurityService.logoffLocal(this.getConfigId());
    this.authState.set(this.initialState);
  }
}

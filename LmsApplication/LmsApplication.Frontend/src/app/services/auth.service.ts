import {Injectable, OnDestroy, signal} from '@angular/core';
import {LoginResponse, OidcSecurityService} from "angular-auth-oidc-client";
import {Subscription} from "rxjs";

@Injectable({
  providedIn: 'root'
})
export class AuthService implements OnDestroy {
  private sub = new Subscription();

  private initialState: AuthState = {
    isAuthenticated: false,
    accessToken: '',
    refreshToken: '',
    userData: null
  }

  public authState = signal(this.initialState);

  constructor(private oidcSecurityService: OidcSecurityService) { }

  public checkAuth() {
    this.sub.add(this.oidcSecurityService.checkAuth().subscribe((response : LoginResponse) => {
      this.authState.set({
        isAuthenticated: response.isAuthenticated,
        accessToken: response.accessToken,
        refreshToken: response.idToken,
        userData: response.userData
      });
      console.log(this.authState());
    }));
  }

  public authorize() {
    this.oidcSecurityService.authorize();
  }

  public logoff() {
    return this.oidcSecurityService.logoff();
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}

export interface AuthState {
  isAuthenticated: boolean;
  accessToken: string;
  refreshToken: string;
  userData: any;
}

import {Injectable, OnDestroy, signal} from '@angular/core';
import {LoginResponse, OidcSecurityService} from "angular-auth-oidc-client";
import {Subscription} from "rxjs";
import {UserService} from "./user.service";
import {RouteConfigLoadEnd, Router} from "@angular/router";
import {AuthState} from "../types/users/auth-state";

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

  constructor(private oidcSecurityService: OidcSecurityService, private userService: UserService, private router: Router) { }

  public checkAuth() {
    // this isnt the best
    // wait until the route is loaded
    if (this.router.url === '/') {
      setTimeout(() => {
        this.checkAuth();
      }, 500);
    }

    this.sub.add(this.oidcSecurityService.checkAuth().subscribe((response : LoginResponse) => {
      this.sub.add(this.userService.getMe().subscribe((userData) => {
        this.authState.set({
          isAuthenticated: response.isAuthenticated,
          accessToken: response.accessToken,
          refreshToken: response.idToken,
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
    this.oidcSecurityService.authorize();
  }

  public logoff() {
    return this.oidcSecurityService.logoff();
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}

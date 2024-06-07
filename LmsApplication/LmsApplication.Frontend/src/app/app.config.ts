import {ApplicationConfig, importProvidersFrom} from '@angular/core';
import {provideRouter, withRouterConfig} from '@angular/router';

import {routerConfig, routes} from './app.routes';
import {provideAuth} from 'angular-auth-oidc-client';
import { authConfig } from './auth/auth.config';
import {HttpClientModule, provideHttpClient} from "@angular/common/http";
import {authHttpInterceptorProvider} from "./interceptors/AuthHttpInterceptorProvider";

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes, withRouterConfig(routerConfig)),
    provideAuth(authConfig),
    provideHttpClient(),
    importProvidersFrom(HttpClientModule),
    // authHttpInterceptorProvider,
  ]
};

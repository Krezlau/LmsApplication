import {Provider} from "@angular/core";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {AuthInterceptor} from "angular-auth-oidc-client";
import {TenantAuthInterceptor} from "./tenant-auth-interceptor";

export const authHttpInterceptorProvider: Provider = {
  provide: HTTP_INTERCEPTORS,
  useClass: TenantAuthInterceptor,
  multi: true,
}

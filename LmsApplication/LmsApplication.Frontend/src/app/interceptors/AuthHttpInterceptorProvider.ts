import {Provider} from "@angular/core";
import {HTTP_INTERCEPTORS} from "@angular/common/http";
import {AuthInterceptor} from "angular-auth-oidc-client";

export const authHttpInterceptorProvider: Provider = {
  provide: HTTP_INTERCEPTORS,
  useClass: AuthInterceptor,
  multi: true,
}

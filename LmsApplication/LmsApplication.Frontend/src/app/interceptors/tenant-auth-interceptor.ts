import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Observable } from "rxjs";
import { Location } from "@angular/common";
import { OidcSecurityService } from "angular-auth-oidc-client";

export class TenantAuthInterceptor implements HttpInterceptor {
  constructor(private location: Location, private oidcSecurityService: OidcSecurityService) {
  }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const tenant = this.location.path().split('/')[1];
    const token = this.oidcSecurityService.getIdToken();
    if (tenant) {
      console.log(tenant);
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
          Tenant: tenant,
        },
      });
    }

    return next.handle(request);
  }
}

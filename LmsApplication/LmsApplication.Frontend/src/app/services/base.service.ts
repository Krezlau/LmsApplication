import {ActivatedRoute, Router} from "@angular/router";
import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {Subscription} from "rxjs";
import {HttpClient, HttpHeaders} from "@angular/common/http";

@Injectable()
export abstract class BaseService implements OnDestroy, OnInit {

  protected sub = new Subscription();

  protected constructor(protected router: Router, protected http: HttpClient) { }

  protected headers() {
    const tenantId = this.getTenantId();
    return new HttpHeaders().set("X-Tenant-Id", tenantId || "");
  }

  public getTenantId() {
    return this.router.url.toString().split('/')[1];
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}

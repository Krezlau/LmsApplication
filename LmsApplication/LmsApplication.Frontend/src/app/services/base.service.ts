import {ActivatedRoute, Router} from "@angular/router";
import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {Subscription} from "rxjs";
import {HttpClient, HttpHeaders} from "@angular/common/http";
import {Location} from "@angular/common";

@Injectable()
export abstract class BaseService implements OnDestroy, OnInit {

  protected sub = new Subscription();

  protected constructor(protected location: Location, protected http: HttpClient) { }

  protected headers(token?: string) {
    const tenantId = this.getTenantId();
    let headers;
    if (token) {
      headers = new HttpHeaders().set("X-Tenant-Id", tenantId || "").set("Authorization", `Bearer ${token}`);
    }
    else {
      headers = new HttpHeaders().set("X-Tenant-Id", tenantId || "");
    }
    return headers;
  }

  public getTenantId() {
    return this.location.path().toString().split('/')[1];
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}

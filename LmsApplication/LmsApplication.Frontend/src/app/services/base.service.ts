import {ActivatedRoute, Router} from "@angular/router";
import {Injectable, OnDestroy, OnInit} from "@angular/core";
import {Subscription} from "rxjs";
import {HttpHeaders} from "@angular/common/http";

@Injectable()
export abstract class BaseService implements OnDestroy, OnInit {

  sub = new Subscription();

  protected constructor(protected router: Router) { }

  protected headers() {
    let tenantId = this.router.url.toString().split('/')[1];
    console.log(tenantId);
    return new HttpHeaders().set("X-Tenant-Id", tenantId || "");
  }

  ngOnInit(): void {
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
  }
}

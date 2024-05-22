import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BaseService} from "./base.service";
import {ActivatedRoute, Router} from "@angular/router";

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService{

  constructor(private http: HttpClient, route: Router) {
    super(route);
  }

  public getMe() {
    return this.http.get("http://localhost:8080/api/Auth", {headers: this.headers() });
  }

  public getUsers() {
    return this.http.get("http://localhost:8080/api/Auth/users", {headers: this.headers() });
  }
}

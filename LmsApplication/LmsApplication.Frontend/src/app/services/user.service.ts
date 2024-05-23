import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BaseService} from "./base.service";
import {Router} from "@angular/router";
import {UserModel} from "../types/users/user-model";

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService{

  constructor(http: HttpClient, route: Router) {
    super(route, http);
  }

  public getMe() {
    return this.http.get<UserModel>("http://localhost:8080/api/Auth", { headers: this.headers() });
  }

  public getUsers() {
    return this.http.get<UserModel[]>("http://localhost:8080/api/Auth/users", { headers: this.headers() });
  }
}

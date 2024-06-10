import { Injectable } from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {BaseService} from "./base.service";
import {UserModel} from "../types/users/user-model";
import {Location} from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class UserService extends BaseService{

  constructor(http: HttpClient, location: Location) {
    super(location, http);
  }

  public getMe(token?: string) {
    console.log(token);
    return this.http.get<UserModel>("http://localhost:8080/api/Auth", { headers: this.headers(token) });
  }

  public getUser(email: string) {
    return this.http.get<UserModel>(`http://localhost:8080/api/Auth/${email}`, { headers: this.headers() });
  }
  public getUsers() {
    return this.http.get<UserModel[]>("http://localhost:8080/api/Auth/users", { headers: this.headers() });
  }
}

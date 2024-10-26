import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserModel } from '../types/users/user-model';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';
import { env } from '../../env';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(
    private authService: AuthService,
    private http: HttpClient,
  ) {}

  public getMe(token?: string) {
    return this.http.get<ApiResponse<UserModel>>(
      `${env.apiUrl}/api/users/current`,
      {
        headers: {
          Authorization: `Bearer ${token ?? this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getUser(email: string) {
    return this.http.get<ApiResponse<UserModel>>(
      `http://localhost:8080/api/Auth/${email}`,
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }

  public getUsers() {
    return this.http.get<ApiResponse<UserModel[]>>(
      'http://localhost:8080/api/Auth/users',
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }
}

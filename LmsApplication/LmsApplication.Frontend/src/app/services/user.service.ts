import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { UserModel } from '../types/users/user-model';
import { AuthService } from './auth.service';
import { UserRole } from '../types/users/user-role';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(
    private authService: AuthService,
    private http: HttpClient,
  ) {}

  public register(
    email: string,
    firstName: string,
    surname: string,
    password: string,
  ) {
    return this.http.post<any>(`${environment.apiUrl}/register`, {
      email,
      name: firstName,
      surname,
      password,
    });
  }

  public getMe(token?: string) {
    return this.http.get<UserModel>(`${environment.apiUrl}/api/users/current`, {
      headers: {
        Authorization: `Bearer ${token ?? this.authService.authState().accessToken}`,
      },
    });
  }

  public getUser(email: string) {
    return this.http.get<UserModel>(`${environment.apiUrl}/api/users/${email}`, {
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }

  public getUsers() {
    return this.http.get<UserModel[]>(`${environment.apiUrl}/api/users`, {
      headers: {
        Authorization: `Bearer ${this.authService.authState().accessToken}`,
      },
    });
  }

  public updateUser(name: string, surname: string, bio: string | null) {
    return this.http.put<UserModel>(
      `${environment.apiUrl}/api/users/current`,
      { name, surname, bio },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getUsersByCourseEdition(courseEditionId: string) {
    return this.http.get<UserModel[]>(
      `${environment.apiUrl}/api/users/by-course-edition/${courseEditionId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public searchUsersByEmail(email: string) {
    return this.http.get<UserModel[]>(
      `${environment.apiUrl}/api/users/search/${email}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public updateUserRole(userId: string, role: UserRole) {
    return this.http.put<null>(
      `${environment.apiUrl}/api/users/${userId}/role`,
      { role: +role },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

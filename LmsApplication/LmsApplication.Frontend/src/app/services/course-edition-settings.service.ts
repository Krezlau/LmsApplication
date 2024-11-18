import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AuthService } from './auth.service';
import { env } from '../../env';
import { ApiResponse } from '../types/api-response';
import { CourseEditionSettings } from '../types/courses/course-edition-settings';

@Injectable({
  providedIn: 'root',
})
export class CourseEditionSettingsService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getCourseEdtionSettings(editionId: string) {
    return this.http.get<ApiResponse<CourseEditionSettings>>(
      `${env.apiUrl}/api/editions/${editionId}/settings`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public updateCourseEditionSettings(
    editionId: string,
    allowAllToPost: boolean,
  ) {
    return this.http.put<ApiResponse<null>>(
      `${env.apiUrl}/api/editions/${editionId}/settings`,
      { allowAllToPost },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

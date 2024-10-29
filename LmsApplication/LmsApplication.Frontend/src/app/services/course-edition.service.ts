import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseEditionModel } from '../types/courses/course-edition-model';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';
import { env } from '../../env';

@Injectable({
  providedIn: 'root',
})
export class CourseEditionService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getCourseEditions() {
    return this.http.get<ApiResponse<CourseEditionModel[]>>(
      `${env.apiUrl}/api/courses/editions/my-courses`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  getCourseEditionsByCourseId(courseId: string) {
    return this.http.get<ApiResponse<CourseEditionModel[]>>(
      `${env.apiUrl}/api/courses/editions/by-course/${courseId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

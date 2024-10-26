import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseEditionModel } from '../types/courses/course-edition-model';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';

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
      'http://localhost:8080/api/courses/editions/my-courses',
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }

  getCourseEditionsByCourseId(courseId: string) {
    return this.http.get<ApiResponse<CourseEditionModel[]>>(
      `http://localhost:8080/api/courses/editions/by-course/${courseId}`,
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }
}

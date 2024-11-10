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

  getMyCourseEditions() {
    return this.http.get<ApiResponse<CourseEditionModel[]>>(
      `${env.apiUrl}/api/courses/editions/my-courses`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  getOpenRegistrationCourseEditions() {
    return this.http.get<ApiResponse<CourseEditionModel[]>>(
      `${env.apiUrl}/api/courses/editions/registration-open`,
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

  getCourseEditionById(editionId: string) {
    return this.http.get<ApiResponse<CourseEditionModel>>(
      `${env.apiUrl}/api/courses/editions/${editionId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  createCourseEdition(
    courseId: string,
    title: string,
    studentLimit: number,
    startDate: Date,
    registrationStartDateUtc: Date | null = null,
    registrationEndDateUtc: Date | null = null,
  ) {
    return this.http.post<ApiResponse<CourseEditionModel>>(
      `${env.apiUrl}/api/courses/editions`,
      {
        courseId,
        title,
        studentLimit,
        startDateUtc: startDate,
        registrationStartDateUtc,
        registrationEndDateUtc,
      },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  registerForCourseEdition(editionId: string) {
    return this.http.post<ApiResponse<null>>(
      `${env.apiUrl}/api/courses/editions/${editionId}/register`,
      {},
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

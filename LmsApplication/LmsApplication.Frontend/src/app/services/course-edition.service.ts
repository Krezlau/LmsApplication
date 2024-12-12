import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseEditionModel } from '../types/courses/course-edition-model';
import { AuthService } from './auth.service';
import { ApiResponse } from '../types/api-response';
import { environment } from '../../environments/environment';
import { CollectionResource } from '../types/collection-resource';

@Injectable({
  providedIn: 'root',
})
export class CourseEditionService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  getMyCourseEditions(page: number = 1, pageSize: number = 10) {
    return this.http.get<ApiResponse<CollectionResource<CourseEditionModel>>>(
      `${environment.apiUrl}/api/courses/editions/my-courses?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  getOpenRegistrationCourseEditions(page: number = 1, pageSize: number = 10) {
    return this.http.get<ApiResponse<CollectionResource<CourseEditionModel>>>(
      `${environment.apiUrl}/api/courses/editions/registration-open?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  getCourseEditionsByCourseId(courseId: string, page: number = 1, pageSize: number = 10) {
    return this.http.get<ApiResponse<CollectionResource<CourseEditionModel>>>(
      `${environment.apiUrl}/api/courses/editions/by-course/${courseId}?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  getCourseEditionById(editionId: string) {
    return this.http.get<ApiResponse<CourseEditionModel>>(
      `${environment.apiUrl}/api/courses/editions/${editionId}`,
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
      `${environment.apiUrl}/api/courses/editions`,
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
      `${environment.apiUrl}/api/courses/editions/${editionId}/register`,
      {},
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  addUserToCourseEdition(editionId: string, userId: string) {
    return this.http.post<ApiResponse<null>>(
      `${environment.apiUrl}/api/courses/editions/${editionId}/add-user`,
      { userId },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  removeUserFromCourseEdition(editionId: string, userId: string) {
    return this.http.post<ApiResponse<null>>(
      `${environment.apiUrl}/api/courses/editions/${editionId}/remove-user`,
      { userId },
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

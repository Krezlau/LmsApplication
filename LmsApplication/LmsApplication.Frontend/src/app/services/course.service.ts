import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseModel } from '../types/courses/course-model';
import { CoursePostModel } from '../types/courses/course-post-model';
import { AuthService } from './auth.service';
import { CourseCategory } from '../types/courses/course-category';
import { ApiResponse } from '../types/api-response';
import { environment } from '../../environments/environment';
import { CollectionResource } from '../types/collection-resource';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getAllCourses(page: number = 1, pageSize: number = 8) {
    return this.http.get<ApiResponse<CollectionResource<CourseModel>>>(
      `${environment.apiUrl}/api/courses?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getCourseById(courseId: string) {
    return this.http.get<ApiResponse<CourseModel>>(
      `${environment.apiUrl}/api/courses/${courseId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public createCourse(course: CoursePostModel) {
    return this.http.post<ApiResponse<CourseModel>>(
      `${environment.apiUrl}/api/courses`,
      course,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getCategories() {
    return this.http.get<ApiResponse<CourseCategory[]>>(
      `${environment.apiUrl}/api/courses/categories`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteCourse(courseId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${environment.apiUrl}/api/courses/${courseId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public searchCourses(query: string, page: number = 1, pageSize: number = 5) {
    return this.http.get<ApiResponse<CollectionResource<CourseModel>>>(
      `${environment.apiUrl}/api/courses/search/${query}?page=${page}&pageSize=${pageSize}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

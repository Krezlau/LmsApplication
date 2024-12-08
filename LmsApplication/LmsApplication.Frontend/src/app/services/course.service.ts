import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseModel } from '../types/courses/course-model';
import { CoursePostModel } from '../types/courses/course-post-model';
import { AuthService } from './auth.service';
import { CourseCategory } from '../types/courses/course-category';
import { ApiResponse } from '../types/api-response';
import { env } from '../../env';

@Injectable({
  providedIn: 'root',
})
export class CourseService {
  constructor(
    private http: HttpClient,
    private authService: AuthService,
  ) {}

  public getAllCourses() {
    return this.http.get<ApiResponse<CourseModel[]>>(
      `${env.apiUrl}/api/courses`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public getCourseById(courseId: string) {
    return this.http.get<ApiResponse<CourseModel>>(
      `${env.apiUrl}/api/courses/${courseId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public createCourse(course: CoursePostModel) {
    return this.http.post<ApiResponse<CourseModel>>(
      `${env.apiUrl}/api/courses`,
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
      `${env.apiUrl}/api/courses/categories`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }

  public deleteCourse(courseId: string) {
    return this.http.delete<ApiResponse<null>>(
      `${env.apiUrl}/api/courses/${courseId}`,
      {
        headers: {
          Authorization: `Bearer ${this.authService.authState().accessToken}`,
        },
      },
    );
  }
}

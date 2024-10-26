import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CourseModel } from '../types/courses/course-model';
import { CoursePostModel } from '../types/courses/course-post-model';
import { AuthService } from './auth.service';
import { CourseCategory } from '../types/courses/course-category';
import { ApiResponse } from '../types/api-response';

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
      'http://localhost:8080/api/courses',
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }

  public getCourseById(courseId: string) {
    return this.http.get<ApiResponse<CourseModel>>(
      `http://localhost:8080/api/courses/${courseId}`,
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }

  public createCourse(course: CoursePostModel) {
    return this.http.post<ApiResponse<CourseModel>>(
      'http://localhost:8080/api/courses',
      course,
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }

  public getCategories() {
    return this.http.get<ApiResponse<CourseCategory[]>>(
      'http://localhost:8080/api/courses/categories',
      {
        headers: {
          Authorization: this.authService.authState().accessToken,
        },
      },
    );
  }
}

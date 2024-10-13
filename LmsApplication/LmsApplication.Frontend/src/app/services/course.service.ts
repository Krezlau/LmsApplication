import { Injectable } from '@angular/core';
import {BaseService} from "./base.service";
import {HttpClient} from "@angular/common/http";
import {CourseModel} from "../types/courses/course-model";
import {CoursePostModel} from "../types/courses/course-post-model";
import {Location} from "@angular/common";
import {AuthService} from "./auth.service";
import {CourseCategory} from "../types/courses/course-category";
import {ApiResponse} from "../types/api-response";

@Injectable({
  providedIn: 'root'
})
export class CourseService extends BaseService {

  constructor(location: Location, http: HttpClient, private authService: AuthService) {
    super(location,  http);
  }

  public getAllCourses() {
    return this.http.get<ApiResponse<CourseModel[]>>('http://localhost:8080/api/courses',
      { headers: this.headers(this.authService.authState().accessToken) });
  }

  public getCourseById(courseId: string) {
    return this.http.get<ApiResponse<CourseModel>>(`http://localhost:8080/api/courses/${courseId}`,
      { headers: this.headers(this.authService.authState().accessToken) });
  }

  public createCourse(course: CoursePostModel) {
    return this.http.post<ApiResponse<CourseModel>>('http://localhost:8080/api/courses', course,
      { headers: this.headers(this.authService.authState().accessToken) });
  }

  public getCategories() {
    return this.http.get<ApiResponse<CourseCategory[]>>('http://localhost:8080/api/courses/categories',
      { headers: this.headers(this.authService.authState().accessToken) });
  }
}

import { Injectable } from '@angular/core';
import {BaseService} from "./base.service";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {CourseModel} from "../types/courses/course-model";

@Injectable({
  providedIn: 'root'
})
export class CourseService extends BaseService {

  constructor(router: Router, http: HttpClient) {
    super(router,  http);
  }

  public getAllCourses() {
    return this.http.get<CourseModel[]>('http://localhost:8080/api/courses', { headers: this.headers() });
  }
}

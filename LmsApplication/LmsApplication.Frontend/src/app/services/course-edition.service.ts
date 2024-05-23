import { Injectable } from '@angular/core';
import {BaseService} from "./base.service";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {CourseEditionModel} from "../types/courses/course-edition-model";

@Injectable({
  providedIn: 'root'
})
export class CourseEditionService extends BaseService{

  constructor(router: Router, http: HttpClient) {
    super(router, http);
  }

  getCourseEditions() {
    return this.http.get<CourseEditionModel[]>('http://localhost:8080/api/courses/editions/my-courses', {headers: this.headers()});
    // return this.http.get<CourseEditionModel[]>('http://localhost:8080/api/courses/editions/all', {headers: this.headers()});
  }
}

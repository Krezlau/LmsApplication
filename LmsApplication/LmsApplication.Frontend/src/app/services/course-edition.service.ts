import { Injectable } from '@angular/core';
import {BaseService} from "./base.service";
import {Router} from "@angular/router";
import {HttpClient} from "@angular/common/http";
import {CourseEditionModel} from "../types/courses/course-edition-model";
import {Location} from "@angular/common";

@Injectable({
  providedIn: 'root'
})
export class CourseEditionService extends BaseService{

  constructor(location: Location, http: HttpClient) {
    super(location, http);
  }

  getCourseEditions() {
    return this.http.get<CourseEditionModel[]>('http://localhost:8080/api/courses/editions/my-courses', {headers: this.headers()});
  }

  getCourseEditionsByCourseId(courseId: string) {
    return this.http.get<CourseEditionModel[]>(`http://localhost:8080/api/courses/editions/by-course/${courseId}`, {headers: this.headers()});
  }
}

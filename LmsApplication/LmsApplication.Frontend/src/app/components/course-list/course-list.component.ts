import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {CourseAddFormComponent} from "../course-add-form/course-add-form.component";
import {NgForOf} from "@angular/common";
import {AuthService} from "../../services/auth.service";
import {Router} from "@angular/router";
import {CourseService} from "../../services/course.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
    CourseAddFormComponent,
    NgForOf
  ],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent implements OnInit, OnDestroy{
  courses: CourseModel[] = [];

  authState = this.authService.authState;

  sub = new Subscription();

  constructor(private authService: AuthService, private router: Router, private courseService: CourseService) {
  }

  async redirectToDetails(courseId: string) {
    await this.router.navigate([this.authState().tenantId, 'courses', courseId]);
  }

  ngOnInit() {
    this.sub.add(this.courseService.getAllCourses().subscribe(courses => {
      this.courses = courses;
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}

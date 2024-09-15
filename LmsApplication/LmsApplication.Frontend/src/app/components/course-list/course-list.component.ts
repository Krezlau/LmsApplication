import {Component, Input, OnDestroy, OnInit} from '@angular/core';
import {CourseModel} from "../../types/courses/course-model";
import {CourseAddFormComponent} from "../course-add-form/course-add-form.component";
import {NgForOf, NgIf} from "@angular/common";
import {AuthService} from "../../services/auth.service";
import {Router} from "@angular/router";
import {CourseService} from "../../services/course.service";
import {Subscription} from "rxjs";

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [
    CourseAddFormComponent,
    NgForOf,
    NgIf
  ],
  templateUrl: './course-list.component.html'
})
export class CourseListComponent implements OnInit, OnDestroy{
  courses: CourseModel[] = [];

  authState = this.authService.authState;

  sub = new Subscription();

  coursesLoading = false;

  constructor(private authService: AuthService, private router: Router, private courseService: CourseService) {
  }

  async redirectToDetails(courseId: string) {
    await this.router.navigate([this.authState().tenantId, 'courses', courseId]);
  }

  ngOnInit() {
    this.coursesLoading = true;
    this.sub.add(this.courseService.getAllCourses().subscribe(courses => {
      this.courses = courses;
      this.coursesLoading = false;
    }));
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }

}

import { Component, OnDestroy, OnInit } from '@angular/core';
import { CourseModel } from '../../types/courses/course-model';
import { CourseAddFormComponent } from '../course-add-form/course-add-form.component';
import { NgForOf, NgIf } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { Router } from '@angular/router';
import { CourseService } from '../../services/course.service';
import { Subscription, tap } from 'rxjs';
import { AlertService } from '../../services/alert.service';

@Component({
  selector: 'app-course-list',
  standalone: true,
  imports: [CourseAddFormComponent, NgForOf, NgIf],
  templateUrl: './course-list.component.html',
})
export class CourseListComponent implements OnInit, OnDestroy {
  courses: CourseModel[] = [];

  authState = this.authService.authState;

  sub = new Subscription();

  coursesLoading = false;

  constructor(
    private authService: AuthService,
    private router: Router,
    private courseService: CourseService,
    private alertService: AlertService,
  ) {}

  async redirectToDetails(courseId: string) {
    await this.router.navigate(['courses', courseId]);
  }

  addCourse(course: CourseModel) {
    this.courses = [...this.courses, course];
  }

  ngOnInit() {
    this.coursesLoading = true;
    this.sub.add(
      this.courseService
        .getAllCourses()
        .pipe(
          tap({
            next: (courses) => {
              this.courses = courses.data!;
              this.coursesLoading = false;
            },
            error: (err) => {
              this.alertService.show(err.error.message, 'error');
              this.coursesLoading = false;
            },
          }),
        )
        .subscribe(),
    );
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}

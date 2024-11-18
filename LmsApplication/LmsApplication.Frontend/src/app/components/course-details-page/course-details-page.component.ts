import { Component, OnDestroy, OnInit } from '@angular/core';
import { CourseEditionService } from '../../services/course-edition.service';
import { CourseService } from '../../services/course.service';
import { ActivatedRoute } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { CourseModel } from '../../types/courses/course-model';
import { CourseEditionModel } from '../../types/courses/course-edition-model';
import { CourseDetailsComponent } from '../course-details/course-details.component';
import { CourseEditionListComponent } from '../course-edition-list/course-edition-list.component';
import { AsyncPipe } from '@angular/common';
import { ApiResponse } from '../../types/api-response';
import { CourseResourcesListComponent } from '../course-resources-list/course-resources-list.component';

@Component({
  selector: 'app-course-details-page',
  standalone: true,
  imports: [CourseDetailsComponent, CourseEditionListComponent, AsyncPipe, CourseResourcesListComponent],
  templateUrl: './course-details-page.component.html',
})
export class CourseDetailsPageComponent implements OnInit, OnDestroy {
  course$: Observable<ApiResponse<CourseModel>> | null = null;
  courseEditions$: Observable<ApiResponse<CourseEditionModel[]>> | null = null;

  courseId = '';

  sub = new Subscription();
  constructor(
    private courseService: CourseService,
    private courseEditionService: CourseEditionService,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.sub = this.route.params.subscribe((params) => {
      this.courseId = params['courseId'];
      this.course$ = this.courseService.getCourseById(this.courseId);
      this.courseEditions$ =
        this.courseEditionService.getCourseEditionsByCourseId(
          this.courseId,
        );
    });
  }

  ngOnDestroy() {
    this.sub.unsubscribe();
  }
}
